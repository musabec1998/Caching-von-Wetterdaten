using System;

namespace Caching_Wetterdaten.Models
{
    public class WetterInfo
    {
        public string Beschreibung { get; set; }

        public string Region { get; set; }
        public DateTime WetterDatum { get; set; }
        public string Dateiname { get; set; }

        public DateTime DownloadDatum { get; set; }
    }
}