﻿using Crosscommerce.SortNumber.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosscommerce.SortNumber.Contract
{
    public interface ISortNumbers
    {
        ApiResult<List<double>> GetSortedNumbers();

    }
}
