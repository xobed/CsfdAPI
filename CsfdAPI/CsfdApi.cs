using System.Collections.Generic;
using CsfdAPI.Model;

namespace CsfdAPI
{
    public class CsfdApi
    {
        private readonly CinemaProgramParser cinemaProgramParser = new CinemaProgramParser();
        private readonly MovieParser movieParser = new MovieParser();

        public Movie GetMovie(string url)
        {
            return movieParser.GetMovie(url);
        }

        public Movie SearchMovie(string query)
        {
            return movieParser.SearchAndGetMovie(query);
        }

        public IEnumerable<Cinema> GetAllCinemaListings()
        {
            return cinemaProgramParser.GetAllCinemas();
        }

        // Url example 
        // http://www.csfd.cz/kino/?district-filter=55
        public IEnumerable<Cinema> GetCinemaListing(string url)
        {
            return cinemaProgramParser.GetCinemaListing(url);
        }
    }
}