using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using TodoList.Controllers;
using TodoList.Controllers;
using TodoList.Dtos;
using TodoList.Models;
using TodoList.Models;
using TodoList.Repositories;
using TodoList.Repositories;
using TodoList.Result;
using Xunit;

namespace TodoList.Tests
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductRepository> _mockRepo;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockRepo = new Mock<IProductRepository>();
            _controller = new ProductsController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetProducts_ReturnsOk_WithProductDto()
        {
            //Arrange
            var productDto = new ProductDto
            {
                Id = 1,
                Name = "Apple",
                CategoryId = 2,
                CategoryPath = "Fruits > Apples"
            };

            var result = Result<ProductDto>.Success(productDto);


            _mockRepo.Setup(repo => repo.GetProductById(1)).ReturnsAsync(result);
            //Act
            var actionResult = await _controller.GetProductById(1);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
            Assert.Equal(1, returnedProduct.Id);
            Assert.Equal("Apple", returnedProduct.Name);

        } 

     }
}

