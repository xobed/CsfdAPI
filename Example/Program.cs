using System.Threading.Tasks;
using CsfdAPI;

namespace Example
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var csfdApi = new CsfdApi();
            // Get movie by exact URL
            var movie = await csfdApi.GetMovie("https://www.csfd.cz/film/6648-predator/prehled/");

            // Get movie by search
            var searched = await csfdApi.SearchMovie("Predátor (1987)");

            // Get todays Cinema listings
            var listings = await csfdApi.GetAllCinemaListingsToday();
        }
    }
}