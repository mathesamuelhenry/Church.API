using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Church.API.Data.DBContext;
using Church.API.Models;
using Church.API.Data;

namespace Church.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationsController : ControllerBase
    {
        private readonly IronChurchContext _context;

        public OrganizationsController(IronChurchContext context)
        {
            _context = context;
        }

        // GET: api/Organizations
        [HttpGet]
        public async Task<List<Organization>> GetOrganization()
        {
            return await _context.Organization
                .Include(x => x.Industry)
                .ToListAsync(); 
        }

        // GET: api/Organizations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Organization>> GetOrganization(int id)
        {
            var organization = await _context.Organization
                .Include(x => x.Industry)
                .Where(x => x.OrganizationId == id)
                .FirstOrDefaultAsync();

            if (organization == null)
            {
                return NotFound();
            }

            return organization;
        }

        // PUT: api/Organizations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrganization(int id, Organization organization)
        {
            if (id != organization.OrganizationId)
            {
                return BadRequest();
            }

            _context.Entry(organization).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrganizationExists(id))
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

        // POST: api/Organizations
        [HttpPost]
        public async Task<ActionResult<Organization>> PostOrganization(Organization organization)
        {
            if (string.IsNullOrEmpty(organization.Name))
                return BadRequest("Organization name cannot be empty");

            if (organization.IndustryId == 0)
                return BadRequest("Organization name cannot be empty");
            else if (await _context.Industry.FindAsync(organization.IndustryId) == null)
                return NotFound("Invalid Industry/Not found");

            if (string.IsNullOrEmpty(organization.UserAdded))
                return BadRequest("User Added cannot be empty");

            var existsOrganization = _context.Organization
                .Any(x => x.Name.Equals(organization.Name, StringComparison.InvariantCultureIgnoreCase));

            organization.OrganizationId = Utils.GetNextIdAsync(_context, "organization").Result;
            organization.DateAdded = DateTime.UtcNow;
            organization.Phone = string.IsNullOrWhiteSpace(organization.Phone) ? null : organization.Phone;
            organization.Email = string.IsNullOrWhiteSpace(organization.Email) ? null : organization.Email;

            _context.Organization.Add(organization);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrganization", new { id = organization.OrganizationId }, organization);
        }

        // DELETE: api/Organizations/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Organization>> DeleteOrganization(int id)
        {
            var organization = await _context.Organization.FindAsync(id);
            if (organization == null)
            {
                return NotFound();
            }

            _context.Organization.Remove(organization);
            await _context.SaveChangesAsync();

            return organization;
        }

        private bool OrganizationExists(int id)
        {
            return _context.Organization.Any(e => e.OrganizationId == id);
        }
    }
}
