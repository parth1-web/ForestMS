using LE.Account.Infrastructure.Dto;

namespace LE.Account.Service.Services.Interface
{
    public interface LedgerBalanceService
    {
        void save(LedgerBalanceDto ledgerBalance_dto);
        void update(LedgerBalanceDto ledgerBalance_dto);
    }
}
