using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;

namespace LE.Inventory.Service.Assemblers.Interface
{
    public interface StockCategoryPurposeAssembler
    {
        void copy(StockCategoryPurpose category_purpose, StockCategoryPurposeDto category_purpose_dto);
    }
}
