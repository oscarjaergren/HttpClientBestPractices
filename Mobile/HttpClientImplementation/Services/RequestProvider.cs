using System;
using System.Threading.Tasks;

namespace HttpClient.Services
{
    public interface IRequestProvider
    {
        /// <summary> Sends delete request from logged in user </summary>
        /// <param name="uri"> The API we are sending our request to </param>
        /// <param name="token"> Used to identify user by server </param>
        /// <returns> The <see cref="Task"/>. </returns>
        Task DeleteAsync(Uri uri, string token = "");

        /// <summary> Sends delete request from logged in user </summary>
        /// <param name="uri"> The API we are sending our request to </param>
        /// <param name="token"> Used to identify user by server </param>
        /// <returns> The <see cref="Task"/>. </returns>
        Task<TResult> GetAsync<TResult>(Uri uri, string token = "");

        Task<TResult> PostAsync<TResult>(Uri uri, TResult data, string token = "", string header = "");

        Task<TResult> PostAsync<TResult>(Uri uri, string data, string clientId, string clientSecret);

        Task<TResult> PutAsync<TResult>(Uri uri, TResult data, string token = "", string header = "");
    }
}