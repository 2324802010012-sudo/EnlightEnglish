using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Trang chủ quản trị
        //public IActionResult Index()
        //{
        //    var vaiTro = HttpContext.Session.GetString("VaiTro");
        //    if (vaiTro != "Admin")
        //    {
        //        TempData["Error"] = "⚠️ Bạn không có quyền truy cập trang này!";
        //        return RedirectToAction("Index", "Home");
        //    }

        //    return View();
        //}
        public IActionResult Index()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "Admin")
            {
                TempData["Error"] = "⚠️ Bạn không có quyền truy cập trang này!";
                return RedirectToAction("Index", "Home");
            }

            // ✅ Thống kê
            var tongNguoiDung = _context.NguoiDungs.Count();
            var tongKhoaHoc = _context.KhoaHocs.Count();
            var tongLopHoc = _context.LopHocs.Count();
            var tongTest = _context.TestDauVaos.Count();

            var tongHocVienThangNay = _context.HocViens
      .Count(h => h.NgayDangKy.HasValue &&
                  h.NgayDangKy.Value.Month == DateTime.Now.Month &&
                  h.NgayDangKy.Value.Year == DateTime.Now.Year);


            var testHoanThanh = _context.TestDauVaos.Count(t => t.TrangThai == "Đã xác nhận");
            var lopHoatDong = _context.LopHocs.Count(l => l.TrangThai == "Đang hoạt động");

            // ✅ Gửi dữ liệu sang View
            ViewBag.TongNguoiDung = tongNguoiDung;
            ViewBag.TongKhoaHoc = tongKhoaHoc;
            ViewBag.TongLopHoc = tongLopHoc;
            ViewBag.TongTest = tongTest;

            ViewBag.TongHocVienThangNay = tongHocVienThangNay;
            ViewBag.TestHoanThanh = testHoanThanh;
            ViewBag.LopHoatDong = lopHoatDong;

            return View();
        }

        // 📋 Danh sách Test đầu vào
        public IActionResult DuyetTest()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "Admin")
            {
                TempData["Error"] = "⚠️ Bạn không có quyền truy cập trang này!";
                return RedirectToAction("Index", "Home");
            }

            var danhSach = _context.TestDauVaos
               // .Include(t => t.HocVien)
                .ToList();

            return View(danhSach);
        }

        [HttpPost]
        public IActionResult XacNhan(int id)
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "Admin")
            {
                TempData["Error"] = "⚠️ Bạn không có quyền thực hiện thao tác này!";
                return RedirectToAction("Index", "Home");
            }

            var test = _context.TestDauVaos.FirstOrDefault(t => t.MaTest == id);
            if (test == null)
                return NotFound();

            test.TrangThai = "Đã xác nhận";
            _context.SaveChanges();

            TempData["Success"] = "✅ Đã xác nhận bài Test của học viên thành công!";
            return RedirectToAction("DuyetTest");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNguoiDung(int id)
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "Admin")
            {
                TempData["ErrorMessage"] = "⚠️ Chỉ quản trị viên mới được phép xóa người dùng.";
                return RedirectToAction("QuanLyNguoiDung");
            }

            try
            {
                // 🔹 1️⃣ Xóa các bản điểm có MaHocVien = MaNguoiDung (trỏ trực tiếp)
                var diemLienKetNguoiDung = _context.DiemSos
                    .Where(d => d.MaHocVien == id)
                    .ToList();
                if (diemLienKetNguoiDung.Any())
                {
                    _context.DiemSos.RemoveRange(diemLienKetNguoiDung);
                }

                // 🔹 2️⃣ Lấy thông tin người dùng
                var nguoiDung = await _context.NguoiDungs.FindAsync(id);
                if (nguoiDung == null)
                {
                    TempData["ErrorMessage"] = "❌ Không tìm thấy người dùng.";
                    return RedirectToAction("QuanLyNguoiDung");
                }

                // 🔹 3️⃣ Kiểm tra xem người dùng có phải là học viên không
                var hocVien = await _context.HocViens.FirstOrDefaultAsync(h => h.MaNguoiDung == id);

                if (hocVien != null)
                {
                    // 🧹 a. Xóa điểm số của học viên
                    var diemList = _context.DiemSos.Where(d => d.MaHocVien == hocVien.MaHocVien).ToList();
                    if (diemList.Any())
                        _context.DiemSos.RemoveRange(diemList);

                    // 🧹 b. Xóa bài test đầu vào
                    var testList = _context.TestDauVaos.Where(t => t.MaHocVien == hocVien.MaHocVien).ToList();
                    if (testList.Any())
                        _context.TestDauVaos.RemoveRange(testList);

                    // 🧹 c. Xóa đăng ký lớp học
                    var dkList = _context.DkHocVienLopHocs.Where(d => d.MaHocVien == hocVien.MaHocVien).ToList();
                    if (dkList.Any())
                        _context.DkHocVienLopHocs.RemoveRange(dkList);

                    // 🧹 d. Xóa điểm danh
                    var ddList = _context.DiemDanhs.Where(d => d.MaHocVien == hocVien.MaHocVien).ToList();
                    if (ddList.Any())
                        _context.DiemDanhs.RemoveRange(ddList);

                    // 🧹 e. Xóa học viên
                    _context.HocViens.Remove(hocVien);
                }

                // 🔹 4️⃣ Cuối cùng xóa người dùng
                _context.NguoiDungs.Remove(nguoiDung);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "✅ Đã xóa người dùng và toàn bộ dữ liệu liên quan thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "❌ Lỗi khi xóa người dùng: " + ex.Message;
            }

            return RedirectToAction("QuanLyNguoiDung");
        }



    }
}
