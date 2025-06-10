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
        private readonly ILogger<ProductRepository> _logger;
        public ProductRepository(AppDbContext context, ILogger<ProductRepository> logger)
        {
            _context = context;
            _logger = logger;
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



        public async Task<Result<ProductDto>> GetProductById(int? id)
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


        public async Task<List<int>> GetAllNestedCategoryIds(int categoryid)
        {
            var result = new List<int>() { categoryid };
            var queue = new Queue<int>();
            queue.Enqueue(categoryid);
            while (queue.Count > 0)
            {
                var currentId = queue.Dequeue();

                var categoryids = await _context.Categories.Where(c => c.ParentCategoryId == currentId).Select(c => c.Id).ToListAsync();

                foreach(var child in categoryids)
                {
                    result.Add(child);
                    queue.Enqueue(child);
                }
            }
            return result;
        }

        public async Task<Result<List<ProductDto>>> GetProductsInCategory(int categoryId)
        {
            try
            {
                if (categoryId <= 0)
                {
                    _logger.LogWarning("Invalid category ID received: {CategoryId}", categoryId);
                    return Result<List<ProductDto>>.Fail("Invalid category ID.");
                }

                _logger.LogInformation("Fetching products for categoryId: {CategoryId}", categoryId);

                var allCategoryIds = await GetAllNestedCategoryIds(categoryId);
                _logger.LogDebug("Resolved nested category IDs: {NestedCategoryIds}", string.Join(", ", allCategoryIds));

                var products = await _context.Products
                                     .Where(p => allCategoryIds.Contains(p.CategoryId))
                                     .Include(p => p.Category)
                                     .ToListAsync();

                _logger.LogInformation("Fetched {ProductCount} products for categoryId: {CategoryId}", products.Count, categoryId);

                var result = new List<ProductDto>();
                foreach (var product in products)
                {
                    var path = await GetCategoryPathAsync(product.Category);

                    _logger.LogDebug("Mapped product {ProductId} - '{ProductName}' with category path: {CategoryPath}",
                                     product.Id, product.Name, path);

                    result.Add(new ProductDto
                    {
                        Id = product.Id,
                        Name = product.Name,
                        CategoryId = product.CategoryId,
                        CategoryPath = path,
                    });
                }

                _logger.LogInformation("Successfully created product DTO list for categoryId: {CategoryId}", categoryId);

                return Result<List<ProductDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching products for categoryId: {CategoryId}", categoryId);
                return Result<List<ProductDto>>.Fail("Exception occurred while fetching products.", ex);
            }
        }


    }
}