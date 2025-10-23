using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Controllers
{
    public class GiaoVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GiaoVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ------------------ 🏠 TRANG CHÍNH ------------------
        public IActionResult Index()
        {
            ViewData["Title"] = "Trang giảng viên - ENLIGHT";
            return View();
        }

        // ------------------ 📚 LỚP ĐANG DẠY ------------------
        [HttpGet]
        public IActionResult LopDangDay(string? search)
        {
            var maGv = HttpContext.Session.GetInt32("MaGiaoVien");
            if (maGv == null)
                return RedirectToAction("Login", "Account");

            // Lấy danh sách lớp mà giảng viên đang dạy
            var lopQuery = _context.PhanCongGiangDays
                .Include(p => p.LopHoc)
                    .ThenInclude(l => l.MaKhoaHocNavigation)
                .Where(p => p.MaGiaoVien == maGv);

            // Lọc tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                lopQuery = lopQuery.Where(p =>
                    p.LopHoc.TenLop.Contains(search) ||
                    p.LopHoc.MaLop.ToString().Contains(search));
            }

            var lopDangDay = lopQuery.ToList();
            ViewBag.Search = search;
            ViewData["Title"] = "Lớp đang dạy";
            return View(lopDangDay);
        }

        // ------------------ 📖 CHI TIẾT LỚP HỌC ------------------
        [HttpGet]
        public IActionResult ChiTietLop(int id)
        {
            var maGv = HttpContext.Session.GetInt32("MaGiaoVien");
            if (maGv == null)
                return RedirectToAction("Login", "Account");

            // Lấy thông tin lớp học + giảng viên + lịch học
            var lop = _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .Include(l => l.LichHocs)
                .Include(l => l.PhanCongGiangDays)
                    .ThenInclude(pc => pc.GiaoVien)
                        .ThenInclude(gv => gv.NguoiDung)
                .FirstOrDefault(l => l.MaLop == id);

            if (lop == null)
                return NotFound();

            // ✅ Lấy tên giảng viên đầu tiên trong danh sách phân công
            var phanCong = lop.PhanCongGiangDays.FirstOrDefault();
            var giangVien = phanCong?.GiaoVien?.NguoiDung?.HoTen ?? "Chưa phân công";

            // ✅ Truyền dữ liệu sang View
            ViewBag.GiangVien = giangVien;
            ViewBag.TenLop = lop.TenLop;
            ViewBag.KhoaHoc = lop.MaKhoaHocNavigation?.TenKhoaHoc;
            ViewBag.NgayBatDau = lop.NgayBatDau?.ToString("dd/MM/yyyy");
            ViewBag.PhongHoc = lop.LichHocs.FirstOrDefault()?.PhongHoc ?? "Chưa sắp lịch";
            ViewBag.TongBuoi = lop.LichHocs.Count;

            ViewData["Title"] = "Chi tiết lớp học";
            return View(lop);
        }

        // ------------------ 👨‍🎓 DANH SÁCH HỌC VIÊN ------------------
        public IActionResult DanhSachHocVien(int id)
        {
            var hocViens = _context.DkHocVienLopHocs
                .Include(d => d.MaHocVienNavigation)
                .Where(d => d.MaLop == id)
                .ToList();

            ViewBag.MaLop = id;
            ViewData["Title"] = "Danh sách học viên";
            return View(hocViens);
        }

        // ------------------ 🧮 NHẬP ĐIỂM ------------------
        [HttpGet]
        public async Task<IActionResult> NhapDiem(int? maLop)
        {
            var giaoVien = HttpContext.Session.GetInt32("MaGiaoVien");
            if (giaoVien == null)
                return RedirectToAction("Login", "Account");

            // Danh sách lớp mà giảng viên này dạy
            ViewBag.DSLopHoc = await _context.PhanCongGiangDays
                .Include(p => p.LopHoc)
                .Where(p => p.MaGiaoVien == giaoVien)
                .Select(p => new { p.LopHoc.MaLop, p.LopHoc.TenLop })
                .Distinct()
                .ToListAsync();

            if (maLop == null)
                return View(new List<DiemSo>());

            // Danh sách học viên trong lớp để nhập điểm
            var dsHocVien = await _context.DkHocVienLopHocs
                .Include(d => d.MaHocVienNavigation)
                .Where(d => d.MaLop == maLop)
                .Select(d => new DiemSo
                {
                    MaHocVien = d.MaHocVien,
                    MaLop = d.MaLop,
                    MaHocVienNavigation = d.MaHocVienNavigation,
                    DiemGiuaKy = 0,
                    DiemCuoiKy = 0
                })
                .ToListAsync();

            ViewBag.MaLop = maLop;
            ViewData["Title"] = "Nhập điểm học viên";
            return View(dsHocVien);
        }

        // ------------------ 💾 LƯU ĐIỂM ------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LuuDiem(int maLop, List<DiemSo> diemList)
        {
            foreach (var item in diemList)
            {
                var diem = await _context.DiemSos
                    .FirstOrDefaultAsync(d => d.MaHocVien == item.MaHocVien && d.MaLop == maLop);

                if (diem != null)
                {
                    // Cập nhật điểm cũ
                    diem.DiemGiuaKy = item.DiemGiuaKy;
                    diem.DiemCuoiKy = item.DiemCuoiKy;
                    diem.NhanXet = item.NhanXet;
                }
                else
                {
                    // Thêm mới nếu chưa có
                    _context.DiemSos.Add(new DiemSo
                    {
                        MaHocVien = item.MaHocVien,
                        MaLop = maLop,
                        DiemGiuaKy = item.DiemGiuaKy,
                        DiemCuoiKy = item.DiemCuoiKy,
                        NhanXet = item.NhanXet
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "✅ Đã lưu điểm thành công!";
            return RedirectToAction(nameof(NhapDiem), new { maLop });
        }
    }
}
