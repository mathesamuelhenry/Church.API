using Microsoft.EntityFrameworkCore;
using Church.API.Data.DBContext;
using Church.API.Models;

namespace Church.API.Tests.Base
{
    public class TestBase
    {
        public TestBase()
        {
        }

        protected DbContextOptions<IronChurchContext> CreateInMemoryDatabase(string databaseName)
        {
            var options = new DbContextOptionsBuilder<IronChurchContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            var _context = new IronChurchContext(options);

            /*
             Mock Contributor Data
             */
            _context.Contributor.Add(new Contributor { ContributorId = 124, FirstName = "Samuel", LastName = "Henry", FamilyName = "Mathe" });
            _context.Contributor.Add(new Contributor { ContributorId = 122, FirstName = "Sarah", LastName = "Solomon", FamilyName = "Mathe" });

            /*
             Mock Seq Control Data
             */
            _context.SeqControl.Add(new SeqControl { ObjName = "contributor", NextId = 150 });
            _context.SeqControl.Add(new SeqControl { ObjName = "account", NextId = 30 });
            _context.SeqControl.Add(new SeqControl { ObjName = "organization", NextId = 30 });

            /*
             Mock Account Data
             */
            _context.Account.Add(new Account { AccountId = 21, AccountName = "Bank 1", AccountNumber = "1234", BankName = "BOFA", InitialBalance = 4444m});

            /*
             Mock Organizations Data
             */
            _context.Organization.Add(new Organization { OrganizationId = 1, Name = "Sam Test", Website = "www.sam.com", AddressLine1 = "10 Downing Street", City = "Boca Raton", State = "Fl" });
            
            _context.SaveChanges();

            return options;
        }
    }
}