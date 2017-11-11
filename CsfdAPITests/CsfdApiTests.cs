using System;
using System.Collections.Generic;
using System.Linq;
using CsfdAPI;
using CsfdAPI.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CsfdAPITests
{
    [TestClass]
    public class CsfdApiTests
    {
        private readonly CsfdApi _csfdApi = new CsfdApi();

        [TestMethod]
        public void GetMovie_GetsMovie()
        {
            var mov = _csfdApi.GetMovie("http://www.csfd.cz/film/6648-predator/");

            // Check title
            const string expectedTitle = "Predátor / Predator";
            Assert.AreEqual(expectedTitle, mov.Title);

            // Rating should be 0-100
            Assert.IsTrue((mov.Rating >= 0) && (mov.Rating <= 100));

            // Check genres
            var expectedGenres = new List<string> { "Sci-Fi", "Akèní", "Dobrodružný", "Thriller" };
            if (expectedGenres.Except(mov.Genres).Any())
            {
                Assert.Fail($"Incorrect genres, expected {string.Join(",",expectedGenres)}. {Environment.NewLine}"+
                            $"Actual {string.Join(",", mov.Genres)}");
            }

            // Check year
            Assert.AreEqual("1987", mov.Year);

            Assert.IsFalse(string.IsNullOrEmpty(mov.PosterUrl), $"Poster URL was empty or null: '{mov.PosterUrl}'");
        }

        [TestMethod]
        public void GetMovie_WithoutYear()
        {
            var mov = _csfdApi.GetMovie("http://csfd.cz/film/541410-douglas-fairbanks-a-mary-pickfordova-navstevou-v-csr/");

            // Check empty year
            Assert.AreEqual(String.Empty, mov.Year);

            // Check title is not empty
            Assert.AreNotEqual(String.Empty, mov.Title);
        }

        [TestMethod]
        public void GetMovie_GetsMovieWithoutGenres()
        {
            var mov = _csfdApi.GetMovie("http://csfd.cz/film/37558-hedy/");

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
            var movie = _csfdApi.GetMovie("http://www.csfd.cz/film/313887-foto");

            Assert.AreEqual(string.Empty, movie.Plot);
        }

        [TestMethod]
        public void GetSeriesPart()
        {
            var movie = _csfdApi.GetMovie("http://www.csfd.cz/film/71550-30-pripadu-majora-zemana/452532-studna/prehled/");
            Assert.AreEqual(movie.PosterUrl, null);
        }

        [TestMethod]
        public void SearchMovie_FindsMovie()
        {
            var result = _csfdApi.SearchMovie("12 Years a Slave (2013)");
            Assert.AreEqual(result.Url, "http://www.csfd.cz/film/304544-12-let-v-retezech/");
        }

        // Some CSFD search queries redirect directly to movie URL
        [TestMethod]
        public void SearchMovie_DirectRedirect()
        {
            var result = _csfdApi.SearchMovie("Afflicted (2013)");
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
            var result = _csfdApi.GetAllCinemaListingsToday().ToList();
            Assert.IsTrue(result.Count > 0);
            result.ForEach(AssertListingIsCorrect);
        }

        [TestMethod]
        public void GetAllCinemaListingsTomorrowTest()
        {
            var result = _csfdApi.GetAllCinemaListingsTomorrow().ToList();
            Assert.IsTrue(result.Count > 0);
            result.ForEach(AssertListingIsCorrect);
        }

        [TestMethod]
        public void GetAllCinemaListingsTest()
        {
            var result = _csfdApi.GetAllCinemaListings().ToList();
            Assert.IsTrue(result.Count > 0);
            result.ForEach(AssertListingIsCorrect);
        }

        [TestMethod]
        public void GetCinemaListingByUrlTest()
        {
            var result = _csfdApi.GetCinemaListing("http://www.csfd.cz/kino/filtr-1/").ToList();
            Assert.IsTrue(result.Count > 0);
            result.ForEach(AssertListingIsCorrect);
        }
    }
}