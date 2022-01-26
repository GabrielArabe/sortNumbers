using Crosscommerce.SortNumber.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosscommerce.SortNumber.Contract
{
    public interface ICacher
    {
        CacheResult<T> TryGetCached<T>(string key, Func<T> getNewValue, TimeSpan? expiresIn, Func<T> getTempValue = null);
    }
}
