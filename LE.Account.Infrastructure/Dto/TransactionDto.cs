using DateConverter.Core.Service_Factory;
using LE.Account.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Account.Infrastructure.Dto
{
    public class TransactionDto
    {
        private List<LedgerTransactionDto> creditLedgers = new List<LedgerTransactionDto>();
        private List<LedgerTransactionDto> debitLedgers = new List<LedgerTransactionDto>();

        public DateTime transaction_date { get; set; } = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();

        public string remarks { get; set; }
        
        public VoucherType voucher_type { get; set; }

        public long voucher_no { get; set; }

        public bool isDebitBased()
        {
            return debitLedgers.Count == 1;
        }

        public void addDebitData(LedgerTransactionDto transactionDetailDto)
        {
            debitLedgers.Add(transactionDetailDto);
        }

        public void addCreditData(LedgerTransactionDto transactionDetailDto)
        {
            creditLedgers.Add(transactionDetailDto);
        }

        public List<LedgerTransactionDto> getCreditLedgers()
        {
            return creditLedgers;
        }

        public List<LedgerTransactionDto> getDebitLedgers()
        {
            return debitLedgers;
        }
        public decimal getTransactionAmount()
        {
            return calculateTransactionAmount();
        }

        private decimal calculateTransactionAmount()
        {
            decimal transactionAmount = 0;
            if (debitLedgers.Count == creditLedgers.Count)
            {
                foreach (var ta in debitLedgers)
                {
                    transactionAmount += ta.amount;
                    return transactionAmount;
                }
            }
            var dataWithMaximumItem = debitLedgers.Count > creditLedgers.Count ? debitLedgers : creditLedgers;
            foreach (var da in dataWithMaximumItem)
            {
                transactionAmount += da.amount;
            }
            return transactionAmount;
        }


        public bool isTransactionAmountValid()
        {
            decimal drAmount = 0, crAmount = 0;
            foreach (var x in debitLedgers)
            {
                if (x.amount < 0)
                    return false;
                drAmount += x.amount;
            }
            foreach (var y in creditLedgers)
            {
                if (y.amount < 0)
                    return false;
                crAmount += y.amount;
            }
            if (drAmount != crAmount)
                return false;
            return true;

        }

        public bool isTransactionPerformedValid()
        {
            int debitLedgerCount = debitLedgers == null ? 0 : debitLedgers.Count;
            int creditLedgerCount = creditLedgers == null ? 0 : creditLedgers.Count;
            if (debitLedgerCount == 0 || creditLedgerCount == 0)
                return false;
            //if (debitLedgerCount > 1 && creditLedgerCount > 1)
            //    return false;
            return true;
        }

        public bool isTransactionDateValid()
        {
            if (transaction_date > DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone())
                return false;
            return true;
        }
    }
}
