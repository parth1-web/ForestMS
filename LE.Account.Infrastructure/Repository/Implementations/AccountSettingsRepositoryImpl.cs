using LE.Account.Entities;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
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
            AccountSettings transaction_sequence = new AccountSettings();
            var transactionSequence = AppDbContext.account_settings.Where(a => a.key == "TRANSACTION_SEQUENCE").SingleOrDefault();
            if (transactionSequence == null)
            {
                transaction_sequence.key = "TRANSACTION_SEQUENCE";
                transaction_sequence.value = 1;
                this.insert(transaction_sequence);
                return transaction_sequence.value;
            }
            else
            {
                transactionSequence.value += 1;
                this.update(transactionSequence);
                return transactionSequence.value;
            }
        }
    }
}
