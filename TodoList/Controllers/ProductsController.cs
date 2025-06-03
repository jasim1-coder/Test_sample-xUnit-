using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.Data;
using TodoList.Dtos;
using TodoList.Models;
using TodoList.Repositories;

namespace TodoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IProductRepository _repository;
        
        public ProductsController(AppDbContext context, IProductRepository repository)
        {
            _context = context;
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {

            var result = await _repository.CreateProduct(product);
            if (!result.IsSuccess) 
            {
                return BadRequest(result.ErrorMessage);
            }

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }


        


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
           var result = await _repository.GetProductById(id);
            if (!result.IsSuccess) 
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Value);
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
