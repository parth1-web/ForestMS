using LE.Common.Enums;
using LE.Context.Data;
using LE.Entities.User;
using LE.Service.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly AppDbContext _dbContext;

        public NavbarViewComponent(ModuleRepository moduleRepo, UserRoleRepository userRoleRepo, DynamicMenuRepository dynamicMenuRepo, AuthenticationRepository authenticationRepo, RolePermissionMapRepository rolePermissionMapRepo, AppDbContext dbContext)
        {
            _moduleRepo = moduleRepo;
            _userRoleRepo = userRoleRepo;
            _dynamicMenuRepo = dynamicMenuRepo;
            _authenticationRepo = authenticationRepo;
            _rolePermissionMapRepo = rolePermissionMapRepo;
            _dbContext = dbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var loggedInAuthenticationId = Request.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(loggedInAuthenticationId))
            {
                return View();
            }

            long loggedInUserId = _authenticationRepo.getById(Convert.ToInt64(loggedInAuthenticationId)).type_id;

            List<Role> rolesAssignedToUser = await _dbContext.user_roles
                .Where(a => a.type == UserType.user && a.type_id == loggedInUserId)
                .Include(a => a.role)
                .Select(a => a.role)
                .ToListAsync();

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
