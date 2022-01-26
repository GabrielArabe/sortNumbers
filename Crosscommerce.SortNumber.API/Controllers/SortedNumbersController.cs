using Crosscommerce.SortNumber.API;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using Crosscommerce.SortNumber.Contract;
using Crosscommerce.SortNumber.Common;

namespace Crosscommerce.SortNumber.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SortedNumbersController : ControllerBase
    {
        private ILogger _logger;
        private ISortNumbersBusiness _sortnumbers;
        public SortedNumbersController(ILogger logger, ISortNumbersBusiness sortNumbers)
        {
            _logger = logger;
            _sortnumbers = sortNumbers;
        }

        [HttpGet]
        public ApiResult<List<double>> Get()
        {
            try
            {
                var resultSortedNumbers = _sortnumbers.GetSortedNumbers();

                if (resultSortedNumbers.isSuccess)
                {
                    if (resultSortedNumbers.RequestStatus == ApiResult<List<double>>.ApiRequestStatus.Pending)
                        _logger.Information($"Request in progress...");
                    else if (resultSortedNumbers.RequestStatus == ApiResult<List<double>>.ApiRequestStatus.Completed)
                        _logger.Information($"Amount of numbers: {resultSortedNumbers.Data?.Count ?? 0}");
                }

                HttpContext.Response.StatusCode = resultSortedNumbers.StatusCode;
                return resultSortedNumbers;
            }
            catch (Exception ex)
            {
                _logger.Error("Error getting result: ", ex);
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return new ApiResult<List<double>>() { Exception = ex, RequestStatus = ApiResult<List<double>>.ApiRequestStatus.Error, StatusCode = (int)System.Net.HttpStatusCode.InternalServerError };
            }
        }
    }
}





