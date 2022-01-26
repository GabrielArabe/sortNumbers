
using Crosscommerce.SortNumber.Common;
using Crosscommerce.SortNumber.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosscommerce.SortNumber.Business
{
    //TODO: implement logging
    //TODO: add Unit Test
    public class SortNumbers : ISortNumbers
    {
        private const string _CACHE_KEY_ALL_NUMBERS = "ALLNUMBERSCACHE";

        private ISorter _sorter;
        private IDienekesApiClient _fetcher;
        ICacher _cacher;
        public SortNumbers(ISorter sorter, IDienekesApiClient fetcher, ICacher cacher)
        {
            _sorter = sorter;
            _fetcher = fetcher;
            _cacher = cacher;
        }

        public ApiResult<List<double>> GetSortedNumbers()
        {
            try
            {
                var cacheResult = _cacher.TryGetCached(
                    key: _CACHE_KEY_ALL_NUMBERS,
                    getNewValue: () =>
                    {
                        var allNumbers = _fetcher.GetAllNumbers();

                        if (allNumbers.Count > 0)
                        {
                            var sortedNumbers = _sorter.QuickSort(allNumbers);

                            return new ApiResult<List<double>>() { Data = sortedNumbers, RequestStatus = ApiResult<List<double>>.ApiRequestStatus.Completed };
                        }

                        return new ApiResult<List<double>>() { Data = new List<double>(), RequestStatus = ApiResult<List<double>>.ApiRequestStatus.Completed };

                    },
                    expiresIn: TimeSpan.FromMinutes(10),
                    getTempValue: () => new ApiResult<List<double>>() { RequestStatus = ApiResult<List<double>>.ApiRequestStatus.Pending, StatusCode = (int)System.Net.HttpStatusCode.AlreadyReported, Message = "The request is being processed. Try again in some minutes." });

                if (!cacheResult.isSuccess)
                {
                    throw cacheResult.Exception;
                }

                return cacheResult.Data;
            }
            catch (Exception ex)
            {
                //TODO: add error log
                return new ApiResult<List<double>>() { StatusCode = (int)System.Net.HttpStatusCode.InternalServerError, RequestStatus = ApiResult<List<double>>.ApiRequestStatus.Error, Exception = ex, Message = "Error getting sorted numbers" };
            }

        }

    }
}
