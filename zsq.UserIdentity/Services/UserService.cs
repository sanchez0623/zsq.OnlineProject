using DnsClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Resilience;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
//using System.Net.Http;
using System.Threading.Tasks;
using zsq.UserIdentity.Config;
using zsq.UserIdentity.Dtos;

namespace zsq.UserIdentity.Services
{
    public class UserService : IUserService
    {
        private string _userServiceUrl;
        //private IHttpClientFactory _clientFactory;
        private IHttpClient _httpClient;
        private ILogger<UserService> _logger;

        public UserService(
            //IHttpClientFactory clientFactory, 
            IHttpClient httpclient,
            IDnsQuery query,
            IOptions<ServiceDiscoveryOptions> options,
            ILogger<UserService> logger)
        {
            //_clientFactory = clientFactory;
            _httpClient = httpclient;
            _logger = logger;
            var address = query.ResolveService("service.consul", options.Value.UserServiceName);
            var addressList = address.First().AddressList;
            var host = addressList.Any() ?
                addressList.First().ToString() : address.First().HostName.Substring(0, address.First().HostName.Length - 1);
            var port = address.First().Port;
            _userServiceUrl = $"http://{host}:{port}";
        }

        public async Task<UserInfo> CheckOrCreateAsync(string phone)
        {
            try
            {
                var form = new Dictionary<string, string>
                {
                    { "phone" , phone }
                };
                //var content = new FormUrlEncodedContent(form);
                //var httpResponse = await _clientFactory.CreateClient().PostAsync(_userServiceUrl + "/api/users/create", content);
                var httpResponse = await _httpClient.FormPostAsync(_userServiceUrl + "/api/users/create", form);

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    var userInfoStr = await httpResponse.Content.ReadAsStringAsync();
                    var userinfo = JsonConvert.DeserializeObject<UserInfo>(userInfoStr);
                    //int.TryParse(userIdStr, out int userId);
                    return userinfo;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(UserService.CheckOrCreateAsync)} 重试后失败：{ex.Message} \n {ex.StackTrace}");
                throw;
            }
        }
    }
}
