using System;
using System.IO;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace CsfdAPI
{
    public class HtmlLoader
    {
        /// <summary>
        ///     Get HTMLDocument by URL
        /// </summary>
        /// <param name="url">URL to load HTML from</param>
        /// <returns>HtmlDocument instance</returns>
        public HtmlDocument GetDocumentByUrl(string url)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            var response = request.GetResponse();
            using (var stream = response.GetResponseStream())
            {
                if (stream != null)
                {
                    var reader = new StreamReader(stream, Encoding.UTF8);
                    var responseString = reader.ReadToEnd();
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(responseString);
                    return htmlDocument;
                }
            }

            throw new Exception($"Failed to load document with url {url}");
        }
    }
}