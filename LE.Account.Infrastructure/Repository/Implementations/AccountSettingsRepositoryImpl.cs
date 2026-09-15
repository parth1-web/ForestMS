using LE.Account.Entities;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LE.Account.Infrastructure.Repository.Implementations
{
    public class AccountSettingsRepositoryImpl : BaseRepositoryImpl<AccountSettings>, AccountSettingsRepository
    {
        private AppDbContext AppDbContext;
        public AccountSettingsRepositoryImpl(AppDbContext _AppDbContext) : base(_AppDbContext)
        {
            AppDbContext = _AppDbContext;
        }

        public AccountSettings getByKey(string key)
        {
            return AppDbContext.account_settings.Where(a => a.key == key).SingleOrDefault();
        }

        public long getTransactionSequence()
        {
            // P2/UoW fix: was a read-modify-write whose update() no longer self-saves
            // (and was racy for concurrent vouchers — same defect class as P1/B2 bill
            // numbers). Increment atomically in one UPDATE ... RETURNING; a missing
            // key row is created race-free first (value starts at 0 so the first
            // voucher keeps transaction_id 1).
            AppDbContext.Database.ExecuteSqlRaw(
                "INSERT INTO account_settings (\"key\", \"value\") VALUES ({0}, 0) ON CONFLICT DO NOTHING", "TRANSACTION_SEQUENCE");
            return AppDbContext.account_settings
                .FromSqlRaw("UPDATE account_settings SET \"value\" = \"value\" + 1 WHERE \"key\" = {0} RETURNING settings_id, \"key\", \"value\"", "TRANSACTION_SEQUENCE")
                .AsEnumerable()
                .Select(a => a.value)
                .FirstOrDefault();
        }
    }
}
