using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsfdAPI.Model;

namespace CsfdAPI
{
    public class CsfdApi
    {
        private readonly CinemaProgramParser _cinemaProgramParser = new CinemaProgramParser();
        private readonly MovieParser _movieParser = new MovieParser();
        private readonly SearchParser _searchParser= new SearchParser();

        public async Task<Movie> GetMovie(string url)
        {
            return await _movieParser.GetMovie(url);
        }

        public async Task<Movie> SearchMovie(string query)
        {
            var result = (await _searchParser.SearchMovies(query)).First();
            return await _movieParser.GetMovie(result);
        }
        public async Task<List<string>> SearchMovies(string query)
        {
            return await _searchParser.SearchMovies(query);
        }

        public async Task<IEnumerable<Cinema>> GetAllCinemaListingsToday()
        {
            return await _cinemaProgramParser.GetAllCinemaListingsToday();
        }

        public async Task<IEnumerable<Cinema>> GetAllCinemaListingsTomorrow()
        {
            return await _cinemaProgramParser.GetAllCinemaListingsTomorrow();
        }

        public async Task<IEnumerable<Cinema>> GetAllCinemaListings()
        {
            return await _cinemaProgramParser.GetAllCinemaListings();
        }

        // Url example 
        // http://www.csfd.cz/kino/?district-filter=55
        public async Task<IEnumerable<Cinema>> GetCinemaListing(string url)
        {
            return await _cinemaProgramParser.GetCinemaListing(url);
        }
    }
}