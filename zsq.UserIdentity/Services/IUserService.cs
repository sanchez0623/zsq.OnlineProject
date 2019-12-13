using System.Threading.Tasks;
using zsq.UserIdentity.Dtos;

namespace zsq.UserIdentity.Services
{
    public interface IUserService
    {
        Task<UserInfo> CheckOrCreateAsync(string phone);
    }
}
