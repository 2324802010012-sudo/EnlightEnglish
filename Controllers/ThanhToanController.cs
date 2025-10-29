using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Controllers
{
    public class ThanhToanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThanhToanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Trang xác nhận lớp -> chuyển qua thanh toán
        public IActionResult ChonLop(int id) // id = MaLop
        {
            int? maHocVien = HttpContext.Session.GetInt32("MaNguoiDung");
            if (maHocVien == null)
                return RedirectToAction("Login", "Account");

            var lop = _context.LopHocs
                .Include(x => x.MaKhoaHocNavigation)
                .FirstOrDefault(x => x.MaLop == id);

            if (lop == null)
                return NotFound();

            return View(lop);
        }

        // ✅ Tạo phiếu đăng ký lớp và qua trang thanh toán
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TaoPhieuDangKy(int maLop)
        {
            int? maNguoiDung = HttpContext.Session.GetInt32("MaNguoiDung");
            if (maNguoiDung == null)
                return RedirectToAction("Login", "Account");

            var hocVien = _context.HocViens.FirstOrDefault(h => h.MaNguoiDung == maNguoiDung);
            if (hocVien == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin học viên!";
                return RedirectToAction("Index", "HocVien");
            }

            var lop = _context.LopHocs.FirstOrDefault(x => x.MaLop == maLop);
            if (lop == null)
                return NotFound();

            // ✅ Tạo đơn học phí
            var don = new DonHocPhi
            {
                MaHocVien = hocVien.MaHocVien,
                MaLop = maLop,
                NgayTao = DateTime.Now,
                TongTien = lop.HocPhi ?? 0,
                TrangThai = "Chờ thanh toán"
            };
            _context.DonHocPhis.Add(don);

            // ✅ Ghi danh lớp
            var dk = new DkHocVienLopHoc
            {
                MaHocVien = hocVien.MaHocVien,
                MaLop = maLop
            };
            _context.DkHocVienLopHocs.Add(dk);

            // ✅ Cập nhật sĩ số lớp
            lop.SiSoHienTai = (lop.SiSoHienTai ?? 0) + 1;

            _context.SaveChanges();

            // ✅ Chuyển đúng sang trang "ThanhToanDon"
            return RedirectToAction("ThanhToanDon", "ThanhToan", new { id = don.MaDon });

        }



        // ✅ Trang Thanh Toán
        public IActionResult ThanhToanDon(int id)
        {
            var don = _context.DonHocPhis
                .Include(x => x.HocVien)
                .Include(x => x.LopHoc)
                .ThenInclude(l => l.MaKhoaHocNavigation)
                .FirstOrDefault(x => x.MaDon == id);

            if (don == null)
                return NotFound();

            return View(don);
        }
        [HttpGet]
        public IActionResult DanhSachLopTheoKhoa(int khoaHocId)
        {
            var lopMo = _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .Where(l => l.MaKhoaHoc == khoaHocId && l.TrangThai == "Đang mở")
                .OrderBy(l => l.NgayBatDau)
                .ToList();

            var khoa = _context.KhoaHocs.Find(khoaHocId);
            ViewBag.Khoa = khoa;

            return View(lopMo);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XacNhanThanhToan(int maDon)
        {
            var don = _context.DonHocPhis
      .Include(d => d.LopHoc)
      .ThenInclude(l => l.MaKhoaHocNavigation)
      .FirstOrDefault(d => d.MaDon == maDon);


            if (don == null)
            {
                TempData["Error"] = "Không tìm thấy đơn đăng ký.";
                return RedirectToAction("Index", "HocVien");
            }

            don.TrangThai = "Đã thanh toán";
            don.NgayThanhToan = DateTime.Now;
            _context.SaveChanges();

            // ✅ Cập nhật sĩ số lớp
            if (don.MaLop != null)
            {
                var lop = _context.LopHocs.FirstOrDefault(l => l.MaLop == don.MaLop);
                if (lop != null)
                {
                    lop.SiSoHienTai = (lop.SiSoHienTai ?? 0) + 1;
                    _context.SaveChanges();
                }
            }

            TempData["Success"] = "💰 Xác nhận thanh toán thành công!";
            return RedirectToAction("Index", "HocVien");
        }


    }
}

