using System.Collections.Generic;

namespace Crosscommerce.SortNumber.Contract
{
    public interface IDienekesApiClient
    {
        List<double> GetAllNumbers();
    }
}
