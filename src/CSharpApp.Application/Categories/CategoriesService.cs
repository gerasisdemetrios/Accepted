namespace CSharpApp.Application.Categories;

public class CategoriesService : ICategoriesService
{
    private readonly HttpClient _httpClient;
    private readonly RestApiSettings _restApiSettings;
    private readonly ILogger<CategoriesService> _logger;

    public CategoriesService(IOptions<RestApiSettings> restApiSettings, HttpClient httpClient,
        ILogger<CategoriesService> logger)
    {
        _restApiSettings = restApiSettings.Value;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyCollection<Category>> GetCategories()
    {
        var response = await _httpClient.GetAsync(_restApiSettings.Categories);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<List<Category>>(content)?? new List<Category>();
        
        return res.AsReadOnly();
    }

    public async Task<Category> GetCategory(int id)
    {
        var response = await _httpClient.GetAsync($"{_restApiSettings.Categories}/{id}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<Category>(content) ?? new Category();

        return res;
    }

    public async Task CreateCategory(Category category)
    {
        var jsonContent = new StringContent(JsonSerializer.Serialize(category), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _httpClient.PostAsync($"{_restApiSettings.Categories}", jsonContent);
        string responseBody = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateCategory(int id, Category category)
    {
        var jsonContent = new StringContent(JsonSerializer.Serialize(category), Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"{_restApiSettings.Categories}/{id}", jsonContent);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteCategory(int id)
    {
        var response = await _httpClient.DeleteAsync($"{_restApiSettings.Categories}/{id}");
        response.EnsureSuccessStatusCode();
    }
}