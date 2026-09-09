using LE.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LE.Entities.User
{
    public class User
    {
        private string _fullName, _addressLine1, _primaryContact;

        [Key]
        public long user_id { get; set; }

        [Required]
        public string full_name
        {
            get => _fullName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new NonEmptyValueException("Full name must be specified.");
                }
                _fullName = value;
            }
        }

        [Required]
        public string address_line_1
        {
            get => _addressLine1;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new NonEmptyValueException("Address must be specified.");
                }
                _addressLine1 = value;
            }
        }

        public string address_line_2 { get; set; }

        [Required]
        public string primary_contact
        {
            get => _primaryContact;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new NonEmptyValueException("Primary contact must be specified.");
                }
                _primaryContact = value;
            }
        }

        public string secondary_contact { get; set; }

        public string email { get; set; }

        public bool is_active { get; set; } = true;

        public long created_by { get; set; }
        public DateTime created_date { get; set; } = DateTime.Now;

        public string image_path { get; set; }

        public virtual List<UserRole> roles { get; set; }

        public void disable()
        {
            is_active = false;
        }

        public void enable()
        {
            is_active = true;
        }
    }
}
