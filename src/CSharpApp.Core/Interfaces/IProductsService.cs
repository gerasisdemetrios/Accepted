namespace CSharpApp.Core.Interfaces;

public interface IProductsService
{
    Task<IReadOnlyCollection<Product>> GetProducts();

    Task<Product> GetProduct(int id);

    Task CreateProduct(Product product);

    Task UpdateProduct(int id, Product product);

    Task DeleteProduct(int id);
}