using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TodoList.Data;
using TodoList.Dtos;
using TodoList.Models;
using TodoList.Result;

namespace TodoList.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private AppDbContext _context;
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Result<Product>> CreateProduct(Product product)
        {
            try
            {
                if (string.IsNullOrEmpty(product.Name))
                {
                    return Result<Product>.Fail("Product Name not found");
                }

                var category = await _context.Categories.Include(c => c.ChildCategories).FirstOrDefaultAsync(C => C.Id == product.CategoryId);
                if (category == null)
                {
                    return Result<Product>.Fail("Category Not found");
                }
                if (category.ChildCategories != null && category.ChildCategories.Any())
                {
                    return Result<Product>.Fail("Product can be only added to the leaf node or the child categories");
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return Result<Product>.Success(product);

            }
            catch (Exception ex)
            {
                return Result<Product>.Fail("Excepiton occured", ex);
            }
        }
        private async Task<string> GetCategoryPathAsync(Category category)
        {
            var path = new List<string>();
            while (category != null)
            {
                path.Insert(0, category.Name);
                if (category.ParentCategoryId == null) break;
                category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == category.ParentCategoryId);
            }
            return string.Join(" > ", path);
        }



        public async Task<Result<ProductDto>> GetProductById(int id)
        {
            try
            {
                var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(P => P.Id == id);
                if (product == null) return Result<ProductDto>.Fail("Product Not exists");

                var path = await GetCategoryPathAsync(product.Category);

                var productDto = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    CategoryId = product.CategoryId,
                    CategoryPath = path
                };

                return Result<ProductDto>.Success(productDto);

            }
            catch (Exception ex)
            {
                return Result<ProductDto>.Fail($"{ex.Message}", ex);
            }

        }
    }
}