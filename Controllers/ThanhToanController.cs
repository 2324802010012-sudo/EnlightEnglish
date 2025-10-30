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

        // ✅ Học viên tạo đơn đăng ký lớp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TaoPhieuDangKy(int maLop)
        {
            int? maNguoiDung = HttpContext.Session.GetInt32("MaNguoiDung");
            if (maNguoiDung == null)
                return RedirectToAction("Login", "Account");

            // 🔍 Chỉ lấy người dùng đã có bản ghi trong bảng HOCVIEN
            var hocVien = _context.HocViens
             .Include(h => h.NguoiDung)

                .FirstOrDefault(h => h.MaNguoiDung == maNguoiDung);

            if (hocVien == null)
            {
                TempData["Error"] = "⚠️ Tài khoản này chưa được đăng ký là học viên. Vui lòng liên hệ lễ tân!";
                return RedirectToAction("Index", "HocVien");
            }

            var lop = _context.LopHocs.FirstOrDefault(x => x.MaLop == maLop);
            if (lop == null)
                return NotFound();

            // ✅ Tạo đơn học phí đúng học viên
            var don = new DonHocPhi
            {
                MaHocVien = hocVien.MaHocVien,     // ✅ CHUẨN: lấy mã học viên thật
                MaLop = maLop,
                NgayTao = DateTime.Now,
                TongTien = lop.HocPhi ?? 0,
                TrangThai = "Chờ xác nhận lễ tân"
            };
            _context.DonHocPhis.Add(don);
            _context.SaveChanges();

            TempData["Info"] = "🕒 Đơn đăng ký của bạn đang chờ lễ tân xác nhận!";
            return RedirectToAction("ThanhToanDon", new { id = don.MaDon });
        }

        // ✅ Trang xem chi tiết đơn thanh toán
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

        // ✅ Danh sách lớp theo khóa (hiển thị cho học viên)
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
        // ✅ Xác nhận thanh toán thành công (Lễ tân hoặc hệ thống)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XacNhanThanhToan(int maDon)
        {
            // 1️⃣ Tìm đơn học phí
            var don = _context.DonHocPhis
                .Include(d => d.LopHoc)
                .Include(d => d.HocVien)
                .FirstOrDefault(d => d.MaDon == maDon);

            if (don == null)
            {
                TempData["Error"] = "Không tìm thấy đơn học phí!";
                return RedirectToAction("Index", "LeTan");
            }

            // 2️⃣ Cập nhật trạng thái thanh toán
            don.TrangThai = "Đã thanh toán";
            _context.SaveChanges();

            // 3️⃣ Thêm học viên vào lớp nếu chưa có trong DK_HocVien_LopHoc
            bool daCo = _context.DkHocVienLopHocs.Any(dk =>
                dk.MaHocVien == don.MaHocVien && dk.MaLop == don.MaLop);

            if (!daCo)
            {
                var dk = new DkHocVienLopHoc
                {
                    MaHocVien = don.HocVien?.MaNguoiDung ?? 0, // ✅ Trỏ đúng tới NguoiDung
                    MaLop = don.MaLop,
                    NgayDangKy = DateTime.Now,
                    TrangThai = "Đã thanh toán",
                    TrangThaiHoc = "Chưa bắt đầu"
                };

                _context.DkHocVienLopHocs.Add(dk);

                // Cập nhật sĩ số lớp
                var lop = _context.LopHocs.FirstOrDefault(l => l.MaLop == don.MaLop);
                if (lop != null)
                {
                    lop.SiSoHienTai = (lop.SiSoHienTai ?? 0) + 1;
                }

                _context.SaveChanges();
            }

            TempData["Success"] = "💰 Đã xác nhận thanh toán và thêm học viên vào lớp!";
            return RedirectToAction("DanhSachDon", "LeTan"); // hoặc "Index" tuỳ vai trò
        }

    }
}
