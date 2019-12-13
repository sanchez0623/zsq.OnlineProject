using MediatR;
using System.Threading;
using System.Threading.Tasks;
using zsq.Project.Domain.AggregatesModel;
using zsq.Project.Domain.Exceptions;

namespace zsq.ProjectApi.Applications.Commands
{
    public class ViewProjectCommandHandler : IRequestHandler<ViewProjectCommand, bool>
    {
        private IProjectRepository _projectRepository;

        public ViewProjectCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<bool> Handle(ViewProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetAsync(request.ProjectId);

            if (project == null)
            {
                throw new ProjectDomainException($"项目 {request.ProjectId} 在哪里??");
            }

            if (project.UserId == request.UserId)
            {
                throw new ProjectDomainException("自己查看自己项目是怎么肥事??");
            }

            project.AddViewer(request.UserId, request.UserName, request.Avatar);
            var result = await _projectRepository.UnitOfWork.SaveEntitiesAsync();

            return result;
        }
    }
}
