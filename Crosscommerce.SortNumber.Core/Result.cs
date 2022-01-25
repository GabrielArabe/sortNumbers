using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crosscommerce.SortNumber.API
{
    public class Result
    {        
        public Exception Exception { get; set; }
        public bool isSuccess { get => Exception == null; }
    }

    public class Result<T> : Result
    {
        public T Data { get; set; }
    }
}
