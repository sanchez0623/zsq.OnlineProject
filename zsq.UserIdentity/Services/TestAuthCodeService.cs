using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace zsq.UserIdentity.Services
{
    public class TestAuthCodeService : IAuthCodeService
    {
        public bool Validate(string phone, string code)
        {
            return true;
        }
    }
}
