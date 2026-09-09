using LE.Account.Entities;
using System.Collections.Generic;

namespace LE.Account.Service.Services.Interface
{
    public interface LedgerSetupService
    {
        void saveOrUpdate(string key, string value);
        void saveOrUpdate(List<LedgerSetup> keyValue);

    }

}
