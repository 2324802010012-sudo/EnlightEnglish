using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Controllers
{
    public class HocVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        // ✅ Constructor để inject DbContext
        public HocVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            int? maNguoiDung = HttpContext.Session.GetInt32("MaNguoiDung");
            if (maNguoiDung == null)
                return RedirectToAction("Login", "Account");

            // ✅ Lấy test gần nhất của học viên
            var test = _context.TestDauVaos
                .Where(t => t.MaHocVien == maNguoiDung)
                .OrderByDescending(t => t.NgayTest)
                .FirstOrDefault();

            if (test != null)
                ViewBag.TestStatus = test.TrangThai;
            else
                ViewBag.TestStatus = null;

            return View();
        }
    }
}
