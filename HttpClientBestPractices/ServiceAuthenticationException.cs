using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace HttpClientBestPractices
{
    /// <summary> Custom exception for API related failures. </summary>
    public class ServiceAuthenticationException : Exception
    {
        public ServiceAuthenticationException(HttpStatusCode statusCode, string content)
        {
            StatusCode = statusCode;
            Content = content;
        }

        public ServiceAuthenticationException()
        {
        }

        public ServiceAuthenticationException(string message)
            : base(message)
        {
        }

        public ServiceAuthenticationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        ///     Gets or sets the content.
        ///     Correspond to whatever we get returned from API
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///     Gets or sets the status code.
        ///     Correspond to Http Status code
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
    }
}