using Crosscommerce.SortNumber.Common;
using Crosscommerce.SortNumber.Contract;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosscommerce.SortNumber.Core
{
    public class ApiClient : IApiClient
    {
        private ILogger _logger;
        public ApiClient(ILogger logger)
        {
            _logger = logger;
        }
        public Result<T> GetApiResult<T>(string url)
        {

            try
            {
                using var client = new RestClient(url);
                var request = new RestRequest("", method: Method.Get);

                var response = client.ExecuteAsync<T>(request).Result;
                if (!response.IsSuccessful)
                {
                    if (response != null)
                    {
                        _logger.Error($"Get API response failed {response.StatusDescription} {response.StatusCode}");
                        return new Result<T>() { Exception = new Exception($"{response.StatusDescription} ({response.StatusCode.ToString()})"), Data = response.Data };
                    }
                    throw new Exception();
                }

                _logger.Information($"successful getting API result");
                return new Result<T>() { Data = response.Data };
            }
            catch (Exception e)
            {
                _logger.Error($"Error getting api response: {e.Message}");

                return new Result<T>() { Exception = e };
            }
        }


    }
}
