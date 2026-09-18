using AutoMapper;
using DateConverter.Core.Service_Factory;
using LE.Account.Entities;
using LE.Account.Infrastructure.Dto;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Account.Service.Services.Interface;
using LE.Web.Areas.Accounting.FilterModel;
using LE.Web.Areas.Accounting.Models;
using LE.Web.Areas.Accounting.ViewModels;
using LE.Web.Controllers;
using LE.Web.Helpers;
using LE.Web.LEPagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Web.Areas.Accounting.Controllers
{
    [Authorize]
    [Area("accounting")]
    [Route("accounting/ledgers")]
    public class LedgerController : BaseController
    {
        private readonly LedgerRepository _ledgerRepo;
        private readonly LedgerGroupRepository _ledgerGroupRepo;
        private readonly LedgerService _ledgerService;
        private readonly IMapper _mapper;
        private readonly TransactionDetailService _transactionDetailService;
        private PaginatedMetaService _paginatedMetaService;

        public LedgerController(LedgerRepository ledgerRepo, LedgerGroupRepository ledgerGroupRepo, LedgerService ledgerSercvice, PaginatedMetaService paginatedMetaService, IMapper mapper, TransactionDetailService transactionDetailService)
        {
            _ledgerRepo = ledgerRepo;
            _ledgerGroupRepo = ledgerGroupRepo;
            _ledgerService = ledgerSercvice;
            _paginatedMetaService = paginatedMetaService;
            _mapper = mapper;
            _transactionDetailService = transactionDetailService;
        }

        [Route("")]
        [Route("index", Name = "accounting_ledger_index")]
        public IActionResult Index(LedgerFilter filter)
        {
            var ledger = _ledgerRepo.getQueryable()
                .Include(a => a.ledger_group);

            IQueryable<Ledger> query = ledger;
            if (!string.IsNullOrWhiteSpace(filter.name))
            {
                query = query.Where(a => a.name.Contains(filter.name));
            }

            ViewBag.pagerInfo = _paginatedMetaService.GetMetaData(query.Count(), filter.page, filter.number_of_rows);
            var pagedResult = query.OrderByDescending(a => a.ledger_id).Skip(filter.number_of_rows * (filter.page - 1)).Take(filter.number_of_rows);

            var ledgers = pagedResult.ToList();
            LedgerIndexViewModel ledgerIndexVM = getViewModelFrom(ledgers);
            return View(ledgerIndexVM);
        }

        private LedgerIndexViewModel getViewModelFrom(List<Ledger> ledgers)
        {
            LedgerIndexViewModel VM = new LedgerIndexViewModel();
            VM.ledger_details = new List<LedgerDetailModel>();
            foreach (var ledger in ledgers)
            {
                LedgerDetailModel model = new LedgerDetailModel();
                model.ledger_id = ledger.ledger_id;
                model.ledger_group_id = ledger.ledger_group_id;
                model.name = ledger.name;
                model.nep_created_date = ledger.nep_created_date;
                model.created_date = ledger.created_date;
                model.code = ledger.code;
                model.ledger_group = ledger.ledger_group;
                // var ledgerDetail = _mapper.Map<LedgerDetailModel>(ledger);

                model.balance = _transactionDetailService.getOldBalance(ledger.ledger_id, DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone().Date);
                VM.ledger_details.Add(model);
            }
            return VM;
        }

        [HttpGet]
        [Route("new")]
        public IActionResult add()
        {
            LedgerModel ledgerModel = new LedgerModel();
            var ledgerGroups = _ledgerGroupRepo.getQueryable().ToList();
            ViewBag.ledgers = new SelectList(ledgerGroups, "ledger_group_id", "name");
            return View(ledgerModel);
        }

        [HttpPost]
        [Route("new")]
        public IActionResult add(LedgerModel ledgerModel)
        {
            var ledgerGroups = _ledgerGroupRepo.getQueryable().ToList();
            ViewBag.ledgers = new SelectList(ledgerGroups, "ledger_group_id", "name");
            try
            {
                if (ModelState.IsValid)
                {
                    LedgerDto ledgerDto = getLedgerDtoFromProvidedData(ledgerModel);
                    _ledgerService.save(ledgerDto);
                    AlertHelper.setMessage(this, "Ledger created successfully.");
                    return RedirectToAction("index");
                }

            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
            return View(ledgerModel);
        }
        [HttpGet]
        [Route("delete/{ledger_id}")]
        public IActionResult delete(long ledger_id)
        {
            try
            {
                _ledgerService.delete(ledger_id);
                AlertHelper.setMessage(this, "Ledger deleted successfully.");
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpGet]
        [Route("edit/{ledger_id}")]
        public IActionResult edit(long ledger_id)
        {
            try
            {
                var ledgerGroups = _ledgerGroupRepo.getQueryable().ToList();
                ViewBag.ledgers = new SelectList(ledgerGroups, "ledger_group_id", "name");
                var ledgerDetails = _ledgerRepo.getById(ledger_id);
                var ledgerModel = getLederModelFromLedgerDetails(ledgerDetails);
                return View(ledgerModel);
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpPost]
        [Route("edit")]
        public IActionResult edit(LedgerModel ledgerModel)
        {
            var ledgerGroups = _ledgerGroupRepo.getQueryable().ToList();
            ViewBag.ledgers = new SelectList(ledgerGroups, "ledger_group_id", "name");
            try
            {
                if (ModelState.IsValid)
                {
                    LedgerDto ledgerDto = getLedgerDtoFromProvidedData(ledgerModel);
                    _ledgerService.update(ledgerDto);
                    AlertHelper.setMessage(this, "Ledger Updated successfully.", messageType.success);
                    return RedirectToAction("index");
                }
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
            return View(ledgerModel);
        }

        private LedgerDto getLedgerDtoFromProvidedData(LedgerModel ledgerModel)
        {
            LedgerDto ledgerDto = _mapper.Map<LedgerDto>(ledgerModel);
            ledgerDto.user_id = getLoggedInUserId();
            return ledgerDto;
        }

        private LedgerModel getLederModelFromLedgerDetails(Ledger ledgerDetails)
        {
            LedgerModel ledgerModel = _mapper.Map<LedgerModel>(ledgerDetails);
            return ledgerModel;
        }

        [HttpGet]
        [Route("get-ledger-end-balance/{ledger_id}")]
        public IActionResult getLedgerEndBalance(long ledger_id)
        {
            decimal endBalance = _transactionDetailService.getOldBalance(ledger_id, DateTime.Now);
            return Json(endBalance);
        }

        [HttpGet]
        [Route("update-balance/{ledger_id}")]
        public IActionResult updateBalance(long ledger_id)
        {
            try
            {
                _transactionDetailService.updateBalanceAmount(ledger_id);
                AlertHelper.setMessage(this, "Success", messageType.success);
                return RedirectToAction("index");
            }
            catch (Exception e)
            {
                ExceptionMessageHelper.setMessage(this, e, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpGet]
        [Route("update-balance-all")]
        public IActionResult updateBalanceAll()
        {
            try
            {
                var allLedger = _ledgerRepo.getAll().Select(a => a.ledger_id).ToList();

                foreach (var ledger in allLedger)
                {
                    _transactionDetailService.updateBalanceAmount(ledger);
                }
                AlertHelper.setMessage(this, "Success", messageType.success);
                return RedirectToAction("index");
            }
            catch (Exception e)
            {
                ExceptionMessageHelper.setMessage(this, e, messageType.error);
                return RedirectToAction("index");
            }
        }
    }
}