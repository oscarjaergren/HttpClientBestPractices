using System;
using System.Collections.Generic;

using HttpClient.Common;

using Newtonsoft.Json;

namespace JOSHttpClient.Version0
{
    public class GitHubClient
    {
        public IReadOnlyCollection<GitHubRepositoryDto> GetRepositories()
        {
            using (var httpClient =
                new System.Net.Http.HttpClient { BaseAddress = new Uri(GitHubConstants.ApiBaseUrl) })
            {
                string result = httpClient.GetStringAsync(GitHubConstants.RepositoriesPath).Result;
                return JsonConvert.DeserializeObject<List<GitHubRepositoryDto>>(result);
            }
        }
    }
}