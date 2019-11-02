using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Church.API.Data;
using Church.API.Data.DBContext;
using Church.API.Models.AppModel.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Church.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IronChurchContext _context;

        public AdminController(IronChurchContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Admin Change User Password
        /// </summary>
        /// <response code="200">Ok</response>
        /// <response code="500">Internal Server error</response>
        [HttpPost]
        [Route("AdminChangeUserPassword")]
        public async Task<IActionResult> AdminChangeUserPassword(ChangePassword request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var currentUserInfo = await this._context.Users
                .Where(u => u.Email == request.Email)
                .FirstOrDefaultAsync();

            if (currentUserInfo != null)
            {
                var newPasswordHash = Utils.EncrytPassword(request.NewPassword);
                var confirmNewPasswordHash = Utils.EncrytPassword(request.ConfirmNewPassword);

                if (newPasswordHash != confirmNewPasswordHash)
                {
                    return BadRequest("New password and Confirm password don't match");
                }

                currentUserInfo.Password = newPasswordHash;
                currentUserInfo.DateChanged = DateTime.UtcNow;
                currentUserInfo.UserChanged = request.Email;

                _context.Entry(currentUserInfo).Property(x => x.Password).IsModified = true;
                _context.Entry(currentUserInfo).Property(x => x.DateChanged).IsModified = true;
                _context.Entry(currentUserInfo).Property(x => x.UserChanged).IsModified = true;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest("Invalid User/does not exist");
            }

            return NoContent();
        }
    }
}