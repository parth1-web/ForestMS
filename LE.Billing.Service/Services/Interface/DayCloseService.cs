using LE.Billing.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Billing.Service.Services.Interface
{
    public interface DayCloseService
    {
        void insert(DayCloseDto day_close_dto);
       
    }
}
