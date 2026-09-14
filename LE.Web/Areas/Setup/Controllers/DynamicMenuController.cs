using LE.Entities.User;
using LE.Infrastructure.Dto;
using LE.Service.Repository.Interface;
using LE.Service.Services.Interface;
using LE.Web.Areas.Setup.FilterModel;
using LE.Web.Areas.Setup.ViewModels;
using LE.Web.Helpers;
using LE.Web.LEPagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Web.Areas.Setup.Controllers
{
    [Authorize]
    [Area("setup")]
    [Route("setup/menu")]
    public class DynamicMenuController : Controller
    {
        private readonly DynamicMenuService _dynamicMenuService;
        private readonly DynamicMenuRepository _dynamicMenuRepo;
        private readonly PaginatedMetaService _paginatedMetaService;
        private readonly ModuleRepository _moduleRepo;

        public DynamicMenuController(DynamicMenuService dynamicMenuService, DynamicMenuRepository dynamicMenuRepo, PaginatedMetaService paginatedMetaService, ModuleRepository moduleRepo)
        {
            _dynamicMenuRepo = dynamicMenuRepo;
            _dynamicMenuService = dynamicMenuService;
            _paginatedMetaService = paginatedMetaService;
            _moduleRepo = moduleRepo;
        }

        [HttpGet]
        [Route("")]
        [Route("index")]
        public IActionResult Index([FromQuery] DynamicMenuFilter filter = null)
        {
            try
            {
                var dynamicMenuQueryable = _dynamicMenuRepo.getQueryable();
                if (!string.IsNullOrWhiteSpace(filter.name))
                {
                    dynamicMenuQueryable = dynamicMenuQueryable.Where(a => a.menu_name.Contains(filter.name));
                    ViewBag.menu_name = filter.name;
                }

                ViewBag.pagerInfo = _paginatedMetaService.GetMetaData(dynamicMenuQueryable.Count(), filter.page, filter.number_of_rows);

                dynamicMenuQueryable = dynamicMenuQueryable.Skip(filter.number_of_rows * (filter.page - 1)).Take(filter.number_of_rows);


                var menus = dynamicMenuQueryable.ToList();
                List<DynamicMenuViewModel> responseData = getOnlyRequiredDatas(menus);
                return View(responseData);

            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return View(new List<DynamicMenuViewModel>());
            }
        }

        [HttpGet]
        [Route("new")]
        public IActionResult add()
        {
            try
            {
                var modules = _moduleRepo.getQueryable().Select(a => new
                {
                    a.module_id,
                    a.module_name
                }).ToList();

                ViewBag.modules = new SelectList(modules, "module_id", "module_name");

                var parentMenus = _dynamicMenuRepo.getQueryable().Select(a => new
                {
                    dynamic_menu_id = (long?)a.dynamic_menu_id,
                    menu_name = a.menu_name
                }).ToList();

                var initialItem = new
                {
                    dynamic_menu_id = (long?)null,
                    menu_name = "select parent menu",

                };
                parentMenus.Insert(0, initialItem);

                ViewBag.parent_menus = new SelectList(parentMenus, "dynamic_menu_id", "menu_name");

                return View();
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpPost]
        [Route("new")]
        public IActionResult add(DynamicMenuDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    _dynamicMenuService.save(model);
                    AlertHelper.setMessage(this, "Menu saved successfully.", messageType.success);
                    return RedirectToAction("index");
                }
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
            }
            finally
            {
                var modules = _moduleRepo.getQueryable().Select(a => new
                {
                    a.module_id,
                    a.module_name
                }).ToList();

                ViewBag.modules = new SelectList(modules, "module_id", "module_name");

                var parentMenus = _dynamicMenuRepo.getQueryable().Select(a => new
                {
                    a.dynamic_menu_id,
                    a.menu_name
                }).ToList();

                ViewBag.parent_menus = new SelectList(parentMenus, "dynamic_menu_id", "menu_name");
            }
            return View(model);
        }

        [HttpGet]
        [Route("edit/{dynamic_menu_id}")]
        public IActionResult edit(long dynamic_menu_id)
        {
            try
            {
                DynamicMenu dynamicMenu = _dynamicMenuRepo.getById(dynamic_menu_id);
                RouteData.Values.Remove("dynamic_menu_id");

                DynamicMenuDto dto = new DynamicMenuDto()
                {
                    dynamic_menu_id = dynamicMenu.dynamic_menu_id,
                    module_id = dynamicMenu.module_id,
                    parent_menu_id = dynamicMenu.parent_menu_id,
                    menu_name = dynamicMenu.menu_name,
                    icon = dynamicMenu.icon,
                    web_url = dynamicMenu.web_url,
                    api_url = dynamicMenu.api_url,
                    display_order = dynamicMenu.display_order
                };

                var modules = _moduleRepo.getQueryable().Select(a => new
                {
                    a.module_id,
                    a.module_name
                }).ToList();

                ViewBag.modules = new SelectList(modules, "module_id", "module_name");

                var parentMenus = _dynamicMenuRepo.getQueryable().Select(a => new
                {
                    dynamic_menu_id = (long?)a.dynamic_menu_id,
                    menu_name = a.menu_name
                }).ToList();

                var initialItem = new
                {
                    dynamic_menu_id = (long?)null,
                    menu_name = "select parent menu",

                };
                parentMenus.Insert(0, initialItem);

                ViewBag.parent_menus = new SelectList(parentMenus, "dynamic_menu_id", "menu_name");

                return View(dto);
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }

        }

        [HttpPost]
        [Route("edit")]
        public IActionResult edit(DynamicMenuDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _dynamicMenuService.update(model);
                    return RedirectToAction("index");
                }
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
            }
            finally
            {
                var modules = _moduleRepo.getQueryable().Select(a => new
                {
                    a.module_id,
                    a.module_name
                }).ToList();

                ViewBag.modules = new SelectList(modules, "module_id", "module_name");

                var parentMenus = _dynamicMenuRepo.getQueryable().Select(a => new
                {
                    a.dynamic_menu_id,
                    a.menu_name
                }).ToList();

                ViewBag.parent_menus = new SelectList(parentMenus, "dynamic_menu_id", "menu_name");
            }
            return View(model);
        }

        private List<DynamicMenuViewModel> getOnlyRequiredDatas(List<DynamicMenu> menus)
        {
            List<long> distinctModuleIdsUsedInMenus = menus.Select(a => a.module_id).Distinct().ToList();
            var modules = _moduleRepo.getQueryable().Where(a => distinctModuleIdsUsedInMenus.Contains(a.module_id)).Select(a => new
            {
                a.module_id,
                a.module_name
            }).ToList();

            List<long> parentMenuIdsUsedInMenus = menus.Where(a => a.parent_menu_id.HasValue).Select(a => (long)a.parent_menu_id).Distinct().ToList();

            var parentMenus = _dynamicMenuRepo.getQueryable().Where(a => parentMenuIdsUsedInMenus.Contains(a.dynamic_menu_id)).Select(a => new
            {
                a.dynamic_menu_id,
                a.menu_name
            }).ToList();


            return menus.Select(a => new DynamicMenuViewModel()
            {
                dynamic_menu_id = a.dynamic_menu_id,
                module_name = modules.Where(b => a.module_id == b.module_id).Single().module_name,
                parent_menu_name = parentMenus.Where(b => a.parent_menu_id == b.dynamic_menu_id).SingleOrDefault()?.menu_name,
                menu_name = a.menu_name,
                api_url = a.api_url,
                web_url = a.web_url,
                display_order = a.display_order
            }).ToList();
        }

        [HttpGet]
        [Route("delete/{dynamic_menu_id}")]
        public IActionResult delete(long dynamic_menu_id)
        {
            try
            {
                _dynamicMenuService.delete(dynamic_menu_id);
                AlertHelper.setMessage(this, "Menu deleted successfully.");
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }
    }
}