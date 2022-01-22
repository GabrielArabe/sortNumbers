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
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");

            var fetcher = new Fetcher();
            var result = fetcher.GetAllNumbers();

            return new JsonResult(result.Count);
        }

        private void writeNumbers(List<string> numbers)
        {
            int i = 0;
            foreach (var n in numbers)
            {
                var x = new JsonResult($"{++i} {n}");
            }
        }

        //private Result<List<string>> getNumbers(int page)
        //{
        //    try
        //    {
        //        var path = "http://challenge.dienekes.com.br/api/";

        //        var client = new RestClient(path);
        //        var request = new RestRequest("numbers", Method.Get).AddParameter("pages", page);

        //        var response = client.ExecuteAsync<PagesModel>(request).Result;

        //        var result = JsonConvert.DeserializeObject<List<string>>(response.Content);

        //        return new Result<List<string>>() { Data = result };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new Result<List<string>>() { Exception = new Exception("Error getting numbers", ex) };
        //    }

        //}

        //private Result<List<string>> getAllNumbers()
        //{
        //    try
        //    {

        //        List<string> allnumbers = new List<string>();
        //        for (int i = 0; i < 5; i++)
        //        {
        //            var x = getNumbers(i).Data;
        //            allnumbers.AddRange(x);
        //        }             


        //        return new Result<List<string>>() { Data = allnumbers };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new Result<List<string>>() { Exception = new Exception("Error getting all numbers", ex) };
        //    }


        //private void quickSort()
        //{           
        //    var allNumbers = getAllNumbers();
        //    sort(0, allNumbers.Data.Count() - 1, allNumbers.Data);
        //    Console.WriteLine(allNumbers);
        //}

        //private void sort(int left, int right, List<string> allNumbers)
        //{
        //    double pivot;
        //    int leftend, rightend;

        //    leftend = left;
        //    rightend = right;
        //    pivot = Convert.ToDouble(allNumbers[left]);

        //    while (left < right)
        //    {
        //        while ((Convert.ToDouble(allNumbers[right]) >= pivot) && (left < right))
        //        {
        //            right--;
        //        }

        //        if (left != right)
        //        {
        //            allNumbers[left] = allNumbers[right];
        //            left++;
        //        }

        //        while ((Convert.ToDouble(allNumbers[left]) <= pivot) && (left < right))
        //        {
        //            left++;
        //        }

        //        if (left != right)
        //        {
        //            allNumbers[right] = allNumbers[left];
        //            right--;
        //        }
        //    }

        //    allNumbers[left] = pivot.ToString();
        //    pivot = left;
        //    left = leftend;
        //    right = rightend;

        //    if (left < pivot)
        //    {
        //        sort(left, Convert.ToInt32(pivot - 1), null);
        //    }

        //    if (right > pivot)
        //    {
        //        sort(Convert.ToInt32(pivot + 1), right, null);
        //    }
        //}

    }
}


public class Fetcher
{

    private int lastPage = 0;
    private static object @lock = new Object();
    private List<string> fullListOfNumbers = new List<string>();

    public List<string> GetAllNumbers()
    {
        var threadCount = Environment.ProcessorCount;
        var workers = new List<Task>();
        for (int i = 0; i < threadCount; i++)
        {
            workers.Add(startWorker());
        }

        Task.WaitAll(workers.ToArray());

        return fullListOfNumbers;
    }

    private Task startWorker()
    {
        var t = new Task(() =>
        {
            FetchModel nextParameter = getNextParameters(true);
            while (nextParameter != null)
            {
                var t = startNewTask(nextParameter);
                t.Wait();

                nextParameter = taskCompleted(nextParameter);
            }
        });

        t.Start();
        return t;
    }

    private FetchModel taskCompleted(FetchModel fetchParameters)
    {
        lock (@lock)
        {
            if (fetchParameters.Numbers?.Count > 0)
                fullListOfNumbers.AddRange(fetchParameters.Numbers);
            else
                return null;

            return getNextParameters(false);
        }
    }

    private Task startNewTask(FetchModel fetchModel)
    {
        var t = new Task(new Action<object>((p) => getPage((FetchModel)p)), fetchModel, System.Threading.Tasks.TaskCreationOptions.None);
        t.Start();
        return t;
    }

    private FetchModel getNextParameters(bool shouldLock)
    {
        FetchModel createFetchModel()
        {
            var fetchParameters = new FetchModel() { Page = ++lastPage };
            return fetchParameters;
        };

        if (shouldLock)
        {
            lock (@lock)
            {
                return createFetchModel();
            }
        }
        else
        {
            return createFetchModel();
        }
    }

    private void getPage(FetchModel fetchtModel)
    {
        var keepTrying = true;
        var attempts = 1;
        var maxAttempts = 10;

        Exception handleError(int maxAttempts, ref int attempts, string errorMessage)
        {
            attempts++;
            if (attempts >= maxAttempts)
            {
                keepTrying = false;
                return new Exception(errorMessage);
            }

            return null;
        }

        while (keepTrying)
        {
            try
            {
                var path = "http://challenge.dienekes.com.br/api/";

                var client = new RestClient(path);
                var request = new RestRequest("numbers", Method.Get).AddParameter("pages", fetchtModel.Page);

                var response = client.ExecuteAsync<PagesModel>(request).Result;
                if (!response.IsSuccessful)
                {
                    var e = handleError(maxAttempts, ref attempts, $"{response.StatusDescription}: {response.Data?.error ?? string.Empty}");
                    if (e != null)
                        throw e;
                    else
                    {
                        Task.Delay(2000);
                        continue;
                    }
                }

                if (response.Content != null)
                {

                    try
                    {

                        var result = JsonConvert.DeserializeObject<List<string>>(response.Content);

                        fetchtModel.Numbers = result;

                        keepTrying = false;
                    }
                    catch (Exception e)
                    {

                        throw;
                    }

                    System.Diagnostics.Debug.WriteLine($"Page {fetchtModel.Page} fetched with {fetchtModel.Numbers.Count} numbers");
                }
                else
                {
                    string message = $"Page {fetchtModel.Page} with null content";
                    System.Diagnostics.Debug.WriteLine(message);
                    throw new Exception(message);
                }

            }
            catch (Exception ex)
            {
                var e = handleError(maxAttempts, ref attempts, ex.Message);
                if (e != null)
                    throw e;
                else
                {
                    Task.Delay(2000);
                    continue;
                }
            }
        }
    }

}

public class FetchModel
{
    public int Page { get; set; }
    public List<string> Numbers { get; set; }
    public bool IsSuccess { get; set; }

}



