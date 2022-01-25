
using Crosscommerce.SortNumber.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosscommerce.SortNumber.Business
{
    public class SortNumbers : ISortNumbers
    {
        private ISorter _sorter;
        private IFetcher _fetcher;
        public SortNumbers(ISorter sorter, IFetcher fetcher)
        {
            _sorter = sorter;
            _fetcher = fetcher;
        }

        public List<double> GetSortedNumbers()
        {
            try
            {
                var allNumbers = _fetcher.GetAllNumbers();

                if (allNumbers.Count > 0)
                {
                    var sortedNumbers = _sorter.QuickSort(allNumbers);

                    return sortedNumbers;
                }
                return new List<double>();
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting sorted numbers: ", ex);
            }

        }

    }
}
