using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Controllers
{
    public class LichKhaiGiangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LichKhaiGiangController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Lấy danh sách lớp + khóa học + (giảng viên -> người dùng)
            var lichKhaiGiang = _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .Include(l => l.MaGiaoVienNavigation)
                    .ThenInclude(gv => gv.NguoiDung)
                .Where(l =>
                       l.TrangThai == "Đang mở"
                    || l.TrangThai == "Đang học"
                    || l.TrangThai == "Chưa xác định"
                    || l.TrangThai == null)
                .OrderBy(l => l.MaKhoaHocNavigation!.NgayBatDau)   // ✅ ngày từ KhoaHoc
                .AsNoTracking()
                .ToList();

            return View(lichKhaiGiang);
        }
    }
}
