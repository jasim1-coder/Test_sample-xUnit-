using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Repositories;
using Moq;
using TodoList.Controllers;
using TodoList.Models;
using Microsoft.AspNetCore.Mvc;

namespace TodoList.Tests
{
    public class CategoriesControllerTests
    {
        private readonly Mock<ICategoryRepository> _mockRepo;
        private readonly CategoriesController _controller;

        public CategoriesControllerTests()
        {
            _mockRepo = new Mock<ICategoryRepository>();
            _controller = new CategoriesController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetCategories_ReturnsOk_WithListOfCategories()
        {   //arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "WORK " },
                new Category { Id = 2, Name = "Personal " },
                new Category{Id = 3 , Name =   "Professional" },
                new Category {Id = 4, Name = "Socail"}
            };

            _mockRepo.Setup(repo => repo.GetCategories(null))
                .ReturnsAsync(categories);
            //act
            var result = await _controller.GetCategories(null);

            //assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returedCategories = Assert.IsAssignableFrom<List<Category>>(okResult.Value);
            Assert.Equal(2, returedCategories.Count);
        }
    }
}
