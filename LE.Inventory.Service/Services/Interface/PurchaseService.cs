using LE.Inventory.Infrastructure.Dto;

namespace LE.Inventory.Service.Services.Interface
{
    public interface PurchaseService
    {
        void makePurchase(PurchaseDto purchase_dto);
        void delete(long purchaseId);
    }
}
