using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zsq.ContactApi.Dtos;
using zsq.ContactApi.Models;

namespace zsq.ContactApi.Data
{
    public class ContactRepository : IContactRepository
    {
        private readonly ContactContext _contactContext;
        private ILogger<ContactRepository> _logger;

        public ContactRepository(ContactContext contactContext,
            ILogger<ContactRepository> logger)
        {
            _contactContext = contactContext;
            _logger = logger;
        }

        public async Task<bool> AddContactAsync(int contactId, UserInfo contactInfo, CancellationToken cancellationToken)
        {
            if (await _contactContext.ContactBooks.CountDocumentsAsync(c => c.UserId == contactId) <= 0)
            {
                await _contactContext.ContactBooks.InsertOneAsync(new ContactBook { UserId = contactId });
            }

            var filter = Builders<ContactBook>.Filter.Eq(c => c.UserId, contactId);
            var update = Builders<ContactBook>.Update.AddToSet(c => c.Contacts, new Contact
            {
                UserId = contactInfo.UserId,
                Avatar = contactInfo.Avatar,
                Cpmpany = contactInfo.Company,
                Name = contactInfo.Name,
                Title = contactInfo.Title
            });

            var result = await _contactContext.ContactBooks.UpdateOneAsync(filter, update, null, cancellationToken);
            return result.IsAcknowledged && result.ModifiedCount == 1 && result.MatchedCount == result.ModifiedCount;
        }

        public async Task<List<Contact>> GetContactsAsync(int userId, CancellationToken cancellationToken)
        {
            var contactBook = await
                (await _contactContext.ContactBooks
                    .FindAsync(c => c.UserId == userId, null, cancellationToken)).FirstOrDefaultAsync();

            if (contactBook == null)
            {
                _logger.LogWarning($"未找到该用户 {userId} 的通讯录");
                return new List<Contact>();
            }

            return contactBook.Contacts;
        }

        public async Task<bool> UpdateContactInfoAsync(UserInfo userInfo, CancellationToken cancellationToken)
        {
            var userIdFilter = Builders<ContactBook>.Filter.Eq(c => c.UserId, userInfo.UserId);

            var contactBook = (await _contactContext.ContactBooks.FindAsync(userIdFilter, null, cancellationToken)).FirstOrDefault();
            if (contactBook == null)
            {
                return true;
                //throw new Exception($"找不到 {userInfo.UserId} 的通讯录");
            }

            var contactIds = contactBook.Contacts.Select(c => c.UserId);
            var filter = Builders<ContactBook>.Filter.And(
                    Builders<ContactBook>.Filter.In(c => c.UserId, contactIds),
                    Builders<ContactBook>.Filter.ElemMatch(c => c.Contacts, contact => contact.UserId == userInfo.UserId)
                );

            var contactInfoUpdate = Builders<ContactBook>.Update
                .Set("Contacts.$.Name", userInfo.Name)
                .Set("Contacts.$.Company", userInfo.Company)
                .Set("Contacts.$.Title", userInfo.Title)
                .Set("Contacts.$.Avatar", userInfo.Avatar);

            var updateResult = _contactContext.ContactBooks.UpdateMany(filter, contactInfoUpdate);
            return updateResult.IsAcknowledged && updateResult.MatchedCount == updateResult.ModifiedCount;
        }

        public async Task<bool> TagContactAsync(int userId, int contactId, List<string> tags, CancellationToken cancellationToken)
        {
            var filter = Builders<ContactBook>.Filter.And(
                    Builders<ContactBook>.Filter.Eq(c => c.UserId, userId),
                    Builders<ContactBook>.Filter.Eq("Contacts.UserId", contactId)
                );

            var update = Builders<ContactBook>.Update
                .Set("Contacts.$.Tags", tags);

            var result = await _contactContext.ContactBooks.UpdateOneAsync(filter, update, null, cancellationToken);
            return result.IsAcknowledged && result.MatchedCount == 1 && result.MatchedCount == result.ModifiedCount;
        }
    }
}
