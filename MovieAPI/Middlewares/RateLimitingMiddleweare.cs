namespace MovieAPI.Middlewares
{
    public class RateLimitingMiddleweare
    {
        private readonly RequestDelegate _next;
        
        private static readonly Dictionary<string, (int Count, DateTime Timestamp)> _requests = new();
        private const int _maxRequests = 1;
        private static readonly TimeSpan _interval = TimeSpan.FromMinutes(1);
        private static readonly object _lock = new();

        public RateLimitingMiddleweare(RequestDelegate next, ILogger<RateLimitingMiddleweare> logger)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            lock (_lock)
            {
                if (_requests.ContainsKey(ip))
                {
                    var (count, timestamp) = _requests[ip];
                    if ((DateTime.UtcNow - timestamp) < _interval)
                    {
                        if (count >= _maxRequests)
                        {
                            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                            context.Response.Headers["Retry-After"] = "60";
                            context.Response.ContentType = "text/plain";
                            context.Response.WriteAsync("Too many requests. Please try again later.").Wait();
                            return;
                        }
                        _requests[ip] = (count + 1, timestamp);
                    }
                    else
                    {
                        _requests[ip] = (1, DateTime.UtcNow);
                    }
                }
                else
                {
                    _requests[ip] = (1, DateTime.UtcNow);
                }
            }

            await _next(context);
        }
    }
}
