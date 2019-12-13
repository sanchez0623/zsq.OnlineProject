﻿using DotNetCore.CAP;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using zsq.Project.Domain.Events;
using zsq.ProjectApi.Applications.IntergrationEvents;

namespace zsq.ProjectApi.Applications.DomainEventHandlers
{
    public class ProjectJoinDomainHandler : INotificationHandler<ProjectJoinedEvent>
    {
        private ICapPublisher _capPublisher;

        public ProjectJoinDomainHandler(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public async Task Handle(ProjectJoinedEvent notification, CancellationToken cancellationToken)
        {
            var @event = new ProjectJoinedIntegrationEvent
            {
                Company = notification.Company,
                Avatar = notification.Avatar,
                Introduction = notification.Introduction,
                Contributor = notification.Contributor
            };

            await _capPublisher.PublishAsync("zsq.onlinePrject.projectapi.projectviewed", @event);
            return;
        }
    }
}