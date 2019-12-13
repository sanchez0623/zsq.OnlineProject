using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace zsq.UserIdentity.Authentication
{
    public class ProfileService : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject ?? throw new ArgumentException(nameof(context.Subject));

            var subjectId = subject.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            if (!int.TryParse(subjectId, out int userId))
            {
                throw new ArgumentException("非法用户");
            }
            context.IssuedClaims = context.Subject.Claims.ToList();
            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            var subject = context.Subject ?? throw new ArgumentException(nameof(context.Subject));

            var subjectId = subject.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            context.IsActive = int.TryParse(subjectId, out int userId);
            return Task.CompletedTask;
        }
    }
}
