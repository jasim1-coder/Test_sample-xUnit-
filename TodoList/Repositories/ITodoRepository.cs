using TodoList.Models;

namespace TodoList.Repositories
{
    public interface ITodoRepository
    {
        IEnumerable<TodoItems> GetAll();

        void Add(TodoItems item);

        TodoItems GetById(int id);

    }
}
