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
			return await connection.QueryFirstOrDefaultAsync<FinancialYear>("SELECT * FROM financial_year WHERE Running = TRUE");
		}

		public async Task<FinancialYear> GetFiscalYearByDate(DateTime date)
		{
			await using var connection = _connectionProvider.GetDbConnection();
			return await connection.QueryFirstOrDefaultAsync<FinancialYear>("SELECT * FROM financial_year WHERE CAST(StartDate AS DATE) <= @date AND CAST(EndDate AS DATE) >= @date", new { date });
		}

		/// <summary>
		/// Which side the year-close voucher posts to the selected ledger.
		/// Asset/expense ledgers are debited, everything else credited.
		/// Compares the group TYPE id (never the display name: names are
		/// pluralized/free-text and string matching silently misfires).
		/// Shared by CloseYear and the year-close preview so both agree.
		/// </summary>
		public async Task<bool> IsCloseDebitAsync(long ledgerId)
		{
			await using var conn = _connectionProvider.GetDbConnection();
			return await IsCloseDebitAsync(ledgerId, conn, null);
		}

		private async Task<bool> IsCloseDebitAsync(long ledgerId, Npgsql.NpgsqlConnection conn, Npgsql.NpgsqlTransaction tx)
		{
			const string sql = "SELECT lg.ledger_group_type FROM ledger_group lg INNER JOIN ledger l ON lg.ledger_group_id = l.ledger_group_id WHERE l.ledger_id = @ledgerId";
			var groupType = await conn.QueryFirstOrDefaultAsync<int?>(new CommandDefinition(sql, new { ledgerId }, tx));
			return groupType == (int)LedgerGroupType.asset || groupType == (int)LedgerGroupType.expenses;
		}

		public async Task CloseYear(FiscalYearCloseDto dto)
		{
			// P1/B13 fix: this whole flow ran on a Dapper connection separate from the EF
			// shared context while _transactionService writes through EF, so the ambient
			// TransactionScope never covered both. The EF transaction below is started on
			// the shared context first so every write participates in one real transaction.
			var efTransaction = _transactionService.beginTransaction();

			try
			{
				await using var conn = _connectionProvider.GetDbConnection();
				conn.Open();
				using var connTx = conn.BeginTransaction();

				await EnsureOlderFiscalYearIsClosed(conn, connTx);
				var currentYear = await UpdateForThisYear(conn, connTx, dto);

				var debitSelectedLedger = await IsCloseDebitAsync(dto.LedgerId, conn, connTx);

				var transactionDto = new TransactionDto();
				transactionDto.remarks = "Being year closed";

				if (debitSelectedLedger)
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

				connTx.Commit();
				efTransaction?.Commit();
			}
			catch
			{
				efTransaction?.Rollback();
				throw;
			}
		}

		private async Task EnsureOlderFiscalYearIsClosed(Npgsql.NpgsqlConnection conn, Npgsql.NpgsqlTransaction tx)
		{
			var years = "SELECT * FROM financial_year";
			var fiscalYears = (await conn.QueryAsync<FinancialYear>(new CommandDefinition(years, transaction: tx))).ToList();
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

		private async Task<FinancialYear> UpdateForThisYear(Npgsql.NpgsqlConnection conn, Npgsql.NpgsqlTransaction tx, FiscalYearCloseDto dto)
		{
			var currentYear = await GetRunningFinancialYear(conn, tx);
			if (currentYear == null)
			{
				throw new Exception("No running financial year found");
			}
			if (currentYear.EndDate.Date != dto.Date.Date)
			{
				throw new Exception("Please close financial year on the last day of the year");
			}

			var updateQuery = "UPDATE financial_year SET OpeningStock = @OpeningStock, ClosingStock = @ClosingStock, Type = @Type, PlAmount = @PlAmount WHERE Running = TRUE";
			await conn.ExecuteAsync(new CommandDefinition(updateQuery, new
			{
				OpeningStock = dto.OpeningStock,
				ClosingStock = dto.ClosingStock,
				Type = dto.Type,
				PlAmount = dto.Amount
			}, tx));

			var markAsClosedQuery =
				"UPDATE financial_year SET Running = FALSE, Closed = TRUE, ClosedDate = @ClosedDate WHERE Running = TRUE";
			await conn.ExecuteAsync(new CommandDefinition(markAsClosedQuery, new { ClosedDate = currentYear.EndDate }, tx));

			// P1/B13 fix: assumed the next fiscal year is exactly 'Id + 1' and blindly set it
			// Running; if that row does not exist the database is left with NO running
			// fiscal year and every subsequent transaction NREs. The next year row must
			// actually exist and be unclosed before it can be activated.
			var nextYearSql = "SELECT * FROM financial_year WHERE Id > @Id ORDER BY Id LIMIT 1";
			var nextYear = await conn.QueryFirstOrDefaultAsync<FinancialYear>(new CommandDefinition(nextYearSql, new { Id = currentYear.Id }, tx));
			if (nextYear == null)
			{
				throw new Exception("No next financial year defined. Please create the next fiscal year before closing this one.");
			}
			if (nextYear.Closed)
			{
				throw new Exception("The next financial year is already closed. Please create a new fiscal year before closing this one.");
			}

			var markAsRunningQuery = "UPDATE financial_year SET Running = TRUE WHERE Id = @Id";
			await conn.ExecuteAsync(new CommandDefinition(markAsRunningQuery, new { Id = nextYear.Id }, tx));

			return currentYear;
		}

		private async Task<FinancialYear> GetRunningFinancialYear(Npgsql.NpgsqlConnection conn, Npgsql.NpgsqlTransaction tx)
		{
			return await conn.QueryFirstOrDefaultAsync<FinancialYear>(new CommandDefinition("SELECT * FROM financial_year WHERE Running = TRUE", transaction: tx));
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
