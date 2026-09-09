using LE.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace LE.Entities.User
{
    public class DynamicMenu
    {
        private string _menuName;

        [Key]
        public long dynamic_menu_id { get; set; }

        [Required]
        public long module_id { get; set; }
        public long? parent_menu_id { get; set; }

        [Required]
        [MaxLength(30)]
        public string menu_name
        {
            get => _menuName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new NonEmptyValueException("Menu name cannot be empty.");
                }
                _menuName = value;
            }
        }

        [MaxLength(20)]
        public string icon { get; set; }

        [MaxLength(50)]
        public string web_url { get; set; }

        [MaxLength(50)]
        public string api_url { get; set; }

        public int display_order { get; set; } = 1;

        [ForeignKey(nameof(parent_menu_id))]
        public virtual DynamicMenu parent_menu { get; set; }

        [ForeignKey(nameof(module_id))]
        public virtual Module module { get; set; }

        public virtual List<DynamicMenu> sub_menus { get; set; } = new List<DynamicMenu>();

        public bool hasSubMenus() => sub_menus.Count() > 0;
    }
}
