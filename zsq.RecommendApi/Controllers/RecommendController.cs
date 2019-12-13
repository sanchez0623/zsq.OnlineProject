using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using zsq.RecommendApi.Data;

namespace zsq.RecommendApi.Controllers
{
    [Route("api/recommends")]
    [ApiController]
    public class RecommendController : BaseController
    {
        private RecommendContext _recommendContext;

        public RecommendController(RecommendContext recommendContext)
        {
            _recommendContext = recommendContext;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            var recommends = await _recommendContext.ProjectRecommends.AsNoTracking()
                .Where(r => r.UserId == UserIdentity.UserId).ToListAsync();

            return Ok(recommends);
        }
    }
}
