using System.Threading.Tasks;
using zsq.Project.Domain.SeedWork;

namespace zsq.Project.Domain.AggregatesModel
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<Project> AddAsync(Project project);

        void Update(Project project);

        Task<Project> GetAsync(int projectId);
    }
}
