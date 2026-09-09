using LE.Inventory.Infrastructure.Dto;

namespace LE.Inventory.Service.Services.Interface
{
    public interface WoodDetailsService
    {
        void save(WoodDetailsDto wood_details_dto);
        void update(WoodDetailsDto wood_details_dto);
        //void transferredToChairan(long wood_details_id);
        //void cancelTransferredToChairan(long wood_details_id);
        void delete(long wood_details_id);
    }
}
