using DotNetCore.CAP;
using System;
using System.Threading.Tasks;
using zsq.RecommendApi.Data;
using zsq.RecommendApi.IntegrationEvents;
using zsq.RecommendApi.Models;
using zsq.RecommendApi.Services;

namespace zsq.RecommendApi.IntegrationEventHandlers
{
    public class ProjectCreatedIntegrationEventHandler : ICapSubscribe
    {
        private RecommendContext _context;
        private IUserService _userService;
        private IContactService _contactService;

        public ProjectCreatedIntegrationEventHandler(RecommendContext context,
            IUserService userService,
            IContactService contactService)
        {
            _context = context;
            _userService = userService;
            _contactService = contactService;
        }

        [CapSubscribe("zsq.onlinePrject.projectapi.projectcreated")]
        public async Task<bool> CreateRecommendFromProject(ProjectCreatedIntegrationEvent @event)
        {
            var fromUserInfo = await _userService.GetUserInfoAsync(@event.UserId);
            var contactsInfo = await _contactService.GetContactInfoByUser(@event.UserId);

            //添加一度好友的推荐
            foreach (var contactInfo in contactsInfo)
            {
                var recommend = new ProjectRecommend
                {
                    CreateTime = @event.CreateTime,
                    ProjectAvatar = @event.Avatar,
                    ProjectCompany = @event.Company,
                    ProjectFinStage = @event.FinStage,
                    ProjectId = @event.ProjectId,
                    ProjectIntroduction = @event.Introduction,
                    ProjectTags = @event.Tags,
                    RecommendTime = DateTime.Now,
                    RecommendType = EnumRecommendType.Friend,
                    FromUserId = @event.UserId,
                    FromUserAvatar = fromUserInfo.Avatar,
                    FromUserName = fromUserInfo.Name,
                    UserId = contactInfo.UserId
                };

                await _context.ProjectRecommends.AddAsync(recommend);
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
