﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SimpleFramework.Core.Abstraction.Data;
using SimpleFramework.Core.Entitys;
using SimpleFramework.Core.Web.SmartTable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using SimpleFramework.Core.Security;
using SimpleFramework.Core.Common;

namespace SimpleFramework.Module.Backend.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/users")]
    public class UserApiController : Core.Web.Base.Controllers.ControllerBase
    {
        private readonly IRepositoryWithTypedId<UserEntity, long> userRepository;
        private readonly ISecurityService _securityService;
        public UserApiController(IRepositoryWithTypedId<UserEntity, long> userRepository,
            ISecurityService securityService,
            IServiceCollection service,
            ILogger<UserApiController> logger) : base(service, logger)
        {
            this.userRepository = userRepository;
            this._securityService = securityService;
        }

        [HttpPost("grid")]
        public ActionResult List([FromBody] SmartTableParam param)
        {
            var query = userRepository.Queryable()
                // .Include(x => x.Roles).ThenInclude(r => r.Role)
                .Where(x => !x.IsDeleted);

            if (param.Search.PredicateObject != null)
            {
                dynamic search = param.Search.PredicateObject;

                if (search.Email != null)
                {
                    string email = search.Email;
                    query = query.Where(x => x.Email.Contains(email));
                }

                if (search.FullName != null)
                {
                    string fullName = search.FullName;
                    query = query.Where(x => x.FullName.Contains(fullName));
                }

                if (search.CreatedOn != null)
                {
                    if (search.CreatedOn.before != null)
                    {
                        DateTimeOffset before = search.CreatedOn.before;
                        before = before.Date.AddDays(1);
                        // query = query.Where(x => x.CreatedDate <= before);
                    }

                    if (search.CreatedOn.after != null)
                    {
                        DateTimeOffset after = search.CreatedOn.after;
                        after = after.Date;
                        // query = query.Where(x => x.CreatedDate >= after);
                    }
                }
            }

            var users = query.ToSmartTableResult(
                param,
                user => new
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    // CreatedOn = user.CreatedDate,
                    //   Roles = string.Join(", ", user.Roles.Select(x => x.Role.Name))
                });

            return Json(users);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult> GetUserById(string id)
        {
            var retVal = await _securityService.FindByIdAsync(id, UserDetails.Full);
            return Content(retVal.ToJson());
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(long id)
        {
            var user = userRepository.Queryable().FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return new NotFoundResult();
            }

            user.IsDeleted = true;
            userRepository.SaveChange();
            return Json(true);
        }

        /// <summary>
        /// Get current user details
        /// </summary>
        [HttpGet]
        [Route("currentuser")]
        public async Task<ActionResult> GetCurrentUser()
        {
            var retVal = await _securityService.FindByNameAsync(User.Identity.Name, UserDetails.Full);
            return Content(retVal.ToJson());
        }

        /// <summary>
        /// Get user details by user name
        /// </summary>
        /// <param name="userName"></param>
        [HttpGet]
        [Route("{userName}")]
        public async Task<ActionResult> GetUserByName(string userName)
        {
            var retVal = await _securityService.FindByNameAsync(userName, UserDetails.Full);
            return Content(retVal.ToJson());
        }


        /// <summary>
        /// Check specified user has passed permissions in specified scope
        /// </summary>
        /// <param name="userName">security account name</param>
        /// <param name="permissions">checked permissions Example: ?permissions=read&amp;permissions=write </param>
        /// <param name="scopes">security bounded scopes. Read mode: http://docs.virtocommerce.com/display/vc2devguide/Working+with+platform+security </param>
        /// <returns></returns>
        [HttpGet]
        [Route("{userName}/hasPermissions")]
        public ActionResult UserHasAnyPermission(string userName, string[] permissions, string[] scopes)
        {
            var retVal = new { Result = _securityService.UserHasAnyPermission(userName, scopes, permissions) };
            return Content(retVal.ToJson());
        }
    }
}
