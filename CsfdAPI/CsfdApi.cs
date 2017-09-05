using System.Collections.Generic;
using CsfdAPI.Model;

namespace CsfdAPI
{
    public class CsfdApi
    {
        private readonly CinemaProgramParser _cinemaProgramParser = new CinemaProgramParser();
        private readonly MovieParser _movieParser = new MovieParser();

        public Movie GetMovie(string url)
        {
            return _movieParser.GetMovie(url);
        }

        public Movie SearchMovie(string query)
        {
            return _movieParser.SearchAndGetMovie(query);
        }

        public IEnumerable<Cinema> GetAllCinemaListingsToday()
        {
            return _cinemaProgramParser.GetAllCinemaListingsToday();
        }

        public IEnumerable<Cinema> GetAllCinemaListingsTomorrow()
        {
            return _cinemaProgramParser.GetAllCinemaListingsTomorrow();
        }

        public IEnumerable<Cinema> GetAllCinemaListings()
        {
            return _cinemaProgramParser.GetAllCinemaListings();
        }

        // Url example 
        // http://www.csfd.cz/kino/?district-filter=55
        public IEnumerable<Cinema> GetCinemaListing(string url)
        {
            return _cinemaProgramParser.GetCinemaListing(url);
        }
    }
}