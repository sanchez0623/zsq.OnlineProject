using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zsq.Project.Domain.AggregatesModel;

namespace zsq.ProjectApi.Applications.IntergrationEvents
{
    public class ProjectViewedIntegrationEvent
    {
        public string Company { get; set; }

        public string Introduction { get; set; }

        public string Avatar { get; set; }

        public ProjectViewer Viewer { get; set; }
    }
}
