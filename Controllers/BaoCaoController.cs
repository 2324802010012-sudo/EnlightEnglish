using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Controllers
{
    public class BaoCaoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BaoCaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Trang hiển thị danh sách báo cáo
        public IActionResult Index()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "Admin")
            {
                TempData["Error"] = "⚠️ Bạn không có quyền truy cập chức năng này!";
                return RedirectToAction("Index", "Home");
            }

            // Lấy toàn bộ bảng BaoCao (có cả thông tin người lập)
            var dsBaoCao = _context.BaoCaos
                .Include(b => b.NguoiLapNavigation)
                .OrderByDescending(b => b.NgayLap)
                .ToList();

            return View(dsBaoCao);
        }
    }
}
