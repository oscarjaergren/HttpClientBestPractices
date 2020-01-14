using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpClient.MobileAppService.Controllers
{
    public class ProjectResponseDto
    {
        public ProjectResponseDto(IReadOnlyCollection<ProjectDto> repositories)
        {
            Repositories = repositories;
        }

        public IReadOnlyCollection<ProjectDto> Repositories { get; }
        public int NumberOfProjects => Repositories.Count;
    }

    public class ProjectDto
    {
        public ProjectDto(string name, string url, int stars)
        {
            Name = name;
            Url = url;
            Stars = stars;
        }

        public string Name { get; }
        public string Url { get; }
        public int Stars { get; }
    }
}
