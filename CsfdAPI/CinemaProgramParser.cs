using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using CsfdAPI.Model;
using HtmlAgilityPack;

namespace CsfdAPI
{
    internal class CinemaProgramParser
    {
        private readonly HtmlLoader htmlLoader = new HtmlLoader();

        internal IEnumerable<Cinema> GetAllCinemaListingsToday()
        {
            var allCinemaList = new List<Cinema>();

            // CZ
            allCinemaList.AddRange(GetCinemaListing("http://www.csfd.cz/kino/filtr-1/?district-filter=0"));
            // SK
            allCinemaList.AddRange(GetCinemaListing("http://www.csfd.cz/kino/filtr-2/?district-filter=0"));

            return allCinemaList;
        }

        // Todo - fix datetime
        internal IEnumerable<Cinema> GetAllCinemaListingsTomorrow()
        {
            var allCinemaList = new List<Cinema>();

            // CZ
            allCinemaList.AddRange(GetCinemaListing("http://www.csfd.cz/kino/filtr-1/?period=tomorrow&district-filter=0"));
            // SK
            allCinemaList.AddRange(GetCinemaListing("http://www.csfd.cz/kino/filtr-2/?period=tomorrow&district-filter=0"));

            return allCinemaList;
        }
        
        internal IEnumerable<Cinema> GetAllCinemaListings()
        {
            var allCinemaList = new List<Cinema>();

            // CZ
            allCinemaList.AddRange(GetCinemaListing("http://www.csfd.cz/kino/filtr-1/?period=all&district-filter=0"));
            // SK
            allCinemaList.AddRange(GetCinemaListing("http://www.csfd.cz/kino/filtr-2/?period=all&district-filter=0"));

            return allCinemaList;
        }

        internal List<Cinema> GetCinemaListing(string url)
        {
            var document = htmlLoader.GetDocumentByUrl(url);

            var cinemaElements = GetCinemaElements(document);

            if (cinemaElements == null)
            {
                return new List<Cinema>();
            }

            var cinemaListing = new List<Cinema>();

            foreach (var cinema in cinemaElements)
            {
                var titleNode = cinema.SelectSingleNode("./div/h2");
                var title = titleNode.InnerText;

                var dateTableElements = cinema.SelectNodes("./div[@class='content']/table");
                var movieList = new List<CinemaMovie>();
                foreach (var tableElement in dateTableElements)
                {
                    var dateNode = tableElement.SelectSingleNode("./caption");
                    var date = GetDateFromDateNode(dateNode);

                    var movieNodes = GetMovieElements(tableElement);
                    movieList.AddRange(movieNodes.Select(n => GetMovie(n, date)));
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
            string format = "d.M.yyyy";
            return DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture);
        }

        private HtmlNodeCollection GetCinemaElements(HtmlDocument doc)
        {
            var result = doc.DocumentNode.SelectNodes("//*[contains(@class,'cinema')]");
            return result;
        }

        private HtmlNodeCollection GetMovieElements(HtmlNode cinemaNode)
        {
            var result = cinemaNode.SelectNodes("./tr");
            return result;
        }

        private CinemaMovie GetMovie(HtmlNode movieNode, DateTime date)
        {
            var titleNode = movieNode.SelectSingleNode("./th/a");

            var title = titleNode.InnerText;
            var url = $"http://csfd.cz{titleNode.GetAttributeValue("href", "")}";
            var yearNode = movieNode.SelectSingleNode("./th/span");
            var year = yearNode?.InnerText;

            var timeNodes = movieNode.SelectNodes("./td[not(@class)]");

            var timeList = GetTimeList(timeNodes, date);

            var flags = GetFlags(movieNode);

            return new CinemaMovie
            {
                MovieName = $"{title} {year}",
                Times = timeList,
                Url = url,
                Flags = flags
            };
        }

        private List<DateTime> GetTimeList(HtmlNodeCollection timeNodes, DateTime date)
        {
            var timesAsString = from timeNode in timeNodes where !string.IsNullOrEmpty(timeNode.InnerText) select timeNode.InnerText;
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
            var flagNodes = movieNode.SelectNodes("./td[@class='flags']/span");

            var flagList = new List<string>();
            if (flagNodes != null)
            {
                foreach (var flagNode in flagNodes)
                {
                    var flag = string.Empty;

                    switch (flagNode.InnerText)
                    {
                        case "D":
                            flag = "Dabing";
                            break;
                        case "T":
                            flag = "Titulky";
                            break;
                        case "3D":
                            flag = "3D";
                            break;
                    }
                    flagList.Add(flag);
                }
            }

            return flagList;
        }
    }
}