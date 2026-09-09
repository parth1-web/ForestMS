using AutoMapper;
using DateConverter.Core.Service_Factory;
using LE.Account.Common.Enums;
using LE.Account.Entities;
using LE.Account.Infrastructure.Dto;
using LE.Account.Infrastructure.Reports.Reporter;
using LE.Account.Infrastructure.Reports.Reporter.ValueObjects;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Account.Service.Services.FinancialYearService;
using LE.Account.Service.Services.Interface;
using LE.Common.Enums;
using LE.Common.Library.DateConverter.Entity;
using LE.Service.Repository.Interface;
using LE.Service.Services.Interface;
using LE.Web.Areas.Accounting.Models;
using LE.Web.Areas.Accounting.ViewModels;
using LE.Web.Areas.Inventory.Controllers;
using LE.Web.Controllers;
using LE.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Web.Areas.Accounting.Controllers
{
	[Authorize]
	[Area("accounting")]
	[Route("accounting/report")]
	public class ReportController : BaseController
	{
		public string org_name, address, logo;
		private readonly ReportService _reportService;
		private IMapper _mapper;
		private readonly LedgerRepository _ledgerRepo;
		private readonly LedgerGroupRepository _ledgerGroupRepo;
		private readonly OrganizationSetupRepository _orgSetupRepo;
		private readonly AccountSettingsRepository _accountSettingRepo;
		private readonly TransactionDetailRepository _transactionDetailRepo;
		private readonly ReceiptRepository _receiptRepository;
		private readonly LedgerIdProvider _ledgerIdProvider;
		private readonly LedgerSetupRepository _ledgerSetupRepository;
		private readonly TransactionRepository _transactionRepo;
		private readonly TransactionDetailService _transactionDetailService;
		private readonly IAccountingReportReporter _accountingReportReporter;
		private readonly FinancialYearServices _financialYearService;
		private readonly DateConverterService _dateConverter;

		public ReportController(ReportService reportService, IMapper mapper, LedgerRepository ledgerRepo, LedgerGroupRepository ledgerGroupRepo, OrganizationSetupRepository orgSetupRepo, AccountSettingsRepository accountSettingRepo, TransactionDetailRepository transactionDetailRepo, ReceiptRepository receiptRepository, LedgerIdProvider ledgerIdProvider, LedgerSetupRepository ledgerSetupRepository, TransactionRepository transactionRepo, TransactionDetailService transactionDetailService, IAccountingReportReporter accountingReportReporter, FinancialYearServices financialYearService, DateConverterService dateConverter)
		{
			_reportService = reportService;
			_mapper = mapper;
			_ledgerRepo = ledgerRepo;
			_ledgerGroupRepo = ledgerGroupRepo;
			_orgSetupRepo = orgSetupRepo;
			_accountSettingRepo = accountSettingRepo;
			_transactionDetailRepo = transactionDetailRepo;
			_receiptRepository = receiptRepository;
			_ledgerIdProvider = ledgerIdProvider;
			_ledgerSetupRepository = ledgerSetupRepository;
			_transactionRepo = transactionRepo;
			_transactionDetailService = transactionDetailService;
			_accountingReportReporter = accountingReportReporter;
			_financialYearService = financialYearService;
			org_name = _orgSetupRepo.getByKey(OrganizationSetup.Organization_Name.ToString())?.value;
			address = _orgSetupRepo.getByKey(OrganizationSetup.Address.ToString())?.value;
			logo = _orgSetupRepo.getByKey(OrganizationSetup.Logo.ToString())?.value;
			_dateConverter = dateConverter;
		}

		[HttpGet]
		[Route("ledger-group-report")]
		public IActionResult LedgerGroupReport(ListLedgerIndexViewModel ledgerVM)
		{
			var fiscalYear = _transactionDetailService.GetRunningFinancialYear().Result;
			var nepFiscalStartDate = _dateConverter.toBS(fiscalYear.StartDate.Date);
			var nepFiscalEndDate = _dateConverter.toBS(fiscalYear.EndDate.Date);

			if (ledgerVM.start_date == null)
			{
				ledgerVM.start_date = nepFiscalStartDate;
			}

			if (ledgerVM.end_date == null)
			{
				ledgerVM.end_date = nepFiscalEndDate;
			}

			DateTime start_date = _dateConverter.toAD(ledgerVM.start_date);
			DateTime end_date = _dateConverter.toAD(ledgerVM.end_date);
			var ledgerGroup = _ledgerGroupRepo.getQueryable().ToList();
			ViewBag.ledgerGroups = new SelectList(ledgerGroup, "ledger_group_id", "name");
			var ledgerGroups = _ledgerGroupRepo.getQueryable().Where(a => a.ledger_group_id == ledgerVM.ledger_group_id).ToList();
			ListLedgerIndexViewModel viewModel = getViewModelFrom(ledgerGroups, start_date, end_date);
			if (ledgerVM.all == "on")
			{
				ledgerGroups = _ledgerGroupRepo.getAll();
				viewModel = getViewModelFrom(ledgerGroups, start_date, end_date);
				viewModel.all = "on";
			}
			viewModel.start_date = ledgerVM.start_date;
			viewModel.end_date = ledgerVM.end_date;
			return View(viewModel);
		}

		[HttpGet]
		[Route("ledger-group-report-print")]
		public IActionResult ledgerGroupReportPrint(ListLedgerIndexViewModel ledgerVM)
		{
			var fiscalYear = _transactionDetailService.GetRunningFinancialYear().Result;
			var nepFiscalStartDate = _dateConverter.toBS(fiscalYear.StartDate);
			var nepFiscalEndDate = _dateConverter.toBS(fiscalYear.EndDate);

			if (ledgerVM.start_date == null)
			{
				ledgerVM.start_date = nepFiscalStartDate;
			}

			if (ledgerVM.end_date == null)
			{
				ledgerVM.end_date = nepFiscalEndDate;
			}
			var dateService = DateConverterFactory.getDateConverterService();
			DateTime start_date = dateService.ToAD(ledgerVM.start_date).getFormattedDate();
			DateTime end_date = dateService.ToAD(ledgerVM.end_date).getFormattedDate();
			var ledgerGroups = _ledgerGroupRepo.getQueryable().Where(a => a.ledger_group_id == ledgerVM.ledger_group_id).ToList();
			ListLedgerIndexViewModel viewModel = getViewModelFrom(ledgerGroups, start_date, end_date);
			if (ledgerVM.all == "on")
			{
				ledgerGroups = _ledgerGroupRepo.getAll();
				viewModel = getViewModelFrom(ledgerGroups, start_date, end_date);
				viewModel.all = "on";
			}
			viewModel.organization_name = org_name;
			viewModel.logo = logo;
			viewModel.address = address;
			viewModel.start_date = ledgerVM.start_date;
			viewModel.end_date = ledgerVM.end_date;
			return View(viewModel);
		}

		[HttpGet]
		[Route("ledger-group-report-pdf")]
		public ViewAsPdf ledgerGroupReportPdf(ListLedgerIndexViewModel ledgerVM)
		{
			var fiscalYear = _transactionDetailService.GetRunningFinancialYear().Result;
			var nepFiscalStartDate = _dateConverter.toBS(fiscalYear.StartDate);
			var nepFiscalEndDate = _dateConverter.toBS(fiscalYear.EndDate);

			if (ledgerVM.start_date == null)
			{
				ledgerVM.start_date = nepFiscalStartDate;
			}

			if (ledgerVM.end_date == null)
			{
				ledgerVM.end_date = nepFiscalEndDate;
			}
			var dateService = DateConverterFactory.getDateConverterService();
			DateTime start_date = dateService.ToAD(ledgerVM.start_date).getFormattedDate();
			DateTime end_date = dateService.ToAD(ledgerVM.end_date).getFormattedDate();
			var ledgerGroups = _ledgerGroupRepo.getQueryable().Where(a => a.ledger_group_id == ledgerVM.ledger_group_id)
				.ToList();
			ListLedgerIndexViewModel viewModel = getViewModelFrom(ledgerGroups, start_date, end_date);
			if (ledgerVM.all == "on")
			{
				ledgerGroups = _ledgerGroupRepo.getAll();
				viewModel = getViewModelFrom(ledgerGroups, start_date, end_date);
				viewModel.all = "on";
			}
			viewModel.organization_name = org_name;
			viewModel.logo = logo;
			viewModel.address = address;
			viewModel.start_date = ledgerVM.start_date;
			viewModel.end_date = ledgerVM.end_date;
			return new ViewAsPdf("ledgerGroupReportPrint", viewModel);
		}

		private ListLedgerIndexViewModel getViewModelFrom(List<LedgerGroup> groups, DateTime startDate, DateTime endDate)
		{
			ListLedgerIndexViewModel listViewModel = new ListLedgerIndexViewModel();
			listViewModel.listLedgerindexVM = new List<LedgerIndexViewModel>();
			foreach (var group in groups)
			{
				LedgerIndexViewModel VM = new LedgerIndexViewModel();
				VM.ledger_details = new List<LedgerDetailModel>();
				foreach (var ledger in group.ledgers)
				{
					LedgerDetailModel model = new LedgerDetailModel();
					model.ledger_id = ledger.ledger_id;
					model.ledger_group_id = ledger.ledger_group_id;
					model.name = ledger.name;
					model.nep_created_date = ledger.nep_created_date;
					model.created_date = ledger.created_date;
					model.code = ledger.code;
					model.ledger_group = ledger.ledger_group;
					model.balance = _transactionDetailRepo.getLedgerBalanceAmountBetweenDates(ledger.ledger_id, startDate, endDate);
					VM.ledger_details.Add(model);
				}
				VM.group_name = group.name;
				listViewModel.listLedgerindexVM.Add(VM);
			}
			return listViewModel;
		}

		[HttpGet]
		[Route("journal-register")]
		public IActionResult transaction(TransactionIndexViewModel transactionVM)
		{
			transactionVM = getTransactionsWithin(transactionVM);
			return View(transactionVM);
		}

		[HttpGet]
		[Route("journal-register-print")]
		public IActionResult transactionPrint(TransactionIndexViewModel transactionVM)
		{
			transactionVM = getTransactionsWithin(transactionVM);
			return View(transactionVM);
		}

		[HttpGet]
		[Route("journal-pdf")]
		public IActionResult journalPdf(TransactionIndexViewModel transactionVM)
		{
			transactionVM = getTransactionsWithin(transactionVM);
			return new ViewAsPdf("transactionPrint", transactionVM);
		}

		private TransactionIndexViewModel getTransactionsWithin(TransactionIndexViewModel transactionVM)
		{
			var dateConverterService = DateConverterFactory.getDateConverterService();
			var startDate = dateConverterService.ToAD(transactionVM.start_date).getFormattedDate();
			var endDate = dateConverterService.ToAD(transactionVM.end_date).getFormattedDate();
			var transactions = _reportService.getTransactionWithin(startDate.Date, endDate.Date).ToList();
			transactionVM = getViewModelFromTransaction(transactions);
			transactionVM.organization_name = org_name;
			transactionVM.address = address;
			transactionVM.logo = logo;
			return transactionVM;
		}

		[HttpGet]
		[Route("account-statement")]
		public IActionResult statementOfLedger(StatementOfLedgerIndexViewModel statementOfLedgerIndexVM)
		{
			var ledger = _ledgerRepo.getQueryable().ToList();
			ViewBag.ledgers = new SelectList(ledger, "ledger_id", "name");
			statementOfLedgerIndexVM = getStatementOfLedger(statementOfLedgerIndexVM);
			return View(statementOfLedgerIndexVM);
		}

		[HttpGet]
		[Route("account-statement-print")]
		public IActionResult statementOfLedgerPrint(StatementOfLedgerIndexViewModel statementOfLedgerIndexVM)
		{
			return View(getStatementOfLedger(statementOfLedgerIndexVM));
		}

		[HttpGet]
		[Route("account-statement-pdf")]
		public ViewAsPdf statementOfLedgerPdf(StatementOfLedgerIndexViewModel statementOfLedgerIndexVM)
		{
			statementOfLedgerIndexVM = getStatementOfLedger(statementOfLedgerIndexVM);
			return new ViewAsPdf("statementOfLedgerPrint", statementOfLedgerIndexVM);
		}

		private StatementOfLedgerIndexViewModel getStatementOfLedger(StatementOfLedgerIndexViewModel statementOfLedgerIndexVM)
		{
			var fiscalYear = _transactionDetailService.GetRunningFinancialYear().Result;
			var nepFiscalStartDate = _dateConverter.toBS(fiscalYear.StartDate);
			var nepFiscalEndDate = _dateConverter.toBS(fiscalYear.EndDate);

			if (statementOfLedgerIndexVM.start_date == null || statementOfLedgerIndexVM.end_date == null)
			{
				statementOfLedgerIndexVM.start_date = nepFiscalStartDate;
				statementOfLedgerIndexVM.end_date = nepFiscalEndDate;
			}

			var dateConverterService = DateConverterFactory.getDateConverterService();
			var startDate = dateConverterService.ToAD(statementOfLedgerIndexVM.start_date).getFormattedDate();
			var endDate = dateConverterService.ToAD(statementOfLedgerIndexVM.end_date).getFormattedDate();
			var ledger_id = statementOfLedgerIndexVM.ledger_id;
			if (ledger_id > 0)
			{
				var transactionDetails = _reportService
					.getTransactionDetailsWithLedgerAndTransactionId(startDate.Date, endDate.Date, ledger_id).ToList();
				var vm = getViewModelFromTransactionDetails(transactionDetails);
				var ledgerName = _ledgerRepo.getById(ledger_id).name;
				vm.start_date = statementOfLedgerIndexVM.start_date;
				vm.end_date = statementOfLedgerIndexVM.end_date;
				statementOfLedgerIndexVM = vm;
				statementOfLedgerIndexVM.name = ledgerName;
				statementOfLedgerIndexVM.old_balance =
					_transactionDetailRepo.getOldBalance(ledger_id, startDate.AddDays(-1));
			}
			statementOfLedgerIndexVM.organization_name = org_name;
			statementOfLedgerIndexVM.address = address;
			statementOfLedgerIndexVM.logo = logo;
			return statementOfLedgerIndexVM;
		}


		[HttpGet]
		[Route("day-book")]
		public async Task<IActionResult> dayBook(AccountingReportVm vm)
		{
			var fiscalYear = _transactionDetailService.GetRunningFinancialYear().Result;
			var nepFiscalStartDate = _dateConverter.toBS(fiscalYear.StartDate);
			var nepFiscalEndDate = _dateConverter.toBS(fiscalYear.EndDate);

			if (vm.FromDate == null)
			{
				vm.FromDate = nepFiscalStartDate;
			}
			if (vm.ToDate == null)
			{
				vm.ToDate = nepFiscalEndDate;
			}
			var dateConverterService = DateConverterFactory.getDateConverterService();
			var date = dateConverterService.ToAD(vm.ToDate).getFormattedDate();
			vm.RightReport = await _accountingReportReporter.GetDayVoucher(fromDate: date.Date, toDate: date.Date);
			return View(vm);
		}


		[HttpGet]
		[Route("day-book-print")]
		public async Task<IActionResult> dayBookPrint(AccountingReportVm vm)
		{
			var fiscalYear = _transactionDetailService.GetRunningFinancialYear().Result;
			var nepFiscalStartDate = _dateConverter.toBS(fiscalYear.StartDate);
			var nepFiscalEndDate = _dateConverter.toBS(fiscalYear.EndDate);

			if (vm.FromDate == null)
			{
				vm.FromDate = nepFiscalStartDate;
			}
			if (vm.ToDate == null)
			{
				vm.ToDate = nepFiscalEndDate;
			}
			var dateConverterService = DateConverterFactory.getDateConverterService();
			var date = dateConverterService.ToAD(vm.FromDate).getFormattedDate();
			vm.RightReport = await _accountingReportReporter.GetDayVoucher(fromDate: date.Date, toDate: date.Date);
			vm.organization_name = org_name;
			vm.address = address;
			vm.logo = logo;
			return View(vm);
		}

		[HttpGet]
		[Route("day-book-pdf")]
		public async Task<IActionResult> dayBookPdf(AccountingReportVm vm)
		{
			var fiscalYear = _transactionDetailService.GetRunningFinancialYear().Result;
			var nepFiscalStartDate = _dateConverter.toBS(fiscalYear.StartDate);
			var nepFiscalEndDate = _dateConverter.toBS(fiscalYear.EndDate);

			if (vm.FromDate == null)
			{
				vm.FromDate = nepFiscalStartDate;
			}
			if (vm.ToDate == null)
			{
				vm.ToDate = nepFiscalEndDate;
			}
			var dateConverterService = DateConverterFactory.getDateConverterService();
			var date = dateConverterService.ToAD(vm.FromDate).getFormattedDate();
			vm.RightReport = await _accountingReportReporter.GetDayVoucher(fromDate: date.Date, toDate: date.Date);
			vm.organization_name = org_name;
			vm.address = address;
			vm.logo = logo;
			return new ViewAsPdf("dayBookPrint", vm);
		}

		[HttpGet]
		[Route("cash-and-bank-book")]
		public async Task<IActionResult> cashAndBankBook(StatementOfLedgerIndexViewModel vm)
		{
			vm = await getCashAndBankBook(vm);
			return View(vm);
		}

		[HttpGet]
		[Route("cash-and-bank-book-print")]
		public async Task<IActionResult> cashAndBankBookPrint(StatementOfLedgerIndexViewModel vm)
		{
			vm = await getCashAndBankBook(vm);
			return View(vm);
		}

		private async Task<StatementOfLedgerIndexViewModel> getCashAndBankBook(StatementOfLedgerIndexViewModel vm)
		{
			var dateConverterService = DateConverterFactory.getDateConverterService();
			var fromDate = dateConverterService.ToAD(vm.start_date).getFormattedDate();
			var toDate = dateConverterService.ToAD(vm.end_date).getFormattedDate();

			var ledgerType = vm.ledger_type;

			var cashLedgerGroupId = _ledgerRepo
				.getById(Convert.ToInt32(_ledgerSetupRepository.getByKey(SetupKeys.getCashLedgerKey).value))
				.ledger_group_id;

			var bankLedgerGroupId = _ledgerRepo
				.getById(Convert.ToInt32(_ledgerSetupRepository.getByKey(SetupKeys.getBankLedgerKey).value))
				.ledger_group_id;

			vm.Report = ledgerType switch
			{
				StatementOfLedgerIndexViewModel.ACCOUNT_TYPE_CASH => await _accountingReportReporter.GetCashBook(
					fromDate: fromDate, toDate: toDate, ledgerId: cashLedgerGroupId),
				StatementOfLedgerIndexViewModel.ACCOUNT_TYPE_BANK => await _accountingReportReporter.GetCashBook(
					fromDate: fromDate, toDate: toDate, ledgerId: bankLedgerGroupId),
				_ => vm.Report
			};
			return vm;
		}

		[HttpGet]
		[Route("balance-sheet")]
		public async Task<IActionResult> BalanceSheet(AccountingReportVm vm)
		{
			try
			{
				vm = await GetBalanceSheetVM(vm);
				return View(vm);
			}
			catch (Exception e)
			{
				AlertHelper.setMessage(this, e.Message, messageType.error);
				return Redirect("/");
			}
		}

		[HttpGet]
		[Route("balance-sheet-print")]
		public async Task<IActionResult> balanceSheetPrint(AccountingReportVm vm)
		{
			try
			{
				vm = await GetBalanceSheetVM(vm);
				return View("reportPrint", vm);
			}
			catch (Exception e)
			{
				AlertHelper.setMessage(this, e.Message, messageType.error);
				return Redirect("/");
			}
		}

		[HttpGet]
		[Route("balance-sheet-pdf")]
		public async Task<IActionResult> balanceSheetPdf(AccountingReportVm vm)
		{
			try
			{
				vm = await GetBalanceSheetVM(vm);
				return new ViewAsPdf("reportPrint", vm);
			}
			catch (Exception e)
			{
				AlertHelper.setMessage(this, e.Message, messageType.error);
				return Redirect("/");
			}
		}

		private async Task<AccountingReportVm> GetBalanceSheetVM(AccountingReportVm vm)
		{
			var fiscalYear = _transactionDetailService.GetRunningFinancialYear().Result;
			var nepFiscalStartDate = _dateConverter.toBS(fiscalYear.StartDate);
			var nepFiscalEndDate = _dateConverter.toBS(fiscalYear.EndDate);

			if (vm.FromDate == null)
			{
				vm.FromDate = nepFiscalStartDate;
			}
			if (vm.ToDate == null)
			{
				vm.ToDate = nepFiscalEndDate;
			}

			var dateConverterService = DateConverterFactory.getDateConverterService();
			var fromDate = dateConverterService.ToAD(vm.FromDate).getFormattedDate();
			var toDate = dateConverterService.ToAD(vm.ToDate).getFormattedDate();

			var report =
				await _accountingReportReporter.GetBalanceSheet(fromDate: fromDate.Date, toDate: toDate.Date);
			vm.LeftReport = report.Where(x => x.ParentId == 1).ToList();
			vm.LeftReport.Add(new AccountingReportVo
			{
				Name = "Closing Stock",
				Code = "",
				Dr = 0,
				Cr = 0,
				Balance = vm.ClosingBalance,
				PreviousBalance = 0,
			});
			vm.LeftReport[0].Balance += vm.ClosingBalance;
			vm.RightReport = report.Where(x => x.ParentId == 2).ToList();

			vm.organization_name = org_name;
			vm.logo = logo;
			vm.address = address;
			vm.title = "Balance Sheet";
			vm.fiscalYear = _transactionDetailService.GetRunningFinancialYear().Result.Name;
			return vm;
		}

		[HttpGet]
		[Route("trial-balance")]
		public async Task<IActionResult> trialBalance(TrialBalanceDto trialBalanceDto)
		{
			try
			{
				trialBalanceDto = await getTrialBalanceData(trialBalanceDto);
				return View(trialBalanceDto);
			}
			catch (Exception ex)
			{
				AlertHelper.setMessage(this, ex.Message, messageType.error);
				return View(trialBalanceDto);
			}
		}

		[HttpGet]
		[Route("trial-balance-print")]
		public async Task<IActionResult> trialBalancePrint(TrialBalanceDto trialBalanceDto)
		{
			var trialBalance = await getTrialBalanceData(trialBalanceDto);
			return View(trialBalance);
		}

		[HttpGet]
		[Route("trial-balance-pdf")]
		public async Task<ViewAsPdf> trialBalancePdf(TrialBalanceDto trialBalanceDto)
		{
			var trialBalance = await getTrialBalanceData(trialBalanceDto);
			return new ViewAsPdf("trialBalancePrint", trialBalance);
		}

		private async Task<TrialBalanceDto> getTrialBalanceData(TrialBalanceDto trialBalanceDto)
		{
			var fiscalYear = _transactionDetailService.GetRunningFinancialYear().Result;

			var nepFiscalStartDate = _dateConverter.toBS(fiscalYear.StartDate);
			var nepFiscalEndDate = _dateConverter.toBS(fiscalYear.EndDate);

			if (trialBalanceDto.start_date == null)
			{
				trialBalanceDto.start_date = nepFiscalStartDate;
			}

			if (trialBalanceDto.end_date == null)
			{
				trialBalanceDto.end_date = nepFiscalEndDate;
			}
			var dateConverterService = DateConverterFactory.getDateConverterService();
			var startDate = dateConverterService.ToAD(trialBalanceDto.start_date).getFormattedDate();
			var endDate = dateConverterService.ToAD(trialBalanceDto.end_date).getFormattedDate();
			trialBalanceDto.organization_name = org_name;
			trialBalanceDto.address = address;
			trialBalanceDto.logo = logo;
			trialBalanceDto.Report = await _accountingReportReporter.GetTrialBalance(fromDate: startDate, toDate: endDate);
			return trialBalanceDto;
		}

		[HttpGet]
		[Route("year-close")]
		public async Task<IActionResult> YearClose(YearCloseVm vm)
		{
			var dateConverterService = DateConverterFactory.getDateConverterService();
			var closingEnglishDate = DateTime.Now.Date;

			if (!string.IsNullOrEmpty(vm.ClosingDate))
			{
				closingEnglishDate = dateConverterService.ToAD(vm.ClosingDate).getFormattedDate();
			}
			else
			{
				vm.ClosingDate = dateConverterService.ToBS(DateTime.Now.Date, NepaliDate.DateFormats.yMd).getFormattedDate();
			}

			var groups = _ledgerGroupRepo.getQueryable().Where(x =>
					x.ledger_group_type == LedgerGroupType.asset || x.ledger_group_type == LedgerGroupType.liability)
				.Select(x => x.ledger_group_id);

			vm.Ledgers = await _ledgerRepo.getQueryable().Where(x => groups.Contains(x.ledger_group_id)).ToListAsync();

			vm.FinancialYear = await _financialYearService.GetFiscalYearByDate(closingEnglishDate);

			var raw = await GetProfitAndLossData(vm: new AccountingReportVm()
			{
				OpeningBalance = vm.OpeningStock,
				ClosingBalance = vm.ClosingStock
			}, fromDate: vm.FinancialYear.StartDate.Date, vm.FinancialYear.EndDate.Date);

			var dr = raw.LeftReport[0].Balance;
			var cr = raw.RightReport[0].Balance;

			var diff = dr - cr > 0 ? dr - cr : cr - dr;
			if (dr > cr)
			{
				raw.RightReport[0].Balance += diff;
				vm.Type = "Net Loss";
				vm.Amount = diff;
			}

			else
			{
				raw.LeftReport[0].Balance += diff;
				vm.Type = "Net Profit";
				vm.Amount = diff;
			}

			return View(vm);
		}

		[HttpPost]
		[Route("year-close")]
		public async Task<IActionResult> YearClose(YearCloseVm vm, bool close = true)
		{
			try
			{
				if (vm.LedgerId == 0)
				{
					throw new Exception("Please select ledger");
				}

				var dateConverterService = DateConverterFactory.getDateConverterService();
				var dto = new FiscalYearCloseDto()
				{
					Amount = vm.Amount,
					Date = dateConverterService.ToAD(vm.ClosingDate).getFormattedDate(),
					ClosingStock = vm.ClosingStock,
					OpeningStock = vm.OpeningStock,
					Type = vm.Type,
					LedgerId = vm.LedgerId
				};
				await _financialYearService.CloseYear(dto);
				AlertHelper.setMessage(this, "Year closed successfully", messageType.success);
				return Redirect("/accounting/report/year-close");
			}
			catch (Exception e)
			{
				AlertHelper.setMessage(this, e.Message, messageType.error);
				return Redirect("/accounting/report/year-close");
			}
		}

		[HttpGet]
		[Route("profit-loss")]
		public async Task<IActionResult> profitLoss(AccountingReportVm vm)
		{
			try
			{
				var result = await GetProfitLossReport(vm);
				return View(result);
			}
			catch (Exception e)
			{
				AlertHelper.setMessage(this, e.Message, messageType.error);
				return Redirect("/");
			}
		}

		[HttpGet]
		[Route("profitloss-print")]
		public async Task<IActionResult> profitLossPrint(AccountingReportVm vm)
		{
			var result = await GetProfitLossReport(vm);
			return View("reportPrint", result);
		}

		[HttpGet]
		[Route("profitLoss-pdf")]
		public async Task<ViewAsPdf> profitLossPdf(AccountingReportVm vm)
		{
			var report = await GetProfitLossReport(vm);
			return new ViewAsPdf("reportPrint", report);
		}

		private async Task<AccountingReportVm> GetProfitLossReport(AccountingReportVm vm)
		{
			var fiscalYear = _transactionDetailService.GetRunningFinancialYear().Result;
			var nepFiscalStartDate = _dateConverter.toBS(fiscalYear.StartDate);
			var nepFiscalEndDate = _dateConverter.toBS(fiscalYear.EndDate);

			if (vm.FromDate == null)
			{
				vm.FromDate = nepFiscalStartDate;
			}

			if (vm.ToDate == null)
			{
				vm.ToDate = nepFiscalEndDate;
			}

			var dateConverterService = DateConverterFactory.getDateConverterService();
			var fromDate = dateConverterService.ToAD(vm.FromDate).getFormattedDate();
			var toDate = dateConverterService.ToAD(vm.ToDate).getFormattedDate();
			vm = await GetProfitAndLossData(vm, fromDate, toDate);

			decimal dr = 0;
			decimal cr = 0;
			if (vm.LeftReport.Count() > 0)
			{
				dr = vm.LeftReport[0].Balance;
			}
			if (vm.RightReport.Count() > 0)
			{
				cr = vm.RightReport[0].Balance;
			}

			var diff = dr - cr > 0 ? dr - cr : cr - dr;
			if (dr > cr)
			{
				vm.RightReport[0].Balance += diff;
				vm.RightReport.Add(new AccountingReportVo()
				{
					Name = "Net Loss",
					Code = "",
					Dr = 0,
					Cr = 0,
					Balance = diff,
					PreviousBalance = 0,
				});
			}

			else
			{
				vm.LeftReport[0].Balance += diff;
				vm.LeftReport.Add(new AccountingReportVo()
				{
					Name = "Net Profit",
					Code = "",
					Dr = 0,
					Cr = 0,
					Balance = diff,
					PreviousBalance = 0,
				});
			}
			return vm;
		}

		private async Task<AccountingReportVm> GetProfitAndLossData(AccountingReportVm vm, DateTime fromDate,
			DateTime toDate)
		{
			var report = await _accountingReportReporter.GetProfitAndLoss(fromDate: fromDate.Date, toDate: toDate.Date);
			vm.LeftReport = report.Where(x => x.ParentId == 4).ToList();
			vm.RightReport = report.Where(x => x.ParentId == 3).ToList();

			var trading = await _accountingReportReporter.GetTradingReport(fromDate: fromDate.Date, toDate: toDate.Date);
			var dr = trading.Where(x => x.ParentId == 4).Sum(x => x.Balance) + vm.OpeningBalance;
			var cr = trading.Where(x => x.ParentId == 3).Sum(x => x.Balance) + vm.ClosingBalance;
			if (dr < cr)
			{
				if (vm.RightReport.Count() > 0)
				{
					vm.RightReport[0].Balance += cr - dr;
				}
				vm.RightReport.Add(new AccountingReportVo()
				{
					Id = -1,
					Name = "Gross Profit",
					Code = "",
					Dr = 0,
					Cr = 0,
					Balance = cr - dr,
					PreviousBalance = 0,
				});
			}
			else
			{
				if (vm.LeftReport.Count() > 0)
				{
					vm.LeftReport[0].Balance += dr - cr;
				}
				vm.LeftReport.Add(new AccountingReportVo()
				{
					Id = -2,
					Name = "Gross Loss",
					Code = "",
					Dr = 0,
					Cr = 0,
					Balance = cr - dr,
					PreviousBalance = 0,
				});
			}
			vm.organization_name = org_name;
			vm.address = address;
			vm.title = "Profit & Loss A/C";
			vm.fiscalYear = _transactionDetailService.GetRunningFinancialYear().Result.Name;
			return vm;
		}

		[HttpGet]
		[Route("trading-report")]
		public async Task<IActionResult> TradingReport(AccountingReportVm vm)
		{
			try
			{
				var result = await getTradingReport(vm);
				return View(result);
			}
			catch (Exception e)
			{
				AlertHelper.setMessage(this, e.Message, messageType.error);
				return Redirect("/");
			}
		}

		[HttpGet]
		[Route("trading-print")]
		public async Task<IActionResult> tradingReportPrint(AccountingReportVm vm)
		{
			var result = await getTradingReport(vm);
			return View("reportPrint", result);
		}

		[HttpGet]
		[Route("trading-pdf")]
		public async Task<ViewAsPdf> TradingReportPdf(AccountingReportVm vm)
		{
			var report = await getTradingReport(vm);
			return new ViewAsPdf("reportPrint", report);
		}

		private async Task<AccountingReportVm> getTradingReport(AccountingReportVm vm)
		{
			var fiscalYear = _transactionDetailService.GetRunningFinancialYear().Result;
			var nepFiscalStartDate = _dateConverter.toBS(fiscalYear.StartDate);
			var nepFiscalEndDate = _dateConverter.toBS(fiscalYear.EndDate);

			if (vm.FromDate == null)
			{
				vm.FromDate = nepFiscalStartDate;
			}

			if (vm.ToDate == null)
			{
				vm.ToDate = nepFiscalEndDate;
			}

			var dateConverterService = DateConverterFactory.getDateConverterService();
			var fromDate = dateConverterService.ToAD(vm.FromDate).getFormattedDate();
			var toDate = dateConverterService.ToAD(vm.ToDate).getFormattedDate();
			var report = await _accountingReportReporter.GetTradingReport(fromDate: fromDate.Date, toDate: toDate.Date);
			vm.LeftReport = report.Where(x => x.ParentId == 4).ToList();
			vm.RightReport = report.Where(x => x.ParentId == 3).ToList();

			vm.LeftReport.Add(new AccountingReportVo
			{
				Name = "Opening Stock",
				Code = "",
				Dr = 0,
				Cr = 0,
				Balance = vm.OpeningBalance,
				PreviousBalance = 0,
			});
			vm.RightReport.Add(new AccountingReportVo
			{
				Name = "Closing Stock",
				Code = "",
				Dr = 0,
				Cr = 0,
				Balance = vm.ClosingBalance,
				PreviousBalance = 0,
			});
			vm.LeftReport[0].Balance += vm.OpeningBalance;
			vm.RightReport[0].Balance += vm.ClosingBalance;

			if (vm.LeftReport[0].Balance < vm.RightReport[0].Balance)
			{
				var balance = vm.RightReport[0].Balance - vm.LeftReport[0].Balance;
				vm.LeftReport[0].Balance += balance;
				vm.LeftReport.Add(new AccountingReportVo()
				{
					Name = "Gross Profit",
					Code = "",
					Dr = 0,
					Cr = 0,
					Balance = balance,
					PreviousBalance = 0,
				});
			}
			else
			{
				var balance = vm.LeftReport[0].Balance - vm.RightReport[0].Balance;
				vm.RightReport[0].Balance += balance;
				vm.RightReport.Add(new AccountingReportVo()
				{
					Name = "Gross Loss",
					Code = "",
					Dr = 0,
					Cr = 0,
					Balance = balance,
					PreviousBalance = 0,
				});
			}
			vm.organization_name = org_name;
			vm.address = address;
			vm.title = "Trading A/C";
			vm.fiscalYear = _transactionDetailService.GetRunningFinancialYear().Result.Name;
			return vm;
		}

		private StatementOfLedgerIndexViewModel getViewModelFromTransactionDetails(
			List<TransactionDetail> transactionDetails)
		{
			StatementOfLedgerIndexViewModel statementModel = new StatementOfLedgerIndexViewModel();

			statementModel.transactionDetail = new List<TransactionDetailModel>();
			foreach (var details in transactionDetails.Where(a => a.ledger_id != 0))
			{
				TransactionDetailModel detailModel = new TransactionDetailModel();
				detailModel.cr_amount = details.cr_amount;
				detailModel.dr_amount = details.dr_amount;
				detailModel.transaction_date = details.transaction_date;
				detailModel.balance = details.balance;
				detailModel.ledger = details.ledger;
				detailModel.ledger_id = details.ledger_id;
				detailModel.ref_ledger_id = details.ref_ledger_id;
				detailModel.name = "Opening Balance";
				detailModel.transaction = details.transaction;
				if (details.ledger_id != 0)
				{
					detailModel.name = _ledgerRepo.getById(details.ledger_id).name;
				}
				statementModel.transactionDetail.Add(detailModel);
			}
			return statementModel;
		}

		private TransactionIndexViewModel getViewModelFromTransaction(List<Transaction> transactions)
		{
			TransactionIndexViewModel transactionVM = new TransactionIndexViewModel();
			transactionVM.transaction = new List<TransactionModel>();

			foreach (var transaction in transactions)
			{
				TransactionModel tmodel = new TransactionModel();
				tmodel.entry_date = transaction.entry_date;
				tmodel.transaction_date = transaction.transaction_date;
				tmodel.remarks = transaction.remarks;
				tmodel.voucher_no = transaction.voucher_no;
				tmodel.voucher_type = transaction.voucher_type;
				tmodel.nep_entry_date = transaction.nep_entry_date;
				tmodel.nep_transaction_date = transaction.nep_transaction_date;
				tmodel.transactionDetail = new List<TransactionDetailModel>();
				foreach (var transaction_details in transaction.TransactionDetails.Where(a => a.ledger_id != 0))
				{
					TransactionDetailModel transactionDetailModel = new TransactionDetailModel();
					transactionDetailModel.cr_amount = transaction_details.cr_amount;
					transactionDetailModel.dr_amount = transaction_details.dr_amount;
					transactionDetailModel.ledger_id = transaction_details.ledger_id;
					transactionDetailModel.balance = transaction_details.balance;
					transactionDetailModel.ref_ledger_id = transaction_details.ref_ledger_id;
					transactionDetailModel.name = transaction_details.ledger.name;
					tmodel.transactionDetail.Add(transactionDetailModel);
				}
				transactionVM.transaction.Add(tmodel);
			}
			return transactionVM;
		}
	}
}

public class YearCloseVm
{
	public FinancialYear FinancialYear;
	public string ClosingDate { get; set; }
	public long LedgerId { get; set; }
	public decimal OpeningStock { get; set; }
	public decimal ClosingStock { get; set; }
	public string Type { get; set; }
	public decimal Amount { get; set; }
	public List<Ledger> Ledgers = new List<Ledger>();
	public SelectList GetLedgerOptions() =>
		new SelectList(Ledgers, nameof(Ledger.ledger_id), nameof(Ledger.name), LedgerId);
}