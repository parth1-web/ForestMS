using LE.Account.Entities;
using LE.Account.Infrastructure.Dto;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Account.Service.Assemblers.Interface;
using LE.Account.Service.Services.Interface;
using LE.Common.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace LE.Integration.Common
{
    public class AccountTransactionHelperImpl : AccountTransactionHelper
    {
        private readonly TransactionAssembler _transactionMaker;
        private readonly TransactionRepository _transactionRepo;
        private readonly TransactionDetailService _transactionDetailService;

        public AccountTransactionHelperImpl(TransactionAssembler transactionMaker, TransactionRepository transactionRepo, TransactionDetailService transactionDetailService)
        {
            _transactionMaker = transactionMaker;
            _transactionRepo = transactionRepo;
            _transactionDetailService = transactionDetailService;
        }

        public void makeTransaction(TransactionDto transactionDto)
        {
            if (!transactionDto.isTransactionPerformedValid())
                throw new InvalidValueException("More than Two transaction data cannot be in either debit or credit side.");
            if (!transactionDto.isTransactionAmountValid())
                throw new InvalidValueException("Amount cannot be negative and must be equal.");

            Transaction transactionEntity = new Transaction();
            _transactionMaker.copy(transactionEntity, transactionDto);

            List<TransactionDetail> transaction_details = transactionEntity.TransactionDetails.ToList();
            transactionEntity.amount = transactionDto.getTransactionAmount();
            transactionEntity.remarks = transactionDto.remarks;
            long tran_id = transactionEntity.transaction_id;
            transactionEntity.TransactionDetails = null;
            _transactionRepo.insert(transactionEntity);

            foreach (var transaction_detail in transaction_details)
            {
                transaction_detail.transaction_id = tran_id;
                transaction_detail.transaction = transactionEntity;
                _transactionDetailService.addTransactionDetail(transaction_detail);
            }
        }
    }
}
