using MediatR;
using System.Threading;
using System.Threading.Tasks;
using zsq.Project.Domain.AggregatesModel;
using zsq.Project.Domain.Exceptions;

namespace zsq.ProjectApi.Applications.Commands
{
    public class JoinProjectCommandHandler : IRequestHandler<JoinProjectCommand, bool>
    {
        private IProjectRepository _projectRepository;

        public JoinProjectCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<bool> Handle(JoinProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetAsync(request.Contributor.ProjectId);

            if (project == null)
            {
                throw new ProjectDomainException($"项目 {request.Contributor.ProjectId} 在哪里??");
            }

            if (project.UserId == request.Contributor.UserId)
            {
                throw new ProjectDomainException("自己加入自己项目是怎么肥事??");
            }

            project.AddContributor(request.Contributor);
            var result = await _projectRepository.UnitOfWork.SaveEntitiesAsync();

            return result;
        }
    }
}
