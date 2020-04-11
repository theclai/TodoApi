using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using Moq;
using TodoApi;
using TodoApi.Controllers;
using TodoApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace TodoApi.Test
{
    public class Tests
    {
        private TodoContext todoContext;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TodoContext>()
          .UseInMemoryDatabase(databaseName: "TodoList")
          .Options;

            todoContext = new TodoContext(options);
            todoContext.TodoItems.Add(new TodoItem { Id = 1, Title = "Jogging", CreatedDate = DateTime.Now, Description = "Sport", TimeOfExpiry = DateTime.Now.AddDays(3), IsComplete = false, PercentComplete = 100 });
            todoContext.TodoItems.Add(new TodoItem { Id = 2, Title = "Cleaning house", CreatedDate = DateTime.Now, Description = "Cleaning", TimeOfExpiry = DateTime.Now.AddDays(4), IsComplete = true, PercentComplete = 30 });
            todoContext.TodoItems.Add(new TodoItem { Id = 3, Title = "Ride Bike", CreatedDate = DateTime.Now, Description = "Sport", TimeOfExpiry = DateTime.Now.AddDays(5), IsComplete = true, PercentComplete = 50 });
            todoContext.SaveChanges();

        }

        [Test]
        public void GetAll_NoParameters_ReturnsData()
        {
            TodoController todoController = new TodoController(todoContext);
            var expectedResult = 3;
            var actrualResult = todoController.GetAll().ToList().Count;

            Assert.AreEqual(expectedResult, actrualResult);
            
        }

        [Test]
        public void GetAll_ExistingIdPassed_ReturnsData()
        {
            TodoController todoController = new TodoController(todoContext);
    
            var result = todoController.GetById(1);
            var okResult = result as ObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }
    }
}