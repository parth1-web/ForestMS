using DateConverter.Core.Service_Factory;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Service.Assemblers.Interface;
using System;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Inventory.Service.Assemblers.Implementations
{
    public class StockItemAvailabilityAssemblerImpl :StockItemAvailabilityAssembler
    {
        public void copy(StockItemAvailability stock_item_availability, StockItemAvailabilityDto stock_item_availability_dto)
        {
            var dateConverterService = DateConverterFactory.getDateConverterService();

            stock_item_availability.stock_item_availability_id = stock_item_availability_dto.stock_item_availability_id;

            stock_item_availability.stock_item_id = stock_item_availability_dto.stock_item_id;

            stock_item_availability.qty = stock_item_availability_dto.qty;

            stock_item_availability.last_updated_date =stock_item_availability_dto.last_updated_date;
            stock_item_availability.nep_last_updated_date= dateConverterService.ToBS(stock_item_availability_dto.last_updated_date.Date,DateFormats.yMd).getFormattedDate();

        }
    }
}
