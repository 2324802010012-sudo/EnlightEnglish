using Microsoft.AspNetCore.Mvc;

namespace EnlightEnglishCenter.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
