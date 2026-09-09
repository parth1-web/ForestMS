using LE.Common.Enums;
using LE.Common.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace LE.Entities.User
{
    public class Authentication
    {
        private long _typeId;
        private string _username, _password;

        [Key]
        public long authentication_id { get; set; }

        [Required]
        public long type_id
        {
            get => _typeId;
            set
            {
                if (value <= 0)
                {
                    throw new InvalidValueException("Type id cannot be less than or equal to zero.");
                }
                _typeId = value;
            }

        }

        [Required]
        public UserType type { get; set; }

        [Required]
        public string username
        {
            get => _username;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new NonEmptyValueException("Username must be provided.");
                }
                _username = value;
            }
        }

        [Required]
        public string password
        {
            get => _password;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new NonEmptyValueException("Password must be provided.");
                }
                _password = value;
            }
        }

        public bool is_enabled { get; set; } = true;

        public void activate()
        {
            is_enabled = true;
        }

        public void deactivate()
        {
            is_enabled = false;
        }
    }
}
