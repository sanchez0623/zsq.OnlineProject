using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zsq.ContactApi.Dtos;
using zsq.ContactApi.Models;

namespace zsq.ContactApi.Data
{
    public interface IContactRepository
    {
        Task<bool> UpdateContactInfoAsync(UserInfo userInfo, CancellationToken cancellationToken);

        Task<bool> AddContactAsync(int contactId, UserInfo contactInfo, CancellationToken cancellationToken);

        Task<List<Contact>> GetContactsAsync(int userId, CancellationToken cancellationToken);

        Task<bool> TagContactAsync(int userId, int contactId, List<string> tags, CancellationToken cancellationToken);
    }
}
