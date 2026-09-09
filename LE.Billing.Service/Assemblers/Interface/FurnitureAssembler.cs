using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;

namespace LE.Billing.Service.Assemblers.Interface
{
    public interface FurnitureAssembler
    {
        void copy(Furniture furniture, FurnitureDto furniture_dto);
    }
}
