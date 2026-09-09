using System;
using System.Collections.Generic;
using LE.Account.Infrastructure.Dto;
using LE.Account.Service.Extensions;

namespace LE.Account.Service.Assemblers.Implementations
{
    public class TransactionDetailDtoAssemblerImpl : TransactionDetailDtoAssembler
    {
        public List<TransactionDetailDto> getTransactionDetails(TransactionDto transaction_dto)
        {
            try
            {
                List<TransactionDetailDto> responseDatas = new List<TransactionDetailDto>();

                List<LedgerTransactionDto> debitLedgers = transaction_dto.getDebitLedgers();
                List<LedgerTransactionDto> creditLedgers = transaction_dto.getCreditLedgers();

                foreach (var debit in debitLedgers)
                {
                    responseDatas.addDebitResponseData(debit);
                }
                foreach (var credit in creditLedgers)
                {
                    responseDatas.addCreditResponseData(credit);
                }



                responseDatas.ForEach(a =>
                {
                    a.transaction_date = transaction_dto.transaction_date;
                });

                return responseDatas;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
