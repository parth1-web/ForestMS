using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Service.Assemblers.Interface;

namespace LE.Billing.Service.Assemblers.Implementations
{
    public class DayCloseAssemblerImpl : DayCloseAssembler
    {
        public void copy(DayClose dayClose, DayCloseDto day_close_dto)
        {
            dayClose.day_close_id = day_close_dto.day_close_id;
            dayClose.eng_close_date = day_close_dto.eng_close_date;
            dayClose.nep_close_date = day_close_dto.nep_close_date;
        }
    }
}
