using Crosscommerce.SortNumber.Common;
using Crosscommerce.SortNumber.Contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Crosscommerce.SortNumber.Core
{
    public class Sorter : ISorter
    {
        private Result sort(int left, int right, List<double> allNumbers)
        {
            try
            {
                //_logger.Information("Started sort");
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

                //_logger.Information("Finished sort");
                return new Result();
            }
            catch (Exception ex)
            {
                //_logger.Error("Error sorting: ", ex);
                return new Result() { Exception = new Exception("Error sorting: ", ex) };
            }
        }
        public List<double> QuickSort(List<double> allNumbers)
        {
            if (allNumbers == null)
                throw new ArgumentNullException(nameof(allNumbers));
            try
            {
                //var fetcher = new Fetcher(_logger);
                //var allNumbers = fetcher.GetAllNumbers().Data;

                sort(0, allNumbers.Count() - 1, allNumbers);

                //_logger.Information("Success sorting all numbers");
                return  allNumbers ;
            }
            catch (Exception ex)
            {
                //_logger.Error("Error getting quick sort", ex);
                throw new Exception("Error getting quick sort", ex) ;
            }

        }
    }
}
