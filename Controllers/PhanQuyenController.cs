using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Controllers
{
    public class PhanQuyenController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhanQuyenController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Trang danh sách người dùng + vai trò
        public IActionResult Index()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "Admin")
            {
                TempData["Error"] = "⚠️ Bạn không có quyền truy cập chức năng này!";
                return RedirectToAction("Index", "Home");
            }

            var dsNguoiDung = _context.NguoiDungs
                .Include(nd => nd.MaVaiTroNavigation)
                .ToList();

            return View(dsNguoiDung);
        }

        // ✅ Giao diện sửa quyền
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var nguoiDung = _context.NguoiDungs
                .Include(nd => nd.MaVaiTroNavigation)
                .FirstOrDefault(nd => nd.MaNguoiDung == id);

            if (nguoiDung == null)
                return NotFound();

            ViewBag.VaiTro = _context.VaiTros.ToList();
            return View(nguoiDung);
        }

        // ✅ Lưu thay đổi quyền
        [HttpPost]
        public IActionResult Edit(int id, int maVaiTro)
        {
            var nguoiDung = _context.NguoiDungs.Find(id);
            if (nguoiDung == null)
                return NotFound();

            nguoiDung.MaVaiTro = maVaiTro;
            _context.SaveChanges();

            TempData["Success"] = "✅ Cập nhật quyền người dùng thành công!";
            return RedirectToAction("Index");
        }
    }
}
