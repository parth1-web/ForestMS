using LE.Inventory.Infrastructure.Dto;

namespace LE.Inventory.Service.Services.Interface
{
    public interface StockCategoryPurposeService
    {
        void save(StockCategoryPurposeDto category_dto);
        void update(StockCategoryPurposeDto category_dto);
        void delete(long stock_category_purpose_id);
        void enable(long stock_category_purpose_id);
        void disable(long stock_category_purpose_id);
    }
}
