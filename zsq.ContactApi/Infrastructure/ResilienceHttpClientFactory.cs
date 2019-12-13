using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Polly;
using Resilience;
using System;
using System.Net.Http;

namespace zsq.ContactApi.Infrastructure
{
    public class ResilienceHttpClientFactory
    {
        private ILogger<ResilienceHttpClient> _logger;
        private IHttpContextAccessor _httpContextAccessor;

        private int _retryCount;
        private int _exceptionCountAllowed;

        public ResilienceHttpClientFactory(ILogger<ResilienceHttpClient> logger,
            IHttpContextAccessor httpContextAccessor,
            int retryCount,
            int exceptionCountAllowed)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _retryCount = retryCount;
            _exceptionCountAllowed = exceptionCountAllowed;
        }

        public ResilienceHttpClient GetResilienceHttpClient() =>
            new ResilienceHttpClient("contact_api", origin => CreatePolicy(origin), _logger, _httpContextAccessor);

        private Policy[] CreatePolicy(string origin)
        {
            return new Policy[]
            {
                Policy.Handle<HttpRequestException>().WaitAndRetry(
                    _retryCount,
                    retryAttemp => TimeSpan.FromSeconds(Math.Pow(2, retryAttemp)),
                    (ex, timespan, retryCount, context) =>
                    {
                        var msg = $"第 {retryCount} 次重试 of {context.PolicyKey} at {context.OperationKey} due to {ex}";
                        _logger.LogDebug(msg);
                        _logger.LogWarning(msg);
                    }),

                Policy.Handle<HttpRequestException>().CircuitBreaker(
                    _exceptionCountAllowed,
                    TimeSpan.FromMinutes(1),
                    (ex, duration) =>
                    {
                        _logger.LogTrace("熔断打开");
                    },
                    () =>
                    {
                        _logger.LogTrace("熔断关闭");
                    })
            };
        }
    }
}
