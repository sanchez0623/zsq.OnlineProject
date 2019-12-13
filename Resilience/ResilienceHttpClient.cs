using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Wrap;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using zipkin4net.Transport.Http;

namespace Resilience
{
    public class ResilienceHttpClient : IHttpClient
    {
        private HttpClient _httpClient;
        //根据url origin 创建 policy
        private Func<string, IEnumerable<Policy>> _policyCreator;
        //把policy组合成policy wrap，并使用ConcurrentDictionary进行缓存
        private ConcurrentDictionary<string, PolicyWrap> _policyWrappers;
        private ILogger<ResilienceHttpClient> _logger;
        private IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationName">问了配合zipkin使用加的参数</param>
        /// <param name="policyCreator"></param>
        /// <param name="logger"></param>
        /// <param name="httpContextAccessor"></param>
        public ResilienceHttpClient(string applicationName,
            Func<string, IEnumerable<Policy>> policyCreator,
            ILogger<ResilienceHttpClient> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = new HttpClient(new TracingHandler(applicationName));
            _policyCreator = policyCreator;
            _policyWrappers = new ConcurrentDictionary<string, PolicyWrap>();
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<HttpResponseMessage> DeleteAsync<T>(string uri, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearar")
        {
            throw new NotImplementedException();
        }

        public Task<string> GetStringAsync(string uri, string authorizationToken = null, string authorizationMethod = "Bearar")
        {
            var origin = GetOriginFromUri(uri);

            return HttpInvoker(origin, async (context) =>
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

                SetAuthorizationHeader(requestMessage);
                //requestMessage.Content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");

                if (authorizationToken != null)
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
                }

                var response = await _httpClient.SendAsync(requestMessage);
                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new HttpRequestException();
                }
                return await response.Content.ReadAsStringAsync();
            });
        }

        public async Task<HttpResponseMessage> FormPostAsync(string uri, Dictionary<string, string> form, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearar")
        {
            Func<HttpRequestMessage> func = () => GetHttpRequestMessage(HttpMethod.Post, uri, form);
            return await DoPostPutAsync(HttpMethod.Post, uri, func, authorizationToken, requestId, authorizationMethod);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearar")
        {
            Func<HttpRequestMessage> func = () => GetHttpRequestMessage(HttpMethod.Post, uri, item);
            return await DoPostPutAsync(HttpMethod.Post, uri, func, authorizationToken, requestId, authorizationMethod);
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearar")
        {
            Func<HttpRequestMessage> func = () => GetHttpRequestMessage(HttpMethod.Post, uri, item);
            return await DoPostPutAsync(HttpMethod.Post, uri, func, authorizationToken, requestId, authorizationMethod);
        }

        private HttpRequestMessage GetHttpRequestMessage<T>(HttpMethod httpMethod, string url, T item)
        {
            return new HttpRequestMessage(httpMethod, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json")
            };
        }

        private HttpRequestMessage GetHttpRequestMessage(HttpMethod httpMethod, string url, Dictionary<string, string> form)
        {
            return new HttpRequestMessage(httpMethod, url)
            {
                Content = new FormUrlEncodedContent(form)
            };
        }

        private Task<HttpResponseMessage> DoPostPutAsync(HttpMethod httpMethod, string uri,
            Func<HttpRequestMessage> getRequestMessage,
            string authorizationToken = null, string requestId = null,
            string authorizationMethod = "Bearar")
        {
            if (httpMethod != HttpMethod.Post && httpMethod != HttpMethod.Put)
            {
                throw new ArgumentException("只支持post和put操作", nameof(HttpMethod));
            }

            var origin = GetOriginFromUri(uri);

            return HttpInvoker(origin, async (context) =>
            {
                //var requestMessage = new HttpRequestMessage(httpMethod, uri);
                var requestMessage = getRequestMessage();

                SetAuthorizationHeader(requestMessage);
                //requestMessage.Content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");

                if (authorizationToken != null)
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
                }
                if (requestId != null)
                {
                    requestMessage.Headers.Add("x-requestId", requestId);
                }

                var response = await _httpClient.SendAsync(requestMessage);
                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new HttpRequestException();
                }
                return response;
            });
        }

        private async Task<T> HttpInvoker<T>(string origin, Func<Context, Task<T>> action)
        {
            var normalizeOrigin = NormalizeOrigin(origin);
            if (!_policyWrappers.TryGetValue(normalizeOrigin, out PolicyWrap policyWrap))
            {
                policyWrap = Policy.Wrap(_policyCreator(normalizeOrigin).ToArray());
                _policyWrappers.TryAdd(normalizeOrigin, policyWrap);
            }
            return await policyWrap.Execute(action, new Context(normalizeOrigin));
        }

        private static string NormalizeOrigin(string origin)
        {
            return origin?.Trim()?.ToLower();
        }

        private static string GetOriginFromUri(string uri)
        {
            var url = new Uri(uri);
            var origin = $"{url.Scheme}://{url.DnsSafeHost}:{url.Port}";
            return origin;
        }

        private void SetAuthorizationHeader(HttpRequestMessage requestMessage)
        {
            var authorizationHeader = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"];
            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                requestMessage.Headers.Add("Authorization", new List<string> { authorizationHeader });
            }
        }
    }
}
