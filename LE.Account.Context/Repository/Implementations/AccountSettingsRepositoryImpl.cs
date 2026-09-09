using LE.Account.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
using LE.Entities.Account;
using System.Linq;

namespace LE.Account.Context.Repository.Implementations
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
            AccountSettings tranactionSequence = new AccountSettings();
            var transactionSequence = AppDbContext.account_settings.Where(a => a.key == "TRANSACTION_SEQUENCE").SingleOrDefault();
            long sequence;
            if (transactionSequence == null)
            {
                tranactionSequence.key = "TRANSACTION_SEQUENCE";
                tranactionSequence.value = 1;
                this.insert(tranactionSequence);
                sequence = 1;
            }
            else
            {
                long newSequence= transactionSequence.value + 1;
                transactionSequence.value = newSequence;
                sequence = newSequence;
                this.update(transactionSequence);
            }
            return sequence;
        }
    }
}
