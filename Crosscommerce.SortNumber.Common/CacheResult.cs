using Crosscommerce.SortNumber.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosscommerce.SortNumber.Core.Common
{
    public class CacheResult<T>:Result<T>
    {
        public bool IsCachedResult { get; set; }
    }
}
