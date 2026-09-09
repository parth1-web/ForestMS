using LE.Billing.Entities;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
using System.Linq;
using System.Transactions;

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

        public long getCounterBillingSequence()
        {
            BillingSettings tranactionSequence = new BillingSettings();
            var billingSequence = AppDbContext.billing_settings.Where(a => a.key == "COUNTER_BILLING").SingleOrDefault();
            long sequence;
            using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
            {
                if (billingSequence == null)
                {
                    tranactionSequence.key = "COUNTER_BILLING";
                    tranactionSequence.value = 1;
                    insert(tranactionSequence);
                    sequence = 1;
                }
                else
                {
                    long newSequence = billingSequence.value + 1;
                    billingSequence.value = newSequence;
                    sequence = newSequence;
                    this.update(billingSequence);
                }
                tx.Complete();
            }
            return sequence;
        }

        public long getWoodBillingSequence()
        {
            BillingSettings tranactionSequence = new BillingSettings();
            var billingSequence = AppDbContext.billing_settings.Where(a => a.key == "WOOD_BILLING").SingleOrDefault();
            long sequence;
            using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
            {
                if (billingSequence == null)
                {
                    tranactionSequence.key = "WOOD_BILLING";
                    tranactionSequence.value = 1;
                    insert(tranactionSequence);
                    sequence = 1;
                }
                else
                {
                    long newSequence = billingSequence.value + 1;
                    billingSequence.value = newSequence;
                    sequence = newSequence;
                    this.update(billingSequence);
                }
                tx.Complete();
            }
            return sequence;
        }

        public long getChiranSalesSequence()
        {
            BillingSettings tranactionSequence = new BillingSettings();
            var billingSequence = AppDbContext.billing_settings.Where(a => a.key == "CHIRAN").SingleOrDefault();
            long sequence;
            using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
            {
                if (billingSequence == null)
                {
                    tranactionSequence.key = "CHIRAN";
                    tranactionSequence.value = 1;
                    insert(tranactionSequence);
                    sequence = 1;
                }
                else
                {
                    long newSequence = billingSequence.value + 1;
                    billingSequence.value = newSequence;
                    sequence = newSequence;
                    this.update(billingSequence);
                }
                tx.Complete();
            }
            return sequence;
        }

        public long getFireWoodBillingSequence()
        {
            BillingSettings tranactionSequence = new BillingSettings();
            var billingSequence = AppDbContext.billing_settings.Where(a => a.key == "FIREWOOD_BILLING").SingleOrDefault();
            long sequence;
            using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
            {
                if (billingSequence == null)
                {
                    tranactionSequence.key = "FIREWOOD_BILLING";
                    tranactionSequence.value = 1;
                    insert(tranactionSequence);
                    sequence = 1;
                }
                else
                {
                    long newSequence = billingSequence.value + 1;
                    billingSequence.value = newSequence;
                    sequence = newSequence;
                    this.update(billingSequence);
                }
                tx.Complete();
            }
            return sequence;
        }

        public long getFurnitureBillingSequence()
        {
            BillingSettings tranactionSequence = new BillingSettings();
            var billingSequence = AppDbContext.billing_settings.Where(a => a.key == "FURNITURE_BILLING").SingleOrDefault();
            long sequence;
            using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
            {
                if (billingSequence == null)
                {
                    tranactionSequence.key = "FURNITURE_BILLING";
                    tranactionSequence.value = 1;
                    insert(tranactionSequence);
                    sequence = 1;
                }
                else
                {
                    long newSequence = billingSequence.value + 1;
                    billingSequence.value = newSequence;
                    sequence = newSequence;
                    this.update(billingSequence);
                }
                tx.Complete();
            }
            return sequence;
        }
    }
}
