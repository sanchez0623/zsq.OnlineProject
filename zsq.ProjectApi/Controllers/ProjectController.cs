using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using zsq.Project.Domain.AggregatesModel;
using zsq.ProjectApi.Applications.Commands;
using zsq.ProjectApi.Applications.Queries;
using zsq.ProjectApi.Services;
using ProjectEntity = zsq.Project.Domain.AggregatesModel.Project;

namespace zsq.ProjectApi.Controllers
{
    [Route("api/projects")]
    [ApiController]
    public class ProjectController : BaseController
    {
        private IMediator _mediator;
        private IRecommendService _recommendService;
        private IProjectQuery _projectQuery;

        public ProjectController(IMediator mediator,
            IRecommendService recommendService,
            IProjectQuery projectQuery)
        {
            _mediator = mediator;
            _recommendService = recommendService;
            _projectQuery = projectQuery;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetProjects()
        {
            var projects = await _projectQuery.GetProjectsByUserIdAsync(UserIdentity.UserId);
            return Ok(projects);
        }

        [HttpGet]
        [Route("detail/{projectId}")]
        public async Task<IActionResult> GetMyProjectDetail(int projectId)
        {
            var project = await _projectQuery.GetProjectDetailAsync(projectId);
            //自己的项目才有权查看
            if (project.UserId == UserIdentity.UserId)
            {
                return Ok(project);
            }

            return BadRequest("无权查看");
        }

        [HttpGet]
        [Route("recommends/{projectId}")]
        public async Task<IActionResult> GetRecommendProjectDetail(int projectId)
        {
            if (!(await _recommendService.IsProjectInRecommend(projectId, UserIdentity.UserId)))
            {
                return BadRequest("同志，那可不是你能看的项目哦~");
            }

            var project = await _projectQuery.GetProjectDetailAsync(projectId);
            return Ok(project);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateProject([FromBody]ProjectEntity request)
        {
            //Todo: 参数正确性判断

            if (request == null)
            {
                throw new ArgumentNullException("参数为空");
            }

            request.UserId = UserIdentity.UserId;
            var command = new CreateProjectCommand { Project = request };
            var project = await _mediator.Send(command);
            return Ok(project);
        }

        [Route("view/{projectId}")]
        [HttpPut]
        public async Task<IActionResult> ViewProject(int projectId)
        {
            if (!(await _recommendService.IsProjectInRecommend(projectId, UserIdentity.UserId)))
            {
                //可以抛全局异常
                return BadRequest("同志，那可不是你能看的项目哦~");
            }
            //Todo: 参数正确性判断

            var command = new ViewProjectCommand
            {
                Avatar = UserIdentity.Avatar,
                ProjectId = projectId,
                UserId = UserIdentity.UserId,
                UserName = UserIdentity.Name
            };
            var project = await _mediator.Send(command);
            return Ok(project);
        }

        [Route("join")]
        [HttpPut]
        public async Task<IActionResult> JoinProject(ProjectContributor contributor)
        {
            if (!(await _recommendService.IsProjectInRecommend(contributor.ProjectId, UserIdentity.UserId)))
            {
                //可以抛全局异常
                return BadRequest("同志，那可不是你能看的项目哦~");
            }

            //Todo: 参数正确性判断

            var command = new JoinProjectCommand { Contributor = contributor };
            var project = await _mediator.Send(command);
            return Ok(project);
        }
    }
}
