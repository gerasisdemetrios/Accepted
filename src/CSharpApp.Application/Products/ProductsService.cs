namespace CSharpApp.Application.Products;

public class ProductsService : IProductsService
{
    private readonly HttpClient _httpClient;
    private readonly RestApiSettings _restApiSettings;

    public ProductsService(IOptions<RestApiSettings> restApiSettings, HttpClient httpClient,
        ILogger<ProductsService> logger)
    {
        _restApiSettings = restApiSettings.Value;
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyCollection<Product>> GetProducts()
    {
        var response = await _httpClient.GetAsync(_restApiSettings.Products);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<List<Product>>(content)?? new List<Product>();
        return res.AsReadOnly();
    }

    public async Task<Product> GetProduct(int id)
    {
        var response = await _httpClient.GetAsync($"{_restApiSettings.Products}/{id}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<Product>(content);
        return res;
    }

    public async Task CreateProduct(Product product)
    {
        var jsonContent = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _httpClient.PostAsync($"{_restApiSettings.Products}", jsonContent);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateProduct(int id, Product product)
    {
        var jsonContent = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"{_restApiSettings.Products}/{id}", jsonContent);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteProduct(int id)
    {
        var response = await _httpClient.DeleteAsync($"{_restApiSettings.Products}/{id}");
        response.EnsureSuccessStatusCode();
    }
}