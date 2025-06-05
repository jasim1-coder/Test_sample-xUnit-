using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Data;
using TodoList.Models;
using TodoList.Repositories;

namespace TodoList.Tests
{
    public class TodoRepositoryTests
    {
        // Helper method to create an in-memory database context
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())  // In-memory DB
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public void Add_ShouldSaveTodoItem()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new TodoRepository(context);
            var item = new TodoItems { Title = "Test Task", isCompleted = false };

            // Act
            repository.Add(item);

            // Assert
            var saved = context.TodoItems.FirstOrDefault();
            Assert.NotNull(saved);
            Assert.Equal("Test Task", saved.Title);
        }

        [Fact]
        public void GetById_ShouldReturnCorrectItem()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var expectedItem = new TodoItems { Id = 1, Title = "Sample", isCompleted = false };
            context.TodoItems.Add(expectedItem);
            context.SaveChanges();

            var repository = new TodoRepository(context);

            // Act
            var result = repository.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Sample", result.Title);
            Assert.False(result.isCompleted);
        }

        [Fact]
        public async Task CreateCategories_ThrowsIfNameIsNull()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new CategoryRepository(context);

            var invalidCategory = new Category
            {
                Name = null,
                ParentCategoryId = null
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => repository.CreateCategories(invalidCategory));
        }
        [Fact]
        public async Task CreateCategories_ThrowsIfParentCategoryDoesNotExist()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new CategoryRepository(context);

            var category = new Category
            {
                Name = "Orphan Category",
                ParentCategoryId = 99 // does not exist
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => repository.CreateCategories(category));
        }
        [Fact]
        public async Task CreateCategories_SetsParentCategoryIdToNullIfZero()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new CategoryRepository(context);

            var category = new Category
            {
                Name = "Root Category",
                ParentCategoryId = 0 // should be converted to null
            };

            // Act
            await repository.CreateCategories(category);

            // Assert
            var createdCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Root Category");
            Assert.NotNull(createdCategory);
            Assert.Null(createdCategory.ParentCategoryId);
        }

    }
}
