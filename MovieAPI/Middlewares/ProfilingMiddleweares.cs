using System.Diagnostics;

namespace MovieAPI.Middlewares
{
    public class ProfilingMiddleweares
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ProfilingMiddleweares> _logger;
        public ProfilingMiddleweares(RequestDelegate next, ILogger<ProfilingMiddleweares> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var watch = Stopwatch.StartNew();
            await _next(context);
            watch.Stop();
            var elapsedMilliseconds = watch.ElapsedMilliseconds;
            _logger.LogInformation($"Request {context.Request.Method} {context.Request.Path} took {elapsedMilliseconds} ms");
        }
    }
}
