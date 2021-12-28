using System;
using System.Collections.Generic;
using System.Text;

namespace ManyHelpers.Web {
    public static class EmailHelper {
        public static bool IsValidEmail(this string email) {
            try {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            } catch {
                return false;
            }
        }
    }
}
