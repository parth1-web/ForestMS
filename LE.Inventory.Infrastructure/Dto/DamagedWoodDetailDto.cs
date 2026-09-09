using LE.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Inventory.Infrastructure.Dto
{
    public class DamagedWoodDetailDto
    {
        public long damaged_wood_details_id { get; set; }
        public long wood_details_id { get; set; }
        public decimal damaged_first_size { get; set; }
        public decimal damaged_second_size { get; set; }
        public decimal damaged_third_size { get; set; }
        public decimal damaged_fourth_size { get; set; }
        public decimal damaged_fifth_size { get; set; }
        public decimal total_damaged_size { get; set; }

        public decimal damaged_feet_size { get; set; }

       
    }
}
