using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;

namespace LE.Billing.Service.Assemblers.Interface
{
    public interface FurnitureCategoryAssembler
    {
        void copy(FurnitureCategory furniture_category, FurnitureCategoryDto furniture_category_dto);
    }
}
