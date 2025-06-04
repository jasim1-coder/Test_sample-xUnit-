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

        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var result = await _repository.GetProductsInCategory(categoryId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Value);
        }

    }
}
