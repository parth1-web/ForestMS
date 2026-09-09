using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;

namespace LE.Billing.Service.Assemblers.Interface
{
    public interface DayCloseAssembler
    {
        void copy(DayClose dayClose, DayCloseDto dayCloseDto);
    }
}
