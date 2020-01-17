using System;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientBestPractices
{
    public interface IRequestProvider
    {
        /// <summary> Sends DELETE request from logged in user </summary>
        /// <param name="uri"> The API we are sending our DELETE request to </param>
        /// <param name="cancellationToken"> Used to cancel the job </param>
        /// <param name="token"> Used to identify user by server </param>
        /// <returns> The <see cref="Task"/>. </returns>
        Task DeleteAsync(Uri uri, CancellationToken cancellationToken, string token = "");

        /// <summary> Sends GET request from authenticated user </summary>
        /// <param name="uri"> The API we are sending our GET request to </param>
        /// <param name="cancellationToken"> Used to cancel the job </param>
        /// <param name="token"> Used to identify user by server </param>
        /// <typeparam name="TResult"> returns the generic results </typeparam>
        /// <returns> The <see cref="Task"/>. </returns>
        Task<TResult> GetAsync<TResult>(Uri uri, CancellationToken cancellationToken, string token = "");

        /// <summary> Send POST request to API from authenticated user </summary>
        /// <param name="uri"> The API we are posting to. </param>
        /// <param name="data"> The data we are posting. </param>
        /// <param name="cancellationToken"> Used to cancel the job </param>
        /// <param name="token"> Used to identify user by server </param>
        /// <param name="header"> The header type we are using to post with HTTP client. </param>
        /// <typeparam name="TResult"> returns the generic results </typeparam>
        /// <returns> The result of the task is returned. <see cref="Task" />. </returns>
        Task<TResult> PostAsync<TResult>(Uri uri, TResult data, CancellationToken cancellationToken, string token = "", string header = "");


        /// <summary> Send POST request to API from unauthenticated user </summary>
        /// <param name="uri"> The uri we are posting to. </param>
        /// <param name="data"> The data we are posting. </param>
        /// <param name="clientId"> The client id (who we are, i.e mobile). </param>
        /// <param name="clientSecret"> The client secret. (encryption) </param>
        /// <param name="cancellationToken"> Used to cancel the job </param>
        /// <typeparam name="TResult"> The result if it is success or failure </typeparam>
        /// <returns> The task is returned <see cref="Task" />. </returns>
        Task<TResult> PostAsync<TResult>(Uri uri, string data, string clientId, string clientSecret, CancellationToken cancellationToken);
    }
}