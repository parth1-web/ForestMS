using LE.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LE.Entities.User
{
    public class Module
    {
        private string _moduleName;

        [Key]
        public long module_id { get; set; }

        [Required]
        [MaxLength(30)]
        public string module_name
        {
            get => _moduleName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new NonEmptyValueException("Module name must be provided.");
                }
                _moduleName = value;
            }
        }

        [MaxLength(15)]
        public string module_code { get; set; }

        [MaxLength(50)]
        public string display_icon { get; set; }

        public virtual List<DynamicMenu> menus { get; set; } = new List<DynamicMenu>();

        public bool hasMenus() => menus.Count > 0;
    }
}
