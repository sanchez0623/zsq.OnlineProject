using System;
using System.Collections.Generic;
using System.Text;
using zsq.Project.Domain.SeedWork;

namespace zsq.Project.Domain.AggregatesModel
{
    public class ProjectViewer : Entity
    {
        public int ProjectId { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Avatar { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
