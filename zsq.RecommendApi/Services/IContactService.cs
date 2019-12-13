using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zsq.RecommendApi.Dtos;

namespace zsq.RecommendApi.Services
{
    public interface IContactService
    {
        Task<List<ContactInfo>> GetContactInfoByUser(int userId);
    }
}
