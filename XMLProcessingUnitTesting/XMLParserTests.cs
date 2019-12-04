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
        
        [TestClass]
        public class SettingFiltersTests
        {
            Filter filter;

            [TestMethod] 
            public void ParameterDefault_PriceTo_PlusInf()
            {
                filter = new Filter();

                float expected = float.MaxValue;
                float actual = filter.PriceTo;

                Assert.AreEqual(expected, actual);
            }

            [TestMethod]
            public void ParameterDefault_Title_Empty()
            {
                filter = new Filter();

                string expected = string.Empty;
                string actual = filter.Title;

                Assert.AreEqual(expected, actual);
            }

            [TestMethod] 
            public void ParameterIsSet_TextToPriceFrom_PlusInf()
            {
                filter = new Filter();
                filter.SetFilter("PriceFrom", "someText");

                float expected = float.MaxValue;
                float actual = filter.PriceFrom;

                Assert.AreEqual(expected, actual);
            }

            [TestMethod]
            public void ParameterIsSet_EmptyAfterInvalidToYearFrom_MinusInf()
            {
                filter = new Filter();
                filter.SetFilter("YearFrom", "someText");
                filter.SetFilter("YearFrom", "");

                float expected = float.MinValue;
                float actual = filter.PriceFrom;

                Assert.AreEqual(expected, actual);
            }
        }

        [TestClass]
        public class ParsingWithLINQTests
        {
            Filter filter;
            LINQParser parser = new LINQParser("../../../XMLProcessing/data.xml");

            [TestMethod]
            public void SizeOfMatch_EmptyFilter_25()
            {
                filter = new Filter();

                int expected = 25;
                int actual = parser.FilterBy(filter).Titles.Length; // assuming that every title is unique

                Assert.AreEqual(expected, actual);
            }

            [TestMethod]
            public void SizeOfMatch_InvalidFilter_0()
            {
                filter = new Filter();
                filter.SetFilter("Title", "blues");
                filter.SetFilter("PriceTo", "30");
                filter.SetFilter("YearFrom", "werew");

                int expected = 0;
                int actual = parser.FilterBy(filter).Titles.Length;

                Assert.AreEqual(expected, actual);
            }

            [TestMethod]
            public void SuggestionList_Country_Norway()
            {
                filter = new Filter();
                filter.SetFilter("Country", "норвегия");

                int expected = 1;
                int actual = parser.FilterBy(filter).Countries.Length;

                Assert.AreEqual(expected, actual);
            }
        }

        [TestClass]
        public class ParsingWithDOMTests
        {
            Filter filter;
            DOMParser parser = new DOMParser("../../../XMLProcessing/data.xml");

            [TestMethod]
            public void SizeOfMatch_TitleBlues_1()
            {
                filter = new Filter();
                filter.SetFilter("Title", " blues  ");

                int expected = 1;
                int actual = parser.FilterBy(filter).Titles.Length;

                Assert.AreEqual(expected, actual);
            }

            [TestMethod]
            public void SizeOfMatch_PriceFrom10_5()
            {
                filter = new Filter();
                filter.SetFilter("PriceFrom", "10");

                int expected = 5;
                int actual = parser.FilterBy(filter).Titles.Length;

                Assert.AreEqual(expected, actual);
            }

            [TestMethod]
            public void SizeOfMatch_CompanyPolydor_3()
            {
                filter = new Filter();
                filter.SetFilter("Company", "Полидор");

                int expected = 3;
                int actual = parser.FilterBy(filter).Titles.Length;

                Assert.AreEqual(expected, actual);
            }
        }

        [TestClass]
        public class ParsingWithSAXTests
        {
            Filter filter;
            SAXParser parser = new SAXParser("../../../XMLProcessing/data.xml");

            [TestMethod]
            public void SizeOfMatch_ArtistPavarotti_1()
            {
                filter = new Filter();
                filter.SetFilter("Artist", " паваротти");

                int expected = 1;
                int actual = parser.FilterBy(filter).Titles.Length;

                Assert.AreEqual(expected, actual);
            }

            [TestMethod]
            public void SizeOfMatch_YearFrom1990_12()
            {
                // here I realized that my lab doesn't works as it should
                filter = new Filter();
                filter.SetFilter("YearFrom", "1990");

                int expected = 12;
                int actual = parser.FilterBy(filter).Titles.Length;

                Assert.AreEqual(expected, actual);
            }
        }
    }
}
