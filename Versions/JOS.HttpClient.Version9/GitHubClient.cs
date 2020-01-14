using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using HttpClient.Common;

namespace JOSHttpClient.Version9
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
            var request = CreateRequest();
            var result = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            using (var contentStream = await result.Content.ReadAsStreamAsync())
            {
                return await JsonSerializer.DeserializeAsync<List<GitHubRepositoryDto>>(contentStream, DefaultJsonSerializerOptions.Options);
            }
        }

        private static HttpRequestMessage CreateRequest()
        {
            return new HttpRequestMessage(HttpMethod.Get, GitHubConstants.RepositoriesPath);
        }
    }
}
