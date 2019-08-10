using Church.API.Data.DBContext;
using Church.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
