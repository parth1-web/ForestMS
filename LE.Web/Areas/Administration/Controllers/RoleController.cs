using AutoMapper;
using LE.Entities.User;
using LE.Infrastructure.Dto;
using LE.Service.Repository.Interface;
using LE.Service.Services.Interface;
using LE.Web.Areas.Administration.FilterModel;
using LE.Web.Areas.Administration.Models;
using LE.Web.Areas.Administration.ViewModels;
using LE.Web.Helpers;
using LE.Web.LEPagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Web.Areas.Administration.Controllers
{
    [Authorize]
    [Area("administration")]
    [Route("administration/role")]
    public class RoleController : Controller
    {
        private readonly RoleRepository _roleRepo;
        private readonly RoleService _roleService;
        private readonly PaginatedMetaService _paginatedMetaService;
        private readonly ModuleRepository _moduleRepo;
        private readonly RolePermissionMapRepository _rolePermissionMapRepo;
        private readonly IMapper _mapper;

        public RoleController(RoleRepository roleRepo, IMapper mapper, RoleService roleService, PaginatedMetaService paginatedMetaService, ModuleRepository moduleRepo, RolePermissionMapRepository rolePermissionMapRepo)
        {
            _roleRepo = roleRepo;
            _mapper = mapper;
            _roleService = roleService;
            _paginatedMetaService = paginatedMetaService;
            _moduleRepo = moduleRepo;
            _rolePermissionMapRepo = rolePermissionMapRepo;
        }

        [Route("")]
        [Route("index", Name = "administration_role_index")]
        public IActionResult Index(RoleFilter filter)
        {
            try
            {
                var role = _roleRepo.getQueryable();
                if (!string.IsNullOrWhiteSpace(filter.name))
                {
                    role = role.Where(a => a.name.Contains(filter.name));
                }
                ViewBag.pagerInfo = _paginatedMetaService.GetMetaData(role.Count(), filter.page, filter.number_of_rows);

                var roles = role.OrderBy(a => a.role_id).Skip(filter.number_of_rows * (filter.page - 1)).Take(filter.number_of_rows).ToList();
                var roleIndexVM = getViewModelFrom(roles);
                return View(roleIndexVM);
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return Redirect("/home");
            }
        }

        [HttpGet]
        [Route("new")]
        public IActionResult add()
        {
            try
            {
                RoleModel roleModel = new RoleModel();
                var modules = _moduleRepo.getQueryable().Select(a => new
                {
                    a.module_id,
                    a.module_name
                }).ToList();

                roleModel.permission_datas = modules.Select(a => new PermissionModel()
                {
                    module_id = a.module_id,
                    module_name = a.module_name,
                    is_checked = false
                }).ToList();

                return View(roleModel);
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpPost]
        [Route("new")]
        public IActionResult add(RoleModel model, List<PermissionModel> permission)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RoleDto roleDto = new RoleDto()
                    {
                        name = model.name,
                        module_ids = permission.Where(a => a.is_checked).Select(a => a.module_id).ToList(),
                        is_active = model.is_enabled
                    };
                    _roleService.save(roleDto);
                    AlertHelper.setMessage(this, "Role saved successfully.", messageType.success);
                    return RedirectToAction("index");
                }
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
            }
            finally
            {
                model.permission_datas = permission;
            }
            return View(model);
        }

        [HttpGet]
        [Route("edit/{role_id}")]
        public IActionResult edit(long role_id)
        {
            try
            {
                Role role = _roleRepo.getById(role_id);
                RoleModel roleModel = _mapper.Map<RoleModel>(role);

                var modules = _moduleRepo.getQueryable().Select(a => new
                {
                    a.module_id,
                    a.module_name
                }).ToList();

                roleModel.permission_datas = modules.Select(a => new PermissionModel()
                {
                    module_id = a.module_id,
                    module_name = a.module_name,
                    is_checked = role.permissions.Where(b => b.module_id == a.module_id).Count() > 0
                }).ToList();

                RouteData.Values.Remove("role_id");
                return View(roleModel);
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }

        }

        [HttpPost]
        [Route("edit")]
        public IActionResult edit(RoleModel model, List<PermissionModel> permission)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RoleDto roleDto = new RoleDto()
                    {
                        role_id = model.role_id,
                        name = model.name,
                        module_ids = permission.Where(a => a.is_checked).Select(a => a.module_id).ToList(),
                        is_active = model.is_enabled
                    };
                    _roleService.update(roleDto);
                    return RedirectToAction("index");
                }
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
            }
            return View(model);
        }

        [HttpGet]
        [Route("enable/{role_id}")]
        public IActionResult enable(long role_id)
        {
            try
            {
                _roleService.enable(role_id);
                AlertHelper.setMessage(this, "Role enabled successfully.", messageType.success);
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("disable/{role_id}")]
        public IActionResult disable(long role_id)
        {
            try
            {
                _roleService.disable(role_id);
                AlertHelper.setMessage(this, "Role disabled successfully.", messageType.success);
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("delete/{role_id}")]
        public IActionResult delete(long role_id)
        {
            try
            {
                _roleService.delete(role_id);
                AlertHelper.setMessage(this, "Role deleted successfully.", messageType.success);
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
            }
            return RedirectToAction(nameof(Index));
        }

        private RoleIndexViewModel getViewModelFrom(List<Role> roles)
        {
            List<long> roleIds = roles.Select(a => a.role_id).Distinct().ToList();

            List<RolePermissionMap> rolePermissionMapsAllowedForRoles = _rolePermissionMapRepo.getQueryable().Where(a => roleIds.Contains(a.role_id)).ToList();

            List<long> moduleIdsAllowedForRoles = rolePermissionMapsAllowedForRoles.Select(a => a.module_id).ToList();

            var modulesAllowedForRoles = _moduleRepo.getQueryable().Where(a => moduleIdsAllowedForRoles.Contains(a.module_id)).Select(a => new
            {
                a.module_id,
                a.module_name
            }).ToList();


            RoleIndexViewModel vm = new RoleIndexViewModel();
            vm.role_details = new List<RoleDetailModel>();
            foreach (var role in roles)
            {
                var roleDetail = _mapper.Map<RoleDetailModel>(role);
                roleDetail.is_active = role.is_enabled;

                //list of module names
                List<long> moduleIdsForRole = rolePermissionMapsAllowedForRoles.Where(a => a.role_id == role.role_id).Select(a => a.module_id).ToList();


                roleDetail.permissions = modulesAllowedForRoles.Where(a => moduleIdsForRole.Contains(a.module_id)).Select(a => a.module_name).ToList();

                vm.role_details.Add(roleDetail);
            }
            return vm;
        }
    }
}
