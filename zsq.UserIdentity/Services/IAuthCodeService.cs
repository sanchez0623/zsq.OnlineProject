﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace zsq.UserIdentity.Services
{
    public interface IAuthCodeService
    {
        bool Validate(string phone, string code);
    }
}