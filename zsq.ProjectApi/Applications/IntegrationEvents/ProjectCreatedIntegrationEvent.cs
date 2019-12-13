using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace zsq.ProjectApi.Applications.IntergrationEvents
{
    public class ProjectCreatedIntegrationEvent
    {
        public int ProjectId { get; set; }

        public string Avatar { get; set; }

        public string Company { get; set; }

        public string Introduction { get; set; }

        public string Tags { get; set; }

        public string FinStage { get; set; }

        public int UserId { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
