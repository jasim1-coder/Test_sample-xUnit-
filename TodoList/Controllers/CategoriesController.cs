using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.Data;
using TodoList.Models;
using TodoList.Repositories;

namespace TodoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // GET api/categories?parentId=5
        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] int? parentId)
        {
            var categories = await _categoryRepository.GetCategories(parentId);
            return Ok(categories);

        }

        [HttpPost("Create categories")]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            try
            {
                await _categoryRepository.CreateCategories(category);
                return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);

            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e) 
            {
                return BadRequest(e.Message);
            }

        }

        // GET api/categories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryRepository.GetCategoryById(id);

            if (category == null) return NotFound();

            return Ok(category);
        }
    }
}