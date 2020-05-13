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

            // Get single movie by search
            Movie foundMovie = await csfdApi.SearchMovie("Predátor (1987)");
            
            // Get urls returned by search
            List<string> foundUrls = await csfdApi.SearchMovies("Predátor");

            // Get todays Cinema listings
            IEnumerable<Cinema> listings = await csfdApi.GetAllCinemaListingsToday();
        }
    }
}