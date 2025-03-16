namespace CSharpApp.Application.Products;

public class ProductsService : IProductsService
{
    private readonly HttpClient _httpClient;
    private readonly RestApiSettings _restApiSettings;
    private readonly ILogger<ProductsService> _logger;

    public ProductsService(IOptions<RestApiSettings> restApiSettings, IHttpClientFactory httpClientFactory,
        ILogger<ProductsService> logger)
    {
        _restApiSettings = restApiSettings.Value;
        _logger = logger;

        _httpClient = httpClientFactory.CreateClient("HTTPClient");
        _httpClient.BaseAddress = new Uri(_restApiSettings.BaseUrl!);
    }

    public async Task<IReadOnlyCollection<Product>> GetProducts()
    {
        var response = await _httpClient.GetAsync(_restApiSettings.Products);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<List<Product>>(content);
        
        return res.AsReadOnly();
    }
}