using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace HttpClientBestPractices
{
    class Version2
    {
        private static JsonSerializer jsonSerializer;

        //Test this method
        static async Task HttpGetForLargeFileInRightWay()
        {
            using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
            {
                const string url = "https://github.com/tugberkugurlu/ASPNETWebAPISamples/archive/master.zip";
                using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                {
                    string fileToWriteTo = Path.GetTempFileName();
                    using (Stream streamToWriteTo = File.Open(fileToWriteTo, FileMode.Create))
                    {
                        await streamToReadFrom.CopyToAsync(streamToWriteTo);
                    }
                }
            }
        }

        //public async Task<TResult> PostAsync<TResult>(
        //    Uri uri,
        //    string data,
        //    string clientId,
        //    string clientSecret,
        //    CancellationToken cancellationToken)
        //{
        //    HttpResponseMessage response;
        //    using (HttpClient httpClient = CreateHttpClient(string.Empty))
        //    {
        //        if (!string.IsNullOrWhiteSpace(clientId) && !string.IsNullOrWhiteSpace(clientSecret))
        //            AddBasicAuthenticationHeader(httpClient, clientId, clientSecret);

        //        var content = new StringContent(data);
        //        content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        //        response = await httpClient.PostAsync(uri, content, cancellationToken).ConfigureAwait(false);
        //        content.Dispose();
        //    }

        //    await HandleResponse(response).ConfigureAwait(false);
        //    string serialized = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        //    TResult result = await Task.Run(
        //                         () => System.Text.Json.JsonSerializer.Deserialize<TResult>(serialized, serializerSettings),
        //                         cancellationToken).ConfigureAwait(false);

        //    return result;
        //}


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

        public static void SerializeJsonIntoStream(object value, Stream stream)
        {
            using (var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
            using (var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None })
            {
                jsonSerializer.Serialize(jtw, value);
                jtw.Flush();
            }


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
