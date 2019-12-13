using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zsq.ContactApi.Dtos;

namespace zsq.ContactApi.Services
{
    public interface IUserService
    {
        Task<UserInfo> GetUserInfoAsync(int userId);
    }
}
