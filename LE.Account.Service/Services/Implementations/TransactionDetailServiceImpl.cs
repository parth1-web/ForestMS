using Dapper;
using LE.Account.Entities;
using LE.Account.Infrastructure.Dto;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Account.Service.Assemblers.Implementations;
using LE.Account.Service.Services.Interface;
using LE.Common.Exceptions;
using LE.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Account.Service.Services.Implementations
{
    public class TransactionDetailServiceImpl : TransactionDetailService
    {
        private readonly TransactionDetailRepository transactionDetailRepo;
        private readonly TransactionDetailDtoAssembler _transactionDetailDtoAssembler;
        private readonly TransactionRepository _transactionRepo;
        private readonly LedgerRepository _ledgerRepo;
        private readonly IConnectionProvider _connectionProvider;

        public TransactionDetailServiceImpl(TransactionDetailRepository transactionDetailRepo, TransactionDetailDtoAssembler transactionDetailDtoAssembler, TransactionRepository transactionRepo, LedgerRepository ledgerRepo, IConnectionProvider connectionProvider)
        {
            this.transactionDetailRepo = transactionDetailRepo;
            _transactionDetailDtoAssembler = transactionDetailDtoAssembler;
            _transactionRepo = transactionRepo;
            _ledgerRepo = ledgerRepo;
            _connectionProvider = connectionProvider;
        }

        public decimal getOldBalance(long ledger_id, DateTime last_date)
        {
            return transactionDetailRepo.getOldBalance(ledger_id, last_date);
        }

        public decimal getEndBalance(long ledger_id)
        {
            return transactionDetailRepo.getEndBalance(ledger_id);
        }
		public void addTransactionDetail(TransactionDetail transaction_detail)
		{
			// Runs inside the caller's real EF transaction (see beginTransaction());
			// the previous ambient TransactionScope was a no-op for EF Core.
			decimal old_balance = 0;
			if (transaction_detail.ledger_id > 0)
			{
				old_balance = getOldBalance(transaction_detail.ledger_id, transaction_detail.transaction_date);
			}

			transaction_detail.balance = old_balance + (transaction_detail.dr_amount - transaction_detail.cr_amount);


			transaction_detail.transaction_detail_id = 0;
			transactionDetailRepo.insert(transaction_detail);

			updateBalanceIfBackdateEntryIsMade(transaction_detail.ledger_id, transaction_detail.transaction_date, transaction_detail.balance);
		}

        private void updateBalanceIfBackdateEntryIsMade(long ledger_id, DateTime transaction_date, decimal lastbalance)
        {
            if (ledger_id > 0)
            {
                var isBackDateEntryMade = transactionDetailRepo.isBackDateEntryMade(ledger_id, transaction_date);
                if (isBackDateEntryMade == true)
                {
                    List<TransactionDetail> listOfRowsToBeUpdated = transactionDetailRepo.getAllTransactionDetailOfLedgerLaterThanDate(ledger_id, transaction_date);
                    for (int a = 0; a < listOfRowsToBeUpdated.Count; a++)
                    {
                        listOfRowsToBeUpdated[a].balance = lastbalance + (listOfRowsToBeUpdated[a].dr_amount - listOfRowsToBeUpdated[a].cr_amount);
                        lastbalance = listOfRowsToBeUpdated[a].balance;
                    }
                    transactionDetailRepo.updateLedgerBalances(listOfRowsToBeUpdated);
                }
            }
        }

		public void addTransactionDetail(TransactionDto transaction_dto, long transaction_id)
		{
			// Runs inside the caller's real EF transaction (see beginTransaction());
			// the previous ambient TransactionScope was a no-op for EF Core.
			var transaction = _transactionRepo.getById(transaction_id) ?? throw new ItemNotFoundException($"Transaction with id {transaction_id} doesnot exist.");

			List<TransactionDetailDto> transactionDetailDtos = _transactionDetailDtoAssembler.getTransactionDetails(transaction_dto);

			List<long> ledgerIdsUsedInTransaction = transactionDetailDtos.Select(a => a.ledger_id).Distinct().ToList();

			var ledgers = _ledgerRepo.getQueryable().Where(a => ledgerIdsUsedInTransaction.Contains(a.ledger_id)).ToList();
			var currentFiscalYearId = GetRunningFinancialYear();
			foreach (var transactionDetailDto in transactionDetailDtos)
			{
				if (transactionDetailDto.ledger_id > 0)
				{
					TransactionDetail entity = new TransactionDetail();
					entity.transaction = transaction;
					entity.transaction_id = transaction_id;
					entity.transaction_date = transactionDetailDto.transaction_date;
					entity.ledger_id = transactionDetailDto.ledger_id;
					entity.ref_ledger_id = transactionDetailDto.ref_ledger_id;
					entity.dr_amount = transactionDetailDto.debit_amount;
					entity.cr_amount = transactionDetailDto.credit_amount;
					entity.fiscal_year_id = currentFiscalYearId.Result.Id;

					addTransactionDetail(entity);
				}
			}
		}

        public async Task<FinancialYear> GetRunningFinancialYear()
        {
            await using var connection = _connectionProvider.GetDbConnection();
            return await connection.QueryFirstOrDefaultAsync<FinancialYear>("SELECT * FROM financial_year WHERE Running = True");
        }

        public void updateBalanceAmount(long ledger_id)
        {
            // UoW fix: update()/delete() on the shared context no longer self-save, and this
            // path is invoked directly by the controller (no outer transaction/saveChanges).
            // Without an explicit flush here the recalculation was a silent no-op.
            using (var tx = transactionDetailRepo.beginTransaction())
            {
                decimal lastBalance = 0;
                var currentDate = DateTime.UtcNow.Date;
                var lastYear = currentDate.AddYears(-1).Date;
                var transactionList = transactionDetailRepo.getQueryable().Where(a => a.transaction_date.Date <= lastYear && a.ledger_id == ledger_id).OrderBy(a => a.transaction_date).ToList();
                if (transactionList.Any())
                {
                    lastBalance = transactionList[transactionList.Count() - 1].balance;
                }
                updateBalanceIfBackdateEntryIsMade(ledger_id, lastYear, lastBalance);
                transactionDetailRepo.saveChanges();
                tx.Commit();
            }
        }
    }
}
