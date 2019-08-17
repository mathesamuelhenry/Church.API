using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Church.API.Models;
using Church.API.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using Church.API.Data;

namespace Church.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly IronChurchContext _context;

        public MemberController(IronChurchContext context)
        {
            _context = context;
        }

        // GET: Contributors
        [HttpGet]
        public async Task<List<Contributor>> Get()
        {
            return await _context.Contributor.Where(x => x.Status == 1).ToListAsync();
        }

        // GET: api/Member/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contributor>> Get(int id)
        {
            var contributor = await _context.Contributor.FindAsync(id);

            if (contributor == null)
            {
                return NotFound();
            }

            return contributor;
        }

        // PUT: api/Accounts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContributor(int id, Contributor contributor)
        {
            if (id != contributor.ContributorId)
            {
                return BadRequest();
            }

            _context.Entry(contributor).Property(x => x.FirstName).IsModified = true;
            _context.Entry(contributor).Property(x => x.LastName).IsModified = true;
            _context.Entry(contributor).Property(x => x.FamilyName).IsModified = true;
            _context.Entry(contributor).Property(x => x.DateChanged).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContributorExists(id))
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

        // DELETE: api/Accounts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Contributor>> DeleteContributor(int id)
        {
            var contributor = await _context.Contributor.FindAsync(id);
            if (contributor == null)
            {
                return NotFound();
            }

            if (!this.TransactionsExistForMemberId(id))
            {
                _context.Contributor.Remove(contributor);
                await _context.SaveChangesAsync();
            }
            
            return contributor;
        }

        [HttpPost]
        public async Task<ActionResult<Contributor>> PostContributor(Contributor contributor)
        {
            contributor.ContributorId = Utils.GetNextIdAsync(_context, "contributor").Result;
            contributor.Status = 1;

            _context.Contributor.Add(contributor);
            if (string.IsNullOrEmpty(contributor.FamilyName))
                contributor.FamilyName = null;

            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { id = contributor.ContributorId }, contributor);
        }

        private bool ContributorExists(int id)
        {
            return _context.Contributor.Any(e => e.ContributorId == id);
        }

        private bool TransactionsExistForMemberId(int MemberId)
        {
            return _context.Contribution.Any(e => e.ContributorId == MemberId);
        }
    }
}
