using Crosscommerce.SortNumber.Common;
using System.Collections.Generic;

namespace Crosscommerce.SortNumber.Contract
{
    public interface IDienekesApiClient
    {
        Result<List<double>> GetAllNumbers();
    }
}
