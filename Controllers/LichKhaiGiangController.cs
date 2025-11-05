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

        // ✅ Hiển thị 3 khóa học mặc định
        public IActionResult Index()
        {
            var danhSach = _context.KhoaHocs
                .Where(k =>
                    (k.TrangThai == "Đang mở" || k.TrangThai == "Đang học") &&
                    (k.LoaiKhoaHoc == "ChinhThuc" || k.LoaiKhoaHoc == null))
                .OrderBy(k => k.NgayBatDau);

            // 🔹 Lấy 3 khóa học đầu tiên
            var top3 = danhSach.Take(3).AsNoTracking().ToList();

            ViewBag.Total = danhSach.Count();   // tổng số khóa học
            ViewBag.ShowAll = false;            // đang hiển thị giới hạn

            return View(top3);
        }

        // ✅ Hiển thị toàn bộ danh sách
        public IActionResult TatCa()
        {
            var danhSach = _context.KhoaHocs
                .Where(k =>
                    (k.TrangThai == "Đang mở" || k.TrangThai == "Đang học") &&
                    (k.LoaiKhoaHoc == "ChinhThuc" || k.LoaiKhoaHoc == null))
                .OrderBy(k => k.NgayBatDau)
                .AsNoTracking()
                .ToList();

            ViewBag.Total = danhSach.Count();
            ViewBag.ShowAll = true; // đang hiển thị tất cả

            return View("Index", danhSach);
        }
        public IActionResult ThangSau()
        {
            var now = DateTime.Now.AddMonths(1);
            int nextMonth = now.Month;
            int nextYear = now.Year;

            var danhSach = _context.KhoaHocs
                .Where(k =>
                    k.NgayBatDau.HasValue &&
                    k.NgayBatDau.Value.Month == nextMonth &&
                    k.NgayBatDau.Value.Year == nextYear &&
                    (k.TrangThai == "Đang mở" || k.TrangThai == "Đang học") &&
                    (k.LoaiKhoaHoc == "ChinhThuc" || k.LoaiKhoaHoc == null))
                .OrderBy(k => k.NgayBatDau)
                .AsNoTracking()
                .ToList();

            ViewBag.Thang = nextMonth;
            ViewBag.Nam = nextYear;
            ViewBag.Title = $"Lịch khai giảng tháng {nextMonth}/{nextYear}";

            return View("Index", danhSach);
        }
    }
}
    
