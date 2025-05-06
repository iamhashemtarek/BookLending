using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Common.Errors
{
    public class ApiExceptionErrorResponse : ApiErrorResponse
    {
        public string? Details { get; set; }
        public ApiExceptionErrorResponse(int statusCode, string? message = null, string? details = null)
            : base(statusCode, message)
        {
            Details = details;
        }
    }
}
