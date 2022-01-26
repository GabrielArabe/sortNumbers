using Crosscommerce.SortNumber.Common;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosscommerce.SortNumber.Contract
{
    public interface IApiClient
    {
        Result<T> GetApiResult<T>(string url);
    }
}
