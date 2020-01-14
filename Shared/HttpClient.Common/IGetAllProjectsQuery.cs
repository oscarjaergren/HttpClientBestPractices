using System.Collections.Generic;
using System.Threading.Tasks;

namespace HttpClient.Common
{
    public interface IGetAllProjectsQuery
    {
        Task<IReadOnlyCollection<Project>> Execute();
    }
}