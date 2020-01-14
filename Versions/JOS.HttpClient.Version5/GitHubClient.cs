using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using HttpClient.Common;

using Newtonsoft.Json;

namespace JOSHttpClient.Version5
{
    public class GitHubClient : IGitHubClient
    {
        private readonly System.Net.Http.HttpClient _httpClient;

        public GitHubClient(System.Net.Http.HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IReadOnlyCollection<GitHubRepositoryDto>> GetRepositories()
        {
            var result = await _httpClient.GetStringAsync(GitHubConstants.RepositoriesPath).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<List<GitHubRepositoryDto>>(result);
        }
    }
}
