using Crosscommerce.SortNumber.Common;
using Crosscommerce.SortNumber.Contract;
using Crosscommerce.SortNumber.External;
using Crosscommerce.SortNumber.External.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosscommerce.SortNumbers.Tests
{
    //TODO: Create more tests
    public class DienekesApiClientTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void DienekesApi_SuccessfulCase()
        {
            var mock = new Mock<IApiClient>();

            mock.Setup(api => api.GetApiResult<PagesModel>(It.IsAny<string>())).Returns<string>(
                (url) =>
                {
                    return new Result<PagesModel>()
                    {
                        Data = new PagesModel()
                        {
                            Numbers = getPageFromUrl(url) > 100 ? null : new List<double>() { 1, 2, 3 }
                        }
                    };
                });

            var client = new DienekesApiClient(mock.Object, null);
            var result = client.GetAllNumbers();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data.Count == 300);

        }

        private int getPageFromUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url) || url.IndexOf("=") < 0)
                return -1;

            return System.Convert.ToInt32(url.Substring(url.LastIndexOf("=") + 1));
        }



    }
}
