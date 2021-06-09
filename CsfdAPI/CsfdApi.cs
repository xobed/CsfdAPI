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

        public async Task<IEnumerable<Cinema>> GetAllCinemaListings()
        {
            return await _cinemaProgramParser.GetAllCinemaListings();
        }

        // Url example 
        // https://www.csfd.cz/kino/?district=1&period=all
        public async Task<IEnumerable<Cinema>> GetCinemaListing(string url)
        {
            return await _cinemaProgramParser.GetCinemaListing(url);
        }
    }
}