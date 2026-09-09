using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Service.Assemblers.Interface;

namespace LE.Billing.Service.Assemblers.Implementations
{
    public class ToleAssemblerImpl : ToleAssembler
    {
        public void copy(Tole tole, ToleDto tole_dto)
        {
            tole.tole_id = tole_dto.tole_id;
            tole.tole_no = tole_dto.tole_no;
        }
    }
}
