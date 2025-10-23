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
            // Lấy danh sách các lớp học (lịch khai giảng) có trạng thái đang mở hoặc chưa xác định
            var lichKhaiGiang = _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .Include(l => l.MaGiaoVienNavigation)
                    .ThenInclude(gv => gv.NguoiDung)
                .Where(l => l.TrangThai == "Đang mở" || l.TrangThai == "Chưa xác định" || l.TrangThai == "Đang học")
                .OrderBy(l => l.NgayBatDau)
                .ToList();

            return View(lichKhaiGiang);
        }
    }
}
