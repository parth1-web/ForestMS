using DateConverter.Core.Service_Factory;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Service.Assemblers.Interface;
using System;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Inventory.Service.Assemblers.Implementations
{
    public class StockMovementAssemblerImpl : StockMovementAssembler
    {
        public void copy(StockMovement stock_movement, StockMovementDto stock_movement_dto)
        {
            var dateConverterService =DateConverterFactory.getDateConverterService();

            stock_movement.movement_type = stock_movement_dto.movement_type;
            stock_movement.movement_type_id = stock_movement_dto.movement_type_id;

            stock_movement.movement_date = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
            stock_movement.nep_movement_date = dateConverterService.ToBS(stock_movement.movement_date,DateFormats.yMd).getFormattedDate();
        }
    }
}
