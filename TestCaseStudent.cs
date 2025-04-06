using ASM_SIMS.Controllers;
using ASM_SIMS.DB;
using ASM_SIMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ASM_SIMS.Tests
{
    public class StudentControllerTests
    {
        private readonly SimsDataContext _dbContext;

        // Constructor tạo InMemory DB cho mỗi test
        public StudentControllerTests()
        {
    
            var options = new DbContextOptionsBuilder<SimsDataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new SimsDataContext(options); 

            SeedData();
        }

        private void SeedData()
        {
            _dbContext.Students.AddRange(
                new Student { Id = 1, AccountId = 1, FullName = "Nguyen Van A", Email = "a@example.com", Phone = "123456789", Address = "Hanoi", Status = "Active", CreatedAt = DateTime.Now },
                new Student { Id = 2, AccountId = 1, FullName = "Tran Thi B", Email = "b@example.com", Phone = "987654321", Address = "HCM", Status = "Active", CreatedAt = DateTime.Now }
            );
            _dbContext.SaveChanges(); 
        }

        [Fact]
        public void Create_Get_ReturnsViewResult_WithEmptyModel()
        {
            var controller = new StudentController(_dbContext);

            var result = controller.Create();

            var viewResult = Assert.IsType<ViewResult>(result); 
            var model = Assert.IsType<StudentViewModel>(viewResult.Model); 
            Assert.Null(model.FullName); 
        }

        [Fact]
        public void Create_Post_InvalidModel_ReturnsViewWithModel()
        {
           
            var controller = new StudentController(_dbContext);
            var model = new StudentViewModel { Email = "invalid", FullName = "Duong Quoc Anh", Phone = "999999999" }; 
            controller.ModelState.AddModelError("FullName", "Họ tên là bắt buộc"); 

            var result = controller.Create(model);

            var viewResult = Assert.IsType<ViewResult>(result); 
            var returnedModel = Assert.IsType<StudentViewModel>(viewResult.Model); 
            Assert.Equal(model.Email, returnedModel.Email); 
            Assert.False(controller.ModelState.IsValid); 
        }

        // Test kiểm tra phương thức Edit (GET) với ID hợp lệ
        [Fact]
        public void Edit_Get_ExistingId_ReturnsViewWithModel()
        {
            
            var controller = new StudentController(_dbContext);

            
            var result = controller.Edit(1);

            
            var viewResult = Assert.IsType<ViewResult>(result); 
            var model = Assert.IsType<StudentViewModel>(viewResult.Model);
            Assert.Equal("Nguyen Van A", model.FullName);
            Assert.Equal(1, model.Id); 
        }

        // Test kiểm tra phương thức Edit (GET) với ID không tồn tại
        [Fact]
        public void Edit_Get_NonExistingId_ReturnsNotFound()
        {
            
            var controller = new StudentController(_dbContext);

            
            var result = controller.Edit(999);

            
            Assert.IsType<NotFoundResult>(result); 
        }

        // Test kiểm tra phương thức Edit (POST) khi model không hợp lệ
        [Fact]
        public void Edit_Post_InvalidModel_ReturnsViewWithModel()
        {
            
            var controller = new StudentController(_dbContext);
            var model = new StudentViewModel { Id = 1, Email = "invalid" }; 
            controller.ModelState.AddModelError("FullName", "Họ tên là bắt buộc"); 

            var result = controller.Edit(model);

            var viewResult = Assert.IsType<ViewResult>(result); 
            var returnedModel = Assert.IsType<StudentViewModel>(viewResult.Model); 
            Assert.Equal(1, returnedModel.Id); 
            Assert.False(controller.ModelState.IsValid); 
        }

        // Test kiểm tra phương thức Delete (POST) với ID không tồn tại
        [Fact]
        public void Delete_Post_NonExistingId_ReturnsNotFound()
        {
            
            var controller = new StudentController(_dbContext);

            
            var result = controller.Delete(999);

            
            Assert.IsType<NotFoundResult>(result); 
        }
    }
}