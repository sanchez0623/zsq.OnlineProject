using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zsq.ContactApi.Data;
using zsq.ContactApi.Dtos;
using zsq.ContactApi.IntergrationEvents.Events;

namespace zsq.ContactApi.IntergrationEvents.EventsHandler
{
    public class UserProfileChangedEventHandler : ICapSubscribe
    {
        private IContactRepository _contactRepository;

        public UserProfileChangedEventHandler(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        [CapSubscribe("zsq.onlinePrject.userapi.userprofilechanged")]
        public async Task UpdateContactInfoAsync(UserProfileChangedEvent @event)
        {
            var token = new CancellationToken();

            await _contactRepository.UpdateContactInfoAsync(new UserInfo
            {
                Avatar = @event.Avatar,
                Company = @event.Company,
                Title = @event.Title,
                UserId = @event.UserId,
                Name = @event.Name,
            }, token);
        }
    }
}
