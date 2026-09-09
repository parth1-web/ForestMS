using LE.Common.Enums;
using LE.Common.Exceptions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LE.Entities.User
{
    public class LoginSession
    {
        public long _authenticationId;
        [Key]
        public long login_session_id { get; set; }

        [Required]
        public long authentication_id
        {
            get => _authenticationId;
            set
            {
                if (value <= 0)
                {
                    throw new InvalidValueException("Authentication id cannot be less than or equal to zero.");
                }
                _authenticationId = value;
            }
        }
        public DateTime date_time { get; set; } = DateTime.Now;
        public SessionType type { get; set; } = SessionType.login;

        [ForeignKey("authentication_id")]
        public virtual Authentication authentication { get; set; }
    }
}
