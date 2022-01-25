﻿using Crosscommerce.SortNumber.API;
using Crosscommerce.SortNumber.Contract;
using Crosscommerce.SortNumber.External.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crosscommerce.SortNumber.External
{
    public class Fetcher : IFetcher
    {

        private int lastPage = 0;
        private static object @lock = new Object();
        private List<double> fullListOfNumbers = new List<double>(1000000);

        public List<double> GetAllNumbers()
        {
            try
            {
                var threadCount = Environment.ProcessorCount;
                //_logger.Information($"Starting {threadCount} threads");
                var workers = new List<Task>();
                for (int i = 0; i < threadCount; i++)
                {
                    workers.Add(startWorker());
                }

                Task.WaitAll(workers.ToArray());

                //_logger.Information("Success getting all numbers");
                return  fullListOfNumbers ;
            }
            catch (Exception ex)
            {
                //_logger.Error("Error getting all numbers", ex);
                throw new Exception("Error getting all numbers", ex) ;
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
                //_logger.Information("Success starting worker");
                return t;
            }
            catch (Exception ex)
            {
                //_logger.Error("Error starting worker", ex);
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
                //_logger.Error("Error completing task", ex);
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
                //_logger.Error("Error starting new task", ex);
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

        private void getPage(FetchModel fetchModel)
        {
            var keepTrying = true;
            var attempts = 1;
            var maxAttempts = 10;

            Exception handleError(int maxAttempts, ref int attempts, string errorMessage)
            {
                //_logger.Warning($"Error getting page: {errorMessage}. Attempt number: {attempts}. Next attempt: {attempts + 1}");
                attempts++;
                if (attempts >= maxAttempts)
                {
                    keepTrying = false;
                    //_logger.Error($"reached maximum number of attempts: {attempts}. Error: {errorMessage}");
                    return new Exception(errorMessage);
                }

                return null;
            }

            while (keepTrying)
            {
                try
                {
                    var path = $"http://challenge.dienekes.com.br/api/numbers/?page={fetchModel.Page}";

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
                            fetchModel.Numbers = response.Data.Numbers;

                            keepTrying = false;
                        }
                        catch (Exception)
                        {
                            throw;
                        }

                        System.Diagnostics.Debug.WriteLine($"Page {fetchModel.Page} fetched with {fetchModel.Numbers.Count} numbers");
                    }
                    else
                    {
                        string message = $"Page {fetchModel.Page} with null content";
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
}
