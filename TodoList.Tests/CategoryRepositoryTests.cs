﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Data;
using TodoList.Models;
using TodoList.Repositories;
using Xunit;

namespace TodoList.Tests
{
    public class CategoryRepositoryTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var option = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(option);
        }



        [Fact]
        public async Task GetCategories_ReturnsCategoriesWithGivenParentId()
        {
            //Arrange
            var context = GetInMemoryDbContext();

            context.Categories.Add(new Category { Id = 1, Name = "Parent", ParentCategoryId = null });
            context.Categories.Add(new Category { Id = 2, Name = "Child A", ParentCategoryId = 1 });
            context.Categories.Add(new Category { Id = 3, Name = "Child B", ParentCategoryId = 1 });
            context.Categories.Add(new Category { Id = 4, Name = "Unrelated", ParentCategoryId = null });

            await context.SaveChangesAsync();

            var repository = new CategoryRepository(context);
            //Act
            var result = await repository.GetCategories(1);
            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, c => Assert.Equal(1, c.ParentCategoryId));

        }



        [Fact]
        public  async Task GetCategoryById_ReturnsCorrectCategory()
        {
            //Arrange
            var context = GetInMemoryDbContext();

            context.Categories.Add(new Category { Id = 1, Name = "Test Category", ParentCategoryId = null });
            context.Categories.Add(new Category { Id = 2, Name = "Another Category", ParentCategoryId = 1 });
            await context.SaveChangesAsync();
            var repository = new CategoryRepository(context);
            //Act
            var result = await repository.GetCategoryById(1);
            //Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test Category", result.Name);

        }

        [Fact]
        public async Task CreateCategories_AddsValidCategory()
        {
            //Arrange
            var context = GetInMemoryDbContext();
            context.Categories.Add(new Category { Id = 1, Name = "Parent Category" });
            await context.SaveChangesAsync();
            var repository = new CategoryRepository(context);
            var newCategory = new Category { Name = "Child Category", ParentCategoryId = 1 };
            //Act
            await repository.CreateCategories(newCategory);
            //Assert
            var savedCategory = await context.Categories.FirstOrDefaultAsync(C => C.Name == "Child Category");
            Assert.NotNull(savedCategory);
            Assert.Equal("Child Category", savedCategory.Name);
            Assert.Equal(1, savedCategory.ParentCategoryId);
        }


    }
}