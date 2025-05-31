using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.Data;
using TodoList.Dtos;
using TodoList.Models;

namespace TodoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
                return BadRequest("Product name is required.");

            //check if the category exists
            var category = await _context.Categories
                            .Include(c => c.ChildCategories)
                            .FirstOrDefaultAsync(c => c.Id == product.CategoryId);

            if (category == null)
                return BadRequest("Category does not exist.");

            //check if its a leaf category 
            if (category.ChildCategories != null && category.ChildCategories.Any())
                return BadRequest("Product can only be added to a leaf category with no subcatgories ");

                

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }


        [HttpPost]
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            var categoryPath = await GetCategoryPathAsync(product.Category);

            var dto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                CategoryId = product.CategoryId,
                CategoryPath = categoryPath
            };

            return Ok(dto);
        }

        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {

            // Get all nested category IDs including this one
            var allCategoryIds = await GetAllNestedCategoryIds(categoryId);

            var products = await _context.Products
                .Where(p => allCategoryIds.Contains(p.CategoryId))
                .Include(p => p.Category)
                .ToListAsync();

            var result = new List<ProductDto>();
            foreach (var product in products)
            {
                var path = await GetCategoryPathAsync(product.Category);
                result.Add(new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    CategoryId = product.CategoryId,
                    CategoryPath = path
                });
            }

            return Ok(result);
        }






        private async Task<List<int>> GetAllNestedCategoryIds(int parentId)
        {
            var result = new List<int> { parentId };
            var queue = new Queue<int>();
            queue.Enqueue(parentId);

            while (queue.Count > 0)
            {
                var currentId = queue.Dequeue();
                var children = await _context.Categories
                    .Where(c => c.ParentCategoryId == currentId)
                    .Select(c => c.Id)
                    .ToListAsync();

                foreach (var childId in children)
                {
                    result.Add(childId);
                    queue.Enqueue(childId);
                }
            }

            return result;
        }




    }
}
