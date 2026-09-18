using LE.Entities.User;
using LE.Infrastructure.Dto;
using LE.Service.Repository.Interface;
using LE.Service.Services.Interface;
using LE.Web.Areas.Setup.FilterModel;
using LE.Web.Helpers;
using LE.Web.LEPagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Web.Areas.Setup.Controllers
{
    [Authorize]
    [Area("setup")]
    [Route("setup/module")]
    public class ModuleController : Controller
    {
        private readonly ModuleService _moduleService;
        private readonly ModuleRepository _moduleRepo;
        private readonly PaginatedMetaService _paginatedMetaService;

        public ModuleController(ModuleService moduleService, ModuleRepository moduleRepo, PaginatedMetaService paginatedMetaService)
        {
            _moduleService = moduleService;
            _moduleRepo = moduleRepo;
            _paginatedMetaService = paginatedMetaService;
        }

        [HttpGet]
        [Route("")]
        [Route("index", Name = "setup_module_index")]
        public IActionResult Index([FromQuery]ModuleFilter filter = null)
        {
            try
            {
                var moduleQueryable = _moduleRepo.getQueryable();
                if (!string.IsNullOrWhiteSpace(filter.name))
                {
                    moduleQueryable = moduleQueryable.Where(a => a.module_name.Contains(filter.name));
                }

                ViewBag.pagerInfo = _paginatedMetaService.GetMetaData(moduleQueryable.Count(), filter.page, filter.number_of_rows);

                moduleQueryable = moduleQueryable.OrderBy(a => a.module_id).Skip(filter.number_of_rows * (filter.page - 1)).Take(filter.number_of_rows);


                var modules = moduleQueryable.ToList();
                List<ModuleDto> responseData = getOnlyRequiredDatas(modules);
                return View(responseData);

            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return View(new List<ModuleDto>());
            }
        }

        [HttpGet]
        [Route("new")]
        public IActionResult add()
        {
            try
            {
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
        public IActionResult add(ModuleDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    _moduleService.save(model);
                    AlertHelper.setMessage(this, "Module saved successfully.", messageType.success);
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
        [Route("edit/{module_id}")]
        public IActionResult edit(long module_id)
        {
            try
            {
                Module module = _moduleRepo.getById(module_id);
                RouteData.Values.Remove("module_id");

                ModuleDto dto = new ModuleDto()
                {
                    module_id = module.module_id,
                    module_code = module.module_code,
                    module_name = module.module_name,
                    display_icon=module.display_icon
                };

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
        public IActionResult edit(ModuleDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                  
                    _moduleService.update(model);
                    return RedirectToAction("index");
                }
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
            }
            return View(model);
        }

        private List<ModuleDto> getOnlyRequiredDatas(List<Module> modules)
        {
            return modules.Select(a => new ModuleDto()
            {
                module_id = a.module_id,
                module_name = a.module_name,
                module_code = a.module_code,
                display_icon=a.display_icon
            }).ToList();
        }
    }
}