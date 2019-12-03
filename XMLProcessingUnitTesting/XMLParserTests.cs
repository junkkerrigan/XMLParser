using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XMLProcessing.UnitTesting
{
    [TestClass]
    public class XMLParserTests
    {
        [TestClass]
        public class FilteringTests
        {
            Info disk = new Info()
            {
                Title = "Crying Lightning",
                Artist = "Arctic Monkeys",
                Country = "Great Britain",
                Company = "Domino Recording Co",
                Year = "2009",
                Price = "9.99",
            };

            [TestMethod]
            public void FilterIsMatch_Title_True()
            {
                Filter filter = new Filter();
                filter.SetFilter("Title", "Cryin");

                bool expected = true;
                bool actual = filter.IsMatch(disk);
                Assert.AreEqual(expected, actual);
            }

            [TestMethod]
            public void FilterIsMatch_TitleWithFollowingSpacesAndPrice_True()
            {
                Filter filter = new Filter();
                filter.SetFilter("Title", "    lightn ");
                filter.SetFilter("PriceTo", "10.00");

                bool expected = true;
                bool actual = filter.IsMatch(disk);
                Assert.AreEqual(expected, actual);
            }

            [TestMethod]
            public void FilterIsMatch_TitleAndYear_False()
            {
                Filter filter = new Filter();
                filter.SetFilter("Title", "505");
                filter.SetFilter("YearFrom", "2010");

                bool expected = false;
                bool actual = filter.IsMatch(disk);
                Assert.AreEqual(expected, actual);
            }
        }
        
    }
}
