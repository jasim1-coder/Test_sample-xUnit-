using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TodoList.Data;
using TodoList.Models;

namespace TodoList.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }
      public  async Task<List<Category>> GetCategories(int? ParentId)
            {
            var categories = await _context.Categories.Where(C => C.ParentCategoryId == ParentId).ToListAsync();
            return categories;

            }

        public async Task CreateCategories(Category category)
        {
            if (string.IsNullOrEmpty(category.Name)) 
            
                throw new ArgumentException("Category name is required.");
            

            if(category.ParentCategoryId == 0)
                category.ParentCategoryId = null;
            
            if (category.ParentCategoryId != null)
            {
                var parentCategory = await _context.Categories.AnyAsync(c => c.Id == category.ParentCategoryId);
                if (!parentCategory)
                {
                    throw new  InvalidOperationException("Parent category does not exists");
                }
            }
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task<Category?> GetCategoryById(int Id)
        {
            var category = await _context.Categories.FindAsync(Id);
            return category;
        }


    }
}

