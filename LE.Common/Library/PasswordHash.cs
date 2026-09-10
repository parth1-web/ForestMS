using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Common.Library
{
    public interface PasswordHash
    {
        string CreateHash(string password);
        bool ValidatePassword(string password, string correct_hash);
        bool NeedsRehash(string correct_hash);
    }
}
