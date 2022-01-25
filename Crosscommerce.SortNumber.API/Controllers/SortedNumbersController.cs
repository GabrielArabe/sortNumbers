using Crosscommerce.SortNumber.API;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using Crosscommerce.SortNumber.Contract;

namespace Crosscommerce.SortNumber.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SortedNumbersController : ControllerBase
    {
        private ILogger _logger;
        private ISortNumbers _sortnumbers;
        public SortedNumbersController(ILogger logger, ISortNumbers sortNumbers)
        {
            _logger = logger;
            _sortnumbers = sortNumbers;
        }

        [HttpGet]
        public JsonResult Get()
        {
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
                System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");

                var resultSortedNumbers = _sortnumbers.GetSortedNumbers();

                _logger.Information($"Amount of numbers: {resultSortedNumbers.Count.ToString()}");
                return new JsonResult(resultSortedNumbers.Count);

            }
            catch (Exception ex)
            {
                _logger.Error("Error getting result: ", ex);
                return new JsonResult(ex.Message);
            }
        }
    }
}





