using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TodoList.Data;
using TodoList.Models;
using TodoList.Repositories;

namespace TodoList.Tests
{
    public class ProductRepositoryTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var option = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            return new AppDbContext(option);
        }
        [Fact]
        public async Task GetProductById_ReturnOk_withProductDto()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            // Add Category
            var category = new Category { Id = 1, Name = "Fruits" };
            context.Categories.Add(category);

            // Add Product
            context.Products.Add(new Product { Id = 1, Name = "Orange", CategoryId = 1 });
            await context.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<ProductRepository>>();
            var repository = new ProductRepository(context, loggerMock.Object);

            // Act
            var result = await repository.GetProductById(1);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(1, result.Value.Id);
            Assert.Equal("Orange", result.Value.Name);
            Assert.Equal(1, result.Value.CategoryId);
            Assert.NotNull(result.Value.CategoryPath); // Optional: test expected path value if deterministic

        }


        [Fact]
        public async Task CreateProduct_Success_WhenCategoryIsLeaf()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            // Add a leaf category (no child categories)
            var leafCategory = new Category { Id = 2, Name = "LeafCategory" };
            context.Categories.Add(leafCategory);
            await context.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<ProductRepository>>();
            var repository = new ProductRepository(context, loggerMock.Object);

            var newProduct = new Product
            {
                Name = "Banana",
                CategoryId = 2
            };

            // Act
            var result = await repository.CreateProduct(newProduct);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("Banana", result.Value.Name);
            Assert.Equal(2, result.Value.CategoryId);

            // Verify it's in the database
            var created = await context.Products.FirstOrDefaultAsync(p => p.Name == "Banana");
            Assert.NotNull(created);
        }

    }
}
