using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EnlightEnglishCenter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Courses()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangKyHoTro(string HoTen, string Email, string SoDienThoai)
        {
            if (string.IsNullOrWhiteSpace(HoTen) || string.IsNullOrWhiteSpace(SoDienThoai))
            {
                TempData["Error"] = "⚠️ Vui lòng nhập đầy đủ thông tin!";
                return RedirectToAction("Index");
            }

            // ✅ Tạo bản ghi mới trong bảng LienHeKhachHang (bảng của Lễ Tân)
            var lienHe = new LienHeKhachHang
            {
                HoTen = HoTen.Trim(),
                Email = Email?.Trim(),
                DienThoai = SoDienThoai.Trim(),
                TrangThai = "Đăng ký tư vấn",   // 👈 để hiển thị đúng cột
                NgayLienHe = DateTime.Now,
                GhiChu = "Đăng ký tư vấn từ trang chủ"
            };

            _context.LienHeKhachHang.Add(lienHe);
            _context.SaveChanges();

            TempData["Success"] = "✅ Đăng ký thành công! Bộ phận Lễ Tân sẽ liên hệ với bạn sớm.";
            return RedirectToAction("Index");
        }
    }
}