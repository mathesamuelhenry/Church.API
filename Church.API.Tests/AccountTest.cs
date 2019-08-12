using Church.API.Tests.Base;
using Church.API.Controllers;
using Church.API.Data.DBContext;
using Church.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Church.API.Tests
{
    [TestClass]
    public class AccountTest : TestBase
    {
        public AccountTest() 
            : base()
        { }

        [TestMethod]
        public async Task Get()
        {
            var options = this.CreateInMemoryDatabase("Get_Account_Database");

            using (var _context = new IronChurchContext(options))
            {
                AccountsController accountController = new AccountsController(_context);

                var result = await accountController.Get();

                Assert.IsTrue(result.Count == 1);
            }
        }

        [TestMethod]
        public async Task GetAccountByIdTest()
        {
            var options = this.CreateInMemoryDatabase("Get_Account_By_Id_database");

            using (var _context = new IronChurchContext(options))
            {
                AccountsController accountController = new AccountsController(_context);

                var result = await accountController.GetAccount(21);

                Assert.IsTrue(result.Value.AccountName == "Bank 1");
            }
        }

        [TestMethod]
        public async Task UpdateAccountTest()
        {
            var options = this.CreateInMemoryDatabase("Update_Account_database");

            using (var _context = new IronChurchContext(options))
            {
                AccountsController accountController = new AccountsController(_context);

                var testAccount = new Account { AccountId = 21, AccountName = "Bank 2", AccountNumber = "123444", BankName = "BOFA 2", InitialBalance = 22m, DateChanged = DateTime.UtcNow };

                var result = await accountController.PutAccount(21, testAccount);
                var newResult = await accountController.GetAccount(21);

                Assert.IsTrue(newResult.Value.AccountName.Equals(testAccount.AccountName));
            }
        }

        [TestMethod]
        public async Task AddAccountTest()
        {
            var options = this.CreateInMemoryDatabase("Add_Account_database");

            using (var _context = new IronChurchContext(options))
            {
                AccountsController accountController = new AccountsController(_context);

                var testAccount = new Account { AccountName = "Bank 2", AccountNumber = "123444", BankName = "BOFA 2", InitialBalance = 22m, DateAdded = DateTime.UtcNow };

                var result = await accountController.PostAccount(testAccount);
                var currentCount = accountController.Get().Result.Count;
                // Assert
                Assert.IsTrue(currentCount == 2);
            }
        }

        [TestMethod]
        public async Task EndAccountTest()
        {
            var options = this.CreateInMemoryDatabase("End_Account_database");

            using (var _context = new IronChurchContext(options))
            {
                AccountsController accountController = new AccountsController(_context);

                var testAccount = new Account { AccountId = 21, AccountEndDate = DateTime.UtcNow };
                
                var result = await accountController.EndAccount(21, testAccount);
                
                var newResult = await accountController.GetAccount(21);

                Assert.IsTrue(newResult.Value.AccountEndDate != null);
                Assert.IsTrue(newResult.Value.AccountEndDate.Equals(testAccount.AccountEndDate));
            }
        }
    }
}