using Crosscommerce.SortNumber.Core;
using NUnit.Framework;
using System.Collections.Generic;
using System;

namespace Crosscommerce.SortNumbers.Tests
{
    public class SorterTest
    {
        private Sorter _sorter;

        [SetUp]
        public void Setup()
        {
            _sorter = new Sorter();
        }

        [Test]
        public void QuickSort_NullList()
        {
            System.ArgumentNullException exception = null;
            try
            {
               _sorter.QuickSort(null);
        
            }
            catch (System.ArgumentNullException ex)
            {
                exception = ex;
            }
            Assert.IsNotNull(exception);
            Assert.IsTrue(exception.ParamName == "allNumbers");     
        }
        [Test]
        public void QuickSort_EmptyList()
        {
            var result = _sorter.QuickSort(new System.Collections.Generic.List<double>());

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 0);
        }
        [Test]
        public void QuickSort_IntegerNumbers()
        {
            List<double> input = new List<double>()
            {
                1, 8, 12, 4, 3, 10, 100, 11, 40, 11
            };
            List<double> expectedOutput = new List<double>()
            {
                1, 3, 4, 8, 10, 11, 11, 12, 40, 100
            };

            var output = _sorter.QuickSort(input);
            Assert.IsNotNull(output);
            Assert.IsTrue(output.Count == 10);

            for (int i = 0; i < output.Count; i++)
            {
                Assert.AreEqual(expectedOutput[i], output[i]);
            }
        }
        [Test]
        public void QuickSort_DoubleNumbers()
        {
            List<double> input = new List<double>()
            {
                0.1, 0.8, 0.12, 0.4, 0.3, 0.10, 0.100, 0.0011, 0.04510, 0.0011
            };
            List<double> expectedOutput = new List<double>()
            {
               0.0011, 0.0011, 0.04510, 0.1, 0.10, 0.100, 0.12, 0.3, 0.4 ,0.8
            };

            var output = _sorter.QuickSort(input);
            Assert.IsNotNull(output);
            Assert.IsTrue(output.Count == 10);

            for (int i = 0; i < output.Count; i++)
            {
                Assert.AreEqual(expectedOutput[i], output[i]);
            }
        }

        [Test]
        public void QuickSort_NegativeDoubleNumbers()
        {
            List<double> input = new List<double>()
            {
                -0.1, -0.8, -0.12, -0.4, -0.3, -0.10, -0.100, -0.0011, -0.04510, -0.0011
            };
            List<double> expectedOutput = new List<double>()
            {
               -0.8, -0.4, -0.3, -0.12, -0.100, -0.10, -0.1, -0.04510, -0.0011, -0.0011
            };

            var output = _sorter.QuickSort(input);
            Assert.IsNotNull(output);
            Assert.IsTrue(output.Count == 10);

            for (int i = 0; i < output.Count; i++)
            {
                Assert.AreEqual(expectedOutput[i], output[i]);
            }
        }
    }
}