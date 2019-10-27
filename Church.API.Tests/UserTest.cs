using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Church.API.Data.DBContext;
using Church.API.Models;
using Church.API.Controllers;
using Church.API.Models.AppModel.Response.GetUserResponse;
using System.Linq;

namespace Church.API.Tests
{
    [TestClass]
    public class UserTest
    {
        public UserTest()
        {

        }

        [TestMethod]
        public async Task GetUserDataTest()
        {
            // Arrange
            var options = this.CreateInMemoryDatabase("Get_User_Database");
            UserResponse response = new UserResponse();

            // Act
            using (var _context = new IronChurchContext(options))
            {
                var userController = new UsersController(_context);

                response = await userController.Get(1);
            }

            // Assert
            Assert.IsTrue(response.EmailId.Equals("msamuelhenry@gmail.com"));
            Assert.IsTrue(response.UserOrganization.Where(x => x.UserOrganizationId == 2).FirstOrDefault().OrganizationName == "Test Organization");
            Assert.IsTrue(response.UserRole.Where(x => x.UserRoleId == 1).FirstOrDefault().RoleName == "SystemAdmin");
        }

        private DbContextOptions<IronChurchContext> CreateInMemoryDatabase(string databaseName)
        {
            var options = new DbContextOptionsBuilder<IronChurchContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            var _context = new IronChurchContext(options);

            _context.Users.Add(new Users { UserId = 1, FirstName = "Samuel", LastName = "Mathe", Email = "msamuelhenry@gmail.com", Status = "A", DateAdded = DateTime.Now, UserAdded = "msamuelhenry@gmail.com" });
            _context.Organization.Add(new Organization { OrganizationId = 1, Website = "test.com", Email = "info@test.com", Name = "Test Organization", Industry = "tech", AddressLine1 = "street 1", City = "Boca", State = "FL", Zip = "33433", Phone = "9999999999" });
            _context.UserOrganization.Add(new UserOrganization { UserOrganizationId = 2, OrganizationId = 1, UserId = 1, UserAdded = "msamuelhenry@gmail.com", DateAdded = DateTime.Now });
            _context.Role.Add(new Role { RoleId = 1, RoleName = "SystemAdmin", UserAdded = "msamuelhenry@gmail.com", DateAdded = DateTime.Now });
            _context.UserRole.Add(new UserRole { UserRoleId = 1, UserId = 1, RoleId = 1, UserAdded = "msamuelhenry@gmail.com", DateAdded = DateTime.Now });

            _context.SaveChanges();

            return options;
        }
    }
}
