using LE.Common.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace LE.Inventory.Infrastructure.Dto
{
    public class WoodTypeDto
    {
        private string _name;

        [Key]
        public long wood_type_id { get; set; }

        [Required]
        [MaxLength(50)]
        public string name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new NonEmptyValueException("Wood Type Name cannot be empty.");
                }
                _name = value;
            }
        }
        public decimal default_sales_rate { get; set; }
        [Required]
        public bool is_enabled { get; set; } = true;
    }
}
