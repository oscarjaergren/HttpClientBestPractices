using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientBestPractices
{
    /// <summary>
    ///     The request provider holds our Get/Put request to our APIs in generic format.
    ///     Implement this class in your services and use this to make requests to the API.
    /// </summary>
    public class RequestProvider : IRequestProvider
    {
        /// <summary> Json serialization rules </summary>
        private static JsonSerializerOptions serializerSettings;

        /// <summary> Initializes a new instance of the <see cref="RequestProvider" /> class. </summary>
        public RequestProvider()
        {
            serializerSettings = new JsonSerializerOptions { WriteIndented = true };
        }

        /// <summary> Send a DELETE request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="uri"> The uri we are sending our delete request. </param>
        /// <param name="cancellationToken"> Used to cancel the job </param>
        /// <param name="token"> The token used by the API to authorize and identify. </param>
        /// <returns> The <see cref="Task" />. </returns>
        public static async Task DeleteRequest(Uri uri, CancellationToken cancellationToken, string token = "")
        {
            HttpClient httpClient = CreateHttpClient(token);
            await httpClient.DeleteAsync(uri, cancellationToken).ConfigureAwait(false);
        }

        /// <summary> Gets data from API in stream form authenticated users </summary>
        /// <param name="uri"> The uri we are sending our get request to. </param>
        /// <param name="cancellationToken"> Used to cancel the job </param>
        /// <param name="token"> The token identifying who we are. </param>
        /// <typeparam name="TResult"> Gets the generic results returned back </typeparam>
        /// <returns> The result of the task is returned <see cref="Task" />.</returns>
        public static async Task<TResult> GetAsyncStream<TResult>(
            Uri uri,
            CancellationToken cancellationToken,
            string token = "")
        {
            HttpResponseMessage response;
            using (HttpRequestMessage request = CreateRequest(uri))
            {
                using (HttpClient httpClient = CreateHttpClient(token))
                {
                    response = await httpClient.SendAsync(
                                   request,
                                   HttpCompletionOption.ResponseHeadersRead,
                                   cancellationToken).ConfigureAwait(false);
                }
            }

            await HandleResponse(response).ConfigureAwait(false);

            // Prints the data for us in debug mode
#if DEBUG
            using (Stream contentStream = await response.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(contentStream))
            {
                string text = reader.ReadToEnd();
                Debug.WriteLine("RECEIVED: " + text);
                return await JsonSerializer.DeserializeAsync<TResult>(
                           contentStream,
                           serializerSettings,
                           cancellationToken);
            }

#else
            using (Stream contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                return await JsonSerializer.DeserializeAsync<TResult>(
                           contentStream,
                           serializerSettings,
                           cancellationToken);
            }
#endif
        }

        /// <summary> Enables us to connect to sites with localhost as certificate, enables GZIP decompression </summary>
        /// <returns> The <see cref="HttpClientHandler" />. </returns>
        public static HttpClientHandler GetHttpHandler()
        {
            var handler = new HttpClientHandler
                              {
                                  AutomaticDecompression = DecompressionMethods.GZip,
                                  ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                                      {
                                          if (cert.Issuer.Equals("CN=localhost", StringComparison.Ordinal))
                                          {
                                              return true;
                                          }

                                          return errors == SslPolicyErrors.None;
                                      }
                              };
            return handler;
        }

        /// <summary> Returns the static method </summary>
        public Task DeleteAsync(Uri uri, CancellationToken cancellationToken, string token = "")
        {
            return DeleteRequest(uri, cancellationToken, token);
        }

        /// <summary> Returns the static method </summary>
        public Task<TResult> GetAsync<TResult>(Uri uri, CancellationToken cancellationToken, string token = "")
        {
            return GetAsyncStream<TResult>(uri, cancellationToken, token);
        }

        /// <summary> The post async posts data to an API when logged in </summary>
        /// <param name="uri"> The uri we are posting to. </param>
        /// <param name="data"> The data we are posting. </param>
        /// <param name="cancellationToken"> Used to cancel the job </param>
        /// <param name="token"> The token identifying who we are. </param>
        /// <param name="header"> The header type we are using to post with HTTP client. </param>
        /// <typeparam name="TResult"> returns the generic results </typeparam>
        /// <returns> The result of the task is returned. <see cref="Task" />. </returns>
        public Task<TResult> PostAsync<TResult>(
            Uri uri,
            TResult data,
            CancellationToken cancellationToken,
            string token = "",
            string header = "")
        {
            throw new NotImplementedException();
        }

        /// <summary> Send POST request to API from unauthenticated user </summary>
        /// <param name="uri"> The uri we are posting to. </param>
        /// <param name="data"> The data we are posting. </param>
        /// <param name="clientId"> The client id (who we are, i.e mobile). </param>
        /// <param name="clientSecret"> The client secret. (encryption) </param>
        /// <param name="cancellationToken"> Used to cancel the job </param>
        /// <typeparam name="TResult"> The result if it is success or failure </typeparam>
        /// <returns> The task is returned <see cref="Task" />. </returns>
        public async Task<TResult> PostAsync<TResult>(
            Uri uri,
            string data,
            string clientId,
            string clientSecret,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response;
            using (HttpClient httpClient = CreateHttpClient(string.Empty))
            {
                if (!string.IsNullOrWhiteSpace(clientId) && !string.IsNullOrWhiteSpace(clientSecret))
                {
                    AddBasicAuthenticationHeader(httpClient, clientId, clientSecret);
                }

                var content = new StringContent(data);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                response = await httpClient.PostAsync(uri, content, cancellationToken).ConfigureAwait(false);
                content.Dispose();
            }

            await HandleResponse(response).ConfigureAwait(false);
            string serialized = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var result = await Task.Run(() => JsonSerializer.Deserialize<TResult>(serialized, serializerSettings), cancellationToken)
                             .ConfigureAwait(false);

            return result;
        }

        /// <summary> The create http client. </summary>
        /// <param name="token"> The token default value is empty. </param>
        /// <returns> The <see cref="HttpClient" /> </returns>
        private static HttpClient CreateHttpClient(string token = "")
        {
            HttpClientHandler handler = GetHttpHandler();
            var httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            }

            return httpClient;
        }

        private static HttpRequestMessage CreateRequest(Uri uri)
        {
            return new HttpRequestMessage(HttpMethod.Get, uri);
        }

        /// <summary>
        ///     Handles the response via HTTP via checking if the Https call was valid.
        ///     If not throws exception.
        /// </summary>
        /// <param name="response"> The response. </param>
        /// <returns> The <see cref="Task" />. </returns>
        /// <exception cref="ServiceAuthenticationException"> Throws a service authentication error if not here. </exception>
        /// <exception cref="HttpRequestException"> Throws a general HttpRequestException unless forbidden or unauthorized. </exception>
        private static async Task HandleResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                throw new ServiceAuthenticationException(response.StatusCode, content);
            }
        }

        /// <summary> The add basic authentication header. </summary>
        /// <param name="httpClient"> The http client. </param>
        /// <param name="clientId"> The client id. </param>
        /// <param name="clientSecret"> The client secret.</param>
        private void AddBasicAuthenticationHeader(HttpClient httpClient, string clientId, string clientSecret)
        {
            if (httpClient == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
            {
                return;
            }

            httpClient.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(clientId, clientSecret);
        }
    }
}