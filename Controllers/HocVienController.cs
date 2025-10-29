using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Controllers
{
    public class HocVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HocVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            int? maNguoiDung = HttpContext.Session.GetInt32("MaNguoiDung");
            if (maNguoiDung == null)
                return RedirectToAction("Login", "Account");

            var hocVien = _context.HocViens.FirstOrDefault(h => h.MaNguoiDung == maNguoiDung);
            if (hocVien == null)
                return RedirectToAction("Register", "Account");

            // ✅ Lấy bài test
            var test = _context.TestDauVaos
                .Include(t => t.KhoaHocDeXuatNavigation)
                .FirstOrDefault(t => t.MaHocVien == hocVien.MaHocVien);

            if (test != null)
            {
                ViewBag.TestStatus = test.TrangThai;
                ViewBag.DiemSo = test.TongDiem?.ToString("0.0");
                ViewBag.LopDeXuat = test.KhoaHocDeXuatNavigation?.TenKhoaHoc ?? test.LoTrinhHoc ?? "Chưa xác định";
            }

            // ✅ Lấy danh sách đơn học phí của học viên
            var donHocPhi = _context.DonHocPhis
                .Include(d => d.LopHoc)
                .ThenInclude(l => l.MaKhoaHocNavigation)
                .Where(d => d.MaHocVien == hocVien.MaHocVien)
                .OrderByDescending(d => d.NgayTao)
                .ToList();

            ViewBag.DonHocPhi = donHocPhi;

            return View();
        }

    }
}
