using System.Collections.Generic;

namespace Crosscommerce.SortNumber.Contract
{
    public interface IFetcher
    {
        List<double> GetAllNumbers();
    }
}
