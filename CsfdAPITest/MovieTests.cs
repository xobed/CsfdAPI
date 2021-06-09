using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsfdAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CsfdAPITest
{
    [TestClass]
    public class MovieTests
    {
        private readonly CsfdApi _csfdApi = new CsfdApi();

        [TestMethod]
        public async Task GetMovie_GetsMovie()
        {
            var mov = await _csfdApi.GetMovie("https://www.csfd.cz/film/6648-predator/");

            // Check title
            const string expectedTitle = "Predátor";
            Assert.AreEqual(expectedTitle, mov.Title);

            // Rating should be 0-100
            Assert.IsTrue(mov.Rating >= 0 && mov.Rating <= 100);

            // Check genres
            var expectedGenres = new List<string> {"Akční", "Sci-Fi", "Horor"};
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
                "https://www.csfd.cz/film/541410-douglas-fairbanks-a-mary-pickfordova-navstevou-v-csr/");

            // Check empty year
            Assert.AreEqual(string.Empty, mov.Year);

            // Check title is not empty
            Assert.AreNotEqual(string.Empty, mov.Title);
        }

        [TestMethod]
        public async Task GetMovie_GetsMovieWithoutGenres()
        {
            var mov = await _csfdApi.GetMovie("https://www.csfd.cz/film/37558-hedy/");

            // Check title
            const string expectedTitle = "Hedy";
            Assert.AreEqual(expectedTitle, mov.Title);

            Assert.IsTrue(!mov.Genres.Any(), "Expected no genres for this movie");

            Assert.AreEqual("1966", mov.Year);
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
        public async Task GetMovie_GetsSeriesLinks()
        {
            var result = await _csfdApi.GetMovie("https://www.csfd.cz/film/265160-vetrelci-davnoveku/");
            Assert.IsTrue(result.SeriesLinks.Count > 0);
        }
    }
}