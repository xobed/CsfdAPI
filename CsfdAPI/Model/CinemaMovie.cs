using System;
using System.Collections.Generic;

namespace CsfdAPI.Model
{
    public class CinemaMovie
    {
        public string MovieName { get; set; }
        public string Url { get; set; }
        public IEnumerable<DateTime> Times { get; set; }
        public IEnumerable<string> Flags { get; set; }
    }
}