using LE.Inventory.Infrastructure.Dto;

namespace LE.Inventory.Service.Services.Interface
{
    public interface PilingService
    {
        void save(PilingDto pilingDto);
        void update(PilingDto pilingDto);
        void delete(long piling_id);
        void enable(long piling_id);
        void disable(long piling_id);
    }
}
