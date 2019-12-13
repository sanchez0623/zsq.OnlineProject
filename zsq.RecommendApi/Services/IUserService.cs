using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zsq.RecommendApi.Dtos;

namespace zsq.RecommendApi.Services
{
    public interface IUserService
    {
        Task<UserInfo> GetUserInfoAsync(int userId);
    }
}
