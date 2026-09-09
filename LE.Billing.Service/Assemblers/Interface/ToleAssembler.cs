using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;

namespace LE.Billing.Service.Assemblers.Interface
{
    public interface ToleAssembler
    {
        void copy(Tole tole, ToleDto toleDto);
    }
}
