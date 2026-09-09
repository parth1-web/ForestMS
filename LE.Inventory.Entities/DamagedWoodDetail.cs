using LE.Common.Exceptions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Inventory.Entities
{
    public class DamagedWoodDetail
    {
        private long _woodDetailsId;

        [Key]
        public long damaged_wood_details_id { get; set; }
        [Required]
        public long wood_details_id
        {
            get => _woodDetailsId;
            set
            {
                if (value <= 0)
                    throw new InvalidValueException("Wood Detail not valid.");
                _woodDetailsId = value;
            }
        }
        public decimal damaged_first_size { get; set; }
        public decimal damaged_second_size { get; set; }
        public decimal damaged_third_size { get; set; }
        public decimal damaged_fourth_size { get; set; }
        public decimal damaged_fifth_size { get; set; }

        public long dividor_value { get; set; }

        public decimal damaged_feet_size { get; set; }

        public decimal total_damaged_size { get; set; }

        public void setTotalDamagedSize()
        {
            var q = ((damaged_first_size + damaged_second_size + damaged_third_size + damaged_fourth_size + damaged_fifth_size) / getDividorValue());
            var r = q * (decimal)3.14;
            total_damaged_size = (r * r * damaged_feet_size) / Convert.ToDecimal(2304);
            setDividorValue();
        }

        [ForeignKey("wood_details_id")]
        public virtual WoodDetails WoodDetails { get; set; }

        private long getDividorValue()
        {
            long count = 0;

            if (damaged_first_size > 0)
                count++;
            if (damaged_second_size > 0)
                count++;
            if (damaged_third_size > 0)
                count++;
            if (damaged_fourth_size > 0)
                count++;
            if (damaged_fifth_size > 0)
                count++;

            return count;
        }

        public void setDividorValue()
        {
            dividor_value = getDividorValue();
        }
    }
}
