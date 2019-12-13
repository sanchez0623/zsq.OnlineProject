using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Resilience
{
    public interface IHttpClient
    {
        Task<string> GetStringAsync(string uri, string authorizationToken = null,
            string authorizationMethod = "Bearar");

        Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearar");

        Task<HttpResponseMessage> FormPostAsync(string uri, Dictionary<string, string> form, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearar");

        Task<HttpResponseMessage> DeleteAsync<T>(string uri, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearar");

        Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null,
            string requestId = null, string authorizationMethod = "Bearar");
    }
}
