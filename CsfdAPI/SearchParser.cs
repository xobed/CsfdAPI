using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CsfdAPI
{
    internal class SearchParser
    {
        private readonly HtmlLoader _htmlLoader = new HtmlLoader();
        private const string BaseUrl = "http://www.csfd.cz";

        private string ToFullUrl(string url)
        {
            return $"{BaseUrl}{url}";
        }

        /// <summary>
        ///     Performs search on CSFD cz for given query
        /// </summary>
        /// <param name="query">Query to search for</param>
        /// <returns>List of links in 'Filmy'</returns>
        internal async Task<List<string>> SearchMovies(string query)
        {
            var document = await _htmlLoader.GetDocumentByUrl("http://www.csfd.cz/hledat/?q=" + query);

            var nodes = document.DocumentNode.SelectNodes("//div[@id='search-films']//li/a");

            // Some CSFD searches redirect directly to MovieURL, get current url from 'comments' link
            if (nodes == null)
            {
                var node = document.DocumentNode.SelectSingleNode("//*[@id='main']/div[4]/div[1]/ul/li[1]/a");
                if (node == null) throw new Exception($"Failed to find movie with query '{query}'");
                return new List<string> {ToFullUrl(node.Attributes["href"].Value)};
            }

            return nodes.Select(n => ToFullUrl(n.Attributes["href"].Value)).ToList();
        }
    }
}