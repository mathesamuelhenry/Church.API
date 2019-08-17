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
    public class LoansController : ControllerBase
    {
        private readonly IronChurchContext _context;

        public LoansController(IronChurchContext context)
        {
            _context = context;
        }

        // GET: api/Loans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContributorLoan>>> GetContributorLoan()
        {
            return await _context.ContributorLoan.Include(x => x.Contributor).ToListAsync();
        }

        // GET: api/Loans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ContributorLoan>> GetContributorLoan(int id)
        {
            var contributorLoan = await _context.ContributorLoan.FindAsync(id);

            if (contributorLoan == null)
            {
                return NotFound();
            }

            return contributorLoan;
        }

        // PUT: api/Loans/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContributorLoan(int id, ContributorLoan contributorLoan)
        {
            if (id != contributorLoan.ContributorLoanId)
            {
                return BadRequest();
            }

            _context.Entry(contributorLoan).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContributorLoanExists(id))
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

        // POST: api/Loans
        [HttpPost]
        public async Task<ActionResult<ContributorLoan>> PostContributorLoan(ContributorLoan contributorLoan)
        {
            _context.ContributorLoan.Add(contributorLoan);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContributorLoan", new { id = contributorLoan.ContributorLoanId }, contributorLoan);
        }

        // DELETE: api/Loans/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ContributorLoan>> DeleteContributorLoan(int id)
        {
            var contributorLoan = await _context.ContributorLoan.FindAsync(id);
            if (contributorLoan == null)
            {
                return NotFound();
            }

            _context.ContributorLoan.Remove(contributorLoan);
            await _context.SaveChangesAsync();

            return contributorLoan;
        }

        private bool ContributorLoanExists(int id)
        {
            return _context.ContributorLoan.Any(e => e.ContributorLoanId == id);
        }
    }
}
