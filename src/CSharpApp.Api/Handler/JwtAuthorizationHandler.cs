namespace CSharpApp.Api.Handler
{
    public class JwtAuthorizationHandler : DelegatingHandler
    {
 
        private readonly IJwtTokenService _tokenProvider;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _policy;

        public JwtAuthorizationHandler(IJwtTokenService jwtTokenService)
        {
            _tokenProvider = jwtTokenService;
            _policy = Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
                .RetryAsync((_, _) => jwtTokenService.RefreshTokenAsync());
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => await _policy.ExecuteAsync(async () =>
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _tokenProvider.GetTokenAsync());
                return await base.SendAsync(request, cancellationToken);
            });
    }
}
