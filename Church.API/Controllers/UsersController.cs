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
        /*[HttpGet]
        public async Task<List<UserResponse>> Get()
        {
            var userOrgList = await _context.UserOrganization
                .Include(org => org.Organization)
                .OrderBy(x => x.AuthUserId)
                .GroupBy(x => x.AuthUserId)
                .ToListAsync();

            var authUserList = await _authContext.AuthUser
                .ToListAsync();

            var userRoleList = await _authContext.UserRole
                .Include(role => role.Role)
                .ToListAsync();

            var query = from authUser in authUserList
                           join user in userOrgList
                           on authUser.AuthUserId equals user.Key
                           select new
                           {
                               UserId = user.Key,
                               EmailId = authUser.Email,
                               authUser.FirstName,
                               authUser.LastName,
                               UserStatus = authUser.Status,
                               UserOrg = user
                           };

            var userResponseList = new List<UserResponse>();
            var userOrganizationResponseList = new List<UserOrganizationResponse>();

            foreach (var userInfo in query)
            {
                var userResponse = new UserResponse()
                {
                    UserId = userInfo.UserId,
                    FirstName = userInfo.FirstName,
                    LastName = userInfo.LastName,
                    UserStatus = userInfo.UserStatus,
                    EmailId = userInfo.EmailId,
                    UserOrganization = userInfo.UserOrg.AsEnumerable().Select(x => new UserOrganizationResponse
                    {
                        UserOrganizationId = x.UserOrganizationId,
                        OrganizationId = x.OrganizationId,
                        OrganizationName = x.Organization.Name
                    }).ToList(),
                    UserRole = userRoleList.Where(x => x.AuthUserId == userInfo.UserId).Select(x => new UserRoleResponse
                    {
                        RoleId = x.RoleId,
                        UserRoleId = x.UserRoleId,
                        RoleName = x.Role.RoleName
                    }).ToList()
                };

                userResponseList.Add(userResponse);
            };
            
            return userResponseList;
        }*/

        // GET: api/Users
        /*[HttpGet("{id}")]
        public async Task<UserResponse> Get(int id)
        {
            var userOrgList = await _context.UserOrganization
                .Include(org => org.Organization)
                .Where(x => x.AuthUserId == id)
                .OrderBy(x => x.AuthUserId)
                .GroupBy(x => x.AuthUserId)
                .ToListAsync();

            var authUserList = await _authContext.AuthUser
                .Where(x => x.AuthUserId == id)
                .ToListAsync();

            var userRoleList = await _authContext.UserRole
                .Include(role => role.Role)
                .Where(x => x.AuthUserId == id)
                .ToListAsync();

            var query = from authUser in authUserList
                        join user in userOrgList
                        on authUser.AuthUserId equals user.Key
                        select new
                        {
                            UserId = user.Key,
                            EmailId = authUser.Email,
                            authUser.FirstName,
                            authUser.LastName,
                            UserStatus = authUser.Status,
                            UserOrg = user
                        };

            UserResponse userResponse = null;
            var userOrganizationResponseList = new List<UserOrganizationResponse>();

            foreach (var userInfo in query)
            {
                userResponse = new UserResponse()
                {
                    UserId = userInfo.UserId,
                    FirstName = userInfo.FirstName,
                    LastName = userInfo.LastName,
                    UserStatus = userInfo.UserStatus,
                    EmailId = userInfo.EmailId,
                    UserOrganization = userInfo.UserOrg.AsEnumerable().Select(x => new UserOrganizationResponse
                    {
                        UserOrganizationId = x.UserOrganizationId,
                        OrganizationId = x.OrganizationId,
                        OrganizationName = x.Organization.Name
                    }).ToList(),
                    UserRole = userRoleList.Where(x => x.AuthUserId == userInfo.UserId).Select(x => new UserRoleResponse
                    {
                        RoleId = x.RoleId,
                        UserRoleId = x.UserRoleId,
                        RoleName = x.Role.RoleName
                    }).ToList()
                };
            };

            return userResponse;
        }*/

        /*private Users GetUserObject(RegisterRequest userRequest)
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
            AuthUser currentAuthUserModel = new AuthUser();
            UserOrganization currentUserOrgModel = new UserOrganization();
            AuthUser newAuthUserModel = new AuthUser();
            UserOrganization newUserOrgModel = new UserOrganization();
            if (userRequest != null)
            {
                currentAuthUserModel = this._authContext.AuthUser
                    .Where(x => x.Email == userRequest.Email)
                    .FirstOrDefault();

                var authUserId = currentAuthUserModel?.AuthUserId;

                currentUserOrgModel = this._context.UserOrganization
                    .Include(x => x.AuthUserId == authUserId)
                    .FirstOrDefault();
                
                var existsUser = this._authContext.AuthUser
                    .Where(x => x.Email.Equals(userRequest.Email, StringComparison.InvariantCultureIgnoreCase))
                    .ToListAsync()
                    .Result
                    .Count > 0;

                var authGroupId = this._authContext.AuthGroup
                    .Where(x => x.AuthGroupName == userRequest.AuthGroupName)
                    .FirstOrDefault()
                    .AuthGroupId;
                
                if (!existsUser)
                {
                    if (!string.IsNullOrEmpty(userRequest.Password))
                    {
                        passwordHash = Utils.EncrytPassword(userRequest.Password);
                        newAuthUserModel.Password = passwordHash;
                        newAuthUserModel.AuthUserId = Utils.GetNextIdAsync(_context, "users").Result;
                        newAuthUserModel.DateAdded = DateTime.UtcNow;
                        newAuthUserModel.FirstName = userRequest.FirstName;
                        newAuthUserModel.LastName = userRequest.LastName;
                        newAuthUserModel.Status = userRequest.Status;
                        newAuthUserModel.AuthGroupId = authGroupId;
                        newAuthUserModel.UserAdded = userRequest.UserAdded;

                        foreach (var userOrg in userDbRequestObject.UserOrganization)
                        {
                            if (userOrg.OrganizationId == 0)
                                return BadRequest("Organization cannot be empty");

                            userOrg.UserOrganizationId = Utils.GetNextIdAsync(_context, "user_organization").Result;
                            userOrg.AuthUserId = userDbRequestObject.UserId;
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
        }*/

        /// <summary>
        /// Authenticate
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /*[HttpPost]
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
        }*/


        /// <summary>
        /// Change Password
        /// </summary>
        /// <response code="200">Ok</response>
        /// <response code="500">Internal Server error</response>
        /*[HttpPost]
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
        }*/
    }
}