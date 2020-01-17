using System;
using System.Collections.Generic;
using System.Text;

namespace HttpClientBestPractices
{

    public class ApiException : Exception
    {
        public string Content { get; set; }

        public int StatusCode { get; set; }
    }
}
