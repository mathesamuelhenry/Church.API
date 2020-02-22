using Church.API.Controllers;
using Church.API.Data.DBContext;
using Church.API.Models;
using Church.API.Tests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Church.API.Tests
{
    [TestClass]
    public class OrganizationsTest : TestBase
    {
        public OrganizationsTest()
        {

        }

        [TestMethod]
        public async Task AddOrganizationTest()
        {
            var options = this.CreateInMemoryDatabase("Add_Organizations_Database");

            using(var _context = new IronChurchContext(options))
            {
                OrganizationsController orgController = new OrganizationsController(_context);

                var ExpectedOrg = new Organization { Name = "Sam Test 1" };

                var result = await orgController.PostOrganization(ExpectedOrg);
                var ActualOrganization = orgController.GetOrganization().Result.Where(x => x.OrganizationId == 31).FirstOrDefault();

                Assert.AreEqual(ExpectedOrg.Name, ActualOrganization.Name);
            }
        }
    }
}
