using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeatherSite.Models;

namespace WeatherSite.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
