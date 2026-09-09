using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LE.Infrastructure.Dto
{
    public class DynamicMenuDto
    {
        private long? _parentMenuId;

        public long dynamic_menu_id { get; set; }

        [Required]
        public long module_id { get; set; }

        public long? parent_menu_id
        {
            get => _parentMenuId;
            set
            {
                if (value == 0)
                {
                    _parentMenuId = null;
                }
                else
                {
                    _parentMenuId = value;
                }
            }
        }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Module Name is required")]
        [MaxLength(30)]
        public string menu_name { get; set; }

        [MaxLength(20)]
        public string icon { get; set; }

        [MaxLength(50)]
        public string web_url { get; set; }

        [MaxLength(50)]
        public string api_url { get; set; }

        public int display_order { get; set; }
    }
}
