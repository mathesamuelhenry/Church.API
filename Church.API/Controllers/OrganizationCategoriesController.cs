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
    public class OrganizationCategoriesController : ControllerBase
    {
        private readonly IronChurchContext _context;

        public OrganizationCategoriesController(IronChurchContext context)
        {
            _context = context;
        }

        // GET: api/OrganizationCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrganizationCategory>>> GetOrganizationCategory()
        {
            return await _context.OrganizationCategory
                .Where(x => x.IsActive == 1)
                .ToListAsync();
        }

        // GET: api/OrganizationCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrganizationCategory>> GetOrganizationCategory(int id)
        {
            var organizationCategory = await _context.OrganizationCategory
                .FindAsync(id);

            if (organizationCategory == null)
            {
                return NotFound();
            }

            return organizationCategory;
        }

        [HttpGet]
        [Route("GetCategoryByOrganizationId")]
        public async Task<ActionResult<OrganizationCategory>> GetCategoryByOrganizationId(int orgId)
        {
            var organizationCategory = await _context.OrganizationCategory
                .Where(x => x.OrganizationId == orgId && x.IsActive == 1)
                .FirstOrDefaultAsync();

            if (organizationCategory == null)
            {
                return NotFound();
            }

            return organizationCategory;
        }

        // PUT: api/OrganizationCategories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrganizationCategory(int id, OrganizationCategory organizationCategory)
        {
            if (id != organizationCategory.OrganizationCategoryId)
            {
                return BadRequest($"Invalid request. ID {id}, OrgCat {organizationCategory.OrganizationCategoryId}");
            }

            if (!OrganizationCategoryExists(id))
                return BadRequest("Org Category Id does not exist");

            if (string.IsNullOrWhiteSpace(organizationCategory.CategoryName))
                return BadRequest("Category name cannot be empty");
            
            if (_context.OrganizationCategory.Any(x => x.OrganizationId == organizationCategory.OrganizationId &&
                        x.OrganizationCategoryId != organizationCategory.OrganizationCategoryId &&
                        x.CategoryName.Equals(organizationCategory.CategoryName)))
            {
                return BadRequest("Category name already exists");
            }

            if (string.IsNullOrWhiteSpace(organizationCategory.UserChanged))
                return BadRequest("User Login id cannot be empty");

            organizationCategory.DateChanged = DateTime.UtcNow;

            _context.Entry(organizationCategory).Property(x => x.CategoryName).IsModified = true;
            _context.Entry(organizationCategory).Property(x => x.DateChanged).IsModified = true;
            _context.Entry(organizationCategory).Property(x => x.UserChanged).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrganizationCategoryExists(id))
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

        // POST: api/OrganizationCategories
        [HttpPost]
        public async Task<ActionResult<OrganizationCategory>> PostOrganizationCategory(OrganizationCategory organizationCategory)
        {
            var existsOrganization = await _context.Organization
                .Where(x => x.OrganizationId == organizationCategory.OrganizationId)
                .FirstOrDefaultAsync();

            if (existsOrganization == null)
                return NotFound("Organization is either invalid/not found");

            if (string.IsNullOrWhiteSpace(organizationCategory.CategoryName))
                return BadRequest("Category name cannot be empty");
            else if (_context.OrganizationCategory.Any(x => x.OrganizationId == organizationCategory.OrganizationId &&
                        x.CategoryName.Equals(organizationCategory.CategoryName)))
                return BadRequest("Category name already exists");

            if (string.IsNullOrWhiteSpace(organizationCategory.UserAdded))
                return BadRequest("User added cannot be empty");

            organizationCategory.OrganizationCategoryId = await Utils.GetNextIdAsync(_context, "organization_category");
            organizationCategory.DateAdded = DateTime.UtcNow;
            organizationCategory.IsActive = 1;

            _context.OrganizationCategory.Add(organizationCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrganizationCategory", new { id = organizationCategory.OrganizationCategoryId }, organizationCategory);
        }

        // DELETE: api/OrganizationCategories/5
        /*[HttpDelete("{id}")]
        public async Task<ActionResult<OrganizationCategory>> DeleteOrganizationCategory(int id)
        {
            var organizationCategory = await _context.OrganizationCategory.FindAsync(id);
            if (organizationCategory == null)
            {
                return NotFound();
            }

            _context.OrganizationCategory.Remove(organizationCategory);
            await _context.SaveChangesAsync();

            return organizationCategory;
        }*/

        private bool OrganizationCategoryExists(int id)
        {
            return _context.OrganizationCategory.Any(e => e.OrganizationCategoryId == id);
        }
    }
}
