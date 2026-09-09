using LE.Billing.Infrastructure.Dto;

namespace LE.Billing.Service.Services.Interface
{
    public interface FurnitureCategoryService
    {
        void insert(FurnitureCategoryDto furniture_category_dto);
        void update(FurnitureCategoryDto furniture_category_dto);
        void delete(long furniture_category_id);
        void enable(long furniture_category_id);
        void disable(long furniture_category_id);
    }
}
