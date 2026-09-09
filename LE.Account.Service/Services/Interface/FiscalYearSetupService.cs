using LE.Account.Entities;
using System.Collections.Generic;

namespace LE.Account.Service.Services.Interface
{
    public interface FiscalYearSetupService
    {
        void saveOrUpdate(string key, long value);
        void saveOrUpdate(List<AccountSettings> keyValue);

    }

}
