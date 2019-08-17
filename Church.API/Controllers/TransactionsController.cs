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
    public class TransactionsController : ControllerBase
    {
        private readonly IronChurchContext _context;

        public TransactionsController(IronChurchContext context)
        {
            _context = context;
        }

        // GET: api/Transactions
        [HttpGet]
        [Route("SearchTransactions/{page}/{limit}")]
        public List<Contribution> SearchTransactions(int page, int limit)
        {
            if (page == 0)
                page = 1;
            
            if (limit == 0)
                limit = int.MaxValue;

            var skip = (page - 1) * limit;

            return _context.Contribution
                .Include(mem => mem.Contributor)
                .OrderByDescending(x => x.TransactionDate)
                .Skip(skip)
                .Take(limit)
                .ToList();
        }

        // GET: api/Transactions/5
        [HttpGet("{id}")]
        public Contribution GetContribution(int id)
        {
            var contribution = _context.Contribution
                .Where(x => x.ContributionId == id)
                .Include(mem => mem.Contributor)
                .FirstOrDefault();

            if (contribution == null)
            { }

            return contribution;
        }

        // PUT: api/Transactions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContribution(int id, Contribution contribution)
        {
            if (id != contribution.ContributionId)
            {
                return BadRequest();
            }

            _context.Entry(contribution).Property(x => x.ContributionName).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContributionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Transactions
        [HttpPost]
        public async Task<ActionResult<Contribution>> PostContribution(Contribution contribution)
        {
            _context.Contribution.Add(contribution);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContribution", new { id = contribution.ContributionId }, contribution);
        }

        // DELETE: api/Transactions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Contribution>> DeleteContribution(int id)
        {
            var contribution = await _context.Contribution.FindAsync(id);
            if (contribution == null)
            {
                return NotFound();
            }

            _context.Contribution.Remove(contribution);
            await _context.SaveChangesAsync();

            return contribution;
        }

        private bool ContributionExists(int id)
        {
            return _context.Contribution.Any(e => e.ContributionId == id);
        }
    }
}
