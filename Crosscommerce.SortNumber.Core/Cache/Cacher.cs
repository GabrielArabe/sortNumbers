using Crosscommerce.SortNumber.Contract;
using Crosscommerce.SortNumber.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosscommerce.SortNumber.Core.Cache
{
    public class Cacher : ICacher
    {

        private List<CacheModel> cacheList { get; set; } = new List<CacheModel>();

        public CacheResult<T> TryGetCached<T>(string key, Func<T> getNewValue, TimeSpan? expiresIn, Func<T> getTempValue = null)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(key))
                    throw new ArgumentNullException(nameof(key));

                var result = new CacheResult<T>() { IsCachedResult = true };
                var currentTime = DateTime.UtcNow;

                var foundItem = (from c in cacheList
                                 where string.Equals(c.Key, key, StringComparison.InvariantCultureIgnoreCase) && (!c.ExpiresIn.HasValue || c.ExpiresIn.Value >= currentTime)
                                 orderby c.ExpiresIn.HasValue ? c.ExpiresIn.Value : currentTime descending
                                 select c).FirstOrDefault();

                if (foundItem == null)
                {
                    T newItem = default(T);

                    if (getTempValue != null)
                    {
                        newItem = getTempValue();

                        foundItem = new CacheModel()
                        {
                            Data = newItem,
                            Key = key,
                            ExpiresIn = null
                        };

                        cacheList.Add(foundItem);
                    }

                    newItem = getNewValue();

                    if (foundItem == null)
                    {
                        foundItem = new CacheModel()
                        {
                            Data = newItem,
                            Key = key,
                            ExpiresIn = expiresIn.HasValue ? (DateTime.UtcNow.AddMilliseconds(expiresIn.Value.TotalMilliseconds)) : null
                        };
                        cacheList.Add(foundItem);
                    }
                    else
                    {
                        foundItem.Data = newItem;
                        foundItem.ExpiresIn = expiresIn.HasValue ? (DateTime.UtcNow.AddMilliseconds(expiresIn.Value.TotalMilliseconds)) : null;
                    }

                    result.IsCachedResult = false;
                }

                result.Data = (T)foundItem.Data;

                return result;
            }
            catch (Exception e)
            {
                return new CacheResult<T>() { Exception = e };
            }
        }

    }
}
