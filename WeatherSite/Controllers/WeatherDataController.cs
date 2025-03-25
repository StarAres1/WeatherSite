using Microsoft.AspNetCore.Mvc;

namespace WeatherSite.Controllers
{
    public class WeatherDataController : Controller
    {
        public IActionResult Getting()
        {
            return View();
        }

        public IActionResult Adding()
        {
            return View();
        }
    }
}
