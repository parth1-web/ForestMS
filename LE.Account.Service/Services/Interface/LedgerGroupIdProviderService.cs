using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Account.Service.Services.Interface
{
    public interface LedgerGroupIdProviderService
    {
        long getDebtorsGroupId();
        long getCreditorsGroupId();
    }
}
