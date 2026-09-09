using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Service.Assemblers.Interface;

namespace LE.Inventory.Service.Assemblers.Implementations
{
    public class StockCategoryPurposeAssemblerImpl :StockCategoryPurposeAssembler
    {
        public void copy(StockCategoryPurpose category, StockCategoryPurposeDto category_dto)
        {
            category.stock_category_purpose_id = category_dto.stock_category_purpose_id;
            category.name = category_dto.name;
            category.is_enabled = category_dto.is_enabled;
        }
    }
}
