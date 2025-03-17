namespace CSharpApp.Application.Communication
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly HttpClient _httpClient;
        private readonly RestApiSettings _settings;
        private string? _accessToken;
        private string? _refreshToken;

        public JwtTokenService(HttpClient httpClient, IOptions<RestApiSettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;
        }

        public async Task<string> GetTokenAsync()
        {
            // Return cached token if still valid
            if (!string.IsNullOrEmpty(_accessToken))
            {
                return _accessToken;
            }

            // Try to refresh the token if a refresh token exists
            if (!string.IsNullOrEmpty(_refreshToken))
            {
                return await RefreshTokenAsync();
            }

            // Otherwise, fetch a new token
            return await FetchNewTokenAsync();
        }

        private async Task<string> FetchNewTokenAsync()
        {
            var requestBody = new AuthRequest
            {
                Password = _settings.Password,
                Email = _settings.Email,
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_settings.Auth, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to fetch JWT token");
            }

            await ParseTokenResponse(response);
            return _accessToken!;
        }

        public async Task<string> RefreshTokenAsync()
        {
            var requestBody = new AuthRequest
            {
                Password = _settings.Password,
                Email = _settings.Email,
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_settings.Auth, content);

            if (!response.IsSuccessStatusCode)
            {
                return await FetchNewTokenAsync();
            }

            await ParseTokenResponse(response);
            return _accessToken!;
        }

        private async Task ParseTokenResponse(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var res = JsonSerializer.Deserialize<AuthResponse>(content);

            _accessToken = res?.AccessToken;
            _refreshToken = res?.RefreshToken;
        }
    }
}
