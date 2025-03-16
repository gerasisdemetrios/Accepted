using Polly.Extensions.Http;
using Polly;

namespace CSharpApp.Api.Helper
{
    public static class HttpClientHelper
    {
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount, int sleepDuration)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError() // Handles 5xx, 408, and network failures
                .WaitAndRetryAsync(
                    retryCount: retryCount, 
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(sleepDuration), // 2s, 4s, 6s sleep
                    onRetry: (response, delay, retryAttempt, context) =>
                    {
                        Console.WriteLine($"Retry {retryAttempt} after {delay.TotalSeconds} seconds");
                    });
        }
    }
}
