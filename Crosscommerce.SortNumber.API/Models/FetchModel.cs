
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crosscommerce.SortNumber.API.Models
{
    public class FetchModel
    {
        public int Page { get; set; }
        public List<double> Numbers { get; set; }

    }
}
