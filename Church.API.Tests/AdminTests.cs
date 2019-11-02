using Church.API.Controllers;
using Church.API.Data.DBContext;
using Church.API.Models;
using Church.API.Models.AppModel.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Church.API.Tests
{
    [TestClass]
    public class AdminTests
    {
        public AdminTests()
        {

        }

        [TestMethod]
        public async Task AdminChangePassword_Success()
        {
            // Arrange
            var options = this.CreateInMemoryDatabase("Change_Password_Database");
            var expectedHashPassword = "4852a5de8b9eaaaf18c6932ff02ee99e7aa3c55d856bfc6b881468c7e3e0f8cc";
            
            // Act
            using (var _context = new IronChurchContext(options))
            {
                var adminController = new AdminController(_context);

                var result = await adminController.AdminChangeUserPassword(new ChangePassword
                {
                    Email = "msamuelhenry@gmail.com",
                    NewPassword = "abc123$$",
                    ConfirmNewPassword = "abc123$$"
                });

                // Assert
                var actualHashPassword = _context.Users
                    .Where(x => x.UserId == 1)
                    .FirstOrDefaultAsync()
                    .Result
                    .Password;

                Assert.AreEqual(204, ((Microsoft.AspNetCore.Mvc.NoContentResult)result).StatusCode);
                Assert.AreEqual(expectedHashPassword, actualHashPassword);
            }
        }

        [TestMethod]
        public async Task AdminChangePassword_UserInvalid()
        {
            // Arrange
            var options = this.CreateInMemoryDatabase("Change_Password_InvalidUser_Database");
            
            // Act
            using (var _context = new IronChurchContext(options))
            {
                var adminController = new AdminController(_context);

                var result = await adminController.AdminChangeUserPassword(new ChangePassword
                {
                    Email = "msamuelhenry1@gmail.com",
                    NewPassword = "abc123$$",
                    ConfirmNewPassword = "abc123$$"
                });

                // Assert
                Assert.AreEqual(400, ((Microsoft.AspNetCore.Mvc.BadRequestObjectResult)result).StatusCode);
                Assert.AreEqual("Invalid User/does not exist", ((Microsoft.AspNetCore.Mvc.BadRequestObjectResult)result).Value);
            }
        }

        [TestMethod]
        public async Task AdminChangePassword_NewConfirmNoMatch()
        {
            // Arrange
            var options = this.CreateInMemoryDatabase("Change_Password_NewConfirm_Database");

            // Act
            using (var _context = new IronChurchContext(options))
            {
                var adminController = new AdminController(_context);

                var result = await adminController.AdminChangeUserPassword(new ChangePassword
                {
                    Email = "msamuelhenry@gmail.com",
                    NewPassword = "abc123$$",
                    ConfirmNewPassword = "abc12377$$"
                });

                // Assert
                Assert.AreEqual(400, ((Microsoft.AspNetCore.Mvc.BadRequestObjectResult)result).StatusCode);
                Assert.AreEqual("New password and Confirm password don't match", ((Microsoft.AspNetCore.Mvc.BadRequestObjectResult)result).Value);
            }
        }
        private DbContextOptions<IronChurchContext> CreateInMemoryDatabase(string databaseName)
        {
            var options = new DbContextOptionsBuilder<IronChurchContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            var _context = new IronChurchContext(options);

            _context.Users.Add(new Users { UserId = 1, FirstName = "Samuel", LastName = "Mathe", Email = "msamuelhenry@gmail.com", Status = "A", DateAdded = DateTime.Now, UserAdded = "msamuelhenry@gmail.com" });
            
            _context.SaveChanges();

            return options;
        }
    }
}
