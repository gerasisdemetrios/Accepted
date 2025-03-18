namespace CSharpApp.Api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (HttpRequestException httpEx)
            {
                await HandleHttpRequestExceptionAsync(context, httpEx);
            }
            catch (Exception ex)
            {
                await HandleGeneralExceptionAsync(context, ex);
            }
        }

        private Task HandleHttpRequestExceptionAsync(HttpContext context, HttpRequestException exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadGateway;
            context.Response.ContentType = "application/json";

            var errorMessage = $"Error making HTTP request: {exception.Message}";

            return context.Response.WriteAsJsonAsync(new { error = errorMessage });
        }

        private Task HandleGeneralExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var errorMessage = $"An unexpected error occurred: {exception.Message}";

            return context.Response.WriteAsJsonAsync(new { error = errorMessage });
        }
    }
}
