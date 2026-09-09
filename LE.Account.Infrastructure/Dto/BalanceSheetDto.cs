using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Account.Infrastructure.Dto
{
    public class BalanceSheetDto:GenericDto
    {
        private List<LedgerAmountDto> assets_ledger_dtos { get; set; } = new List<LedgerAmountDto>();
        private List<LedgerAmountDto> liabilities_ledger_dtos { get; set; } = new List<LedgerAmountDto>();

        public decimal profit_loss_amount { get; set; }
        public  decimal closing_balance { get; set; }
        public string start_date { get; set; } 
        public string end_date { get; set; } 


        public void addAssetsLedgerAmountDto(List<LedgerAmountDto> assetLedgerDto)
        {
            assets_ledger_dtos.AddRange(assetLedgerDto);
        }

        public void addLiabilitiesLedgerAmountDto(List<LedgerAmountDto> liabilitesLedgerDto)
        {
            liabilities_ledger_dtos.AddRange(liabilitesLedgerDto);
        }

        public List<LedgerAmountDto> getLiabilitiesLedgerAmountDtos() => liabilities_ledger_dtos;


        public List<LedgerAmountDto> getAssetsLedgerAmountDtos() => assets_ledger_dtos;
    }
}
