using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Controllers
{
    public class LopHocController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LopHocController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===============================
        // 📘 Xem chi tiết lớp học
        // ===============================
        public IActionResult ChiTiet(int id)
        {
            var lop = _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .Include(l => l.PhanCongGiangDays)
                    .ThenInclude(pc => pc.GiaoVien)
                    .ThenInclude(gv => gv.NguoiDung)
                .Include(l => l.LichHocs)
                .FirstOrDefault(l => l.MaLop == id);

            if (lop == null)
            {
                TempData["Error"] = "Không tìm thấy lớp học.";
                return RedirectToAction("Index", "HocVien");
            }

            return View(lop);
        }
    }
}
