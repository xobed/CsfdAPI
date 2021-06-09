using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsfdAPI;
using CsfdAPI.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CsfdAPITest
{
    [TestClass]
    public class KinoTests
    {
        private readonly CsfdApi _csfdApi = new CsfdApi();
        private void AssertListingIsCorrect(Cinema cinema)
        {
            Assert.IsFalse(string.IsNullOrEmpty(cinema.CinemaName));
            foreach (var cinemaMovie in cinema.Movies)
            {
                Assert.IsNotNull(cinemaMovie.Url);
                Assert.IsFalse(string.IsNullOrEmpty(cinemaMovie.MovieName));
                Assert.IsTrue(cinemaMovie.Times.Any());
            }
        }

        [TestMethod]
        public async Task GetAllCinemaListingsTest()
        {
            var result = (await _csfdApi.GetAllCinemaListings()).ToList();
            Assert.IsTrue(result.Count > 0);
            result.ForEach(AssertListingIsCorrect);
        }

        [TestMethod]
        public async Task GetCinemaListingByUrlTest()
        {
            var result = (await _csfdApi.GetCinemaListing("https://www.csfd.cz/kino/?district=1&period=all")).ToList();
            Assert.IsTrue(result.Count > 0);
            result.ForEach(AssertListingIsCorrect);
        }
    }
}