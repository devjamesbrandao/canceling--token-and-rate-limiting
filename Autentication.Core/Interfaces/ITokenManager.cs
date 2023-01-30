using Autentication.Core.DTO;
using Microsoft.AspNetCore.Http;

namespace Autentication.Core.Interfaces
{
    public interface ITokenManager
    {
        bool IsCurrentActiveToken();
        void DeactivateCurrentToken();
        ClientStatistics GetClientStatisticsByKey(string key);
        string GenerateClientKey(HttpContext context) => $"{context.Request.Path}_{context.Connection.RemoteIpAddress}";
        void UpdateClientStatisticsStorage(string key, int maxRequests);
    }
}