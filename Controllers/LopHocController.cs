using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;

namespace EnlightEnglishCenter.Controllers
{
    public class LopHocController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LopHocController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ChiTiet(int id)
        {
            int? maNguoiDung = HttpContext.Session.GetInt32("MaNguoiDung");
            if (maNguoiDung == null)
                return RedirectToAction("Login", "Account");

            var hocVien = _context.HocViens.FirstOrDefault(h => h.MaNguoiDung == maNguoiDung);
            if (hocVien == null)
                return RedirectToAction("Index", "HocVien");

            // ✅ Lấy lớp học + các thông tin liên quan
            var lop = _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .Include(l => l.LichHocs)
                .Include(l => l.DiemSos.Where(d => d.MaHocVien == hocVien.MaHocVien))
                    .ThenInclude(d => d.MaHocVienNavigation)
                .AsNoTracking()
                .FirstOrDefault(l => l.MaLop == id);

            if (lop == null)
                return NotFound();

            // ✅ Lấy giảng viên trực tiếp từ bảng GiaoVien (thông qua PhanCongGiangDay)
            var giaoViens = _context.PhanCongGiangDays
                .Include(pc => pc.GiaoVien)
                .Where(pc => pc.MaLop == id)
                .Select(pc => pc.GiaoVien.HoTen)   // ⚠️ Lấy trực tiếp từ GiaoVien.HoTen
                .ToList();

            string tenGiangVien = (giaoViens != null && giaoViens.Any())
                ? string.Join(", ", giaoViens)
                : "— Chưa phân công —";

            // ✅ Truyền dữ liệu ra View
            ViewBag.GiangVien = tenGiangVien;
            ViewBag.KhoaHoc = lop.MaKhoaHocNavigation?.TenKhoaHoc ?? "—";
            ViewBag.TenLop = lop.TenLop;
            ViewBag.NgayBatDau = lop.NgayBatDau?.ToString("dd/MM/yyyy");
            ViewBag.NgayKetThuc = lop.NgayKetThuc?.ToString("dd/MM/yyyy");
            ViewBag.SiSoHienTai = lop.SiSoHienTai;
            ViewBag.SiSoToiDa = lop.SiSoToiDa;
            ViewBag.TrangThai = lop.TrangThai;

            return View(lop);
        }
    }
}
