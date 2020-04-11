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
using System.Net;

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
            todoContext.TodoItems.Add(new TodoItem { Id = 3, Title = "Ride Bike", CreatedDate = DateTime.Now.AddDays(1), Description = "Sport", TimeOfExpiry = DateTime.Now.AddDays(5), IsComplete = true, PercentComplete = 50 });
            todoContext.TodoItems.Add(new TodoItem { Id = 4, Title = "Work in project A", CreatedDate = DateTime.Now, Description = "Work", TimeOfExpiry = DateTime.Now.AddDays(30), IsComplete = false, PercentComplete = 1 });
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
            var objTodo = (TodoItem)okResult.Value;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(objTodo.Id, 1);
        }

        [Test]
        public void GetIncomingTodoForToday_ExistingDatetimePassed_ReturnsData()
        {
            TodoController todoController = new TodoController(todoContext);

            var param = DateTime.Now;
            var result = todoController.GetIncomingTodoForToday(param);
            var okResult = result as ObjectResult;
            var objTodo = (TodoItem) okResult.Value;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(objTodo.CreatedDate.Day, param.Day);
        }

        [Test]
        public void GetIncomingTodoForNextDay_ExistingNextDatetimePassed_ReturnsData()
        {
            TodoController todoController = new TodoController(todoContext);

            var param = 1;
            var expectedResult = DateTime.Now.AddDays(1);
            var result = todoController.GetIncomingTodoForNextDay(param);
            var okResult = result as ObjectResult;
            var objTodo = okResult.Value as TodoItem;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(objTodo.CreatedDate.Date, expectedResult.Date);
        }

        [Test]
        public void GetIncomingTodoForCurrenWeek_ExistingWeekDatetimePassed_ReturnsData()
        {
            TodoController todoController = new TodoController(todoContext);

            var param = DateTime.Now;
            var result = todoController.GetIncomingTodoForCurrenWeek(param);
            var okResult = result as ObjectResult;
            var objTodo = (TodoItem) okResult.Value;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(objTodo.CreatedDate.Date, param.Date);
        }

        [Test]
        public void Create_ExistingTodoItemPassed_ReturnCreated()
        {
            TodoController todoController = new TodoController(todoContext);

            var param = new TodoItem()
            {
                Id = 5,
                CreatedDate = DateTime.Now,
                Title = "Create unit test for todo",
                Description = "Coding",
                IsComplete = false,
                PercentComplete = 70,
                TimeOfExpiry = DateTime.Now.AddDays(1)
            };

            var result = todoController.Create(param);
            var okResult = result as ObjectResult;
            var objTodo = (TodoItem)okResult.Value;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(objTodo.CreatedDate.Date, param.CreatedDate.Date);
        }

        [Test]
        public void Update_ExistingTodoItemUpdate_Return()
        {
            TodoController todoController = new TodoController(todoContext);

            var paramCreate = new TodoItem()
            {
                Id = 6,
                CreatedDate = DateTime.Now,
                Title = "Create unit test for todo",
                Description = "Coding",
                IsComplete = false,
                PercentComplete = 70,
                TimeOfExpiry = DateTime.Now.AddDays(1)
            };

            todoController.Create(paramCreate);

            paramCreate.PercentComplete = 80;

            todoController.Update(paramCreate.Id, paramCreate);

            var result = todoController.GetById(paramCreate.Id);
            var okResult = result as ObjectResult;
            var objTodo = (TodoItem)okResult.Value;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(objTodo.PercentComplete, paramCreate.PercentComplete);
        }

        [Test]
        public void SetTodoPercent_ExistingPercent_Return()
        {
            TodoController todoController = new TodoController(todoContext);

            var paramCreate = new TodoItem()
            {
                Id = 6,
                CreatedDate = DateTime.Now,
                Title = "Create unit test for todo",
                Description = "Coding",
                IsComplete = false,
                PercentComplete = 70,
                TimeOfExpiry = DateTime.Now.AddDays(1)
            };

            todoController.Create(paramCreate);

            float percent = 90;

            todoController.SetTodoPercent(paramCreate.Id, percent);

            var result = todoController.GetById(paramCreate.Id);
            var okResult = result as ObjectResult;
            var objTodo = (TodoItem)okResult.Value;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(objTodo.PercentComplete, paramCreate.PercentComplete);
        }

        [Test]
        public void MarkTodo_SetIsTodo_Return()
        {
            TodoController todoController = new TodoController(todoContext);

            var paramCreate = new TodoItem()
            {
                Id = 6,
                CreatedDate = DateTime.Now,
                Title = "Create unit test for todo",
                Description = "Coding",
                IsComplete = false,
                PercentComplete = 70,
                TimeOfExpiry = DateTime.Now.AddDays(1)
            };

            todoController.Create(paramCreate);

            bool isTodo = true;

            todoController.MarkTodo(paramCreate.Id, isTodo);

            var result = todoController.GetById(paramCreate.Id);
            var okResult = result as ObjectResult;
            var objTodo = (TodoItem)okResult.Value;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(objTodo.IsTodo, paramCreate.IsTodo);
        }

        [Test]
        public void Delete_ExistingId_Return()
        {
            TodoController todoController = new TodoController(todoContext);

            var paramCreate = new TodoItem()
            {
                Id = 6,
                CreatedDate = DateTime.Now,
                Title = "Create unit test for todo",
                Description = "Coding",
                IsComplete = false,
                PercentComplete = 70,
                TimeOfExpiry = DateTime.Now.AddDays(1)
            };

            todoController.Create(paramCreate);
            todoController.Delete(paramCreate.Id);

            var result = todoController.GetById(paramCreate.Id);
            var expectedResult = result as NotFoundObjectResult;

            Assert.IsNull(expectedResult);
        }
    }
}