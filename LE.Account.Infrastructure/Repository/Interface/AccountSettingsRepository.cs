using LE.Account.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LE.Account.Infrastructure.Repository.Interface
{
    public interface AccountSettingsRepository
    {
        void insert(AccountSettings transaction_sequence);
        void update(AccountSettings transaction_sequence);
        void delete(AccountSettings transaction_sequence);
        List<AccountSettings> getAll();
        AccountSettings getById(long transaction_sequence_id);
        long getTransactionSequence();
        AccountSettings getByKey(string key);
        IQueryable<AccountSettings> getQueryable();
    }
}
