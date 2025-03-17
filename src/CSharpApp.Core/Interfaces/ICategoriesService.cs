namespace CSharpApp.Core.Interfaces;

public interface ICategoriesService
{
    Task<IReadOnlyCollection<Category>> GetCategories();

    Task<Category> GetCategory(int id);

    Task CreateCategory(Category category);

    Task UpdateCategory(int id, Category category);

    Task DeleteCategory(int id);
}