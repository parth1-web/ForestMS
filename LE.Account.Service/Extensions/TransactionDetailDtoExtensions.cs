using LE.Account.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Account.Service.Extensions
{
    public static class TransactionDetailDtoExtensions
    {
        public static void addDebitResponseData(this List<TransactionDetailDto> response_datas, LedgerTransactionDto debit_ledger, LedgerTransactionDto credit_ledger = null)
        {
            response_datas.Add(new TransactionDetailDto()
            {
                ledger_id = debit_ledger.ledger_id,
                ref_ledger_id = credit_ledger == null ? 0 : credit_ledger.ledger_id,
                debit_amount = debit_ledger.amount,
                credit_amount = 0
            });
        }

        public static void addCreditResponseData(this List<TransactionDetailDto> response_datas, LedgerTransactionDto credit_ledger, LedgerTransactionDto debit_ledger = null)
        {
            response_datas.Add(new TransactionDetailDto()
            {
                ledger_id = credit_ledger.ledger_id,
                ref_ledger_id = debit_ledger == null ? 0 : debit_ledger.ledger_id,
                credit_amount = credit_ledger.amount,
                debit_amount = 0
            });
        }
    }
}
