using System.Net;
using Autentication.API.Configuration;
using Autentication.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Autentication.API.Middleware
{
    public class TokenManagerMiddleware : IMiddleware
    {
        private readonly ITokenManager _tokenManager;

        public TokenManagerMiddleware(ITokenManager tokenManager)
        {
            _tokenManager = tokenManager;
        }
        
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var endpoint = context?.GetEndpoint();

            var decorator = endpoint?.Metadata.GetMetadata<LimitRequests>();

            if (decorator is not null)
            {
                var key = _tokenManager.GenerateClientKey(context);

                var clientStatistics = _tokenManager.GetClientStatisticsByKey(key);

                if (
                    clientStatistics != null 
                    && 
                    DateTime.UtcNow < clientStatistics.LastSuccessfulResponseTime.AddSeconds(decorator.TimeWindowInSeconds) 
                    && 
                    clientStatistics.NumberOfRequestsCompletedSuccessfully == decorator.MaxRequests)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    return;
                }

                _tokenManager.UpdateClientStatisticsStorage(key, decorator.MaxRequests);
            }

            if((endpoint is null) || (endpoint?.Metadata.Any(p => p.GetType().Name.ToString() == "AllowAnonymousAttribute") ?? false))
            {
                await next(context);
                
                return;
            }

            if (_tokenManager.IsCurrentActiveToken())
            {
                await next(context);
                
                return;
            }
            
            context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
        }
    }
}