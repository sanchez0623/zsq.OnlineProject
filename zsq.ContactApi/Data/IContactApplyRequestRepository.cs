using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using zsq.ContactApi.Models;

namespace zsq.ContactApi.Data
{
    public interface IContactApplyRequestRepository
    {
        Task<bool> AddRequestAsync(ContactApplyRequest request, CancellationToken cancellationToken);

        Task<bool> ApprovalAsync(int userId, int applierId, CancellationToken cancellationToken);

        Task<List<ContactApplyRequest>> GetRequestListAsync(int userId, CancellationToken cancellationToken);
    }
}
