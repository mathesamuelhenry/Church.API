using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Church.API.Data;
using Church.API.Data.DBContext;
using Church.API.Models;
using Church.API.Models.AppModel.Request;
using Church.API.Models.AppModel.Request.User;
using Church.API.Models.AppModel.Response;
using Church.API.Models.AppModel.Response.GetUserResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Church.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private const string userStatusActive = "A";
        private readonly IronChurchContext _context;

        public UsersController(IronChurchContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<List<Users>> Get()
        {
            return await _context.Users
                .Include(org => org.UserOrganization)
                .Include(role => role.UserRole)
                .ToListAsync();
        }

        // GET: api/Users
        [HttpGet("{id}")]
        public async Task<UserResponse> Get(int id)
        {
            var userResponseResult = await _context.Users
                .Include(org => org.UserOrganization)
                .Include(role => role.UserRole)
                .Where(x => x.UserId == id)
                .FirstOrDefaultAsync();

            List<UserOrganizationResponse> orgList = new List<UserOrganizationResponse>();

            foreach (UserOrganization userOrg in userResponseResult.UserOrganization)
            {
                orgList.Add(new UserOrganizationResponse
                {
                    UserOrganizationId = userOrg.UserOrganizationId,
                    OrganizationId = userOrg.OrganizationId,
                    OrganizationName = _context.Organization
                        .Where(x => x.OrganizationId == userOrg.OrganizationId)
                        .FirstOrDefaultAsync().Result.Name
                });
            }

            List<UserRoleResponse> roleList = new List<UserRoleResponse>();

            foreach (UserRole userRole in userResponseResult.UserRole)
            {
                roleList.Add(new UserRoleResponse
                {
                    UserRoleId = userRole.UserRoleId,
                    RoleId = userRole.RoleId,
                    RoleName = _context.Role
                        .Where(x => x.RoleId == userRole.RoleId)
                        .FirstOrDefaultAsync().Result.RoleName
                });
            }

            return new UserResponse
            {
                UserId = userResponseResult.UserId,
                FirstName = userResponseResult.FirstName,
                LastName = userResponseResult.LastName,
                EmailId = userResponseResult.Email,
                UserStatus = userResponseResult.Status,
                UserOrganization = orgList,
                UserRole = roleList
            };
        }

        private Users GetUserObject(RegisterRequest userRequest)
        {
            if (userRequest == null)
                throw new Exception("Request cannot be null");

            var UserOrganizationCollection = new HashSet<UserOrganization>();
            foreach (var orgId in userRequest.OrganizationIdList)
            {
                UserOrganizationCollection.Add(new UserOrganization()
                {
                    OrganizationId = orgId,
                    UserAdded = userRequest.UserAdded
                });
            }

            var UserSecurityQuestionAnswerCollection = new HashSet<UserSecurityQuestion>();
            foreach(var questionAnswerReq in userRequest.UserQuestionAnswerList)
            {
                UserSecurityQuestionAnswerCollection.Add(new UserSecurityQuestion()
                {
                    SecurityQuestionId = questionAnswerReq.UserSecurityQuestionId,
                    Answer = questionAnswerReq.Answer,
                    UserAdded = userRequest.UserAdded
                });
            }

            var UserRoleCollection = new HashSet<UserRole>();
            foreach (var roleId in userRequest.UserRoleIdList)
            {
                UserRoleCollection.Add(new UserRole()
                {
                    RoleId = roleId,
                    UserAdded = userRequest.UserAdded
                });
            }

            var dbRegisterUser = new Users
            {
                FirstName = userRequest.FirstName,
                LastName = userRequest.LastName,
                Email = userRequest.Email,
                Password = userRequest.Password,
                Status = userRequest.Status,
                UserAdded = userRequest.UserAdded,
                UserOrganization = UserOrganizationCollection,
                UserRole = UserRoleCollection,
                UserSecurityQuestion = UserSecurityQuestionAnswerCollection
            };

            return dbRegisterUser;
        }

        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="userRequest"></param>
        /// <returns></returns>
        /// <response code="200">Ok</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server error</response>
        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult<Users>> Register(RegisterRequest userRequest)
        {
            string passwordHash = string.Empty;
            Users userDbRequestObject = null;

            if (userRequest != null)
            {
                userDbRequestObject = this.GetUserObject(userRequest);

                var existsUser = this._context.Users
                    .Where(u => u.Email.Equals(userRequest.Email) && u.Status == userStatusActive)
                    .ToListAsync()
                    .Result
                    .Count > 0;
                
                if (!existsUser)
                {
                    if (!string.IsNullOrEmpty(userRequest.Password))
                    {
                        passwordHash = Utils.EncrytPassword(userRequest.Password);
                        userDbRequestObject.Password = passwordHash;
                        userDbRequestObject.UserId = Utils.GetNextIdAsync(_context, "users").Result;
                        userDbRequestObject.DateAdded = DateTime.UtcNow;

                        foreach (var userOrg in userDbRequestObject.UserOrganization)
                        {
                            if (userOrg.OrganizationId == 0)
                                return BadRequest("Organization cannot be empty");

                            userOrg.UserOrganizationId = Utils.GetNextIdAsync(_context, "user_organization").Result;
                            userOrg.UserId = userDbRequestObject.UserId;
                            userOrg.DateAdded = DateTime.UtcNow;
                            userOrg.UserAdded = string.IsNullOrEmpty(userOrg.UserAdded) ? userRequest.UserAdded : userOrg.UserAdded;
                        }

                        foreach (var userRole in userDbRequestObject.UserRole)
                        {
                            if (userRole.RoleId == 0)
                                return BadRequest("Role Id cannot be empty");

                            userRole.UserRoleId = Utils.GetNextIdAsync(_context, "user_role").Result;
                            userRole.UserId = userDbRequestObject.UserId;
                            userRole.DateAdded = DateTime.UtcNow;
                        }

                        foreach (var userQuestion in userDbRequestObject.UserSecurityQuestion)
                        {
                            if (userQuestion.SecurityQuestionId == 0)
                                return BadRequest("question Id cannot be empty");

                            userQuestion.UserSecurityQuestionId = Utils.GetNextIdAsync(_context, "user_security_question").Result;
                            userQuestion.UserId = userDbRequestObject.UserId;
                            userQuestion.DateAdded = DateTime.UtcNow;
                        }

                        _context.Users.Add(userDbRequestObject);

                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    return BadRequest($"User with emailId [{userRequest.Email}] already exists.");
                }
            }

            userDbRequestObject.Password = string.Empty;

            return CreatedAtAction("Get", new { id = userDbRequestObject.UserId }, userDbRequestObject);
        }

        /// <summary>
        /// Authenticate
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Authenticate")]
        public async Task<ActionResult<Users>> Authenticate(SignIn request)
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
                var requestPasswordHash = Utils.EncrytPassword(request.Password);

                if (requestPasswordHash != currentUserInfo.Password)
                {
                    return BadRequest("Invalid Password");
                }
            }
            else
            {
                return BadRequest("Invalid User/does not exist");
            }

            currentUserInfo.Password = string.Empty;

            return CreatedAtAction("Get", new { id = currentUserInfo.UserId }, currentUserInfo);
        }


        /// <summary>
        /// Change Password
        /// </summary>
        /// <response code="200">Ok</response>
        /// <response code="500">Internal Server error</response>
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordFromOriginal request)
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
                var requestOriginalPasswordHash = Utils.EncrytPassword(request.OriginalPassword);
                
                if (requestOriginalPasswordHash != currentUserInfo.Password)
                {
                    return BadRequest("Old Password does not match your current password");
                }

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
                catch(DbUpdateConcurrencyException ex)
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