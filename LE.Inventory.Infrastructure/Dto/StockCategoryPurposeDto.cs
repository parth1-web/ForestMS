using LE.Common.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace LE.Inventory.Infrastructure.Dto
{
    public class StockCategoryPurposeDto
    {
        private string _name;

        [Key]
        public long stock_category_purpose_id { get; set; }

        [Required]
        [MaxLength(50)]
        public string name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new NonEmptyValueException("Category Purpose Name cannot be empty.");
                }
                _name = value;
            }
        }

        [Required]
        public bool is_enabled { get; set; } = true;
    }
}
