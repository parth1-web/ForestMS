using LE.Account.Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Account.Factories.AbstractFactory.Interface
{
    public interface ReceiptServiceFactory
    {
        ReceiptService getReceiptService();
    }
}
