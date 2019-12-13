using MediatR;
using zsq.Project.Domain.AggregatesModel;

namespace zsq.Project.Domain.Events
{
    public class ProjectViewedEvent : INotification
    {
        public string Company { get; set; }

        public string Introduction { get; set; }

        public string Avatar { get; set; }

        public ProjectViewer Viewer { get; set; }
    }
}
