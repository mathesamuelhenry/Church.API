using Church.API.Data.DBContext;
using Church.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Church.API.Data
{
    public class Utils
    {
        public static int GetNextId(IronChurchContext dbContext, string table_name)
        {
            var nextId = dbContext.SeqControl.Where(x => x.ObjName == table_name).FirstOrDefault().NextId + 1;
            var newSeqcontrol = new SeqControl() { ObjName = "table_name", NextId = nextId };
            dbContext.Entry(newSeqcontrol).Property(x => x.NextId).IsModified = true;
            dbContext.SaveChangesAsync();

            return nextId;
        }

        public async static Task<int> GetNextIdAsync(IronChurchContext dbContext, string table_name)
        {
            var tableNextIDObject = await dbContext.SeqControl.Where(x => x.ObjName == table_name).FirstOrDefaultAsync();
            tableNextIDObject.NextId += 1;
            dbContext.Entry(tableNextIDObject).Property(x => x.NextId).IsModified = true;
            await dbContext.SaveChangesAsync();

            return tableNextIDObject.NextId;
        }

        /// <summary>
        /// Check if Member exists 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public static bool MemberExists(IronChurchContext dbContext, int memberId)
        {
            return dbContext.Contributor.Any(e => e.ContributorId == memberId);
        }

        /// <summary>
        /// Check if Transactions exist for Member Id
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="MemberId"></param>
        /// <returns></returns>
        public static bool TransactionsExistForMemberId(IronChurchContext dbContext, int MemberId)
        {
            return dbContext.Contribution.Any(e => e.ContributorId == MemberId);
        }

        public static string EncrytPassword(string inputString)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(inputString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        public static bool ValidOrganization(Organization organization, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(organization.Name))
            {
                errorMessage = "Organization name cannot be empty";
                return false;
            }

            return true;
        }
    }
}
