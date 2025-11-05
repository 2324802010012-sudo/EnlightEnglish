using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EnlightEnglishCenter.Controllers
{
    public class BaoCaoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BaoCaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Danh sách báo cáo chung
        public IActionResult Index()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "Admin")
            {
                TempData["Error"] = "⚠️ Bạn không có quyền truy cập chức năng này!";
                return RedirectToAction("Index", "Home");
            }

            var dsBaoCao = _context.BaoCaos
                .Include(b => b.NguoiLapNavigation)
                .OrderByDescending(b => b.NgayLap)
                .ToList();

            return View(dsBaoCao);
        }

        // =====================================
        // 📊 BÁO CÁO TỔNG HỢP: Lễ tân + Kế toán
        // =====================================
        public IActionResult BaoCaoTaiChinh()
        {
            // === 1️⃣ Báo cáo LỄ TÂN (Doanh thu học phí) ===
            var baoCaoLeTan = _context.BaoCaos
                .Include(b => b.NguoiLapNavigation)
                .Where(b => b.LoaiBaoCao.Contains("Doanh thu học phí"))
                .OrderByDescending(b => b.NgayLap)
                .ToList();

            var tongDoanhThu = _context.HocPhis
                .Where(h => h.TrangThai == "Đã thanh toán")
                .Sum(h => (decimal?)h.SoTienPhaiDong) ?? 0;

            var soHocVienDaDong = _context.HocPhis
                .Count(h => h.TrangThai == "Đã thanh toán");

            // === 2️⃣ Báo cáo KẾ TOÁN (Lương giáo viên) ===
            var baoCaoKeToan = _context.BaoCaos
                .Include(b => b.NguoiLapNavigation)
                .Where(b => b.LoaiBaoCao.Contains("Lương giáo viên"))
                .OrderByDescending(b => b.NgayLap)
                .ToList();

            var tongLuong = _context.LuongGiaoViens
                .Sum(l => (decimal?)l.TongLuong) ?? 0;

            var soGiaoVien = _context.LuongGiaoViens
                .Select(l => l.MaGiaoVien)
                .Distinct()
                .Count();

            // === 3️⃣ Lợi nhuận tạm tính ===
            var loiNhuan = tongDoanhThu - tongLuong;

            // Truyền sang View
            ViewBag.TongHocPhi = tongDoanhThu;
            ViewBag.SoHocVienDaDong = soHocVienDaDong;
            ViewBag.TongLuong = tongLuong;
            ViewBag.SoGiaoVien = soGiaoVien;
            ViewBag.LoiNhuan = loiNhuan;

            ViewBag.BaoCaoLeTan = baoCaoLeTan;
            ViewBag.BaoCaoKeToan = baoCaoKeToan;

            return View();
        }
    }
}
