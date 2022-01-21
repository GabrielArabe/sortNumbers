using Crosscommerce.SortNumber.API.Model;
using Crosscommerce.SortNumber.API.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crosscommerce.SortNumber.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SortedNumbersController : ControllerBase
    {
        [HttpGet]
        public JsonResult Get()
        {
            var x = GetNumbers(2);
            return new JsonResult("");
        }

        public Result<PagesModel> GetNumbers(int page)
        {
            try
            {
                var path = "http://challenge.dienekes.com.br/api/";

                var client = new RestClient(path);
                var request = new RestRequest("numbers", Method.Get).AddParameter("pages", page);

                var response = client.ExecuteAsync<PagesModel>(request).Result;

                var result = JsonConvert.DeserializeObject<PagesModel>(response.Content);

                return new Result<PagesModel>() { Data = result };
            }
            catch (Exception ex)
            {
                return new Result<PagesModel>() { Exception = new Exception("Error getting numbers", ex) };
            }
            
        }

        public Result<List<PagesModel>> GetAllNumbers(PagesModel numbers)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
