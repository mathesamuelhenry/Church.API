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
    public class UserOrganizationsController : ControllerBase
    {
        private readonly IronChurchContext _context;

        public UserOrganizationsController(IronChurchContext context)
        {
            _context = context;
        }

        // GET: api/UserOrganizations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserOrganization>>> GetUserOrganization()
        {
            return await _context.UserOrganization.ToListAsync();
        }

        // GET: api/UserOrganizations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserOrganization>> GetUserOrganization(int id)
        {
            var userOrganization = await _context.UserOrganization.FindAsync(id);

            if (userOrganization == null)
            {
                return NotFound();
            }

            return userOrganization;
        }

        // GET
        [HttpGet]
        [Route("GetByAuthUserId/{authUserId}")]
        public async Task<ActionResult<List<UserOrganization>>> GetByAuthUserId(int authUserId)
        {
            var userOrganization = await _context.UserOrganization
                .Where(x => x.AuthUserId == authUserId)
                .ToListAsync();

            if (userOrganization == null)
            {
                return NotFound();
            }

            return userOrganization;
        }

        // PUT: api/UserOrganizations/5
        /*[HttpPut("{id}")]
        public async Task<IActionResult> PutUserOrganization(int id, UserOrganization userOrganization)
        {
            if (id != userOrganization.UserOrganizationId)
            {
                return BadRequest();
            }

            _context.Entry(userOrganization).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserOrganizationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }*/

        // POST: api/UserOrganizations
        [HttpPost]
        public async Task<ActionResult<UserOrganization>> PostUserOrganization(UserOrganization userOrganization)
        {
            if (userOrganization.AuthUserId == 0)
                return BadRequest("Auth User Id cannot be empty");

            if (userOrganization.OrganizationId == 0)
                return BadRequest("Organization Id cannot be empty");

            if (string.IsNullOrWhiteSpace(userOrganization.UserAdded))
                return BadRequest("User added cannot be empty");

            userOrganization.DateAdded = DateTime.UtcNow;
            userOrganization.UserOrganizationId = await Utils.GetNextIdAsync(_context, "user_organization");

            _context.UserOrganization.Add(userOrganization);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserOrganization", new { id = userOrganization.UserOrganizationId }, userOrganization);
        }

        // DELETE: api/UserOrganizations/5
        /*[HttpDelete("{id}")]
        public async Task<ActionResult<UserOrganization>> DeleteUserOrganization(int id)
        {
            var userOrganization = await _context.UserOrganization.FindAsync(id);
            if (userOrganization == null)
            {
                return NotFound();
            }

            _context.UserOrganization.Remove(userOrganization);
            await _context.SaveChangesAsync();

            return userOrganization;
        }*/

        private bool UserOrganizationExists(int id)
        {
            return _context.UserOrganization.Any(e => e.UserOrganizationId == id);
        }
    }
}
