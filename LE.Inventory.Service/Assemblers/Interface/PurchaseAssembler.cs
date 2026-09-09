using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;

namespace LE.Inventory.Service.Assemblers.Interface
{
    public interface PurchaseAssembler
    {
        void copy(Purchase purchase, PurchaseDto purchase_dto);
    }
}
