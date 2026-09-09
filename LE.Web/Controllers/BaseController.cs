using LE.Entities.User;
using LE.Service.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace LE.Web.Controllers
{
    public class BaseController : Controller
    {
        public UserRepository _userRepo { get; set; }
        public UserRoleRepository _userRoleRepo { get; set; }
        public AuthenticationRepository _authenticationRepo { get; set; }
        protected long getLoggedInUserId()
        {
            long authenticationId = getLoggedInAuthenticationId();
            var authentication = _authenticationRepo.getById(authenticationId);

            if (authentication == null)
            {
                return 0;
            }

            return authentication.type_id;
        }

        protected void clearUserCookies()
        {
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
        }
        protected long getLoggedInAuthenticationId()
        {
            string loggedInAuthenticationId = Request.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(loggedInAuthenticationId))
            {
                return 0;
            }
            return Convert.ToInt64(loggedInAuthenticationId);
        }

        protected User getLoggedInUserDetail()
        {
            var userId = getLoggedInUserId();

            return _userRepo.getById(userId);
        }

        protected UserRole getLoggedInUserRoleDetail()
        {
            var userId = getLoggedInUserId();

            return _userRoleRepo.getQueryable().Where(a => a.type_id == userId).FirstOrDefault();
        }
    }
}