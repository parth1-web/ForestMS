using LE.Common.Exceptions;
using LE.Inventory.Common.Enums;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Infrastructure.Repository.Interface;
using LE.Inventory.Service.Adapter.Interface;
using LE.Inventory.Service.Assemblers.Interface;
using LE.Inventory.Service.Services.Interface;
using System;

namespace LE.Inventory.Service.Services.Implementations
{
    public class PurchaseServiceImpl : PurchaseService
    {
        private readonly PurchaseRepository _purchaseRepo;
        private readonly PurchaseAssembler _purchaseAssembler;
        private readonly StockItemAvailabilityRepository _stockItemAvailabilityRepo;
        private readonly StockMovementAssembler _stockMovementAssembler;
        private readonly StockMovementRepository _stockMovementRepo;
        private readonly Movement_ItemAvailabilityAdapter _movement_ItemAvailabilityAdapter;

        public PurchaseServiceImpl(PurchaseRepository purchaseRepo, PurchaseAssembler purchaseMaker, StockMovementAssembler stockMovementAssembler, StockMovementRepository stockMovementRepo, Movement_ItemAvailabilityAdapter movement_ItemAvailabilityAdapter,StockItemAvailabilityRepository stockItemAvailabilityRepo)
        {
            _purchaseRepo = purchaseRepo;
            _purchaseAssembler = purchaseMaker;
            _stockMovementAssembler = stockMovementAssembler;
            _stockMovementRepo = stockMovementRepo;
            _stockItemAvailabilityRepo = stockItemAvailabilityRepo;
            _movement_ItemAvailabilityAdapter = movement_ItemAvailabilityAdapter;
        }


        public void makePurchase(PurchaseDto purchase_dto)
        {
            // P1 fix: ambient TransactionScope was a no-op for EF Core; purchase +
            // stock movement now run in one real database transaction.
            using (var tx = _purchaseRepo.beginTransaction())
            {
                var newPurchase = new Purchase();

                _purchaseAssembler.copy(newPurchase, purchase_dto);

                _purchaseRepo.insert(newPurchase);

                long purchaseId = newPurchase.purchase_id;
                recordStockMovement(purchase_dto, purchaseId, StockMovementType.purchase);
                _purchaseRepo.saveChanges();
                tx.Commit();
            }
        }


        public void delete(long purchaseId)
        {
            var purchases = _purchaseRepo.getById(purchaseId);
            if (purchases == null)
            {
                throw new ItemNotFoundException("Purchase Data does not exist.");
            }

            // P1/B9 fix: the check + soft-delete + stock movement ran without any real
            // transaction (the ambient scope was a no-op). A failure in the middle left
            // the purchase deleted but stock unchanged. All steps now run in one
            // database transaction on the shared DbContext.
            using (var tx = _purchaseRepo.beginTransaction())
            {
                var availableStock = _stockItemAvailabilityRepo.getByStockItemId(purchases.stock_item_id);
                if (availableStock == null || availableStock.qty < purchases.qty)
                {
                    throw new ItemUsedException("Stock is already being sold. you cannot delete at a moment.");
                }

                purchases.is_deleted = true;
                _purchaseRepo.update(purchases);
                PurchaseDto purchaseDto = getPurchaseDtoFrom(purchases);
                // P1/B7 fix: 'delete' no longer forces 'increase' in the adapter, so this
                // correctly decreases the availability row when a purchase is removed.
                recordStockMovement(purchaseDto, purchases.purchase_id, StockMovementType.delete);
                _purchaseRepo.saveChanges();
                tx.Commit();
            }
        }

        private PurchaseDto getPurchaseDtoFrom(Purchase purchases)
        {
            PurchaseDto purchaseDto = new PurchaseDto();
            purchaseDto.purchase_id = purchases.purchase_id;
            purchaseDto.purchase_date = purchases.purchase_date;
            purchaseDto.qty = purchases.qty;
            purchaseDto.stock_item_id = purchases.stock_item_id;
            purchaseDto.user_id = purchases.user_id;
            return purchaseDto;
        }

        private void recordStockMovement(PurchaseDto purchase_dto, long purchaseId, StockMovementType movement_type)
        {
            StockMovementDto stockMovementDto = new StockMovementDto();

            stockMovementDto.movement_type_id = purchaseId;
            var operation = MovementOperation.increase;
            if (movement_type == StockMovementType.purchase)
            {
                operation = MovementOperation.increase;
                stockMovementDto.movement_type = StockMovementType.purchase;
            }
            else if (movement_type == StockMovementType.delete)
            {
                operation = MovementOperation.decrease;
                stockMovementDto.movement_type = StockMovementType.delete;
            }
          
            stockMovementDto.addStockMovementDetail(new StockMovementDetailDto()
            {
                stock_item_id = purchase_dto.stock_item_id,
                qty = purchase_dto.qty,
                operation = operation
            });

            if (stockMovementDto.isMovementValid())
            {
                var stockMovement = new StockMovement();

                _stockMovementAssembler.copy(stockMovement, stockMovementDto);

                _stockMovementRepo.insert(stockMovement);

                updateItemAvailability(stockMovementDto);
            }
        }

        private void updateItemAvailability(StockMovementDto stock_movement_dto)
        {
            _movement_ItemAvailabilityAdapter.updateItemAvailability(stock_movement_dto);
        }


    }
}
