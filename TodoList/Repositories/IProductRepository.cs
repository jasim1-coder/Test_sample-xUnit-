using TodoList.Dtos;
using TodoList.Models;
using TodoList.Result;

namespace TodoList.Repositories
{
    public interface IProductRepository
    {
        Task<Result<Product>> CreateProduct(Product product);
        Task<Result<ProductDto>> GetProductById(int id);
    }
}
