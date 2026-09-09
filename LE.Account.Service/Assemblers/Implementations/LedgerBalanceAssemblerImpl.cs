using LE.Account.Entities;
using LE.Account.Infrastructure.Dto;
using LE.Account.Service.Assemblers.Interface;

namespace LE.Account.Service.Assemblers.Implementations
{
    public class LedgerBalanceAssemblerImpl : LedgerBalanceAssembler
    {
        public void copy(LedgerBalance ledgerBalance, LedgerBalanceDto ledgerBalance_dto)
        {
            ledgerBalance.ledger_balance_id = ledgerBalance_dto.ledger_balance_id;
            ledgerBalance.ledger_id = ledgerBalance_dto.ledger_id;
            ledgerBalance.ledger_group_id = ledgerBalance_dto.ledger_group_id;
            ledgerBalance.balance = ledgerBalance_dto.balance;
            ledgerBalance.updated_date = ledgerBalance_dto.updated_date;
        }
    }
}