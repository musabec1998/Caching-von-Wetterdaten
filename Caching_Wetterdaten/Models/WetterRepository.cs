using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Caching_Wetterdaten.Models
{
    public class WetterRepository
    {
        public List<WetterInfo> WetterInfos { get; set; }

        public WetterRepository()
        {
            WetterInfos = new List<WetterInfo>();
        }

        public void AlleDateienRunterladen()
        {
            const string url = "http://opendata.dwd.de/weather/text_forecasts/txt/";

            HttpClient client = new HttpClient();

            string htmlSeite = client.GetStringAsync(url).Result;


            const string regex = "(?!.*pdf)href=\"(.*VHDL.*)\"";

            MatchCollection matches = Regex.Matches(htmlSeite, regex);

            for (int i = 0; i < matches.Count; i++)
            {
                try
                {
                    string downloadName = matches[i].Groups[1].Value;

                    WetterInfo wetterInfo = WetterInfos.AsParallel().SingleOrDefault(w => w.Dateiname == downloadName);

                    if (wetterInfo == null)
                    {
                        EinzelneDateienRunterladen(url, downloadName);
                    }
                }
                catch
                {
                }
            }
            //);

            WetterInfos = WetterInfos.OrderByDescending(w => w.WetterDatum).ThenBy(w => w.Region).ToList();
        }

        private void EinzelneDateienRunterladen(string url, string downloadName)
        {
            WebClient webClient = new WebClient();

            webClient.Encoding = Encoding.UTF7;

            WetterInfo wetterInfo = new WetterInfo();

            wetterInfo.Dateiname = downloadName;

            string fileURL = Path.Combine(url, downloadName);

            string content = webClient.DownloadString(fileURL);

            string[] lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            wetterInfo.Region = Regex.Split(lines[3], "für")[1]?.Trim();

            if (lines.Length > 30)
            {
                string datum = lines[5].Split(',')[1];
                string uhrzeit = lines[5].Split(',')[2];
                uhrzeit = Regex.Replace(uhrzeit, "[a-zA-Z]", "").Trim();
                wetterInfo.WetterDatum = DateTime.Parse(datum + " " + uhrzeit);

                wetterInfo.Beschreibung = string.Join(" ", lines, 6, lines.Length - 7);

                wetterInfo.DownloadDatum = DateTime.Now;

                WetterInfos.Add(wetterInfo);
            }
        }
    }
}