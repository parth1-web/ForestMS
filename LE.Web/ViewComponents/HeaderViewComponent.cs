using LE.Service.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LE.Web.ViewComponents
{
    [ViewComponent(Name = "HeaderView")]
    public class HeaderViewComponent : ViewComponent
    {
        private readonly OrganizationSetupRepository _organizationSetupRepository;
        public readonly UserRepository _userRepository;
        private readonly AuthenticationRepository _authenticationRepo;

        public HeaderViewComponent(OrganizationSetupRepository organizationSetupRepository, UserRepository userRepository, AuthenticationRepository authenticationRepo)
        {
            _organizationSetupRepository = organizationSetupRepository;
            _userRepository = userRepository;
            _authenticationRepo = authenticationRepo;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var loggedInAuthenticationId = Request.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            ViewBag.authenticationId = loggedInAuthenticationId;

            // Error pages can render for anonymous requests; there is no user detail to show then.
            if (string.IsNullOrEmpty(loggedInAuthenticationId))
            {
                ViewBag.userDetail = null;
                ViewBag.setup = _organizationSetupRepository.getQueryable().ToList();
                return View();
            }

            long loggedInUserId = _authenticationRepo.getById(Convert.ToInt64(loggedInAuthenticationId)).type_id;

            var userDetails = _userRepository.getById(Convert.ToInt32(loggedInUserId));
            ViewBag.userDetail = userDetails;

            var setupValues = _organizationSetupRepository.getQueryable().ToList();
            ViewBag.setup = setupValues;
            return View();
        }
    }

}
