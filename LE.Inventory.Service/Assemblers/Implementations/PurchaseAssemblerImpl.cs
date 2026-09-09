using DateConverter.Core.Service_Factory;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Service.Assemblers.Interface;
using System;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Inventory.Service.Assemblers.Implementations
{
    public class PurchaseAssemblerImpl : PurchaseAssembler
    {
        public void copy(Purchase purchase, PurchaseDto purchase_dto)
        {
            var dateConverterService =DateConverterFactory.getDateConverterService();

            purchase.purchase_id = purchase_dto.purchase_id;
            purchase.purchase_date = purchase_dto.purchase_date;
            purchase.nep_purchase_date = dateConverterService.ToBS(purchase_dto.purchase_date.Date,DateFormats.yMd).getFormattedDate();
            purchase.user_id = purchase_dto.user_id;
            purchase.stock_item_id = purchase_dto.stock_item_id;
            purchase.qty = purchase_dto.qty;
        }

    }
}
