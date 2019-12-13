using Dapper;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace zsq.ProjectApi.Applications.Queries
{
    public class ProjectQuery : IProjectQuery
    {
        private string _connStr;

        public ProjectQuery(string connStr)
        {
            _connStr = connStr;
        }

        public async Task<dynamic> GetProjectDetailAsync(int projectId)
        {
            //使用这个方法，需要注入dbcontext
            //using (var connection = new _context.Database.GetDbConnection(_connStr))
            using (var connection = new MySqlConnection(_connStr))
            {
                await connection.OpenAsync();

                var sql = @"SELECT
                                Projects.Company,
	                            Projects.City,
	                            Projects.Area,
	                            Projects.Province,
	                            Projects.FinStage,
	                            Projects.FinMoney,
	                            Projects.Valuation,
	                            Projects.FinPercentage,
	                            Projects.Introduction,
	                            Projects.Userid,
	                            Projects.UserName,
	                            Projects.Income,
	                            Projects.Revenue,
	                            Projects.Avatar,
	                            Projects.BrokerageOptions,
	                            ProjectVisibleRules.Tags,
	                            ProjectVisibleRules.Visible
                            FROM
                                Projects
                                INNER JOIN ProjectVisibleRules ON Projects.Id = ProjectVisibleRules.ProjectId
                            WHERE
                                Projects.Id = @projectId";

                var result = await connection.QueryAsync<dynamic>(sql, new { projectId });
                return result;
            }
        }

        public async Task<dynamic> GetProjectsByUserIdAsync(int userId)
        {
            using (var connection = new MySqlConnection(_connStr))
            {
                await connection.OpenAsync();

                var sql = @"SELECT
	                            Id,
	                            Avatar,
	                            Company,
	                            FinStage,
	                            Introduction,
	                            ShowSecurityInfo,
	                            CreateTime 
                            FROM
	                            Projects 
                            WHERE
	                            UserId = @userId";

                var result = await connection.QueryAsync<dynamic>(sql, new { userId });
                return result;
            }
        }
    }
}
