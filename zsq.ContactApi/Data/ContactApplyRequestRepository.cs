using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zsq.ContactApi.Models;

namespace zsq.ContactApi.Data
{
    public class ContactApplyRequestRepository : IContactApplyRequestRepository
    {
        private readonly ContactContext _contactContext;

        public ContactApplyRequestRepository(ContactContext contactContext)
        {
            _contactContext = contactContext;
        }

        public async Task<bool> AddRequestAsync(ContactApplyRequest request, CancellationToken cancellationToken)
        {
            var filter = Builders<ContactApplyRequest>.Filter
                .Where(c => c.UserId == request.UserId && request.ApplierId == c.ApplierId);

            if ((await _contactContext.ContactApplyRequests.CountDocumentsAsync(filter, null, cancellationToken)) > 0)
            {
                var update = Builders<ContactApplyRequest>.Update.Set(r => r.CreateTime, DateTime.Now);
                var result = await _contactContext.ContactApplyRequests.UpdateOneAsync(filter, update, null, cancellationToken);
                return result.IsAcknowledged && result.MatchedCount == 1 && result.MatchedCount == result.ModifiedCount;
            }

            await _contactContext.ContactApplyRequests.InsertOneAsync(request, null, cancellationToken);
            return true;
        }

        public async Task<bool> ApprovalAsync(int userId, int applierId, CancellationToken cancellationToken)
        {
            var filter = Builders<ContactApplyRequest>.Filter
                .Where(c => c.UserId == userId && c.ApplierId == applierId);

            var update = Builders<ContactApplyRequest>.Update
                .Set(r => r.HandleTime, DateTime.Now)
                .Set(r => r.Approvaled, 1);
            var result = await _contactContext.ContactApplyRequests.UpdateOneAsync(filter, update, null, cancellationToken);
            return result.IsAcknowledged && result.MatchedCount == 1 && result.MatchedCount == result.ModifiedCount;
        }

        public async Task<List<ContactApplyRequest>> GetRequestListAsync(int userId, CancellationToken cancellationToken)
        {
            var requests = (await _contactContext.ContactApplyRequests.FindAsync(r => r.UserId == userId)).ToList(cancellationToken);
            return requests;
        }
    }
}
