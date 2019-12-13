using DnsClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Resilience;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zsq.RecommendApi.Config;
using zsq.RecommendApi.Dtos;

namespace zsq.RecommendApi.Services
{
    public class ContactService : IContactService
    {
        private string _contactServiceUrl;
        private IHttpClient _httpClient;
        private ILogger<ContactService> _logger;

        public ContactService(IHttpClient httpclient,
            IDnsQuery query,
            IOptions<ServiceDiscoveryOptions> options,
            ILogger<ContactService> logger)
        {
            _httpClient = httpclient;
            _logger = logger;
            var address = query.ResolveService("service.consul", options.Value.ContactServiceName);
            var addressList = address.First().AddressList;
            var host = addressList.Any() ?
                addressList.First().ToString() : address.First().HostName.Substring(0, address.First().HostName.Length - 1);
            var port = address.First().Port;
            _contactServiceUrl = $"http://{host}:{port}";
        }

        public async Task<List<ContactInfo>> GetContactInfoByUser(int userId)
        {
            try
            {
                var httpResponse = await _httpClient.GetStringAsync(_contactServiceUrl + $"/api/contacts/{userId}");

                if (!string.IsNullOrEmpty(httpResponse))
                {
                    var contactsInfo = JsonConvert.DeserializeObject<List<ContactInfo>>(httpResponse);
                    _logger.LogInformation($"从contactapi获取 {userId} 的好友列表。");
                    return contactsInfo;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(ContactService.GetContactInfoByUser)} 重试后失败：{ex.Message} \n {ex.StackTrace}");
                throw;
            }
        }
    }
}
