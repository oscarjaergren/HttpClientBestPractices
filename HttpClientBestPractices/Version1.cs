using System;
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
    public class RequestProviderVersion1 : IRequestProvider
    {
        /// <summary> Json serialization rules </summary>
        private readonly JsonSerializerOptions serializerSettings;



        /// <summary> Makes sure we can access the API with insecure certificate. Handler should only be insecure for debug. </summary>
        /// <returns> The <see cref="HttpClientHandler" />. </returns>
        public static HttpClientHandler GetInsecureHandler()
        {
            var handler = new HttpClientHandler
            {
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

        /// <summary> Send a DELETE request to the specified Uri as an asynchronous operation.</summary>
        /// <param name="uri"> The uri we are sending our delete request. </param>
        /// <param name="token"> The token used by the API to authorize and identify. </param>
        /// <returns> The <see cref="Task" />. </returns>
        public async Task DeleteAsync(Uri uri, CancellationToken cancellationToken, string token = "")
        { 
            using (System.Net.Http.HttpClient httpClient = CreateHttpClient(token))
            {
                await httpClient.DeleteAsync(uri).ConfigureAwait(false);
            }
        }

        /// <summary> Gets data from API in stream form, best for larger files. </summary>
        /// <param name="uri"> The uri we are sending our get request to. </param>
        /// <param name="token"> The token identifying who we are. </param>
        /// <typeparam name="TResult"> Gets the generic results returned back </typeparam>
        /// <returns> The result of the task is returned <see cref="Task" />.</returns>
        /// TODO: Check if more perfomant to remove http client from using statement 
        public async Task<TResult> GetAsyncStream<TResult>(Uri uri, CancellationToken cancellationToken, string token = "")
        {
            HttpResponseMessage response;
            using (var request = CreateRequest(uri))
            {
                using (System.Net.Http.HttpClient httpClient = CreateHttpClient(token))
                {
                    response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                }
            }

            using (var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                return await JsonSerializer.DeserializeAsync<TResult>(contentStream, serializerSettings);
            }
        }

        private static HttpRequestMessage CreateRequest(Uri uri)
        {
            return new HttpRequestMessage(HttpMethod.Get, uri);
        }

  


        /// <summary> The post async posts data to an API when logged in </summary>
        /// <param name="uri"> The uri we are posting to. </param>
        /// <param name="data"> The data we are posting. </param>
        /// <param name="token"> The token identifying who we are. </param>
        /// <param name="header"> The header type we are using to post with HTTP client. </param>
        /// <typeparam name="TResult"> returns the generic results </typeparam>
        /// <returns> The result of the task is returned. <see cref="Task" />. </returns>
        public async Task<TResult> PostAsync<TResult>(Uri uri, TResult data, CancellationToken cancellationToken, string token = "", string header = "")
        {
            HttpResponseMessage response;
            using (System.Net.Http.HttpClient httpClient = CreateHttpClient(token))
            {
                if (!string.IsNullOrEmpty(header))
                {
                    AddHeaderParameter(httpClient, header);
                }

                var content = new StringContent(JsonSerializer.Serialize(data));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await httpClient.PostAsync(uri, content).ConfigureAwait(false);
                content.Dispose();
            }

            await HandleResponse(response).ConfigureAwait(false);
            string serialized = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var result = await Task.Run(() => JsonSerializer.Deserialize<TResult>(serialized, serializerSettings))
                             .ConfigureAwait(false);

            return result;
        }



        /// <summary> This post async posts data to an API when not logged in. </summary>
        /// <param name="uri"> The uri we are posting to. </param>
        /// <param name="data"> The data we are posting. </param>
        /// <param name="clientId"> The client id (who we are, i.e mobile). </param>
        /// <param name="clientSecret"> The client secret. (encryption) </param>
        /// <typeparam name="TResult"> The result if it is success or failure </typeparam>
        /// <returns> The task is returned <see cref="Task" />. </returns>
        public async Task<TResult> PostAsync<TResult>(Uri uri, string data, string clientId, string clientSecret, CancellationToken cancellationToken)
        {
            HttpResponseMessage response;
            using (System.Net.Http.HttpClient httpClient = CreateHttpClient(string.Empty))
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

            var result = await Task.Run(() => JsonSerializer.Deserialize<TResult>(serialized, serializerSettings))
                              .ConfigureAwait(false);

            return result;
        }

        /// <summary> This put async gets data from API according to our parameters we send in data. </summary>
        /// <param name="uri"> The uri we are posting to </param>
        /// <param name="data"> The data is the parameters we send in for filtering our data. </param>
        /// <param name="token"> The token identifying who we are. </param>
        /// <param name="header"> The type of header in our HTTP client. </param>
        /// <typeparam name="TResult"> The result of the task </typeparam>
        /// <returns> The result of the task is returned. <see cref="Task" />. </returns>
        public async Task<TResult> PutAsync<TResult>(Uri uri, TResult data, CancellationToken cancellationToken, string token = "", string header = "")
        {
            HttpResponseMessage response;
            using (System.Net.Http.HttpClient httpClient = CreateHttpClient(token))
            {
                if (!string.IsNullOrEmpty(header))
                {
                    AddHeaderParameter(httpClient, header);
                }

                var content = new StringContent(JsonSerializer.Serialize(data));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await httpClient.PutAsync(uri, content, cancellationToken).ConfigureAwait(false);
                content.Dispose();
            }

            await HandleResponse(response).ConfigureAwait(false);
            string serialized = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var result = await Task.Run(() => JsonSerializer.Deserialize<TResult>(serialized, serializerSettings))
                              .ConfigureAwait(false);

            return result;
        }

        /// <summary> The add basic authentication header. </summary>
        /// <param name="httpClient"> The http client. </param>
        /// <param name="clientId"> The client id. </param>
        /// <param name="clientSecret"> The client secret.</param>
        private void AddBasicAuthenticationHeader(System.Net.Http.HttpClient httpClient, string clientId, string clientSecret)
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

        /// <summary> The add header parameter. </summary>
        /// <param name="httpClient"> The http client. </param>
        /// <param name="parameter"> The parameter. </param>
        private void AddHeaderParameter(System.Net.Http.HttpClient httpClient, string parameter)
        {
            if (httpClient == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(parameter))
            {
                return;
            }

            httpClient.DefaultRequestHeaders.Add(parameter, Guid.NewGuid().ToString());
        }

        /// <summary> The create http client. </summary>
        /// <param name="token"> The token default value is empty. </param>
        /// <returns> The <see cref="HttpClient" /> </returns>
        private System.Net.Http.HttpClient CreateHttpClient(string token = "")
        {
            HttpClientHandler handler = GetInsecureHandler();
            var httpClient = new System.Net.Http.HttpClient(handler);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return httpClient;
        }

        /// <summary>
        ///     Handles the response via HTTP via checking if the Https call was valid.
        ///     If not throws exception.
        /// </summary>
        /// <param name="response"> The response. </param>
        /// <returns> The <see cref="Task" />. </returns>
        /// <exception cref="ServiceAuthenticationException"> Throws a service authentication error if not here. </exception>
        /// <exception cref="HttpRequestException"> Throws a general HttpRequestException unless forbidden or unauthorized. </exception>
        private async Task HandleResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                throw new ServiceAuthenticationException(response.StatusCode, content);
            }
        }

        public Task<TResult> GetAsync<TResult>(Uri uri, CancellationToken cancellationToken, string token = "")
        {
            throw new NotImplementedException();
        }

        public Task<TResult> PostAsync<TResult>(Uri uri, string data, string clientId, CancellationToken cancellationToken, string clientSecret)
        {
            throw new NotImplementedException();
        }
    }
}