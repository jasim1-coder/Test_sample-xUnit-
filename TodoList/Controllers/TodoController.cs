using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoList.Models;
using TodoList.Repositories;

namespace TodoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITodoRepository _todoRepository;

        public TodoController(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }
        [HttpGet]
        public IActionResult Get() => Ok(_todoRepository.GetAll());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _todoRepository.GetById(id);
            if (result == null) return  NotFound();
            return Ok(result);

        }

        [HttpPost]
        public IActionResult Add(TodoItems todoItems) {
            _todoRepository.Add(todoItems);
            return CreatedAtAction(nameof(Get), new { id = todoItems.Id }, todoItems);
        
        }
    }
}
