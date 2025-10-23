using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Trang chủ quản trị
        public IActionResult Index()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "Admin")
            {
                TempData["Error"] = "⚠️ Bạn không có quyền truy cập trang này!";
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // 📋 Danh sách Test đầu vào
        public IActionResult DuyetTest()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "Admin")
            {
                TempData["Error"] = "⚠️ Bạn không có quyền truy cập trang này!";
                return RedirectToAction("Index", "Home");
            }

            var danhSach = _context.TestDauVaos
               // .Include(t => t.HocVien)
                .ToList();

            return View(danhSach);
        }

        [HttpPost]
        public IActionResult XacNhan(int id)
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "Admin")
            {
                TempData["Error"] = "⚠️ Bạn không có quyền thực hiện thao tác này!";
                return RedirectToAction("Index", "Home");
            }

            var test = _context.TestDauVaos.FirstOrDefault(t => t.MaTest == id);
            if (test == null)
                return NotFound();

            test.TrangThai = "Đã xác nhận";
            _context.SaveChanges();

            TempData["Success"] = "✅ Đã xác nhận bài Test của học viên thành công!";
            return RedirectToAction("DuyetTest");
        }
    }
}
