using System;
using System.Threading.Tasks;

namespace HttpClient.Services
{
    public interface IRequestProvider
    {
        Task DeleteAsync(Uri uri, string token = "");

        Task<TResult> GetAsync<TResult>(Uri uri, string token = "");

        Task<TResult> PostAsync<TResult>(Uri uri, TResult data, string token = "", string header = "");

        Task<TResult> PostAsync<TResult>(Uri uri, string data, string clientId, string clientSecret);

        Task<TResult> PutAsync<TResult>(Uri uri, TResult data, string token = "", string header = "");
    }
}