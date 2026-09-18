using LE.Common.Exceptions;
using LE.Inventory.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Inventory.Entities
{
    public class WoodDetails
    {
        private long _categoryPurposeId, _stockTypeId, _woodTypeId, _pilingId;
        private string _goliyaNo, _tunaNo;
        private decimal _circleSize, _length;

        [Key]
        public long wood_details_id { get; set; }

        [Required]
        public long stock_category_purpose_id
        {
            get => _categoryPurposeId;
            set
            {
                if (value <= 0)
                {
                    throw new InvalidValueException("Category Purpose is not valid.");
                }
                _categoryPurposeId = value;
            }
        }

        public long piling_id
        {
            get => _pilingId;
            set
            {
                if (value <= 0)
                {
                    throw new InvalidValueException("Piling is not valid.");
                }
                _pilingId = value;
            }
        }

        [Required]
        public long stock_type_id
        {
            get => _stockTypeId;
            set
            {
                if (value <= 0)
                {
                    throw new InvalidValueException("Stock Type is not valid.");
                }
                _stockTypeId = value;
            }
        }

        public long balla_balli_category_id { get; set; }

        [Required]
        [MaxLength(10)]
        public string tuna_no
        {
            get => _tunaNo;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new NonEmptyValueException("Tuna No is required.");
                }
                _tunaNo = value;
            }
        }

        [Required]
        [MaxLength(10)]
        public string goliya_number
        {
            get => _goliyaNo;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new NonEmptyValueException("Goliya No is required.");
                }
                _goliyaNo = value;
            }
        }

        [Required]
        public long wood_type_id
        {
            get => _woodTypeId;
            set
            {
                if (value <= 0)
                {
                    throw new InvalidValueException("Wood Type is not valid.");
                }
                _woodTypeId = value;
            }
        }

        public decimal circle_size
        {
            get => _circleSize;
            set
            {
                if (value < 0)
                {
                    throw new InvalidValueException("Circle Size cannot be negative");
                }
                _circleSize = value;
            }
        }
        public decimal length
        {
            get => _length;
            set
            {
                if (value < 0)
                {
                    throw new InvalidValueException("Length cannot be negative");
                }
                _length = value;
            }
        }

        [Required]
        public string year { get; set; }

        [Required]
        public Grade grade { get; set; }

        [Required]
        public DateTime created_date { get; set; }

        public bool is_sold { get; set; } = false;

        public long? sales_id { get; set; }

        //public bool is_transferred_to_chiran { get; set; } = false;
        //public bool calcel_transfer_to_chiran { get; set; } = false;

        //public void transferredToChiran()
        //{
        //    is_transferred_to_chiran = true;
        //}
        //public void cancelTransferredToChiran()
        //{
        //    is_transferred_to_chiran = false;
        //}
        //public void ifTransferedCanceledToChiran()
        //{
        //    calcel_transfer_to_chiran = true;
        //}
        public void makeSale()
        {
            is_sold = true;
        }

        public decimal getNetTotal()
        {
            return Math.Round(fresh_total_size - getDamagedsize(), 2);
        }

        public void setFreshTotalSize()
        {
            fresh_total_size = Math.Round((circle_size * circle_size * length) / Convert.ToDecimal(2304), 2);
        }

        public decimal getHoleTotalSize()
        {
            return getDamagedsize();
        }


        public decimal fresh_total_size { get; set; }

        [ForeignKey("stock_category_purpose_id")]
        public virtual StockCategoryPurpose category_purpose { get; set; }

        [ForeignKey("piling_id")]
        public virtual Piling piling { get; set; }

        [ForeignKey("wood_type_id")]
        public virtual WoodType wood_type { get; set; }

        public virtual List<DamagedWoodDetail> DamagedWoodDetails { get; set; }

        public decimal getDamagedsize()
        {
            decimal result = 0;
            if (DamagedWoodDetails?.Count > 0)
            {
                foreach (var detail in DamagedWoodDetails)
                {
                    result += detail.total_damaged_size;
                }
            }
            return result;
        }
    }
}
