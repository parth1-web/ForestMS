using DateConverter.Core.Service_Factory;
using LE.Common.Exceptions;
using LE.Inventory.Common.Enums;
using LE.Inventory.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LE.Inventory.Infrastructure.Dto
{
    public class WoodDetailsDto
    {
        private long _categoryPurposeId, _stockTypeId, _woodTypeId, _pilingId;
        private string _goliyaNo, _tunaNo;
        private decimal _circleSize, _length;

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

        [Required]
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

        //public bool is_transferred_to_chiran { get; set; } = false;

        [MaxLength(10)]
        public string tuna_no
        {
            get => _tunaNo;
            set
            {
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

        public decimal net_total_size { get; set; }
        public decimal fresh_total_size { get; set; }
        public decimal hole_total_size { get; set; }

        [Required]
        public Grade grade { get; set; }

        [Required]
        public DateTime created_date { get; set; } = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();

        [Required]
        public string year { get; set; }

        public bool is_sold { get; set; } = false;

        public virtual StockCategoryPurpose category_purpose { get; set; }

        public virtual Piling piling { get; set; }

        public virtual WoodType wood_type { get; set; }

        public virtual List<DamagedWoodDetailDto> DamagedWoodDetailDtos { get; set; }

    }
}
