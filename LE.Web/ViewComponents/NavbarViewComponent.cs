using LE.Entities.User;
using LE.Service.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LE.Web.ViewComponents
{
    [ViewComponent(Name = "Navbar")]
    public class NavbarViewComponent : ViewComponent
    {
        private readonly ModuleRepository _moduleRepo;
        private readonly UserRoleRepository _userRoleRepo;
        private readonly DynamicMenuRepository _dynamicMenuRepo;
        private readonly AuthenticationRepository _authenticationRepo;
        private readonly RolePermissionMapRepository _rolePermissionMapRepo;

        public NavbarViewComponent(ModuleRepository moduleRepo, UserRoleRepository userRoleRepo, DynamicMenuRepository dynamicMenuRepo, AuthenticationRepository authenticationRepo, RolePermissionMapRepository rolePermissionMapRepo)
        {
            _moduleRepo = moduleRepo;
            _userRoleRepo = userRoleRepo;
            _dynamicMenuRepo = dynamicMenuRepo;
            _authenticationRepo = authenticationRepo;
            _rolePermissionMapRepo = rolePermissionMapRepo;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var loggedInAuthenticationId = Request.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            long loggedInUserId = _authenticationRepo.getById(Convert.ToInt64(loggedInAuthenticationId)).type_id;

            List<Role> rolesAssignedToUser = _userRoleRepo.getByTypeId(Common.Enums.UserType.user, loggedInUserId).Select(a => a.role).ToList();

            List<long> roleIdsAssignedToUser = rolesAssignedToUser.Select(a => a.role_id).ToList();

            List<long> moduleIdsAssignedToRoles = _rolePermissionMapRepo.getQueryable().Where(a => roleIdsAssignedToUser.Contains(a.role_id)).Select(a => a.module_id).Distinct().ToList();

            List<Module> modules = _moduleRepo.getQueryable().Where(a => moduleIdsAssignedToRoles.Contains(a.module_id)).ToList();

            List<long> distinctModuleIds = modules.Select(a => a.module_id).ToList();

            List<DynamicMenu> menus = _dynamicMenuRepo.getQueryable().Where(a => distinctModuleIds.Contains(a.module_id)).ToList();

            ViewBag.modules = modules;
            ViewBag.menus = menus;

            return View();
        }
    }
}
