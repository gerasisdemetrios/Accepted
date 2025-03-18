using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace CSharpApp.Api.Tests
{
    public class CSharpAppIntegrationTests: IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CSharpAppIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetProducts_ReturnsExpectedResult()
        {
            // Act
            var response = await _client.GetAsync("/products");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal("Sunny", result);
        }
    }
}
