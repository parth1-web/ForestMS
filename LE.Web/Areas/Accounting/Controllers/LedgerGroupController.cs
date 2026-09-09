using AutoMapper;
using LE.Account.Entities;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Account.Service.Services.Interface;
using LE.Web.Areas.Accounting.FilterModel;
using LE.Web.Areas.Accounting.Models;
using LE.Web.Areas.Accounting.ViewModels;
using LE.Web.Helpers;
using LE.Web.LEPagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Web.Areas.Accounting.Controllers
{
    [Authorize]
    [Area("accounting")]
    [Route("accounting/groups")]
    public class LedgerGroupController : Controller
    {
        private readonly LedgerGroupService _ledgerGroupService;
        private LedgerGroupRepository _ledgerGroupRepo;
        private readonly IMapper _mapper;
        private PaginatedMetaService _paginatedMetaService;


        public LedgerGroupController(LedgerGroupService ledgerGroupService, LedgerGroupRepository ledgerGroupRepo, IMapper mapper, PaginatedMetaService paginatedMetaService)
        {
            _ledgerGroupService = ledgerGroupService;
            _ledgerGroupRepo = ledgerGroupRepo;
            _paginatedMetaService = paginatedMetaService;
            _mapper = mapper;
        }

        public IActionResult Index(LedgerGroupFilter filter = null)
        {
            var ledgerGroup = _ledgerGroupRepo.getQueryable();

            if (!string.IsNullOrWhiteSpace(filter.name))
            {
                ledgerGroup = ledgerGroup.Where(a => a.name.Contains(filter.name));
            }

            ViewBag.pagerInfo = _paginatedMetaService.GetMetaData(ledgerGroup.Count(), filter.page, filter.number_of_rows);
            ledgerGroup = ledgerGroup.OrderByDescending(a => a.ledger_group_id).Skip(filter.number_of_rows * (filter.page - 1)).Take(filter.number_of_rows);

            var ledgerGroups = ledgerGroup.ToList();
            LedgerGroupIndexViewModel ledgerGroupIndexVM = getViewModelFrom(ledgerGroups);
            return View(ledgerGroupIndexVM);
        }

        [HttpGet]
        [Route("new")]
        public IActionResult add()
        {
            LedgerGroupModel ledgerGroupModel = new LedgerGroupModel();
            var ledgerGroups = _ledgerGroupRepo.getQueryable().ToList();
            ViewBag.ledgerGroups = new SelectList(ledgerGroups, "ledger_group_id", "name");
            return View(ledgerGroupModel);
        }

        [HttpPost]
        [Route("new")]
        public IActionResult add(LedgerGroupModel ledgerGroupModel)
        {
            var ledgerGroups = _ledgerGroupRepo.getQueryable().ToList();
            ViewBag.ledgerGroups = new SelectList(ledgerGroups, "ledger_group_id", "name");
            try
            {
                if (ledgerGroupModel.parent_ledger_group_id <= 0)
                    ModelState.Remove("parent_ledger_group_id");
                if (ledgerGroupModel.ledger_group_type <= 0)
                    ModelState.Remove("ledger_group_type");
                if (ModelState.IsValid)
                {
                    LedgerGroup ledgerGroup = new LedgerGroup()
                    {
                        name = ledgerGroupModel.name,
                        parent_ledger_group_id = ledgerGroupModel.parent_ledger_group_id,
                        ledger_group_type = ledgerGroupModel.ledger_group_type
                    };
                    _ledgerGroupService.save(ledgerGroup);
                    AlertHelper.setMessage(this, "Ledger Group saved successfully.", messageType.success);
                    return RedirectToAction("index");
                }
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
                return RedirectToAction("index");
            }
            return View(ledgerGroupModel);
        }

        [HttpGet]
        [Route("delete/{ledger_group_id}")]
        public IActionResult delete(long ledger_group_id)
        {
            try
            {
                _ledgerGroupService.delete(ledger_group_id);
                AlertHelper.setMessage(this, "Ledger Group deleted succssfully!", messageType.success);
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpGet]
        [Route("edit/{ledger_group_id}")]
        public IActionResult edit(long ledger_group_id)
        {
            try
            {

                var ledgerGroup = _ledgerGroupRepo.getById(ledger_group_id);
                var userModel = getLedgerGroupModelFromLedgerGroupDetail(ledgerGroup);
                var ledgerGroups = _ledgerGroupRepo.getQueryable().ToList();
                ViewBag.ledgerGroups = new SelectList(ledgerGroups, "ledger_group_id", "name");
                return View(userModel);
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
                return RedirectToAction("index");
            }

        }

        [HttpPost]
        [Route("edit")]
        public IActionResult edit(LedgerGroupModel ledgerGroupModel)
        {
            var ledgerGroups = _ledgerGroupRepo.getQueryable().ToList();
            ViewBag.ledgerGroups = new SelectList(ledgerGroups, "ledger_group_id", "name");
            if (ledgerGroupModel.parent_ledger_group_id <= 0)
                ModelState.Remove("parent_ledger_group_id");
            if (ledgerGroupModel.ledger_group_type <= 0)
                ModelState.Remove("ledger_group_type");
            try
            {
                if (ModelState.IsValid)
                {
                    LedgerGroup ledgerGroup = new LedgerGroup()
                    {
                        ledger_group_id = ledgerGroupModel.ledger_group_id,
                        name = ledgerGroupModel.name,
                        parent_ledger_group_id = ledgerGroupModel.parent_ledger_group_id,
                        ledger_group_type = ledgerGroupModel.ledger_group_type,
                        code = ledgerGroupModel.code

                    };
                    _ledgerGroupService.update(ledgerGroup);
                    AlertHelper.setMessage(this, "Ledger Group Updated Successfully", messageType.success);
                    return RedirectToAction("index");
                }

            }
            catch (Exception ex)
            {
                //throw ex;
                AlertHelper.setMessage(this, ex.Message, messageType.error);
                return RedirectToAction("index");
            }
            return View(ledgerGroupModel);
        }
        private LedgerGroupIndexViewModel getViewModelFrom(List<LedgerGroup> ledgerGroups)
        {
            LedgerGroupIndexViewModel vm = new LedgerGroupIndexViewModel();
            vm.ledger_group_details = new List<LedgerGroupDetailModel>();
            foreach (var ledgerGroup in ledgerGroups)
            {

                LedgerGroupDetailModel model = new LedgerGroupDetailModel();
                model.is_custom = ledgerGroup.is_custom;
                model.ledgers = ledgerGroup.ledgers;
                model.ledger_group_id = ledgerGroup.ledger_group_id;
                model.ledger_group_type = ledgerGroup.ledger_group_type;
                model.name = ledgerGroup.name;
                model.parent_ledger_group_id = ledgerGroup.parent_ledger_group_id;
                model.parent_group_name = "Primary";
                if (ledgerGroup.parent_ledger_group_id > 0)
                {
                    model.parent_group_name = _ledgerGroupRepo.getById(ledgerGroup.parent_ledger_group_id).name;
                }

                vm.ledger_group_details.Add(model);
            }

            return vm;
        }
        private LedgerGroupModel getLedgerGroupModelFromLedgerGroupDetail(LedgerGroup ledgerGroupDetail)
        {
            var ledgerGroupModel = _mapper.Map<LedgerGroupModel>(ledgerGroupDetail);
            return ledgerGroupModel;
        }
    }


}