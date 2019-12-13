using System;
using System.Collections.Generic;
using System.Text;
using zsq.Project.Domain.SeedWork;

namespace zsq.Project.Domain.AggregatesModel
{
    public class ProjectVisibleRule : Entity
    {
        public int ProjectId { get; set; }

        public bool Visible { get; set; }

        public string Tags { get; set; }
    }
}
