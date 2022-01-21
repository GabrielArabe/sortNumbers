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
            
            var x = GetAllNumbers();
            GetAllNumbers().Data.Count();
            return new JsonResult("");
        }

        public Result<List<string>> GetNumbers(int page)
        {
            try
            {
                var path = "http://challenge.dienekes.com.br/api/";

                var client = new RestClient(path);
                var request = new RestRequest("numbers", Method.Get).AddParameter("pages", page);

                var response = client.ExecuteAsync<PagesModel>(request).Result;

                var result = JsonConvert.DeserializeObject<List<string>>(response.Content);

                return new Result<List<string>>() { Data = result };
            }
            catch (Exception ex)
            {
                return new Result<List<string>>() { Exception = new Exception("Error getting numbers", ex) };
            }
            
        }

        public Result<List<string>> GetAllNumbers()
        {
            try
            {

                List<string> allnumbers = new List<string>();
                for (int i = 0; i < 10; i++)
                {
                    var x = GetNumbers(i).Data;
                    allnumbers.AddRange(x);
                }             


                return new Result<List<string>>() { Data = allnumbers };
            }
            catch (Exception ex)
            {
                return new Result<List<string>>() { Exception = new Exception("Error getting all numbers", ex) };
            }
        }

        public void QuickSort()
        {
            sort(0, GetAllNumbers().Data.Count() - 1);
        }

        public void sort(int left, int right, List<string> allNumbers )
        {
            double pivot;
            int leftend, rightend;

            leftend = (int)left;
            rightend = (int)right;
            pivot = Convert.ToDouble(allNumbers[left]);

            while (left < right)
            {
                while ((Convert.ToDouble(allNumbers[right]) >= pivot) && (left < right))
                {
                    right--;
                }

                if (left != right)
                {
                    allNumbers[left] = allNumbers[right];
                    left++;
                }

                while ((Convert.ToDouble(allNumbers[left]) <= pivot) && (left < right))
                {
                    left++;
                }

                if (left != right)
                {
                    allNumbers[right] = allNumbers[left];
                    right--;
                }
            }

            allNumbers[left] = pivot.ToString() ;
            pivot = left;
            left = leftend;
            right = rightend;

            if (left < pivot)
            {
                sort(left, Convert.ToInt32(pivot - 1), allNumbers);
            }

            if (right > pivot)
            {
                sort(Convert.ToInt32(pivot + 1), right, allNumbers);
            }
        }
    }
}
