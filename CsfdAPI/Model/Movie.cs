﻿using System.Collections.Generic;

namespace CsfdAPI.Model
{
    public class Movie
    {
        public List<string> Genres { get; set; }
        public string Plot { get; set; }
        public string PosterUrl { get; set; }
        public int Rating { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }
        public List<string> SeriesLinks { get; set; }
    }
}