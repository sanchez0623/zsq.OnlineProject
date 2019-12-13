using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zsq.Project.Domain.AggregatesModel;

namespace zsq.ProjectApi.Applications.Commands
{
    public class JoinProjectCommand : IRequest<bool>
    {
        public ProjectContributor Contributor { get; set; }
    }
}
