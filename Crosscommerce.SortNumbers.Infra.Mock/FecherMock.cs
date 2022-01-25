using Crosscommerce.SortNumber.API.Interfaces;
using Crosscommerce.SortNumber.API.Models;
using Crosscommerce.SortNumber.API.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosscommerce.SortNumbers.Infra.Mock
{
    public class FecherMock : IFetcher
    {
        //public Result<List<double>> GetAllNumbers()
        //{
        //    var allNumbers = new List<double>();            
        //    while 
        //}

        public void GetPage(FetchModel fetchModel)
        {
            Random random = new Random();
            var c = 0;
            while (c < 100)
            {
                fetchModel.Numbers.Add(random.NextDouble() * (0.001 - 0.00001) + 0.00001);
                c++;
            }
        }
    }
}
