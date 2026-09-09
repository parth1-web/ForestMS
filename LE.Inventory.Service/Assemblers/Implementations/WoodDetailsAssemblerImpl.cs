using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Service.Assemblers.Interface;

namespace LE.Inventory.Service.Assemblers.Implementations
{
    public class WoodDetailsAssemblerImpl : WoodDetailsAssembler
    {
        public void copy(WoodDetails wood_details, WoodDetailsDto wood_details_dto)
        {
            wood_details.wood_details_id = wood_details_dto.wood_details_id;
            wood_details.wood_type_id = wood_details_dto.wood_type_id;
            wood_details.stock_type_id = wood_details_dto.stock_type_id;
            wood_details.fresh_total_size = wood_details_dto.fresh_total_size;
            wood_details.length = wood_details_dto.length;
            wood_details.is_sold = wood_details_dto.is_sold;
            wood_details.grade = wood_details_dto.grade;
            wood_details.tuna_no = wood_details_dto.tuna_no;
            wood_details.goliya_number = wood_details_dto.goliya_number;
            wood_details.fresh_total_size = wood_details_dto.fresh_total_size;
            wood_details.stock_category_purpose_id = wood_details_dto.stock_category_purpose_id;
            wood_details.piling_id = wood_details_dto.piling_id;
            wood_details.balla_balli_category_id = wood_details_dto.balla_balli_category_id;
            wood_details.circle_size = wood_details_dto.circle_size;
            wood_details.created_date = wood_details_dto.created_date;
            wood_details.year = wood_details_dto.year;
            //wood_details.is_transferred_to_chiran = wood_details_dto.is_transferred_to_chiran;
        }
    }
}
