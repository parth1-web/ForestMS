using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LE.Account.Infrastructure.Dto
{
    public class ProfitAndLossDto:GenericDto
    {
        private List<LedgerAmountDto> incomeLedgerBalanceDtos { get; set; } = new List<LedgerAmountDto>();
        private List<LedgerAmountDto> expenseLedgerBalanceDtos { get; set; } = new List<LedgerAmountDto>();
        private List<LedgerAmountDto> liabilitiesLedgerDtos { get; set; } = new List<LedgerAmountDto>();

        public decimal opening_balance { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }

        public void addIncomeLedgerAmountDto(List<LedgerAmountDto> incomeLedgerAmountDto)
        {
            incomeLedgerBalanceDtos.AddRange(incomeLedgerAmountDto);
        }

        public void addExpensesLedgerAmountDto(List<LedgerAmountDto> expenseLedgerAmountDto)
        {
            expenseLedgerBalanceDtos.AddRange(expenseLedgerAmountDto);
        }

        public void addLiabilitiesLedgerAmountDto(List<LedgerAmountDto> liabilitesLedgerDto)
        {
            liabilitiesLedgerDtos.AddRange(liabilitesLedgerDto);
        }

        public List<LedgerAmountDto> getIncomeLedgerDtosWithAmount() => incomeLedgerBalanceDtos;
        public List<LedgerAmountDto> getExpensesLedgerDtosWithAmount() => expenseLedgerBalanceDtos;
        public List<LedgerAmountDto> getLiabilitiesLedgerDtosWithAmount() => liabilitiesLedgerDtos;

        public decimal getTotalIncomeBalance()
        {
            return incomeLedgerBalanceDtos.Sum(s => s.amount);
        }

        public decimal getTotalExpensesBalance()
        {
            return expenseLedgerBalanceDtos.Sum(s => s.amount);
        }


       
    }
}
