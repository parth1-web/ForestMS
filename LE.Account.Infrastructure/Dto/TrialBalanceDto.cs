using LE.Account.Infrastructure.Reports.Reporter.ValueObjects;
using System.Collections.Generic;

namespace LE.Account.Infrastructure.Dto
{
    public class TrialBalanceDto : GenericDto
    {

        private List<LedgerAmountDto> assetsLedgerBalanceDtos { get; set; } = new List<LedgerAmountDto>();
        private List<LedgerAmountDto> liabilityLedgerBalanceDtos { get; set; } = new List<LedgerAmountDto>();
        private List<LedgerAmountDto> incomeLedgerBalanceDtos { get; set; } = new List<LedgerAmountDto>();
        private List<LedgerAmountDto> expensesLedgerBalanceDtos { get; set; } = new List<LedgerAmountDto>();


        public decimal opening_balance { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }

        public void addAssetsLedgerBalanceDto(List<LedgerAmountDto> assetsLedgerAmountDto)
        {
            assetsLedgerBalanceDtos.AddRange(assetsLedgerAmountDto);
        }

        public void addLiabilityLedgerBalanceDto(List<LedgerAmountDto> liabilyLedgerAmountDto)
        {
            liabilityLedgerBalanceDtos.AddRange(liabilyLedgerAmountDto);
        }

        public void addIncomeLedgerBalanceDto(List<LedgerAmountDto> incomeLedgerAmountDto)
        {
            incomeLedgerBalanceDtos.AddRange(incomeLedgerAmountDto);
        }

        public void addExpensesLedgerBalanceDto(List<LedgerAmountDto> expensesLedgerAmountDto)
        {
            expensesLedgerBalanceDtos.AddRange(expensesLedgerAmountDto);
        }

        public List<LedgerAmountDto> getAssetsLedgerDtoWithBalance() => assetsLedgerBalanceDtos;
        public List<LedgerAmountDto> getLiabilitiesLedgerDtoWithBalance() => liabilityLedgerBalanceDtos;
        public List<LedgerAmountDto> getIncomeLedgerDtoWithBalance() => incomeLedgerBalanceDtos;
        public List<LedgerAmountDto> getExpensesLedgerDtoWithBalance() => expensesLedgerBalanceDtos;
        public List<AccountingReportVo> Report { get; set; } = new List<AccountingReportVo>();
    }
}
