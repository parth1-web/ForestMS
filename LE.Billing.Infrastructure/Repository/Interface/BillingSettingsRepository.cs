using LE.Billing.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    public interface BillingSettingsRepository
    {
        void insert(BillingSettings billing_setting);
        void update(BillingSettings billing_setting);
        void delete(BillingSettings billing_setting);
        List<BillingSettings> getAll();
        BillingSettings getById(long billing_setting_id);
        long getCounterBillingSequence();
        long getWoodBillingSequence();
        long getChiranSalesSequence();
        long getFireWoodBillingSequence();
        long getFurnitureBillingSequence();
        BillingSettings getByKey(string key);
        IQueryable<BillingSettings> getQueryable();
    }
}
