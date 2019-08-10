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
    public class ContributorTest
    {
        public ContributorTest() { }

        [TestMethod]
        public async Task Get()
        {
            var options = this.CreateInMemoryDatabase("Get_Contributor_Database");

            using (var _context = new IronChurchContext(options))
            {
                MemberController contController = new MemberController(_context);

                var result = await contController.Get();

                Assert.IsTrue(result.Count == 2);
            }
        }

        [TestMethod]
        public async Task GetByIdTest()
        {
            var options = this.CreateInMemoryDatabase("Get_By_Id_database");

            using (var _context = new IronChurchContext(options))
            {
                MemberController contController = new MemberController(_context);

                var result = await contController.Get(122);

                Assert.IsTrue(result.Value.FirstName.Equals("Sarah"));
            }
        }

        [TestMethod]
        public async Task UpdateMemberTest()
        {
            var options = this.CreateInMemoryDatabase("Update_Member_database");

            using (var _context = new IronChurchContext(options))
            {
                MemberController contController = new MemberController(_context);

                var testContributor = new Contributor { ContributorId = 124, FirstName = "Samarah", LastName = "Henry", FamilyName = "Mathe" };

                var result = await contController.PutContributor(124, testContributor);
                var newResult = await contController.Get(124);

                Assert.IsTrue(newResult.Value.FirstName.Equals(testContributor.FirstName));
            }
        }

        [TestMethod]
        public async Task AddMemberTest()
        {
            var options = this.CreateInMemoryDatabase("Add_Member_database");

            using (var _context = new IronChurchContext(options))
            {
                MemberController contController = new MemberController(_context);

                var testContributor = new Contributor { FirstName = "John", LastName = "Kenny", FamilyName = "Mathe", DateAdded = DateTime.Now };

                var result = await contController.PostContributor(testContributor);
                var currentCount = contController.Get().Result.Count;
                // Assert
                Assert.IsTrue(currentCount == 3);
            }
        }

        private DbContextOptions<IronChurchContext> CreateInMemoryDatabase(string databaseName)
        {
            var options = new DbContextOptionsBuilder<IronChurchContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            var _context = new IronChurchContext(options);

            _context.Contributor.Add(new Contributor { ContributorId = 124, FirstName = "Samuel", LastName = "Henry", FamilyName = "Mathe" });
            _context.Contributor.Add(new Contributor { ContributorId = 122, FirstName = "Sarah", LastName = "Solomon", FamilyName = "Mathe" });

            _context.SeqControl.Add(new SeqControl { ObjName = "contributor", NextId = 150 });

            _context.SaveChanges();

            return options;
        }
    }
}