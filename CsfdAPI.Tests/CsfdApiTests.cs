using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

            // Rating should be NN%
            var searchPattern = new Regex("\\d{1,2}%", RegexOptions.IgnoreCase);
            Assert.IsTrue(searchPattern.IsMatch(mov.Rating));

            // Check genres
            var expectedGenres = new List<string> {"Sci-Fi", "Akční", "Dobrodružný", "Thriller"};
            Assert.IsFalse(expectedGenres.Except(mov.Genres).Any());

            // Check year
            Assert.AreEqual("1987", mov.Year);

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
    }
}