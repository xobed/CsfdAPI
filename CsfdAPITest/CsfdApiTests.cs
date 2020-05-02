using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsfdAPI;
using CsfdAPI.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CsfdAPITest
{
    [TestClass]
    public class CsfdApiTests
    {
        private readonly CsfdApi _csfdApi = new CsfdApi();

        [TestMethod]
        public async Task GetMovie_GetsMovie()
        {
            var mov = await _csfdApi.GetMovie("http://www.csfd.cz/film/6648-predator/");

            // Check title
            const string expectedTitle = "Predátor / Predator";
            Assert.AreEqual(expectedTitle, mov.Title);

            // Rating should be 0-100
            Assert.IsTrue(mov.Rating >= 0 && mov.Rating <= 100);

            // Check genres
            var expectedGenres = new List<string> {"Sci-Fi", "Thriller", "Horor"};
            foreach (var genre in expectedGenres)
                Assert.IsTrue(expectedGenres.Contains(genre),
                    $"Movie genres do not contain expected genre '{genre}'. Actual genres: '{string.Join(",", mov.Genres)}'");

            // Check year
            Assert.AreEqual("1987", mov.Year);

            Assert.IsFalse(string.IsNullOrEmpty(mov.PosterUrl), $"Poster URL was empty or null: '{mov.PosterUrl}'");
        }

        [TestMethod]
        public async Task GetMovie_WithoutYear()
        {
            var mov = await _csfdApi.GetMovie(
                "http://csfd.cz/film/541410-douglas-fairbanks-a-mary-pickfordova-navstevou-v-csr/");

            // Check empty year
            Assert.AreEqual(string.Empty, mov.Year);

            // Check title is not empty
            Assert.AreNotEqual(string.Empty, mov.Title);
        }

        [TestMethod]
        public async Task GetMovie_GetsMovieWithoutGenres()
        {
            var mov = await _csfdApi.GetMovie("http://csfd.cz/film/37558-hedy/");

            // Check title
            const string expectedTitle = "Hedy";
            Assert.AreEqual(expectedTitle, mov.Title);

            Assert.IsTrue(!mov.Genres.Any(), "Expected no genres for this movie");

            Assert.AreEqual("1966", mov.Year);
            Assert.IsFalse(string.IsNullOrEmpty(mov.PosterUrl));
        }

        [TestMethod]
        public async Task GetMovie_NoPlot()
        {
            // This movie has no plot
            var movie = await _csfdApi.GetMovie("http://www.csfd.cz/film/313887-foto");

            Assert.AreEqual(string.Empty, movie.Plot);
        }

        [TestMethod]
        public async Task GetSeriesPart()
        {
            var movie = await _csfdApi.GetMovie(
                "http://www.csfd.cz/film/71550-30-pripadu-majora-zemana/452532-studna/prehled/");
            Assert.AreEqual(movie.PosterUrl, null);
        }

        [TestMethod]
        public async Task SearchMovie_FindsMovie()
        {
            var result = await _csfdApi.SearchMovie("12 Years a Slave (2013)");
            Assert.AreEqual(result.Url, "http://www.csfd.cz/film/304544-12-let-v-retezech/");
        }     
        
        [TestMethod]
        public async Task GetMovie_GetsSeriesLinks()
        {
            var result = await _csfdApi.GetMovie("https://www.csfd.cz/film/265160-vetrelci-davnoveku/");
            Assert.IsTrue(result.SeriesLinks.Count > 0);
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
        public async Task GetAllCinemaListingsTodayTest()
        {
            var result = (await _csfdApi.GetAllCinemaListingsToday()).ToList();
            Assert.IsTrue(result.Count > 0);
            result.ForEach(AssertListingIsCorrect);
        }

        [TestMethod]
        public async Task GetAllCinemaListingsTomorrowTest()
        {
            var result = (await _csfdApi.GetAllCinemaListingsTomorrow()).ToList();
            Assert.IsTrue(result.Count > 0);
            result.ForEach(AssertListingIsCorrect);
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
            var result = (await _csfdApi.GetCinemaListing("http://www.csfd.cz/kino/filtr-1/")).ToList();
            Assert.IsTrue(result.Count > 0);
            result.ForEach(AssertListingIsCorrect);
        }
    }
}