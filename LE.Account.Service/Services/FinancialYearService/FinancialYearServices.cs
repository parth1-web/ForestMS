using Dapper;
using LE.Account.Common.Enums;
using LE.Account.Entities;
using LE.Account.Infrastructure.Dto;
using LE.Account.Service.Services.Interface;
using LE.Common.Provider;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace LE.Account.Service.Services.FinancialYearService
{
	public class FinancialYearServices
	{
		private readonly IConnectionProvider _connectionProvider;
		private readonly TransactionService _transactionService;

		public FinancialYearServices(IConnectionProvider connectionProvider, TransactionService transactionService)
		{
			_connectionProvider = connectionProvider;
			_transactionService = transactionService;
		}

		public async Task<FinancialYear> GetRunningFinancialYear()
		{
			await using var connection = _connectionProvider.GetDbConnection();
			return await connection.QueryFirstOrDefaultAsync<FinancialYear>("SELECT * FROM financial_year WHERE Running = 1");
		}

		public async Task<FinancialYear> GetFiscalYearByDate(DateTime date)
		{
			await using var connection = _connectionProvider.GetDbConnection();
			return await connection.QueryFirstOrDefaultAsync<FinancialYear>("SELECT * FROM financial_year WHERE CAST(StartDate AS DATE) <= @date AND CAST(EndDate AS DATE) >= @date", new { date });
		}

		public async Task CloseYear(FiscalYearCloseDto dto)
		{
			await using var conn = _connectionProvider.GetDbConnection();

			using var tsc = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
			await EnsureOlderFiscalYearIsClosed();
			await UpdateForThisYear(dto);
			var ledgerType = "SELECT lg.group_type_name from ledger_group as lg INNER JOIN ledger as l ON lg.ledger_group_id = l.ledger_group_id WHERE ledger_id = @ledgerId";
			var transactionDto = new TransactionDto();
			transactionDto.remarks = "Being year closed";
			await conn.ExecuteAsync(ledgerType, new { ledgerId = dto.LedgerId });

			if (ledgerType.ToLower() == "assets" || ledgerType.ToLower() == "expenses")
			{
				transactionDto.addDebitData(new LedgerTransactionDto()
				{
					amount = dto.Amount,
					ledger_id = dto.LedgerId,
				});
			}
			else
			{
				transactionDto.addCreditData(new LedgerTransactionDto()
				{
					amount = dto.Amount,
					ledger_id = dto.LedgerId
				});
			}
			transactionDto.voucher_no = 0;
			transactionDto.voucher_type = VoucherType.YearClosed;
			transactionDto.transaction_date = dto.Date;
			_transactionService.addTransaction(transactionDto);
			tsc.Complete();
		}

		private async Task EnsureOlderFiscalYearIsClosed()
		{
			await using var conn = _connectionProvider.GetDbConnection();
			var years = "SELECT * FROM financial_year";
			var fiscalYears = (await conn.QueryAsync<FinancialYear>(years)).ToList();
			var thisYear = fiscalYears.FirstOrDefault(x => x.Running);
			if (thisYear == null)
			{
				throw new Exception("No running financial year found");
			}

			if (fiscalYears.Any(x => x.Id < thisYear.Id && !x.Closed))
			{
				throw new Exception("Please close older financial year first");
			}
		}

		private async Task UpdateForThisYear(FiscalYearCloseDto dto)
		{
			await using var conn = _connectionProvider.GetDbConnection();
			var currentYear = await GetRunningFinancialYear();
			if (currentYear.EndDate.Date != dto.Date.Date)
			{
				throw new Exception("Please close financial year on the last day of the year");
			}

			var updateQuery = "UPDATE financial_year SET OpeningStock = @OpeningStock, ClosingStock = @ClosingStock, Type = @Type, PlAmount = @PlAmount WHERE Running = 1";
			await conn.ExecuteAsync(updateQuery, new
			{
				OpeningStock = dto.OpeningStock,
				ClosingStock = dto.ClosingStock,
				Type = dto.Type,
				PlAmount = dto.Amount
			});

			var markAsClosedQuery =
				"UPDATE financial_year SET Running = 0, Closed = 1, ClosedDate = @ClosedDate WHERE Running = 1";
			await conn.ExecuteAsync(markAsClosedQuery, new { ClosedDate = currentYear.EndDate });

			var markAsRunningQuery = "UPDATE financial_year SET Running = 1 WHERE Id = @Id";
			await conn.ExecuteAsync(markAsRunningQuery, new { Id = currentYear.Id + 1 });
		}
	}
}

public class FiscalYearCloseDto
{
	public long LedgerId { get; set; }
	public decimal OpeningStock { get; set; }
	public decimal ClosingStock { get; set; }
	public string Type { get; set; }
	public decimal Amount { get; set; }
	public DateTime Date { get; set; }
}
