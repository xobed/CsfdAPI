using System.Collections.Generic;
using System.Threading.Tasks;
using CsfdAPI;
using CsfdAPI.Model;

namespace Example
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            CsfdApi csfdApi = new CsfdApi();
            // Get movie by exact URL
            Movie movie = await csfdApi.GetMovie("https://www.csfd.cz/film/6648-predator/prehled/");

            // Get all Cinema listings
            IEnumerable<Cinema> listings = await csfdApi.GetAllCinemaListings();

            // Get Cinema listings in Prague
            IEnumerable<Cinema> listingsPrague = await csfdApi.GetCinemaListing("https://www.csfd.cz/kino/?district=1&period=all");
        }
    }
}