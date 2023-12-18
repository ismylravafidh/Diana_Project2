using Microsoft.AspNetCore.Mvc;

namespace Diana_Project.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
