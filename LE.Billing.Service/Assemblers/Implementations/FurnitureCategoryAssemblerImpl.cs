using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Service.Assemblers.Interface;

namespace LE.Billing.Service.Assemblers.Implementations
{
    public class FurnitureCategoryAssemblerImpl : FurnitureCategoryAssembler
    {
        public void copy(FurnitureCategory furniture_category, FurnitureCategoryDto furniture_category_dto)
        {
            furniture_category.name = furniture_category_dto.name;
            furniture_category.is_enabled = furniture_category_dto.is_enabled;
        }
    }
}
