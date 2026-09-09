using LE.Common.Exceptions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LE.Entities.OrganizationSetup
{
    public class OrganizationSetup
    {
        private string _key, _value;

        [Key]
        public long organization_setup_id { get; set; }

        [Required]
        [MaxLength(70)]
        public string key
        {
            get => _key;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new NonEmptyValueException("Key is required.");
                }
                _key = value;
            }
        }

        [MaxLength(500)]
        public string value { get; set; }

    }
}
