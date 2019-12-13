using DnsClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Resilience;
using System;
using System.Linq;
using System.Threading.Tasks;
using zsq.RecommendApi.Config;
using zsq.RecommendApi.Dtos;

namespace zsq.RecommendApi.Services
{
    public class UserService : IUserService
    {
        private string _userServiceUrl;
        private IHttpClient _httpClient;
        private ILogger<UserService> _logger;

        public UserService(IHttpClient httpclient,
            IDnsQuery query,
            IOptions<ServiceDiscoveryOptions> options,
            ILogger<UserService> logger)
        {
            _httpClient = httpclient;
            _logger = logger;
            var address = query.ResolveService("service.consul", options.Value.UserServiceName);
            var addressList = address.First().AddressList;
            var host = addressList.Any() ?
                addressList.First().ToString() : address.First().HostName.Substring(0, address.First().HostName.Length - 1);
            var port = address.First().Port;
            _userServiceUrl = $"http://{host}:{port}";
        }

        public async Task<UserInfo> GetUserInfoAsync(int userId)
        {
            try
            {
                var httpResponse = await _httpClient.GetStringAsync(_userServiceUrl + $"/api/users/baseInfo/{userId}");

                if (!string.IsNullOrEmpty(httpResponse))
                {
                    var userinfo = JsonConvert.DeserializeObject<UserInfo>(httpResponse);
                    _logger.LogInformation($"从userapi获取到用户 {userinfo.UserId} 的基本信息");
                    return userinfo;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(UserService.GetUserInfoAsync)} 重试后失败：{ex.Message} \n {ex.StackTrace}");
                throw;
            }
        }
    }
}
