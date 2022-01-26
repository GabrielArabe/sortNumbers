using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosscommerce.SortNumber.Core.Cache
{
    internal class CacheModel
    {
        public string Key { get; set; }
        public DateTime? ExpiresIn { get; set; }
        public object Data { get; set; }
    }
}
