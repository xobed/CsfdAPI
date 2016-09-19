using System.Collections.Generic;

namespace CsfdAPI.Model
{
    public class Cinema
    {
        public string CinemaName { get; set; }
        public IEnumerable<CinemaMovie> Movies { get; set; }
    }
}