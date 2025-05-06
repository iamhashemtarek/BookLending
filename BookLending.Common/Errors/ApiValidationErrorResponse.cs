using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Common.Errors
{
    public class ApiValidationErrorResponse : ApiErrorResponse
    {
        public IEnumerable<string> Errors { get; set; }
        public ApiValidationErrorResponse() : base(400)
        {
            Errors = new List<string>();
        }
    }
}
