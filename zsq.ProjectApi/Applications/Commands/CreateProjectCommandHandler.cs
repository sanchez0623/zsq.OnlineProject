using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using zsq.Project.Domain.AggregatesModel;
using ProjectEntity = zsq.Project.Domain.AggregatesModel.Project;

namespace zsq.ProjectApi.Applications.Commands
{
    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectEntity>
    {
        private IProjectRepository _projectRepository;

        public CreateProjectCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<ProjectEntity> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var result = await _projectRepository.AddAsync(request.Project);
            await _projectRepository.UnitOfWork.SaveEntitiesAsync();

            return result;
        }
    }
}
