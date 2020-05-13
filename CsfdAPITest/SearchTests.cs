using System.Collections.Generic;
using System.Threading.Tasks;
using CsfdAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CsfdAPITest
{
    [TestClass]
    public class SearchTests
    {
        private readonly CsfdApi _csfdApi = new CsfdApi();

        [TestMethod]
        public async Task SearchMovie_FindsMovie()
        {
            var result = await _csfdApi.SearchMovie("12 Years a Slave (2013)");
            Assert.AreEqual(result.Url, "http://www.csfd.cz/film/304544-12-let-v-retezech/");
        }

        [TestMethod]
        public async Task SearchMovies_FindsMovies()
        {
            var result = await _csfdApi.SearchMovies("Predator");

            var expected = new List<string>
            {
                "http://www.csfd.cz/film/6648-predator/",
                "http://www.csfd.cz/film/377500-predator-evoluce/",
                "http://www.csfd.cz/film/10452-predator-2/"
            };

            foreach (var url in expected) CollectionAssert.Contains(result, url);
        }
    }
}