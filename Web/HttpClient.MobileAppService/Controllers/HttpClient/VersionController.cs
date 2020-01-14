using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HttpClient.Common;
using Microsoft.AspNetCore.Mvc;

namespace HttpClient.MobileAppService.Controllers.HttpClient
{

    [Route("api/[controller]")]
    [Produces("application/json")]
    public class VersionController : Controller
    {
        private readonly IGetAllProjectsQuery _getAllProjectsQuery;

        public VersionController(IGetAllProjectsQuery getAllProjectsQuery)
        {
            _getAllProjectsQuery = getAllProjectsQuery ?? throw new ArgumentNullException(nameof(getAllProjectsQuery));
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllProjects()
        {
            var result = await _getAllProjectsQuery.Execute().ConfigureAwait(false);
            var response = new ApiResponse<ProjectResponseDto>(
                (int)HttpStatusCode.OK,
                new ProjectResponseDto(result.Select(x => new ProjectDto(x.Name, x.Url, x.Stars)).OrderByDescending(x => x.Stars).ToArray()));

            return new OkObjectResult(response);
        }
    }
}