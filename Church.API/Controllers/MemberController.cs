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

        [HttpGet]
        [Route("GetByOrganizationId/{orgId}")]
        public async Task<List<Contributor>> GetByOrganizationId(int orgId)
        {
            return await _context.Contributor
                .Where(x => x.Status == 1 && x.OrganizationId == orgId)
                .ToListAsync();
        }

        // GET: Contributors
        [HttpGet]
        [Route("GetFullNamesByOrganizationId/{orgId}")]
        public async Task<List<string>> GetFullNames(int orgId)
        {
            return await _context.Contributor
                .Where(x => x.Status == 1 && x.OrganizationId == orgId)
                .Select(x => string.Concat(x.FirstName, " ", x.LastName))
                .ToListAsync();
        }

        // GET: api/Member/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contributor>> Get(int id)
        {
            var contributor = await _context.Contributor.FindAsync(id);

            if (id == 0)
            {
                return BadRequest($"Member Id must be provided. It cannot be 0");
            }
            else if (contributor == null)
            {
                return NotFound($"Member Id [{id}] is invalid/cannot be found");
            }

            return contributor;
        }

        // PUT: api/Accounts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContributor(int id, Contributor contributor)
        {
            if (id != contributor.ContributorId)
            {
                return BadRequest("Invalid update request. Member Id does not match Member details object");
            }

            contributor.DateChanged = DateTime.UtcNow;

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
                if (!Utils.MemberExists(_context, id))
                {
                    return NotFound($"Member Id [{id}] is invalid/does not exist");
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
                return NotFound($"Member Id [{id}] is invalid/does not exist");
            }

            if (!Utils.TransactionsExistForMemberId(_context, id))
            {
                _context.Contributor.Remove(contributor);
                await _context.SaveChangesAsync();
            }
            else
            {
                return BadRequest($"Cannot delete Member Id [{id}]. It has transactions associated with it.");
            }
            
            return contributor;
        }

        [HttpPost]
        public async Task<ActionResult<Contributor>> PostContributor(Contributor contributor)
        {
            var existsMemberWithName = _context.Contributor
                .Any(x => x.OrganizationId == contributor.OrganizationId && 
                          x.FirstName.Equals(contributor.FirstName, StringComparison.InvariantCultureIgnoreCase) &&
                          x.LastName.Equals(contributor.LastName, StringComparison.InvariantCultureIgnoreCase));

            if (existsMemberWithName)
            {
                return BadRequest($"Member with the same name [{contributor.FirstName} {contributor.LastName}] already exists");
            }

            contributor.ContributorId = Utils.GetNextIdAsync(_context, "contributor").Result;
            contributor.Status = 1;
            contributor.DateAdded = DateTime.UtcNow;

            _context.Contributor.Add(contributor);
            if (string.IsNullOrEmpty(contributor.FamilyName))
                contributor.FamilyName = null;

            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { id = contributor.ContributorId }, contributor);
        }
    }
}
