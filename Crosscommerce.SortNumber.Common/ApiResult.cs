using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosscommerce.SortNumber.Common
{
    public class ApiResult<T> : Result<T>
    {
        public ApiRequestStatus RequestStatus { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; } = 200;

        public enum ApiRequestStatus
        {
            Pending,
            Completed,
            Error
        }
    }
}
