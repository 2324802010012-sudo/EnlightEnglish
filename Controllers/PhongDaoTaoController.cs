using System.Diagnostics;
using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Controllers
{
    public class PhongDaoTaoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PhongDaoTaoController> _logger;

        public PhongDaoTaoController(ApplicationDbContext context, ILogger<PhongDaoTaoController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ===============================
        // 🏠 Index
        // ===============================
        public IActionResult Index()
        {
            ViewData["Title"] = "Phòng Đào Tạo - Trang chủ";
            return View();
        }

        // ===============================
        // 🚪 Logout
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Bạn đã đăng xuất khỏi Phòng đào tạo!";
            return RedirectToAction("Login", "Account");
        }

        // ==========================================================
        // ✅ TEST ĐẦU VÀO
        // ==========================================================
        // 📋 Danh sách Test đầu vào
        public IActionResult DuyetTest()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "Phòng đào tạo")
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
    

        // ==========================================================
        // 👩‍🏫 QUẢN LÝ GIẢNG VIÊN
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> GiaoVien()
        {
            var danhSach = await _context.GiaoViens
                .Include(g => g.NguoiDung)
                .AsNoTracking()
                .ToListAsync();

            return View(danhSach);
        }

        [HttpGet]
        public async Task<IActionResult> ThemGiaoVien()
        {
            await BuildNguoiDungChoGiaoVienViewBag();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemGiaoVien(GiaoVien model)
        {
            if (!ModelState.IsValid)
            {
                await BuildNguoiDungChoGiaoVienViewBag();
                return View(model);
            }

            // Chống trùng theo MaNguoiDung
            bool tonTai = await _context.GiaoViens.AnyAsync(g => g.MaNguoiDung == model.MaNguoiDung);
            if (tonTai)
            {
                ModelState.AddModelError("", "Người dùng này đã có hồ sơ giảng viên.");
                await BuildNguoiDungChoGiaoVienViewBag();
                return View(model);
            }

            _context.GiaoViens.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "✅ Thêm giảng viên thành công!";
            return RedirectToAction(nameof(GiaoVien));
        }

        private async Task BuildNguoiDungChoGiaoVienViewBag()
        {
            // ví dụ vai trò GV = 5
            var daCoGv = await _context.GiaoViens.Select(g => g.MaNguoiDung).ToListAsync();
            var dsNguoiDung = await _context.NguoiDungs
                .Where(nd => nd.MaVaiTro == 5 && !daCoGv.Contains(nd.MaNguoiDung))
                .Select(nd => new { nd.MaNguoiDung, nd.HoTen })
                .OrderBy(x => x.HoTen)
                .ToListAsync();

            ViewBag.DSNguoiDung = new SelectList(dsNguoiDung, "MaNguoiDung", "HoTen");
        }

        // ==========================================================
        // 🧑‍🏫 PHÂN CÔNG GIẢNG VIÊN
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> PhanCong()
        {
            var danhSach = await _context.PhanCongGiangDays
                .Include(p => p.GiaoVien).ThenInclude(g => g.NguoiDung)
                .Include(p => p.LopHoc).ThenInclude(l => l.MaKhoaHocNavigation)
                .OrderByDescending(p => p.NgayPhanCong)
                .AsNoTracking()
                .ToListAsync();

            return View(danhSach);
        }

        [HttpGet]
        public async Task<IActionResult> ThemPhanCong()
        {
            var dsGiaoVien = await _context.GiaoViens
                .Include(g => g.NguoiDung)
                .Select(g => new { g.MaGiaoVien, Ten = g.NguoiDung.HoTen })
                .ToListAsync();

            var dsLop = await _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .Select(l => new
                {
                    l.MaLop,
                    Ten = l.TenLop + " - " + (l.MaKhoaHocNavigation.TenKhoaHoc ?? "")
                })
                .ToListAsync();

            ViewBag.GiaoVien = new SelectList(dsGiaoVien, "MaGiaoVien", "Ten");
            ViewBag.LopHoc = new SelectList(dsLop, "MaLop", "Ten");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemPhanCong(PhanCongGiangDay model)
        {
            if (!ModelState.IsValid)
            {
                await ThemPhanCong();
                return View(model);
            }

            // chống trùng
            bool tonTai = await _context.PhanCongGiangDays
                .AnyAsync(p => p.MaGiaoVien == model.MaGiaoVien && p.MaLop == model.MaLop);

            if (tonTai)
            {
                ModelState.AddModelError("", "⚠️ Giảng viên này đã được phân công lớp này rồi!");
                await ThemPhanCong();
                return View(model);
            }

            model.NgayPhanCong = DateTime.Now;
            _context.PhanCongGiangDays.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Phân công giảng viên thành công!";
            return RedirectToAction(nameof(PhanCong));
        }

        // ==========================================================
        // 📅 LỊCH HỌC (BUỔI HỌC)
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> LichHoc()
        {
            var list = await _context.LichHocs
                .Include(x => x.LopHoc)
                .ThenInclude(l => l.MaKhoaHocNavigation)
                .OrderByDescending(x => x.NgayHoc)
                .AsNoTracking()
                .ToListAsync();

            return View(list);
        }

        // GET: /PhongDaoTao/ThemLichHoc (theo lớp)
        [HttpGet]
        public async Task<IActionResult> ThemLichHoc()
        {
            ViewBag.DSLopHoc = new SelectList(
                await _context.LopHocs
                    .Include(l => l.MaKhoaHocNavigation)
                    .Where(l => l.MaKhoaHocNavigation.TrangThai == "Đang mở")
                    .Select(l => new
                    {
                        l.MaLop,
                        Ten = l.TenLop + " (" + (l.MaKhoaHocNavigation.TenKhoaHoc ?? "") + ")"
                    })
                    .ToListAsync(),
                "MaLop", "Ten"
            );
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemLichHoc(LichHoc model)
        {
            if (!ModelState.IsValid)
            {
                await ThemLichHoc();
                return View(model);
            }

            if (model.MaLop <= 0)
            {
                ModelState.AddModelError(nameof(model.MaLop), "Vui lòng chọn lớp.");
                await ThemLichHoc();
                return View(model);
            }

            _context.LichHocs.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Đã thêm buổi học thành công!";
            return RedirectToAction(nameof(LichHoc));
        }

        // Thêm buổi học nhanh (UI khác)
        [HttpGet]
        public async Task<IActionResult> ThemBuoiHoc()
        {
            var lop = await _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .Where(l => l.MaKhoaHocNavigation.TrangThai == "Đang mở")
                .Select(l => new
                {
                    l.MaLop,
                    Ten = l.TenLop + " (" + (l.MaKhoaHocNavigation.TenKhoaHoc ?? "") + ")"
                })
                .AsNoTracking()
                .ToListAsync();

            ViewBag.LopHoc = new SelectList(lop, "MaLop", "Ten");
            ViewBag.PhongHoc = new List<SelectListItem> {
                new("-- Không chọn --", ""),
                new("P101", "P101"),
                new("P102", "P102"),
                new("P201", "P201"),
                new("Online (Zoom)", "Online-Zoom"),
            };

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemBuoiHoc(LichHoc model)
        {
            if (model.GioBatDau >= model.GioKetThuc)
                ModelState.AddModelError(nameof(model.GioKetThuc), "Giờ kết thúc phải sau giờ bắt đầu.");

            if (!ModelState.IsValid)
            {
                await ThemBuoiHoc();
                return View(model);
            }

            _context.LichHocs.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Đã thêm buổi học!";
            return RedirectToAction(nameof(LichHoc));
        }

        // ==========================================================
        // 🧾 KHOÁ HỌC
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> KhoaHoc()
        {
            var data = await _context.KhoaHocs
                .OrderByDescending(x => x.NgayBatDau)
                .AsNoTracking()
                .ToListAsync();

            return View(data);
        }

        [HttpGet]
        public IActionResult ThemKhoaHoc()
        {
            BuildKhoaMauViewBag();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemKhoaHoc(KhoaHoc model)
        {
            if (!ModelState.IsValid)
            {
                BuildKhoaMauViewBag();
                return View(model);
            }

            if (model.NgayBatDau.HasValue && model.ThoiLuongTuan > 0)
                model.NgayKetThuc = model.NgayBatDau.Value.AddDays(model.ThoiLuongTuan * 7);

            model.TrangThai = "Đang mở";

            _context.KhoaHocs.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Tạo khóa học thành công!";
            return RedirectToAction(nameof(KhoaHoc));
        }

        private void BuildKhoaMauViewBag()
        {
            ViewBag.KhoaMau = new List<dynamic>
            {
                new { Ten="IELTS Foundation",     CapDo="Cơ bản",   HocPhi=3500000, Tuan=8 },
                new { Ten="IELTS Intermediate",   CapDo="Trung bình", HocPhi=4500000, Tuan=10 },
                new { Ten="IELTS Advanced",       CapDo="Nâng cao", HocPhi=5500000, Tuan=12 },

                new { Ten="TOEIC Foundation",     CapDo="Cơ bản",   HocPhi=2500000, Tuan=6 },
                new { Ten="TOEIC Intermediate",   CapDo="Trung bình", HocPhi=3000000, Tuan=8 },
                new { Ten="TOEIC Advanced",       CapDo="Nâng cao", HocPhi=3800000, Tuan=10 },

                new { Ten="Cambridge Starters",   CapDo="Cơ bản",   HocPhi=3000000, Tuan=10 },
                new { Ten="Cambridge Movers",     CapDo="Trung bình", HocPhi=3800000, Tuan=12 },
                new { Ten="Cambridge Flyers",     CapDo="Nâng cao", HocPhi=4500000, Tuan=14 }
            };
        }

        // ==========================================================
        // 🏫 LỚP HỌC
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> LopHoc()
        {
            var dsLop = await _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .OrderByDescending(l => l.MaKhoaHocNavigation!.NgayBatDau)
                .AsNoTracking()
                .ToListAsync();

            ViewData["Title"] = "Quản lý lớp học";
            return View(dsLop);
        }

        [HttpGet]
        public async Task<IActionResult> ThemLopHoc()
        {
            ViewBag.DSKhoaHoc = new SelectList(
                await _context.KhoaHocs
                    .Where(k => k.TrangThai == "Đang mở")
                    .Select(k => new { k.MaKhoaHoc, k.TenKhoaHoc })
                    .ToListAsync(),
                "MaKhoaHoc", "TenKhoaHoc"
            );

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemLopHoc(LopHoc model)
        {
            if (!ModelState.IsValid)
            {
                await ThemLopHoc();
                return View(model);
            }

            _context.LopHocs.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Thêm lớp học thành công!";
            return RedirectToAction(nameof(LopHoc));
        }

        // ==========================================================
        // 📅 LỊCH KHAI GIẢNG = DANH SÁCH KHOÁ HỌC
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> LichKhaiGiang(string? q, string? trangThai = "Tất cả")
        {
            var query = _context.KhoaHocs.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(k => k.TenKhoaHoc.Contains(q) || k.CapDo.Contains(q));

            if (!string.IsNullOrWhiteSpace(trangThai) && trangThai != "Tất cả")
                query = query.Where(k => k.TrangThai == trangThai);

            var data = await query
                .OrderByDescending(k => k.NgayBatDau)
                .ToListAsync();

            ViewBag.CurrentQ = q;
            ViewBag.CurrentTrangThai = trangThai;
            return View(data); // Views/PhongDaoTao/LichKhaiGiang.cshtml
        }

        // 🔄 Mở/đóng nhanh
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiTrangThai(int id)
        {
            var khoaHoc = await _context.KhoaHocs.FindAsync(id);
            if (khoaHoc == null) return NotFound();

            khoaHoc.TrangThai = khoaHoc.TrangThai == "Đang mở" ? "Đã đóng" : "Đang mở";
            await _context.SaveChangesAsync();

            TempData["Success"] = $"🔄 Khóa học '{khoaHoc.TenKhoaHoc}' đã được {(khoaHoc.TrangThai == "Đang mở" ? "mở lại" : "đóng")}!";
            return RedirectToAction(nameof(LichKhaiGiang));
        }

        // 🗑️ Xoá khoá học
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaKhoaHoc(int id)
        {
            var kh = await _context.KhoaHocs.FindAsync(id);
            if (kh == null) return NotFound();

            _context.KhoaHocs.Remove(kh);
            await _context.SaveChangesAsync();

            TempData["Success"] = "🗑️ Đã xóa khóa học.";
            return RedirectToAction(nameof(LichKhaiGiang));
        }

        // ======================== API JSON ========================
        [HttpGet]
        public async Task<IActionResult> ThongTinLop(int maLop)
        {
            var lop = await _context.LopHocs
                .Include(x => x.MaKhoaHocNavigation)
                .FirstOrDefaultAsync(x => x.MaLop == maLop);

            if (lop == null) return Json(null);

            return Json(new
            {
                tenKhoaHoc = lop.MaKhoaHocNavigation?.TenKhoaHoc,
                ngayKhaiGiang = lop.MaKhoaHocNavigation?.NgayBatDau.HasValue == true
                    ? lop.MaKhoaHocNavigation!.NgayBatDau!.Value.ToString("dd/MM/yyyy")
                    : ""
            });
        }

        // ======================== ERROR ===========================
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
