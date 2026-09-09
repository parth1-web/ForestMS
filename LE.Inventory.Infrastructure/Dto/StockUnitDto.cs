using LE.Common.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace LE.Inventory.Infrastructure.Dto
{
    public class StockUnitDto
    {
        private string _name, _shortName;

        public long stock_unit_id { get; set; }

        [Required]
        [MaxLength(50)]
        public string name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new NonEmptyValueException("Name is required.");
                }
                _name = value;
            }
        }

        [Required]
        [MaxLength(10)]
        public string short_name
        {
            get => _shortName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new NonEmptyValueException("Short name is required.");
                }
                _shortName = value;
            }
        }

        public bool is_enabled { get; set; } = true;
    }
}
