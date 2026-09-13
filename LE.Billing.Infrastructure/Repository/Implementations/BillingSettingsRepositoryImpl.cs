using LE.Billing.Entities;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LE.Billing.Infrastructure.Repository.Implementations
{
    public class BillingSettingsRepositoryImpl : BaseRepositoryImpl<BillingSettings>, BillingSettingsRepository
    {
        private AppDbContext AppDbContext;
        public BillingSettingsRepositoryImpl(AppDbContext _AppDbContext) : base(_AppDbContext)
        {
            AppDbContext = _AppDbContext;
        }

        public BillingSettings getByKey(string key)
        {
            return AppDbContext.billing_settings.Where(a => a.key == key).SingleOrDefault();
        }

        // P1/B2 fix: the counter row used to be read outside the transaction and updated with a
        // plain read-modify-write, so concurrent bill inserts collided on the generated PK.
        // The increment now happens in a single atomic UPDATE ... RETURNING statement that
        // PostgreSQL serializes per row; a missing key row is created race-free first
        // (value starts at 0 so the first generated bill number remains 1).
        private long nextSequence(string key)
        {
            AppDbContext.Database.ExecuteSqlRaw(
                "INSERT INTO billing_settings (\"key\", \"value\") VALUES ({0}, 0) ON CONFLICT DO NOTHING", key);
            var sequence = AppDbContext.billing_settings
                .FromSqlRaw("UPDATE billing_settings SET \"value\" = \"value\" + 1 WHERE \"key\" = {0} RETURNING settings_id, \"key\", \"value\"", key)
                .AsEnumerable()
                .Select(s => s.value)
                .FirstOrDefault();
            return sequence;
        }

        public long getCounterBillingSequence()
        {
            return nextSequence("COUNTER_BILLING");
        }

        public long getWoodBillingSequence()
        {
            return nextSequence("WOOD_BILLING");
        }

        public long getChiranSalesSequence()
        {
            return nextSequence("CHIRAN");
        }

        public long getFireWoodBillingSequence()
        {
            return nextSequence("FIREWOOD_BILLING");
        }

        public long getFurnitureBillingSequence()
        {
            return nextSequence("FURNITURE_BILLING");
        }
    }
}
