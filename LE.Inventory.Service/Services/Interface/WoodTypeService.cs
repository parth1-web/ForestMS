using LE.Inventory.Infrastructure.Dto;

namespace LE.Inventory.Service.Services.Interface
{
    public interface WoodTypeService
    {
        void save(WoodTypeDto wood_type_dto);
        void update(WoodTypeDto wood_type_dto);
        void delete(long wood_type_id);
        void enable(long wood_type_id);
        void disable(long wood_type_id);
    }
}
