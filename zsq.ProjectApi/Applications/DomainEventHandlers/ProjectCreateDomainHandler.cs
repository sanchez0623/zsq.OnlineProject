using DotNetCore.CAP;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using zsq.Project.Domain.Events;
using zsq.ProjectApi.Applications.IntergrationEvents;

namespace zsq.ProjectApi.Applications.DomainEventHandlers
{
    public class ProjectCreateDomainHandler : INotificationHandler<ProjectCreatedEvent>
    {
        private ICapPublisher _capPublisher;

        public ProjectCreateDomainHandler(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public async Task Handle(ProjectCreatedEvent notification, CancellationToken cancellationToken)
        {
            var @event = new ProjectCreatedIntegrationEvent
            {
                UserId = notification.Project.UserId,
                ProjectId = notification.Project.Id,
                Avatar = notification.Project.Avatar,
                Company = notification.Project.Company,
                FinStage = notification.Project.FinStage,
                Introduction = notification.Project.Introduction,
                Tags = notification.Project.Tags,
                CreateTime = DateTime.Now
            };

            await _capPublisher.PublishAsync("zsq.onlinePrject.projectapi.projectcreated", @event);
            return;
        }
    }
}
