using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace zsq.UserApi.Infrastructure
{
    public static class StringHelper
    {
        public static bool IsPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return false;
            }

            return Regex.IsMatch(phone, @"^[1]+[3,9]+\d{9}");
        }
    }
}
