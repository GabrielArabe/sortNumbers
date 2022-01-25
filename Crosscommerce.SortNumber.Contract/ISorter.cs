using System;
using System.Collections.Generic;

namespace Crosscommerce.SortNumber.Contract
{
    public interface ISorter
    {
        List<double> QuickSort(List<double> allNumbers);     
    }
}
