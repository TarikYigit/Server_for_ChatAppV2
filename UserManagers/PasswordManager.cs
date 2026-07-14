using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_for_ChatApp.UserManagers
{

    internal class PasswordManager
    {

        public static bool IsPasswordStrong(string password)
        {

            if (string.IsNullOrWhiteSpace(password)) return false;

            bool hasMinLength = password.Length >= 8;

            bool hasUpperCase = password.Any(char.IsUpper);

            bool hasLowerCase = password.Any(char.IsLower);

            bool hasDigits = password.Any(char.IsDigit);

            bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasMinLength && hasUpperCase && hasLowerCase && hasDigits && hasSpecial;

        }
    }
}