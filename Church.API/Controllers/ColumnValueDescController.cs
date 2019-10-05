using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Church.API.Data.DBContext;
using Church.API.Models;

namespace Church.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColumnValueDescController : ControllerBase
    {
        private readonly IronChurchContext _context;

        public ColumnValueDescController(IronChurchContext context)
        {
            _context = context;
        }

        // GET: api/ColumnValueDesc
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ColumnValueDesc>>> GetColumnValueDesc()
        {
            return await _context.ColumnValueDesc.ToListAsync();
        }

        // GET: api/ColumnValueDesc
        [HttpGet]
        [Route("GetCVD/{tableName}/{columnName}")]
        public async Task<ActionResult<List<ColumnValueDesc>>> GetTableColumn(string tableName, string columnName)
        {
            var cvdList = new List<ColumnValueDesc>();

            var taskTableColumnId = await _context.TableColumn
                .Where(tb => tb.TableName == tableName && tb.ColumnName == columnName && tb.Status == 1)
                .FirstOrDefaultAsync();

            if (taskTableColumnId != null)
            {
                var tableColumnId = taskTableColumnId.TableColumnId;

                cvdList = await _context.ColumnValueDesc
                    .Where(x => x.TableColumnId == tableColumnId && x.Status == 1)
                    .ToListAsync();
            }
            else
            {
                return NotFound();
            }

            return cvdList;
        }

        // GET: api/ColumnValueDesc/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ColumnValueDesc>> GetColumnValueDesc(int id)
        {
            var columnValueDesc = await _context.ColumnValueDesc.FindAsync(id);

            if (columnValueDesc == null)
            {
                return NotFound();
            }

            return columnValueDesc;
        }

        // PUT: api/ColumnValueDesc/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutColumnValueDesc(int id, ColumnValueDesc columnValueDesc)
        //{
        //    if (id != columnValueDesc.ColumnValueDescId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(columnValueDesc).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ColumnValueDescExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/ColumnValueDesc
        //[HttpPost]
        //public async Task<ActionResult<ColumnValueDesc>> PostColumnValueDesc(ColumnValueDesc columnValueDesc)
        //{
        //    _context.ColumnValueDesc.Add(columnValueDesc);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetColumnValueDesc", new { id = columnValueDesc.ColumnValueDescId }, columnValueDesc);
        //}

        // DELETE: api/ColumnValueDesc/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<ColumnValueDesc>> DeleteColumnValueDesc(int id)
        //{
        //    var columnValueDesc = await _context.ColumnValueDesc.FindAsync(id);
        //    if (columnValueDesc == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.ColumnValueDesc.Remove(columnValueDesc);
        //    await _context.SaveChangesAsync();

        //    return columnValueDesc;
        //}

        private bool ColumnValueDescExists(int id)
        {
            return _context.ColumnValueDesc.Any(e => e.ColumnValueDescId == id);
        }
    }
}
