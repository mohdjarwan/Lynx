using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lynx.Infrastructure.Response
{
    public class APIResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public IList<T>? ErrorMessages { get; set; }
        public object? Result { get; set; }
        public void SetResponseInfo(HttpStatusCode statusCode, IList<T> errorMessages, object result, bool isSuccess = true)
        {
            StatusCode = statusCode;
            IsSuccess = isSuccess;
            ErrorMessages = errorMessages;
            Result = result;
        }


    }
}
