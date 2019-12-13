using DotNetCore.CAP;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using zsq.Project.Domain.Events;
using zsq.ProjectApi.Applications.IntergrationEvents;

namespace zsq.ProjectApi.Applications.DomainEventHandlers
{
    public class ProjectViewDomainHandler : INotificationHandler<ProjectViewedEvent>
    {
        private ICapPublisher _capPublisher;

        public ProjectViewDomainHandler(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public async Task Handle(ProjectViewedEvent notification, CancellationToken cancellationToken)
        {
            var @event = new ProjectViewedIntegrationEvent
            {
                Company = notification.Company,
                Avatar = notification.Avatar,
                Introduction = notification.Introduction,
                Viewer = notification.Viewer
            };

            await _capPublisher.PublishAsync("zsq.onlinePrject.projectapi.projectjoined", @event);
            return;
        }
    }
}
