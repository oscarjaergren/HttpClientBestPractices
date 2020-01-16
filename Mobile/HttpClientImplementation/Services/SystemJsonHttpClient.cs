using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace HttpClient.Services
{
    public class SystemJsonHttpClient : IRequestProvider
    {
        public static void SerializeJsonIntoStream(object value, Stream stream)
        {
            using (var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
            using (var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None })
            {
                var js = new JsonSerializer();
                js.Serialize(jtw, value);
                jtw.Flush();
            }
        }

        public Task DeleteAsync(Uri uri, string token = "")
        {
            throw new NotImplementedException();
        }

        public Task<TResult> GetAsync<TResult>(Uri uri, string token = "")
        {
            throw new NotImplementedException();
        }

        public Task<TResult> PostAsync<TResult>(Uri uri, TResult data, string token = "", string header = "")
        {
            throw new NotImplementedException();
        }

        public Task<TResult> PostAsync<TResult>(Uri uri, string data, string clientId, string clientSecret)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> PutAsync<TResult>(Uri uri, TResult data, string token = "", string header = "")
        {
            throw new NotImplementedException();
        }

        private static HttpContent CreateHttpContent(object content)
        {
            HttpContent httpContent = null;

            if (content != null)
            {
                var ms = new MemoryStream();
                SerializeJsonIntoStream(content, ms);
                ms.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(ms);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            return httpContent;
        }

        // Question 1: Using is not recommend, check perfomance difference of using static or not
        private static async Task PostStreamAsync(object content, CancellationToken cancellationToken, string Uri)
        {
            using (var client = new System.Net.Http.HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, Uri))
            using (HttpContent httpContent = CreateHttpContent(content))
            {
                request.Content = httpContent;

                using (HttpResponseMessage response = await client.SendAsync(
                                                          request,
                                                          HttpCompletionOption.ResponseHeadersRead,
                                                          cancellationToken).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                }
            }
        }
    }
}