using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.Data;
using TodoList.Models;

namespace TodoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/categories?parentId=5
        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] int? parentId)
        {
            var categories = await _context.Categories
                .Where(c => c.ParentCategoryId == parentId)
                .ToListAsync();

            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                return BadRequest("Category name is required.");

            if (category.ParentCategoryId == 0)
                category.ParentCategoryId = null;

            // Optional: Validate ParentCategoryId if not null exists in DB
            if (category.ParentCategoryId != null)
            {
                var parentExists = await _context.Categories.AnyAsync(c => c.Id == category.ParentCategoryId);
                if (!parentExists)
                    return BadRequest("Parent category does not exist.");
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
        }



        // GET api/categories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _context.Categories
                .Include(c => c.ChildCategories)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();

            return Ok(category);
        }
    }
}