using System.Collections.Generic;
using System.Linq;
using CsfdAPI.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CsfdAPI.Tests
{
    [TestClass]
    public class CsfdApiTests
    {
        private readonly CsfdApi csfdApi = new CsfdApi();

        [TestMethod]
        public void GetMovie_GetsMovie()
        {
            var mov = csfdApi.GetMovie("http://www.csfd.cz/film/6648-predator/");

            // Check title
            const string expectedTitle = "Predátor / Predator";
            Assert.AreEqual(expectedTitle, mov.Title);

            // Rating should be 0-100
            Assert.IsTrue((mov.Rating >= 0) && (mov.Rating <= 100));

            // Check genres
            var expectedGenres = new List<string> {"Sci-Fi", "Akční", "Dobrodružný", "Thriller"};
            Assert.IsFalse(expectedGenres.Except(mov.Genres).Any());

            // Check year
            Assert.AreEqual("1987", mov.Year);

            Assert.IsFalse(string.IsNullOrEmpty(mov.PosterUrl));
        }

        [TestMethod]
        public void GetMovie_GetsMovieWithoutGenres()
        {
            var mov = csfdApi.GetMovie("http://csfd.cz/film/37558-hedy/");

            // Check title
            const string expectedTitle = "Hedy";
            Assert.AreEqual(expectedTitle, mov.Title);

            Assert.IsTrue(!mov.Genres.Any(), "Expected no genres for this movie");

            Assert.AreEqual("1966", mov.Year);
            Assert.IsFalse(string.IsNullOrEmpty(mov.PosterUrl));
        }

        [TestMethod]
        public void GetMovie_NoPlot()
        {
            // This movie has no plot
            var movie = csfdApi.GetMovie("http://www.csfd.cz/film/313887-foto");

            Assert.AreEqual(string.Empty, movie.Plot);
        }

        [TestMethod]
        public void SearchMovie_FindsMovie()
        {
            var result = csfdApi.SearchMovie("12 Years a Slave (2013)");
            Assert.AreEqual(result.Url, "http://www.csfd.cz/film/304544-12-let-v-retezech/");
        }

        // Some CSFD search queries redirect directly to movie URL
        [TestMethod]
        public void SearchMovie_DirectRedirect()
        {
            var result = csfdApi.SearchMovie("Afflicted (2013)");
            Assert.AreEqual(result.Url, "http://www.csfd.cz/film/351411-v-bolestech/");
        }

        private void AssertListingIsCorrect(Cinema cinema)
        {
            Assert.IsFalse(string.IsNullOrEmpty(cinema.CinemaName));
            foreach (var cinemaMovie in cinema.Movies)
            {
                Assert.IsTrue(cinemaMovie.Url.Contains("csfd.cz/film"));
                Assert.IsFalse(string.IsNullOrEmpty(cinemaMovie.MovieName));
                Assert.IsTrue(cinemaMovie.Times.Any());
            }
        }

        [TestMethod]
        public void GetAllCinemaListingsTodayTest()
        {
            var result = csfdApi.GetAllCinemaListingsToday().ToList();
            Assert.IsTrue(result.Count > 0);
            result.ForEach(AssertListingIsCorrect);
        }

        [TestMethod]
        public void GetAllCinemaListingsTomorrowTest()
        {
            var result = csfdApi.GetAllCinemaListingsTomorrow().ToList();
            Assert.IsTrue(result.Count > 0);
            result.ForEach(AssertListingIsCorrect);
        }

        [TestMethod]
        public void GetAllCinemaListingsTest()
        {
            var result = csfdApi.GetAllCinemaListings().ToList();
            Assert.IsTrue(result.Count > 0);
            result.ForEach(AssertListingIsCorrect);
        }

        [TestMethod]
        public void GetCinemaListingByUrlTest()
        {
            var result = csfdApi.GetCinemaListing("http://www.csfd.cz/kino/filtr-1/").ToList();
            Assert.IsTrue(result.Count > 0);
            result.ForEach(AssertListingIsCorrect);
        }
    }
}