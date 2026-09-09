using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Account.Common.Setup
{
    public interface SettingSetup
    {
        string getKeyName(Enums.AccountSetting accountSetting);
    }
}
