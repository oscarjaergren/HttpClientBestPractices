using HttpClient.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace JOSHttpClient.Version2
{
    public class GetAllProjectsQuery : IGetAllProjectsQuery
    {
        private readonly GitHubClient _gitHubClient;

        public GetAllProjectsQuery(GitHubClient gitHubClient)
        {
            _gitHubClient = gitHubClient ?? throw new ArgumentNullException(nameof(gitHubClient));
        }

        public async Task<IReadOnlyCollection<Project>> Execute()
        {
            var response = await _gitHubClient.GetRepositories().ConfigureAwait(false);
            return response.Select(x => new Project(x.Name, x.Url, x.Stars)).ToArray();
        }
    }
}
