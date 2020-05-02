using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace CsfdAPI
{
    public class HtmlLoader
    {
        private readonly HttpClient _client;

        public HtmlLoader()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            _client = new HttpClient(handler);
        }

        /// <summary>
        ///     Get HTMLDocument by URL
        /// </summary>
        /// <param name="url">URL to load HTML from</param>
        /// <returns>HtmlDocument instance</returns>
        public async Task<HtmlDocument> GetDocumentByUrl(string url)
        {
            var responseString = await _client.GetStringAsync(url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(responseString);
            return htmlDocument;
        }
    }
}