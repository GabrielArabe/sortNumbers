using Crosscommerce.SortNumber.Common;
using Crosscommerce.SortNumber.Contract;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosscommerce.SortNumber.Core
{
    //TODO: Implement logging
    public class ApiClient : IApiClient
    {

        public Result<T> GetApiResult<T>(string url)
        {

            try
            {
                using var client = new RestClient(url);
                var request = new RestRequest("", method: Method.Get);

                var response = client.ExecuteAsync<T>(request).Result;
                if (!response.IsSuccessful)
                {
                    return new Result<T>() { Exception = new Exception($"{response.StatusDescription} ({response.StatusCode.ToString()})"), Data = response.Data };
                }

                return new Result<T>() { Data = response.Data };
            }
            catch (Exception e)
            {
                return new Result<T>() { Exception = e };
            }
        }


    }
}
