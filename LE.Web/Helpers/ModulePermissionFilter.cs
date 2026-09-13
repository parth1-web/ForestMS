using LE.Common.Enums;
using LE.Entities.User;
using LE.Service.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace LE.Web.Helpers
{
    // P1/S3 fix: role_permission_maps was only used to render the navbar — a user
    // without a module could still open its URLs directly. This filter enforces,
    // for every page that appears in the navbar (dynamic_menus.web_url), that the
    // logged-in user holds the module that menu belongs to through one of their
    // roles. Requests under a URL prefix no menu covers fall back to the module
    // code set for the MVC area (e.g. area 'billing' -> modules whose module_code
    // is 'billing'; the seed defines Billing and Utilities under that code).
    // Areas with no module mapping (counter POS API) and root-level pages are left
    // to the global authentication requirement.
    public class ModulePermissionFilter : IAuthorizationFilter
    {
        private readonly RolePermissionMapRepository _rolePermissionMapRepo;
        private readonly UserRoleRepository _userRoleRepo;
        private readonly AuthenticationRepository _authenticationRepo;
        private readonly ModuleRepository _moduleRepo;
        private readonly DynamicMenuRepository _dynamicMenuRepo;

        public ModulePermissionFilter(RolePermissionMapRepository rolePermissionMapRepo, UserRoleRepository userRoleRepo, AuthenticationRepository authenticationRepo, ModuleRepository moduleRepo, DynamicMenuRepository dynamicMenuRepo)
        {
            _rolePermissionMapRepo = rolePermissionMapRepo;
            _userRoleRepo = userRoleRepo;
            _authenticationRepo = authenticationRepo;
            _moduleRepo = moduleRepo;
            _dynamicMenuRepo = dynamicMenuRepo;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Anonymous endpoints (login, error pages) are exempt.
            if (context.Filters.OfType<IAllowAnonymousFilter>().Any())
            {
                return;
            }

            var path = context.HttpContext.Request.Path.Value;
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            // Normalize: lowercase, leading slash, no trailing slash.
            path = path.TrimEnd('/').ToLowerInvariant();
            var segments = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length < 2)
            {
                // Root-level pages (/home) and bare endpoints — no module check.
                return;
            }

            // The counter-POS desktop client (ClickOnce, JWT) cannot be redeployed on
            // demand and may log in with staff accounts that hold no browser roles.
            // Its endpoints stay guarded by authentication + server-side identity
            // (see P0 notes) and are exempt from the module check, mirroring the
            // documented antiforgery exemption for the same client.
            if (path.StartsWith("/billing/counter-billing", StringComparison.Ordinal))
            {
                return;
            }

            // "area/controller" prefix shared by the request and the seeded menus.
            var basePrefix = $"{segments[0]}/{segments[1]}";

            // Modules that expose a menu under this URL prefix (same rule the
            // navbar uses, so what a user cannot see they cannot open).
            var menuModules = _dynamicMenuRepo.getQueryable()
                .Where(a => a.web_url != null)
                .Select(a => new { a.web_url, a.module_id })
                .ToList()
                .Where(a => basePrefixOf(a.web_url) == basePrefix)
                .Select(a => a.module_id)
                .Distinct()
                .ToList();

            List<long> allowedModuleIds;
            if (menuModules.Count > 0)
            {
                allowedModuleIds = menuModules;
            }
            else
            {
                // No menu covers this URL prefix (e.g. counter-billing POS endpoints,
                // controller actions without a menu). Fall back to the area's module
                // code set; a user holding any of those modules may proceed.
                var area = context.ActionDescriptor.RouteValues["area"] as string;
                allowedModuleIds = modulesForArea(area);
                if (allowedModuleIds == null)
                {
                    // Unmapped area (e.g. the counter POS API) or no area at all:
                    // authentication (global filter) is the only requirement.
                    return;
                }
            }

            var grantedModuleIds = getGrantedModuleIds(context);
            if (grantedModuleIds == null || !grantedModuleIds.Intersect(allowedModuleIds).Any())
            {
                context.Result = new ForbidResult();
            }
        }

        private static string basePrefixOf(string webUrl)
        {
            var url = webUrl.TrimEnd('/').ToLowerInvariant();
            var parts = url.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                return null;
            }
            return $"{parts[0]}/{parts[1]}";
        }

        // Area (lower-case) -> module ids. Resolved from the modules table: by
        // module_code set for the mapped areas, and by name for the back-office
        // 'setup' area (the seed has no Setup module; setup pages belong with
        // Administration).
        private List<long> modulesForArea(string area)
        {
            if (string.IsNullOrEmpty(area))
            {
                return null;
            }

            var code = area.ToLowerInvariant();
            List<long> moduleIds;
            switch (code)
            {
                case "billing":
                case "inventory":
                case "accounting":
                case "administration":
                    // module_code is not unique across modules (Billing and Utilities
                    // both use 'billing'), so collect the whole set.
                    moduleIds = _moduleRepo.getQueryable()
                        .Where(a => a.module_code.ToLower() == code)
                        .Select(a => a.module_id)
                        .ToList();
                    break;
                case "setup":
                    var adminModule = _moduleRepo.getByName("Administration");
                    moduleIds = adminModule == null ? new List<long>() : new List<long> { adminModule.module_id };
                    break;
                default:
                    return null;
            }
            return moduleIds;
        }

        // Modules granted to the logged-in user through any of their roles;
        // null when the identity cannot be resolved (treated as denied).
        private List<long> getGrantedModuleIds(AuthorizationFilterContext context)
        {
            var authenticationIdValue = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(authenticationIdValue) || !long.TryParse(authenticationIdValue, out long authenticationId))
            {
                return null;
            }

            var authentication = _authenticationRepo.getById(authenticationId);
            if (authentication == null)
            {
                return null;
            }

            List<long> roleIds = _userRoleRepo
                .getByTypeId(UserType.user, authentication.type_id)
                .Select(a => a.role_id)
                .ToList();

            return _rolePermissionMapRepo.getQueryable()
                .Where(a => roleIds.Contains(a.role_id))
                .Select(a => a.module_id)
                .Distinct()
                .ToList();
        }
    }
}
