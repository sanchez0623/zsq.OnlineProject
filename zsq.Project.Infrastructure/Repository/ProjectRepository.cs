using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using zsq.Project.Domain.AggregatesModel;
using zsq.Project.Domain.SeedWork;
using ProjectEntity = zsq.Project.Domain.AggregatesModel.Project;

namespace zsq.Project.Infrastructure.Repository
{
    public class ProjectRepository : IProjectRepository
    {
        private ProjectContext _projectContext;

        public IUnitOfWork UnitOfWork => _projectContext;

        public ProjectRepository(ProjectContext projectContext)
        {
            _projectContext = projectContext;
        }

        public async Task<ProjectEntity> AddAsync(ProjectEntity project)
        {
            if (project.IsTransient())
            {
                return (await _projectContext.AddAsync(project)).Entity;
            }

            return project;
        }

        public async Task<ProjectEntity> GetAsync(int projectId)
        {
            var project = await _projectContext.Projects
                .Include(p => p.Properties)
                .Include(p => p.Viewers)
                .Include(p => p.Contributors)
                .Include(p => p.VisibleRule)
                .SingleOrDefaultAsync(p => p.Id == projectId);
            return project;
        }

        public void Update(ProjectEntity project)
        {
            _projectContext.Update(project);
        }
    }
}
