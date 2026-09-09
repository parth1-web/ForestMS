using Dapper;
using LE.Account.Common.Enums;
using LE.Account.Entities;
using LE.Account.Infrastructure.Dto;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Account.Service.Assemblers.Interface;
using LE.Account.Service.Services.FinancialYearService;
using LE.Account.Service.Services.Interface;
using LE.Common.Exceptions;
using LE.Common.Provider;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace LE.Account.Service.Services.Implementations
{
	public class TransactionServiceImpl : TransactionService
	{
		private readonly TransactionAssembler _transactionMaker;
		private readonly TransactionRepository transactionRepo;
		private readonly TransactionDetailService transactionDetailService;
		private readonly IConnectionProvider _connectionProvider;

		public TransactionServiceImpl(TransactionAssembler transactionMaker, TransactionRepository _transactionRepo, TransactionDetailService _transactionDetailService, IConnectionProvider connectionProvider)
		{
			transactionRepo = _transactionRepo;
			transactionDetailService = _transactionDetailService;
			_transactionMaker = transactionMaker;
			_connectionProvider = connectionProvider;
		}
		public void addTransaction(TransactionDto transactionDto)
		{
			try
			{
				using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
				{
					if (transactionDto.voucher_type != VoucherType.YearClosed)
					{
						if (!transactionDto.isTransactionPerformedValid())
							throw new InvalidValueException("More than Two transaction data cannot be in either debit or credit side.");
						if (!transactionDto.isTransactionAmountValid())
							throw new InvalidValueException("Amount cannot be negative and must be equal.");
					}
					if (!transactionDto.isTransactionDateValid())
					{
						throw new InvalidValueException("You are not allowed to perform transaction in upcoming days.");
					}
					var currentFiscalYear = GetRunningFinancialYear()?.Result;

					if(transactionDto.transaction_date.Date < currentFiscalYear.StartDate.Date)
					{
						throw new CustomException("आर्थिक वर्ष बन्द भएपछि पछिल्लो मितिमा प्रविष्टि गर्न अनुमति छैन। कृपया चालु आर्थिक वर्षमा समायोजन गर्नुहोस्।");
					}

					Entities.Transaction transactionEntity = new Entities.Transaction();
					_transactionMaker.copy(transactionEntity, transactionDto);

					transactionEntity.amount = transactionDto.getTransactionAmount();
					transactionEntity.remarks = transactionDto.remarks;
					long tran_id = transactionEntity.transaction_id;

					transactionRepo.insert(transactionEntity);

					transactionDetailService.addTransactionDetail(transactionDto, tran_id);
					tx.Complete();
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		private async Task<FinancialYear> GetRunningFinancialYear()
		{
			await using var connection = _connectionProvider.GetDbConnection();
			return await connection.QueryFirstOrDefaultAsync<FinancialYear>("SELECT * FROM financial_year WHERE Running = True");
		}
	}
}
