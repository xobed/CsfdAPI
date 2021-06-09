using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsfdAPI.Model;
using HtmlAgilityPack;

namespace CsfdAPI
{
    internal class CinemaProgramParser
    {
        private readonly HtmlLoader _htmlLoader = new HtmlLoader();

        internal async Task<IEnumerable<Cinema>> GetAllCinemaListingsToday()
        {
            var allCinemaList = new List<Cinema>();

            // CZ
            allCinemaList.AddRange(await GetCinemaListing("http://www.csfd.cz/kino/filtr-1/?district-filter=0"));
            // SK
            allCinemaList.AddRange(await GetCinemaListing("http://www.csfd.cz/kino/filtr-2/?district-filter=0"));

            return allCinemaList;
        }

        // Todo - fix datetime
        internal async Task<IEnumerable<Cinema>> GetAllCinemaListingsTomorrow()
        {
            var allCinemaList = new List<Cinema>();

            // CZ
            allCinemaList.AddRange(await GetCinemaListing("http://www.csfd.cz/kino/filtr-1/?period=tomorrow&district-filter=0"));
            // SK
            allCinemaList.AddRange(await GetCinemaListing("http://www.csfd.cz/kino/filtr-2/?period=tomorrow&district-filter=0"));

            return allCinemaList;
        }
        
        internal async Task<IEnumerable<Cinema>> GetAllCinemaListings()
        {
            var allCinemaList = new List<Cinema>();

            // CZ
            allCinemaList.AddRange(await GetCinemaListing("https://www.csfd.cz/kino/?period=all"));
            // SK
            allCinemaList.AddRange(await GetCinemaListing("https://www.csfd.sk/kino/?period=all"));

            return allCinemaList;
        }

        internal async Task<List<Cinema>> GetCinemaListing(string url)
        {
            var uri = new Uri(url);

            var document = await _htmlLoader.GetDocumentByUrl(url);

            var cinemaElements = GetCinemaElements(document);

            if (cinemaElements == null)
            {
                return new List<Cinema>();
            }

            var cinemaListing = new List<Cinema>();

            foreach (var cinema in cinemaElements)
            {
                var titleNode = cinema.SelectSingleNode(".//h2");
                var title = titleNode.InnerText;

                var divNodes = cinema.SelectNodes("./div");
                var movieList = new List<CinemaMovie>();
                DateTime currentDate = GetDateFromDateNode(divNodes.First());

                foreach (var cinemaLineNode in divNodes)
                {
                    var nodeClasses = cinemaLineNode.GetClasses().ToList();
                    if (nodeClasses.Count == 1 && nodeClasses.First() == "box-sub-header")
                    {
                        currentDate = GetDateFromDateNode(cinemaLineNode);
                    }
                    else
                    {
                        var movieNodes = GetMovieElements(cinemaLineNode);
                        movieList.AddRange(movieNodes.Select(n => GetMovie(n, currentDate, uri.Host)));
                    }
                }

                cinemaListing.Add(new Cinema
                {
                    CinemaName = title,
                    Movies = movieList
                });
            }

            return cinemaListing;
        }

        private DateTime GetDateFromDateNode(HtmlNode dateNode)
        {
            string dateString = Regex.Match(dateNode.InnerHtml, @"\d{1,2}\.\d{1,2}\.\d{4}").Value;
            string format = "dd.MM.yyyy";
            return DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture);
        }

        private HtmlNodeCollection GetCinemaElements(HtmlDocument doc)
        {
            return doc.DocumentNode.SelectNodes("//section[contains(@id,'cinema')]");
        }

        private HtmlNodeCollection GetMovieElements(HtmlNode cinemaNode)
        {
            return cinemaNode.SelectNodes(".//tr[not(contains(@class,'tr-mobile-name'))]");
        }

        private CinemaMovie GetMovie(HtmlNode movieNode, DateTime date, string host="www.csfd.cz")
        {
            var titleNode = movieNode.SelectSingleNode(".//a");

            var title = titleNode.InnerText;
            var url = $"https://{host}{titleNode.GetAttributeValue("href", "")}";

            var timeNodes = movieNode.SelectNodes("./td[not(@class)]");

            var timeList = GetTimeList(timeNodes, date);

            var flags = GetFlags(movieNode);

            return new CinemaMovie
            {
                MovieName = title,
                Times = timeList,
                Url = url,
                Flags = flags
            };
        }

        private List<DateTime> GetTimeList(HtmlNodeCollection timeNodes, DateTime date)
        {
            var timesAsString = from timeNode in timeNodes where !string.IsNullOrEmpty(timeNode.InnerText.Trim()) select timeNode.InnerText.Trim();
            return timesAsString.Select(t => GetDateTimeFromString(date, t)).ToList();
        }

        private DateTime GetDateTimeFromString(DateTime date, string timeString)
        {
            var hoursAndMinutes = timeString.Split(':');
            var hourString = hoursAndMinutes.First();
            var minutesString = hoursAndMinutes.Last();
            var hours = int.Parse(hourString);
            var minutes = int.Parse(minutesString);
            var timeFromString = new DateTime(date.Year, date.Month, date.Day, hours, minutes, 0);
            return timeFromString;
        }

        private List<string> GetFlags(HtmlNode movieNode)
        {
            var flagNodes = movieNode.SelectNodes(".//td[@class='flags']/span");
            return flagNodes == null ? new List<string>() : flagNodes.Where(n => !string.IsNullOrEmpty(n.InnerText.Trim())).Select(n => n.InnerText.Trim()).ToList();
        }
    }
}