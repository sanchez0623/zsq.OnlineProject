using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace zsq.ProjectApi.Applications.Queries
{
    public interface IProjectQuery
    {
        Task<dynamic> GetProjectsByUserIdAsync(int userId);

        Task<dynamic> GetProjectDetailAsync(int projectId);
    }
}
