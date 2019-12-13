using MediatR;
using ProjectEntity = zsq.Project.Domain.AggregatesModel.Project;

namespace zsq.Project.Domain.Events
{
    public class ProjectCreatedEvent : INotification
    {
        public ProjectEntity Project { get; set; }
    }
}
