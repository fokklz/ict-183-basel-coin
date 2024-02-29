using System.Net;

namespace BaselCoin.Middlewares
{
    public class IpSecurityMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<IpSecurityMiddleware> _logger;
        private readonly List<IPNetwork> _allowedNetworks = new();

        public IpSecurityMiddleware(RequestDelegate next, ILogger<IpSecurityMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            var allowedIpRanges = configuration.GetSection("IpSecurity:AllowedIpRanges").Get<List<string>>();

            foreach (var ipRange in allowedIpRanges!)
            {
                if (IPNetwork.TryParse(ipRange, out var network))
                {
                    _allowedNetworks.Add(network);
                }
                else
                {
                    _logger.LogError($"Invalid IP range in configuration: {ipRange}");
                }
            }
        }

        public async Task Invoke(HttpContext context)
        {
            var remoteIp = context.Connection.RemoteIpAddress;
            bool allowed = false;

            foreach (var network in _allowedNetworks)
            {
                if (remoteIp == null) break;
                if (network.Contains(remoteIp))
                {
                    allowed = true;
                    break;
                }
            }

            if (!allowed)
            {
                _logger.LogWarning($"Forbidden Request from IP: {remoteIp}");
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }

            await _next(context);
        }
    }
}
