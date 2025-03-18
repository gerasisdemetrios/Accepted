namespace CSharpApp.Api.Middleware
{
    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestTimingMiddleware> _logger;

        public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            // Call the next middleware in the pipeline
            await _next(context);

            stopwatch.Stop();
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            // Log the request details
            _logger.LogInformation(
                "Request {method} {url} took {duration}ms",
                context.Request.Method,
                context.Request.Path,
                elapsedMilliseconds
            );

            Console.WriteLine($"Request {context.Request.Method} {context.Request.Path} took {elapsedMilliseconds}ms");
        }
    }
}
