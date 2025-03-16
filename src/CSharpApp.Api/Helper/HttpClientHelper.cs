using Polly.Extensions.Http;
using Polly;
using CSharpApp.Core.Settings;
using Microsoft.Extensions.Options;

namespace CSharpApp.Api.Helper
{
    public static class HttpClientHelper
    {
        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount, int sleepDuration)
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
        public static void ConfigureHttpClient(IServiceProvider serviceProvider, HttpClient client)
        {
            var httpClientSettings = serviceProvider.GetRequiredService<IOptions<HttpClientSettings>>().Value;
            var restApiSettings = serviceProvider.GetRequiredService<IOptions<RestApiSettings>>().Value;

            client.BaseAddress = new Uri(restApiSettings.BaseUrl!);
        }

        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(IServiceProvider serviceProvider)
        {
            var settings = serviceProvider.GetRequiredService<IOptions<HttpClientSettings>>().Value;
            return GetRetryPolicy(settings.RetryCount, settings.SleepDuration);
        } 
    }
}
