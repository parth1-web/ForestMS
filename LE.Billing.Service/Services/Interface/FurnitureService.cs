using LE.Billing.Infrastructure.Dto;

namespace LE.Billing.Service.Services.Interface
{
    public interface FurnitureService
    {
        void enable(long furniture_id);
        void disable(long furniture_id);
        void delete(long furniture_id);
        void insert(FurnitureDto furniture_dto);
        void update(FurnitureDto furniture_dto);
    }
}
