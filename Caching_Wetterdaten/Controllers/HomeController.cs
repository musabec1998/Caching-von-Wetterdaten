using Caching_Wetterdaten.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace Caching_Wetterdaten.Controllers
{
    public class HomeController : Controller
    {
        private readonly WetterRepository repo = new WetterRepository();

        private readonly IMemoryCache Cache;

        private const string CacheName = "Wetterdaten";

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            Cache = memoryCache;
        }

        public IActionResult Index()
        {
            List<WetterInfo> wetterInfos;

            bool ifExist = Cache.TryGetValue(CacheName, out wetterInfos);

            if (ifExist == false)
            {
                repo.AlleDateienRunterladen();

                wetterInfos = repo.WetterInfos;

                Cache.Set(CacheName, wetterInfos);
            }

            return View(wetterInfos);
        }

        public IActionResult WetterDatenUpdaten()
        {
            repo.WetterInfos = Cache.Get<List<WetterInfo>>(CacheName);

            repo.AlleDateienRunterladen();

            Cache.Set(CacheName, repo.WetterInfos);

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}