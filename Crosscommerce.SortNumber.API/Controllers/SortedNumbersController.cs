using Crosscommerce.SortNumber.API.Model;
using Crosscommerce.SortNumber.API.Models;
using Crosscommerce.SortNumber.API.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using Serilog;
using Serilog.Formatting.Json;
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
        private ILogger _logger;
        public SortedNumbersController(ILogger logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public JsonResult Get()
        {
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
                System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");

                var resultSortedNumbers = quickSort();
                if (resultSortedNumbers.isSuccess)
                {
                    _logger.Information(resultSortedNumbers.Data.Count.ToString());
                    return new JsonResult(resultSortedNumbers.Data.Count);
                }
                _logger.Warning(resultSortedNumbers.Exception.Message);

                return new JsonResult(resultSortedNumbers.Exception);

            }
            catch (Exception ex)
            {
                _logger.Error("Error getting result: ", ex);
                return new JsonResult(ex.Message);
            }
        }


        private Result<List<double>> quickSort()
        {
            try
            {
                var fetcher = new Fetcher(_logger);
                var allNumbers = fetcher.GetAllNumbers().Data;


                sort(0, allNumbers.Count() - 1, allNumbers);

                _logger.Information("Success sorting all numbers");
                return new Result<List<double>>() { Data = allNumbers };
            }
            catch (Exception ex)
            {
                _logger.Error("Error getting quick sort", ex);
                return new Result<List<double>>() { Exception = new Exception("Error getting quick sort", ex) };
            }

        }

        private Result sort(int left, int right, List<double> allNumbers)
        {
            try
            {
                _logger.Information("Started sort");
                double pivot;
                int leftend, rightend;

                leftend = left;
                rightend = right;
                pivot = allNumbers[left];

                while (left < right)
                {
                    while (allNumbers[right] >= pivot && (left < right))
                    {
                        right--;
                    }

                    if (left != right)
                    {
                        allNumbers[left] = allNumbers[right];
                        left++;
                    }

                    while (allNumbers[left] <= pivot && (left < right))
                    {
                        left++;
                    }

                    if (left != right)
                    {
                        allNumbers[right] = allNumbers[left];
                        right--;
                    }
                }

                allNumbers[left] = pivot;
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

                _logger.Information("Finished sort");
                return new Result();
            }
            catch (Exception ex)
            {
                _logger.Error("Error sorting: ", ex);
                return new Result() { Exception = new Exception("Error sorting: ", ex) };
            }

        }

    }
}


public class Fetcher
{

    private int lastPage = 0;
    private static object @lock = new Object();
    private List<double> fullListOfNumbers = new List<double>(1000000);
    private ILogger _logger;
    public Fetcher(ILogger logger)
    {
        _logger = logger;
    }
    public Result<List<double>> GetAllNumbers()
    {
        try
        {
            var threadCount = Environment.ProcessorCount;
            _logger.Information($"Starting {threadCount} threads");
            var workers = new List<Task>();
            for (int i = 0; i < threadCount; i++)
            {
                workers.Add(startWorker());
            }

            Task.WaitAll(workers.ToArray());

            _logger.Information("Success getting all numbers");
            return new Result<List<double>>() { Data = fullListOfNumbers };
        }
        catch (Exception ex)
        {
            _logger.Error("Error getting all numbers", ex);
            return new Result<List<double>>() { Exception = new Exception("Error getting all numbers", ex) };
        }

    }

    private Task startWorker()
    {
        try
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
            _logger.Information("Success starting worker");
            return t;
        }
        catch (Exception ex)
        {
            _logger.Error("Error starting worker", ex);
            throw;
        }
        
    }

    private FetchModel taskCompleted(FetchModel fetchParameters)
    {
        try
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
        catch (Exception ex)
        {
            _logger.Error("Error completing task", ex);
            throw;
        }
        
    }

    private Task startNewTask(FetchModel fetchModel)
    {
        try
        {
            var t = new Task(new Action<object>((p) => getPage((FetchModel)p)), fetchModel, System.Threading.Tasks.TaskCreationOptions.None);
            t.Start();
            return t;
        }
        catch (Exception ex)
        {
            _logger.Error("Error starting new task", ex);
            throw;
        }

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
            _logger.Warning($"Error getting page: {errorMessage}. Attempt number: {attempts}. Next attempt: {attempts+1}");
            attempts++;
            if (attempts >= maxAttempts)
            {
                keepTrying = false;
                _logger.Error($"reached maximum number of attempts: {attempts}. Error: {errorMessage}");
                return new Exception(errorMessage);
            }

            return null;
        }

        while (keepTrying)
        {
            try
            {
                var path = $"http://challenge.dienekes.com.br/api/numbers/?page={fetchtModel.Page}";

                var client = new RestClient(path);
                var request = new RestRequest("", method: Method.Get);

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

                if (response.Data != null)
                {

                    try
                    {
                        fetchtModel.Numbers = response.Data.Numbers;

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





