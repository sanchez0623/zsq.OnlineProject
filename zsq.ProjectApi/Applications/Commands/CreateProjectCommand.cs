using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectEntity = zsq.Project.Domain.AggregatesModel.Project;

namespace zsq.ProjectApi.Applications.Commands
{
    public class CreateProjectCommand : IRequest<ProjectEntity>
    {
        /// <summary>
        /// Todo：可以使用viewModel，而不是用domain的对象
        /// </summary>
        public ProjectEntity Project { get; set; }
    }
}
