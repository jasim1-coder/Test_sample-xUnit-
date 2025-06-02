using TodoList.Models;

namespace TodoList.Repositories
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetCategories(int? ParentId);

        Task CreateCategories(Category category);

        Task<Category?> GetCategoryById(int Id);
    }
}
