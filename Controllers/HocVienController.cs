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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // 🧠 1️⃣ Kiểm tra quyền Admin
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "Admin")
            {
                TempData["ErrorMessage"] = "⚠️ Chỉ quản trị viên mới được phép xóa học viên.";
                return RedirectToAction("Index");
            }

            // 🧠 2️⃣ Tìm học viên cần xóa
            var hocVien = await _context.HocViens
                .Include(h => h.MaNguoiDung)
                .FirstOrDefaultAsync(h => h.MaHocVien == id);

            if (hocVien == null)
            {
                TempData["ErrorMessage"] = "❌ Không tìm thấy học viên.";
                return RedirectToAction("Index");
            }

            try
            {
                // 🧹 Xóa tất cả bài Test đầu vào liên quan tới học viên này
                var tests = _context.TestDauVaos.Where(t => t.MaHocVien == id).ToList();
                if (tests.Any())
                {
                    _context.TestDauVaos.RemoveRange(tests);
                }

                // 🧹 Sau đó xóa học viên
                _context.HocViens.Remove(hocVien);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "✅ Đã xóa học viên và các bài Test đầu vào liên quan thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "❌ Lỗi khi xóa học viên: " + ex.Message;
            }


            return RedirectToAction("Index");
        }


    }
}
