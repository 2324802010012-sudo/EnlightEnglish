using System.Diagnostics;
using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Controllers
{
    public class PhongDaoTaoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PhongDaoTaoController> _logger;

        public PhongDaoTaoController(ApplicationDbContext context, ILogger<PhongDaoTaoController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            // 👉 Gắn layout PhongDaoTaoLayout.cshtml cho view
            ViewData["Layout"] = "~/Views/Shared/PhongDaoTaoLayout.cshtml";
            ViewData["Title"] = "Phòng Đào Tạo - Trang chủ";
            return View();
        }

        public IActionResult Privacy()
        {
            ViewData["Layout"] = "~/Views/Shared/PhongDaoTaoLayout.cshtml";
            ViewData["Title"] = "Chính sách & Quy định";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
