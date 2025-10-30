using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using EnlightEnglishCenter.ViewModels;

namespace EnlightEnglishCenter.Controllers
{
    public class LeTanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeTanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🧾 Đăng ký học viên (từ modal)

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKyHocVien(HocVien model)
        {
            var dangKyList = (from dk in _context.DkHocVienLopHocs
                              join hv in _context.NguoiDungs on dk.MaHocVien equals hv.MaNguoiDung
                              join lop in _context.LopHocs on dk.MaLop equals lop.MaLop
                              select new
                              {
                                  dk.MaHocVien,
                                  dk.MaLop,
                                  hv.HoTen,
                                  Lop = lop.TenLop,
                                  dk.NgayDangKy,
                                  dk.TrangThai,
                                  dk.TrangThaiHoc
                              }).ToList();
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "⚠️ Dữ liệu nhập không hợp lệ.";
                return RedirectToAction("DanhSachHocVien");
            }

            model.NgayDangKy = DateTime.Now;

            _context.HocViens.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Đã đăng ký học viên mới thành công!";
            return RedirectToAction("DanhSachHocVien");
        }



        // ===============================
        // 🧾 2. Danh sách học viên (cho lễ tân)
        // ===============================
        public async Task<IActionResult> DanhSachHocVien()
        {
            var hocViens = await _context.HocViens.ToListAsync();
            return View(hocViens);
        }

        // ===============================
        // ✏️ 3. Sửa học viên
        // ===============================
        // Dùng để load dữ liệu khi bấm "Sửa"

        // ---------------------- TẠO ĐĂNG KÝ MỚI ----------------------
        [HttpGet]
        public async Task<IActionResult> GetHocVien(int id)
        {
            var hv = await _context.HocViens.FindAsync(id);
            if (hv == null) return NotFound();
            return Json(hv);
        }

        // Dùng để lưu thay đổi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHocVien(HocVien model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("DanhSachHocVien");

            _context.Update(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Đã cập nhật thông tin học viên!";
            return RedirectToAction("DanhSachHocVien");
        }
        public IActionResult XacNhanThanhToan()
        {
            var ds = _context.DonHocPhis
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                .ThenInclude(l => l.MaKhoaHocNavigation)
                .Where(d => d.TrangThai == "Chờ thanh toán")
                .OrderByDescending(d => d.NgayTao)
                .ToList();

            return View(ds);
        }

        // ✅ Lễ tân xác nhận thanh toán
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XacNhan(int id)
        {
            var don = _context.DonHocPhis
                .Include(d => d.LopHoc)
                .FirstOrDefault(d => d.MaDon == id);

            if (don == null)
            {
                TempData["Error"] = "Không tìm thấy đơn học phí!";
                return RedirectToAction(nameof(XacNhanThanhToan));
            }

            don.TrangThai = "Đã thanh toán";
            don.NgayThanhToan = DateTime.Now;
            _context.SaveChanges();

            TempData["Success"] = "✅ Đã xác nhận thanh toán cho học viên.";
            return RedirectToAction(nameof(XacNhanThanhToan));
        }
    

        // ===============================
        // ❌ 4. Xóa học viên
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaHocVien(int id)
        {
            var hv = await _context.HocViens.FindAsync(id);
            if (hv == null)
            {
                TempData["Error"] = "Không tìm thấy học viên để xóa!";
                return RedirectToAction("DanhSachHocVien");
            }

            _context.HocViens.Remove(hv);
            await _context.SaveChangesAsync();

            TempData["Success"] = "🗑️ Đã xóa học viên thành công!";
            return RedirectToAction("DanhSachHocVien");
        }


        // ===============================
        // ✅ Tùy chọn: View index tổng hợp (nếu cần)
        // ===============================
        public async Task<IActionResult> Index()
        {
            var hocVienRole = await _context.VaiTros.FirstOrDefaultAsync(v => v.TenVaiTro == "Học viên");
            int? roleId = hocVienRole?.MaVaiTro;

            var ds = _context.NguoiDungs.AsQueryable();
            if (roleId != null)
                ds = ds.Where(n => n.MaVaiTro == roleId);

            var list = await ds.ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> LienHeHocVien()
        {
            var viewModel = new LienHeHocVienViewModel
            {
                HocViens = await _context.HocViens.ToListAsync(),
                KhachHangs = await _context.LienHeKhachHang.ToListAsync()
            };
            return View(viewModel);
        }
        // ===============================
        // 📞 Thêm khách hàng tiềm năng
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemLienHe(LienHeKhachHang model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "⚠️ Vui lòng nhập đầy đủ thông tin liên hệ.";
                return RedirectToAction("LienHeHocVien");
            }

            model.NgayLienHe = DateTime.Now;
            _context.LienHeKhachHang.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Đã thêm khách hàng tiềm năng mới!";
            return RedirectToAction("LienHeHocVien");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaLienHe(int id)
        {
            var lienHe = await _context.LienHeKhachHang.FindAsync(id);
            if (lienHe == null)
            {
                TempData["Error"] = "Không tìm thấy liên hệ để xóa!";
                return RedirectToAction("LienHeHocVien");
            }

            _context.LienHeKhachHang.Remove(lienHe);
            await _context.SaveChangesAsync();

            TempData["Success"] = "🗑️ Đã xóa liên hệ khách hàng thành công!";
            return RedirectToAction("LienHeHocVien");
        }
        // ===============================
        // 💰 Quản lý thu học phí (cho Lễ tân)
        // ===============================
        public IActionResult QuanLyThuHocPhi()
        {
            var ds = _context.DonHocPhis
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                .ThenInclude(l => l.MaKhoaHocNavigation)
                .OrderByDescending(d => d.NgayTao)
                .ToList();

            return View(ds);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XacNhanHocPhi(int id)
        {
            var don = _context.DonHocPhis
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                .FirstOrDefault(d => d.MaDon == id);

            if (don == null)
            {
                TempData["Error"] = "Không tìm thấy đơn học phí!";
                return RedirectToAction(nameof(QuanLyThuHocPhi));
            }

            // ✅ Cập nhật trạng thái thanh toán
            don.TrangThai = "Đã thanh toán";
            don.NgayThanhToan = DateTime.Now;

            // ✅ Cập nhật sĩ số lớp
            var lop = _context.LopHocs.FirstOrDefault(l => l.MaLop == don.MaLop);
            if (lop != null)
            {
                lop.SiSoHienTai = (lop.SiSoHienTai ?? 0) + 1;
            }

            // ✅ Thêm học viên vào bảng DK_HocVien_LopHoc nếu chưa tồn tại
            var daTonTai = _context.DkHocVienLopHocs
                .Any(x => x.MaHocVien == don.MaHocVien && x.MaLop == don.MaLop);

            if (!daTonTai)
            {
                var dk = new DkHocVienLopHoc
                {
                    MaHocVien = don.MaHocVien,
                    MaLop = don.MaLop,
                    NgayDangKy = DateTime.Now,
                    TrangThai = "Đã thanh toán",
                    TrangThaiHoc = "Chưa bắt đầu"
                };
                _context.DkHocVienLopHocs.Add(dk);
            }

            _context.SaveChanges();

            TempData["Success"] = $"💰 Đã xác nhận thanh toán và thêm học viên {don.HocVien?.HoTen} vào lớp {don.LopHoc?.TenLop}.";
            return RedirectToAction(nameof(QuanLyThuHocPhi));
        }


        // Optional: action để lấy chi tiết (AJAX)
        [HttpGet]
        public async Task<IActionResult> ChiTietLienHe(int id, string loai)
        {
            if (loai == "KhachHang")
            {
                var kh = await _context.LienHeKhachHang.FindAsync(id);
                if (kh == null) return NotFound();
                return PartialView("_PartialChiTietKhachHang", kh);
            }
            else if (loai == "HocVien")
            {
                var hv = await _context.HocViens.FindAsync(id);
                if (hv == null) return NotFound();
                return PartialView("_PartialChiTietHocVien", hv);
            }
            return BadRequest();
        }

    }
}
