using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EnlightEnglishCenter.Controllers
{
    public class LeTanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeTanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------------------- TRANG CHÍNH ----------------------
        public IActionResult Index()
        {
            ViewData["Title"] = "Trang lễ tân";
            return View();
        }

        // ---------------------- DANH SÁCH ĐĂNG KÝ ----------------------
        public IActionResult DangKyHocVien()
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
                                  dk.TrangThai
                              }).ToList();

            return View(dangKyList);
        }

        // ---------------------- TẠO ĐĂNG KÝ MỚI ----------------------
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.LopList = _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .Where(l => l.TrangThai == "Đang học")
                .ToList();

            return View();
        }

        // ---------------------- LƯU ĐĂNG KÝ ----------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string TenHocVien, int MaLop, DateTime NgayDangKy, string TrangThai)
        {
            if (string.IsNullOrWhiteSpace(TenHocVien))
            {
                TempData["Error"] = "⚠️ Vui lòng nhập tên học viên!";
                return RedirectToAction(nameof(Create));
            }

            // 🔹 Tạo học viên mới
            var hv = new NguoiDung
            {
                HoTen = TenHocVien.Trim(),
                TenDangNhap = "hv" + DateTime.Now.Ticks, // tên đăng nhập ngẫu nhiên
                MatKhau = "123456", // mật khẩu mặc định
                Email = $"{Guid.NewGuid()}@placeholder.local",
                MaVaiTro = 4, // 4 = học viên
                TrangThai = "Đang hoạt động"
            };

            _context.NguoiDungs.Add(hv);
            _context.SaveChanges();

            // 🔹 Tạo bản ghi đăng ký
            var dk = new DkHocVienLopHoc
            {
                MaHocVien = hv.MaNguoiDung,
                MaLop = MaLop,
                NgayDangKy = NgayDangKy,
                TrangThai = TrangThai
            };

            _context.DkHocVienLopHocs.Add(dk);
            _context.SaveChanges();

            TempData["Message"] = $"✅ Học viên '{TenHocVien}' đã được đăng ký vào lớp thành công!";
            return RedirectToAction(nameof(DangKyHocVien));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int maHocVien, int maLop)
        {
            var dk = _context.DkHocVienLopHocs
                .FirstOrDefault(d => d.MaHocVien == maHocVien && d.MaLop == maLop);

            if (dk == null)
            {
                TempData["Error"] = "❌ Không tìm thấy đăng ký học viên để xóa.";
                return RedirectToAction(nameof(DangKyHocVien));
            }

            // Xóa bản ghi đăng ký
            _context.DkHocVienLopHocs.Remove(dk);

            // Xóa học viên luôn (nếu không còn đăng ký lớp nào khác)
            var soLopKhac = _context.DkHocVienLopHocs
                .Count(d => d.MaHocVien == maHocVien && d.MaLop != maLop);

            if (soLopKhac == 0)
            {
                var hv = _context.NguoiDungs.FirstOrDefault(x => x.MaNguoiDung == maHocVien);
                if (hv != null)
                {
                    _context.NguoiDungs.Remove(hv);
                }
            }

            _context.SaveChanges();

            TempData["Message"] = "🗑️ Đã xóa học viên khỏi hệ thống.";
            return RedirectToAction(nameof(DangKyHocVien));
        }
        public IActionResult LienHeHocVien()
        {
            var hocVienList = (from hv in _context.NguoiDungs
                               join dk in _context.DkHocVienLopHocs
                                   on hv.MaNguoiDung equals dk.MaHocVien
                               where hv.MaVaiTro == 4
                               select new
                               {
                                   hv.MaNguoiDung,
                                   hv.HoTen,
                                   hv.Email,
                                   hv.SoDienThoai,
                                   hv.TrangThai
                               })
                               .Distinct()
                               .ToList();

            return View(hocVienList);
        }

    }
}