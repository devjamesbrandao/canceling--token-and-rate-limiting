using Autentication.Core.DTO;
using Autentication.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Autentication.Core.Services
{
    public class TokenManager : ITokenManager
    {
        private readonly IMemoryCache _memoryCache;
        
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        private readonly IOptions<JwtOptions> _jwtOptions;

        public TokenManager(
            IMemoryCache memoryCache, 
            IHttpContextAccessor httpContextAccessor,
            IOptions<JwtOptions> jwtOptions
        )
        {
            _memoryCache = memoryCache;
            _httpContextAccessor = httpContextAccessor;
            _jwtOptions = jwtOptions;
        }

        public bool IsCurrentActiveToken()
        {
            var token = GetCurrentToken();

            return !(_memoryCache.TryGetValue(GetKey(token), out _));
        }

        public void DeactivateCurrentToken()
        {
            var token = GetCurrentToken();

            var cacheExpiryOptions = new MemoryCacheEntryOptions
            {
                // With Absolute expiration, we can set the actual expiration of the cache entry. 
                // Here it is set as 5 minutes. So, every 5 minutes, without taking into consideration the sliding expiration, the cache will be expired
                AbsoluteExpiration = DateTime.UtcNow.AddMinutes(_jwtOptions.Value.ExpiryMinutes),
                // Sets the priority of keeping the cache entry in the cache. The default setting is Normal. Other options are High, Low and Never Remove
                Priority = CacheItemPriority.High
            };

            _memoryCache.Set(GetKey(token), " ", cacheExpiryOptions);
        }

        public ClientStatistics GetClientStatisticsByKey(string key)
        {   
            _memoryCache.TryGetValue(key, out ClientStatistics clientStatistics);
            
            return clientStatistics;
        }

        public string GenerateClientKey(HttpContext context) => $"{context.Request.Path}_{context.Connection.RemoteIpAddress}";

        public void UpdateClientStatisticsStorage(string key, int maxRequests)
        {
            ClientStatistics clientStat;

            _memoryCache.TryGetValue(key, out clientStat);

            if (clientStat != null)
            {
                clientStat.LastSuccessfulResponseTime = DateTime.UtcNow;

                if (clientStat.NumberOfRequestsCompletedSuccessfully == maxRequests)
                    clientStat.NumberOfRequestsCompletedSuccessfully = 1;

                else
                    clientStat.NumberOfRequestsCompletedSuccessfully++;

                _memoryCache.Set(key, clientStat);
            }
            else
            {
                var clientStatistics = new ClientStatistics
                {
                    LastSuccessfulResponseTime = DateTime.UtcNow,
                    NumberOfRequestsCompletedSuccessfully = 1
                };

                _memoryCache.Set(key, clientStatistics);
            }
        }

        private string GetCurrentToken()
        {
            var authorizationHeader = _httpContextAccessor
                                      .HttpContext.Request.Headers["authorization"];

            return authorizationHeader == StringValues.Empty
                ? string.Empty
                : authorizationHeader.Single().Split(" ").Last();
        }

        private static string GetKey(string token) => $"{token}:deactivated";
    }
}