using System.Collections.Generic;
using System.Threading.Tasks;

namespace HttpClient.Common
{
    public interface IGitHubClient
    {
        Task<IReadOnlyCollection<GitHubRepositoryDto>> GetRepositories();
    }
}