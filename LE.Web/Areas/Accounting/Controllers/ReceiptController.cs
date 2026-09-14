using AutoMapper;
using DateConverter.Core.Service_Factory;
using LE.Account.Common.Enums;
using LE.Account.Infrastructure.Dto;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Account.Service.Services.Interface;
using LE.Common.Enums;
using LE.Service.Repository.Interface;
using LE.Web.Areas.Accounting.Models;
using LE.Web.Areas.Accounting.ViewModels;
using LE.Web.Controllers;
using LE.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Web.Areas.Accounting.Controllers
{
    [Authorize]
    [Area("accounting")]
    [Route("accounting/receipt")]
    public class ReceiptController : BaseController
    {
        private LedgerRepository _ledgerRepo;
        private ReceiptRepository _receiptRepo;
        private IMapper _mapper;
        private ReceiptService _receiptService;
        private OrganizationSetupRepository _organizationSetupRepository;

        public ReceiptController(LedgerRepository ledgerRepo, IMapper mapper, ReceiptService receiptService, ReceiptRepository receiptRepo, OrganizationSetupRepository organizationSetupRepository)
        {
            _organizationSetupRepository = organizationSetupRepository;
            _ledgerRepo = ledgerRepo;
            _mapper = mapper;
            _receiptService = receiptService;
            _receiptRepo = receiptRepo;
        }

        [Route("")]
        [Route("index")]
        public IActionResult index(ReceiptIndexViewModel vm)
        {
            vm = getViewModel(vm);
            return View(vm);
        }

        [Route("report")]
        public IActionResult report(ReceiptIndexViewModel vm)
        {
            ViewBag.organizationName = _organizationSetupRepository.getByKey(OrganizationSetup.Organization_Name.ToString()).value;
            ViewBag.Address = _organizationSetupRepository.getByKey(OrganizationSetup.Address.ToString()).value;

            vm = getViewModel(vm);
            return View(vm);
        }

        [HttpGet]
        [Route("receipt-pdf")]
        public ViewAsPdf receiptPdf(ReceiptIndexViewModel vm)
        {
            vm = getViewModel(vm);
            return new ViewAsPdf("report", vm);
        }

        [Route("bill/{receipt_id}")]
        public IActionResult bill(long receipt_id)
        {

            ViewBag.OrganizationName = _organizationSetupRepository.getByKey(OrganizationSetup.Organization_Name.ToString()).value;
            ViewBag.Address = _organizationSetupRepository.getByKey(OrganizationSetup.Address.ToString()).value;

            ViewBag.Logo = _organizationSetupRepository.getByKey(OrganizationSetup.Logo.ToString()).value;

            ViewBag.User = _userRepo.getById(getLoggedInUserId()).full_name;
            ViewBag.printDate = DateConverterFactory.getDateConverterService().ToBS(DateTime.Now.Date, DateFormats.yMd).getFormattedDate();
            ViewBag.printTime = DateTime.Now.ToShortTimeString();

            var receipt = _receiptRepo.getById(receipt_id);

            ReceiptModel model = _mapper.Map<ReceiptModel>(receipt);
            return View(model);
        }

        private ReceiptIndexViewModel getViewModel(ReceiptIndexViewModel vm)
        {
            var dateConverterService = DateConverterFactory.getDateConverterService();
            var startDate = dateConverterService.ToAD(vm.start_date).getFormattedDate();
            var endDate = dateConverterService.ToAD(vm.end_date).getFormattedDate();

            var receipts = _receiptRepo.getQueryable().Where(a => a.transaction_date.Date >= startDate && a.transaction_date.Date <= endDate && a.is_cancelled == vm.is_cancelled).ToList();

            vm.receipts = new List<ReceiptDetails>();
            foreach (var receipt in receipts)
            {
                var details = new ReceiptDetails();
                details.receipt_from_name = _ledgerRepo.getById(receipt.receipt_from
                    ).name;
                details.receipt_to_name = _ledgerRepo.getById(receipt.receipt_to).name;
                details.nep_transaction_date = receipt.nep_transaction_date;
                details.remarks = receipt.remarks;
                details.discount = receipt.discount;
                details.receipt_id = receipt.receipt_id;
                details.customer_name = receipt.customer_name;
                details.amount = receipt.amount;
                details.cheque_date = receipt.cheque_date;
                details.cheque_no = receipt.cheque_no;
                details.is_cancelled = receipt.is_cancelled;
                vm.receipts.Add(details);

            }
            return vm;
        }

        [HttpGet]
        [Route("new")]
        public IActionResult add()
        {
            ReceiptModel receiptModel = new ReceiptModel();
            var ledgers = _ledgerRepo.getQueryable();
            var receipt_from_Ledgers = ledgers.Where(l => l.ledger_group.ledger_group_type != LedgerGroupType.asset);
            ViewBag.receiptFromLedgers = new SelectList(receipt_from_Ledgers, "ledger_id", "name"); ;

            var receipt_to_ledgers = ledgers.Where(l => l.ledger_group.ledger_group_type == LedgerGroupType.asset);
            ViewBag.receiptToLedgers = new SelectList(receipt_to_ledgers, "ledger_id", "name");
            return View(receiptModel);
        }

        [HttpPost]
        [Route("new")]
        public IActionResult add(ReceiptModel receiptModel)
        {
            try
            {
                ReceiptDto receiptDto = getReceiptDtoFromDetails(receiptModel);
                receiptDto.user_id = getLoggedInUserId();
                var receiptId = _receiptService.makeReceipt(receiptDto);
                AlertHelper.setMessage(this, "Receipt made successfully.", messageType.success);
                return RedirectToAction("add");
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("add");
            }
        }

        private ReceiptDto getReceiptDtoFromDetails(ReceiptModel receiptModel)
        {
            var receiptDto = _mapper.Map<ReceiptDto>(receiptModel);
            var dateConverterService = DateConverterFactory.getDateConverterService();
            var dateFunction = DateFunctionsFactory.getDateFunctionsService();
            var currentDate = dateFunction.getDateTimeByTimeZone();

            receiptDto.transaction_date = dateConverterService.ToAD(receiptModel.nep_transaction_date).getFormattedDate().Add(currentDate.TimeOfDay);
            return receiptDto;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("cancel/{receipt_id}")]
        public IActionResult cancel(long receipt_id)
        {
            try
            {
                long userId = getLoggedInUserId();
                _receiptService.cancel(receipt_id, userId);
                AlertHelper.setMessage(this, "Receipt cancelled Successfully.", messageType.success);
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