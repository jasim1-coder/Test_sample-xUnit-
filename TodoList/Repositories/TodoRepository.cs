using TodoList.Data;
using TodoList.Models;

namespace TodoList.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        private readonly AppDbContext _context;

        public TodoRepository(AppDbContext context)
        {

            _context = context;
        }

        public IEnumerable<TodoItems> GetAll()
        {
            return _context.TodoItems.ToList();
        }

        public TodoItems GetById(int id)
        {

            return _context.TodoItems.Find(id);
        }

        public void Add(TodoItems item)
        {
            _context.Add(item);
            _context.SaveChanges();
        }
    }
}
