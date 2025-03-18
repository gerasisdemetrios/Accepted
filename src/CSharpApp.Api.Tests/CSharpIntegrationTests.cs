namespace CSharpApp.Api.Tests
{
    public class CSharpIntegrationTests : WebApplicationFactory<Program>
    {
        private HttpClient _client;

        [OneTimeSetUp]
        public void Setup()
        {
            var factory = new WebApplicationFactory<Program>();
            _client = factory.CreateClient();
        }

        [Test]
        public async Task GetProducts_ReturnsExpectedResult()
        {
            // Act
            var response = await _client.GetAsync("/api/v1/products");
            var content = await response.Content.ReadAsStringAsync();
            var res = JsonSerializer.Deserialize<List<Product>>(content) ?? new List<Product>();

            // Assert
            ClassicAssert.IsNotEmpty(res);
        }

        [Test]
        public async Task GetProduct_ReturnsExpectedResult()
        {
            // Act
            var response = await _client.GetAsync("/api/v1/products/1");
            var content = await response.Content.ReadAsStringAsync();
            var res = JsonSerializer.Deserialize<Product>(content);

            // Assert
            ClassicAssert.IsNotNull(res);
        }

        [Test]
        public async Task GetCategories_ReturnsExpectedResult()
        {
            // Act
            var response = await _client.GetAsync("/api/v1/categories");
            var content = await response.Content.ReadAsStringAsync();
            var res = JsonSerializer.Deserialize<List<Category>>(content) ?? new List<Category>();

            // Assert
            ClassicAssert.IsNotEmpty(res);
        }

        [Test]
        public async Task GetCategory_ReturnsExpectedResult()
        {
            // Act
            var response = await _client.GetAsync("/api/v1/categories/1");
            var content = await response.Content.ReadAsStringAsync();
            var res = JsonSerializer.Deserialize<Category>(content);

            // Assert
            ClassicAssert.IsNotNull(res);
        }
    }
}
