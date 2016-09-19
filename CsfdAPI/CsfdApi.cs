using CsfdAPI.Model;

namespace CsfdAPI
{
    public class CsfdApi
    {
        private readonly MovieParser movieParser = new MovieParser();

        public Movie GetMovie(string url)
        {
            return movieParser.GetMovie(url);
        }

        public Movie SearchMovie(string query)
        {
            return movieParser.SearchAndGetMovie(query);
        }
    }
}