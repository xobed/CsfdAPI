using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsfdAPI.Model;
using HtmlAgilityPack;

namespace CsfdAPI
{
    internal class MovieParser
    {
        private readonly HtmlLoader _htmlLoader = new HtmlLoader();

        /// <summary>
        ///     Loads movie from Csfd.cz URL and returns movie object
        /// </summary>
        /// <param name="url">URL of movie at CSFD.cz</param>
        /// <returns>MovieParser class</returns>
        internal async Task<Movie> GetMovie(string url)
        {
            var document = await _htmlLoader.GetDocumentByUrl(url);

            var titles = GetTitles(document);
            var year = GetYear(document);
            var genres = GetGenres(document);
            var rating = GetRating(document);
            var plot = GetPlot(document);
            var posterUrl = GetPosterUrl(document);
            var seriesLinks = GetSeriesLinks(document);

            return new Movie
            {
                Url = url,
                Title = titles,
                Year = year,
                Genres = genres,
                Rating = rating,
                Plot = plot,
                PosterUrl = posterUrl,
                SeriesLinks = seriesLinks
            };
        }

        /// <summary>
        ///     Search for movie by query and return first result as movie object
        /// </summary>
        /// <param name="query">Query to search for</param>
        /// <returns>MovieParser object</returns>
        internal async Task<Movie> SearchAndGetMovie(string query)
        {
            return await GetMovie(await SearchMovie(query));
        }

        /// <summary>
        ///     Performs search on CSFD cz for given query
        /// </summary>
        /// <param name="query">Query to search for</param>
        /// <returns>URL of first result</returns>
        private async Task<string> SearchMovie(string query)
        {
            var document = await _htmlLoader.GetDocumentByUrl("http://www.csfd.cz/hledat/?q=" + query);

            var node = document.DocumentNode.SelectSingleNode("//*[@id='search-films']/div[1]/ul[1]/li[1]/div/h3/a");

            // Some CSFD searches redirect directly to MovieURL, get current url from 'comments' link
            if (node == null)
            {
                node = document.DocumentNode.SelectSingleNode("//*[@id='main']/div[4]/div[1]/ul/li[1]/a");
                if (node == null) throw new Exception($"Failed to find movie with query '{query}'");
            }

            var movieUrl = node.Attributes["href"].Value;

            return "http://www.csfd.cz" + movieUrl;
        }

        /// <summary>
        ///     Returns CZ title / International title
        /// </summary>
        /// >
        /// <param name="doc">HtmlDocument loaded from csfd.cz movie</param>
        /// <returns>String in format: "Czech title / International title"</returns>
        private string GetTitles(HtmlDocument doc)
        {
            var node = doc.DocumentNode.SelectNodes("//title");
            var titlesAndYear = node[0].InnerText.Trim();
            // Remove "| CSFD.cz"
            titlesAndYear = titlesAndYear.Substring(0, titlesAndYear.LastIndexOf("|", StringComparison.Ordinal)).Trim();

            // Check if year is present and remove it from title
            var indexOfLeftBrace = titlesAndYear.LastIndexOf("(", StringComparison.Ordinal);
            return indexOfLeftBrace > 0 ? titlesAndYear.Substring(0, indexOfLeftBrace).Trim() : titlesAndYear;
        }

        /// <summary>
        ///     Returns list of genres of the movie
        /// </summary>
        /// >
        /// <param name="doc">HtmlDocument loaded from csfd.cz movie</param>
        /// <returns>List of movie genres</returns>
        private List<string> GetGenres(HtmlDocument doc)
        {
            var node = doc.DocumentNode.SelectNodes("//*[@id='profile']//p[@class='genre']");
            if (node == null) return new List<string>();
            var genres = node[0].InnerText.Trim();
            return genres.Split('/').Select(g => g.Trim()).ToList();
        }

        /// <summary>
        ///     Returns Year the movie was released
        /// </summary>
        /// <param name="doc">HtmlDocument loaded from csfd.cz movie</param>
        /// <returns>String year</returns>
        private string GetYear(HtmlDocument doc)
        {
            var node = doc.DocumentNode.SelectNodes("//title");
            var titlesAndYear = node[0].InnerText.Trim();
            var searchYear = new Regex(@"\(\d{4}\)");
            var result = searchYear.Match(titlesAndYear);
            var year = result.Value.Replace("(", string.Empty);
            year = year.Replace(")", string.Empty);
            return year;
        }

        /// <summary>
        ///     Get movie rating %
        /// </summary>
        /// <param name="document">HtmlDocument</param>
        /// <returns>Rating</returns>
        private int GetRating(HtmlDocument document)
        {
            int rating;

            var node = document.DocumentNode.SelectSingleNode("//h2[@class='average']");

            try
            {
                rating = int.Parse(Regex.Match(node.InnerText, @"\d*").Value);
            }
            catch
            {
                rating = -1;
            }

            return rating;
        }

        /// <summary>
        ///     Get movie plot
        /// </summary>
        /// <param name="doc">HtmlDocument</param>
        /// <returns>Plot</returns>
        private string GetPlot(HtmlDocument doc)
        {
            var node = doc.DocumentNode.SelectNodes("//div[@id='plots']//ul/li");

            if (node?[0] != null) return node[0].InnerText.Trim();
            return string.Empty;
        }

        /// <summary>
        ///     Get url of movie poster
        /// </summary>
        /// <param name="doc">HtmlDocument</param>
        /// <returns>Url of movie poster</returns>
        private string GetPosterUrl(HtmlDocument doc)
        {
            var node = doc.DocumentNode.SelectNodes("//img[@class='film-poster']");
            if (node == null)
                // Not all movies have posters
                return null;
            var posterUrl = node[0].OuterHtml;
            posterUrl = posterUrl.Split('"')[1];
            posterUrl = "http:" + posterUrl;
            return posterUrl;
        }

        /// <summary>
        ///     Get url of movie poster
        /// </summary>
        /// <param name="doc">HtmlDocument</param>
        /// <returns>Url of movie poster</returns>
        private List<string> GetSeriesLinks(HtmlDocument doc)
        {
            var node = doc.DocumentNode.SelectNodes("//div[@id='children']//div[@class='content']//a");
            if (node == null)
            {
                return new List<string>();
            }
            return node.Select(n => $"https://www.csfd.cz/{n.Attributes["href"].Value}").ToList();
        }
    }
}