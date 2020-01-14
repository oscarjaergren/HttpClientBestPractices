using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpClient.MobileAppService.Controllers
{
    public class ApiResponse
    {
        public ApiResponse(int statusCode, IEnumerable<string> errors)
        {
            StatusCode = statusCode;
            Errors = errors ?? Enumerable.Empty<string>();
        }

        public int StatusCode { get; }
        public IEnumerable<string> Errors { get; }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public ApiResponse(int statusCode, T data) : base(statusCode, Enumerable.Empty<string>())
        {
            Data = data;
        }

        public ApiResponse(int statusCode, T data, IEnumerable<string> errors) : base(statusCode, errors)
        {
            Data = data;
        }

        public T Data { get; }
    }
}