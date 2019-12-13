using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using zsq.Project.Domain.SeedWork;
using zsq.Project.Infrastructure.EntityConfiguration;
using ProjectEntity = zsq.Project.Domain.AggregatesModel.Project;

namespace zsq.Project.Infrastructure
{
    public class ProjectContext : DbContext, IUnitOfWork
    {
        private IMediator _mediator;

        public DbSet<ProjectEntity> Projects { get; set; }

        public ProjectContext(DbContextOptions<ProjectContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProjectConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectPropertyConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectViewerConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectContributorConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectVisibleRuleConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await _mediator.DispatchDomainEventsAsync(this);
            //需写在savechanges之前，如果领域事件出异常，则不会保存数据，保证数据正确性

            await base.SaveChangesAsync();
            return true;
            //下面这样是不是保险一点？
            //var result = await base.SaveChangesAsync();
            //return result > 0;
        }
    }
}
