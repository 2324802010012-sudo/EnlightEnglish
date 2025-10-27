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

        // ===========================
        // 🏠 Trang chủ Phòng đào tạo
        // ===========================
        public IActionResult Index()
        {
            ViewData["Title"] = "Phòng Đào Tạo - Trang chủ";
            return View();
        }

        // ===========================
        // 🚪 Đăng xuất khỏi phòng đào tạo
        // ===========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Bạn đã đăng xuất khỏi Phòng đào tạo!";
            return RedirectToAction("Login", "Account");
        }

        // ===========================
        // ✅ DUYỆT TEST ĐẦU VÀO
        // ===========================
        public async Task<IActionResult> DuyetTest()
        {
            var dsTest = await _context.TestDauVaos
                .Include(t => t.HocVien)                     // FK -> NguoiDung
                .Include(t => t.KhoaHocDeXuatNavigation)     // FK -> KhoaHoc
                .AsNoTracking()
                .ToListAsync();

            ViewData["Title"] = "Duyệt test đầu vào";
            return View(dsTest);
        }

        [HttpPost]
        public async Task<IActionResult> CapNhatTrangThai(int id, string trangThai)
        {
            var test = await _context.TestDauVaos
                .Include(t => t.HocVien)
                .Include(t => t.KhoaHocDeXuatNavigation)
                .FirstOrDefaultAsync(t => t.MaTest == id);

            if (test == null) return NotFound();

            test.TrangThai = trangThai;

            // Ví dụ thông báo khi duyệt cho khóa IELTS
            if (trangThai == "Được phép test" && test.KhoaHocDeXuatNavigation?.TenKhoaHoc?.Contains("IELTS") == true)
            {
                await _context.SaveChangesAsync();

                var lop = await _context.LopHocs
                    .Include(l => l.MaKhoaHocNavigation)
                    .FirstOrDefaultAsync(l => l.MaKhoaHoc == test.KhoaHocDeXuat);

                if (lop != null)
                {
                    TempData["LopHoc"] = lop.TenLop;
                    TempData["HocPhi"] = lop.MaKhoaHocNavigation?.HocPhi ?? 0;
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"✅ Đã cập nhật test #{id} - Trạng thái: {trangThai}";
            return RedirectToAction(nameof(DuyetTest));
        }

        // ===========================
        // 👨‍🏫 QUẢN LÝ GIẢNG VIÊN
        // ===========================
        public async Task<IActionResult> GiaoVien()
        {
            var danhSach = await _context.GiaoViens
                .Include(g => g.NguoiDung)
                .AsNoTracking()
                .ToListAsync();

            ViewData["Title"] = "Quản lý giảng viên";
            return View(danhSach);
        }

        [HttpGet]
        public async Task<IActionResult> ThemGiaoVien()
        {
            var dsNguoiDung = await _context.NguoiDungs
                .Where(nd => nd.MaVaiTro == 5 /* 5 = Giáo viên theo seed của bạn */)
                .Select(nd => new { nd.MaNguoiDung, nd.HoTen })
                .ToListAsync();

            ViewBag.DSNguoiDung = new SelectList(dsNguoiDung, "MaNguoiDung", "HoTen");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemGiaoVien(GiaoVien model)
        {
            if (!ModelState.IsValid)
            {
                var dsNguoiDung = await _context.NguoiDungs
                    .Select(nd => new { nd.MaNguoiDung, nd.HoTen })
                    .ToListAsync();
                ViewBag.DSNguoiDung = new SelectList(dsNguoiDung, "MaNguoiDung", "HoTen");
                return View(model);
            }

            _context.GiaoViens.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "✅ Thêm giảng viên thành công!";
            return RedirectToAction(nameof(GiaoVien));
        }

        [HttpGet]
        public async Task<IActionResult> SuaGiaoVien(int id)
        {
            var gv = await _context.GiaoViens.FindAsync(id);
            if (gv == null) return NotFound();

            var dsNguoiDung = await _context.NguoiDungs
                .Select(nd => new { nd.MaNguoiDung, nd.HoTen })
                .ToListAsync();

            ViewBag.DSNguoiDung = new SelectList(dsNguoiDung, "MaNguoiDung", "HoTen", gv.MaNguoiDung);
            return View(gv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaGiaoVien(GiaoVien model)
        {
            if (!ModelState.IsValid)
            {
                var dsNguoiDung = await _context.NguoiDungs
                    .Select(nd => new { nd.MaNguoiDung, nd.HoTen })
                    .ToListAsync();
                ViewBag.DSNguoiDung = new SelectList(dsNguoiDung, "MaNguoiDung", "HoTen");
                return View(model);
            }

            _context.GiaoViens.Update(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "✅ Cập nhật giảng viên thành công!";
            return RedirectToAction(nameof(GiaoVien));
        }

        [HttpPost]
        public async Task<IActionResult> XoaGiaoVien(int id)
        {
            var gv = await _context.GiaoViens.FindAsync(id);
            if (gv == null) return NotFound();

            _context.GiaoViens.Remove(gv);
            await _context.SaveChangesAsync();

            TempData["Success"] = "🗑️ Đã xóa giảng viên thành công!";
            return RedirectToAction(nameof(GiaoVien));
        }

        // ===========================
        // 🧑‍🏫 PHÂN CÔNG GIẢNG VIÊN
        // ===========================
        public async Task<IActionResult> PhanCong()
        {
            var danhSach = await _context.PhanCongGiangDays
                .Include(p => p.GiaoVien).ThenInclude(g => g.NguoiDung)
                .Include(p => p.LopHoc).ThenInclude(l => l.MaKhoaHocNavigation)
                .OrderByDescending(p => p.NgayPhanCong)
                .AsNoTracking()
                .ToListAsync();

            ViewData["Title"] = "Phân công giảng viên";
            return View(danhSach);
        }

        [HttpGet]
        public async Task<IActionResult> ThemPhanCong()
        {
            var giaoViens = await _context.GiaoViens
                .Include(g => g.NguoiDung)
                .Select(g => new { g.MaGiaoVien, HoTen = g.NguoiDung.HoTen })
                .ToListAsync();

            var lopHocs = await _context.LopHocs
                .Select(l => new { l.MaLop, l.TenLop })
                .ToListAsync();

            ViewBag.DSGiaoVien = new SelectList(giaoViens, "MaGiaoVien", "HoTen");
            ViewBag.DSLopHoc = new SelectList(lopHocs, "MaLop", "TenLop");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemPhanCong(PhanCongGiangDay pc)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return View(pc);
            }

            _context.PhanCongGiangDays.Add(pc);
            await _context.SaveChangesAsync();
            TempData["Success"] = "✅ Phân công giảng viên thành công!";
            return RedirectToAction(nameof(PhanCong));
        }

        [HttpPost]
        public async Task<IActionResult> XoaPhanCong(int id)
        {
            var pc = await _context.PhanCongGiangDays.FindAsync(id);
            if (pc == null) return NotFound();

            _context.PhanCongGiangDays.Remove(pc);
            await _context.SaveChangesAsync();
            TempData["Success"] = "🗑️ Đã xóa phân công thành công!";
            return RedirectToAction(nameof(PhanCong));
        }

        private async Task LoadDropdownsAsync()
        {
            var lopHocs = await _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .Select(l => new { l.MaLop, TenLop = l.TenLop })
                .ToListAsync();

            ViewBag.DSLopHoc = new SelectList(lopHocs, "MaLop", "TenLop");
        }


        // ===========================
        // 📅 QUẢN LÝ LỊCH HỌC
        // ===========================
        public async Task<IActionResult> LichHoc()
        {
            var lichHoc = await _context.LichHocs
                .Include(l => l.LopHoc)
                    .ThenInclude(lh => lh.MaKhoaHocNavigation)
                .AsNoTracking()
                .OrderByDescending(l => l.NgayHoc)
                .ToListAsync();

            ViewData["Title"] = "Quản lý lịch học";
            return View(lichHoc);
        }

        [HttpGet]
        public async Task<IActionResult> ThemLichHoc()
        {
            ViewBag.DSLopHoc = new SelectList(
                await _context.LopHocs
                    .Include(l => l.MaKhoaHocNavigation)
                    .Select(l => new
                    {
                        l.MaLop,
                        TenLop = l.TenLop + " (" + (l.MaKhoaHocNavigation.TenKhoaHoc ?? "") + ")"
                    }).ToListAsync(),
                "MaLop", "TenLop");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemLichHoc(LichHoc model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return View(model);
            }

            _context.LichHocs.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "✅ Đã thêm buổi học thành công!";
            return RedirectToAction(nameof(LichHoc));
        }
        [HttpGet]
        public async Task<IActionResult> SuaLichHoc(int id)
        {
            var lh = await _context.LichHocs
                .Include(l => l.LopHoc)
                .FirstOrDefaultAsync(l => l.MaLich == id); // ✅ dùng MaLich

            if (lh == null) return NotFound();

            await LoadDropdownsAsync();
            return View(lh);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaLichHoc(LichHoc model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return View(model);
            }

            _context.LichHocs.Update(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "✅ Đã cập nhật buổi học thành công!";
            return RedirectToAction(nameof(LichHoc));
        }

        [HttpPost]
        public async Task<IActionResult> XoaLichHoc(int id)
        {
            var lh = await _context.LichHocs.FindAsync(id);
            if (lh == null) return NotFound();

            _context.LichHocs.Remove(lh);
            await _context.SaveChangesAsync();

            TempData["Success"] = "🗑️ Đã xóa buổi học thành công!";
            return RedirectToAction(nameof(LichHoc));
        }

        // ===========================
        // 🎓 QUẢN LÝ KHÓA HỌC
        // ===========================
        public async Task<IActionResult> KhoaHoc()
        {
            var dsKhoaHoc = await _context.KhoaHocs
                .OrderByDescending(k => k.NgayBatDau)
                .AsNoTracking()
                .ToListAsync();

            ViewData["Title"] = "Quản lý khóa học";
            return View(dsKhoaHoc);
        }

        [HttpGet]
        public IActionResult ThemKhoaHoc()
        {
            ViewData["Title"] = "Thêm khóa học mới";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemKhoaHoc(KhoaHoc model)
        {
            if (!ModelState.IsValid) return View(model);

            model.TrangThai = "Đang mở";
            _context.KhoaHocs.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "✅ Đã thêm khóa học mới thành công!";
            return RedirectToAction(nameof(KhoaHoc));
        }

        [HttpGet]
        public async Task<IActionResult> SuaKhoaHoc(int id)
        {
            var khoaHoc = await _context.KhoaHocs.FindAsync(id);
            if (khoaHoc == null) return NotFound();

            ViewData["Title"] = "Sửa khóa học";
            return View(khoaHoc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaKhoaHoc(KhoaHoc model)
        {
            if (!ModelState.IsValid) return View(model);

            _context.KhoaHocs.Update(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Đã cập nhật thông tin khóa học thành công!";
            return RedirectToAction(nameof(KhoaHoc));
        }

        [HttpPost]
        public async Task<IActionResult> XoaKhoaHoc(int id)
        {
            var khoaHoc = await _context.KhoaHocs.FindAsync(id);
            if (khoaHoc == null) return NotFound();

            _context.KhoaHocs.Remove(khoaHoc);
            await _context.SaveChangesAsync();

            TempData["Success"] = "🗑️ Đã xóa khóa học thành công!";
            return RedirectToAction(nameof(KhoaHoc));
        }

        [HttpPost]
        public async Task<IActionResult> DoiTrangThai(int id)
        {
            var khoaHoc = await _context.KhoaHocs.FindAsync(id);
            if (khoaHoc == null) return NotFound();

            khoaHoc.TrangThai = khoaHoc.TrangThai == "Đang mở" ? "Đã đóng" : "Đang mở";
            await _context.SaveChangesAsync();

            TempData["Success"] = $"🔄 Khóa học '{khoaHoc.TenKhoaHoc}' đã được {(khoaHoc.TrangThai == "Đang mở" ? "mở lại" : "đóng")}.";
            return RedirectToAction(nameof(KhoaHoc));
        }

        // ===========================
        // 🎓 QUẢN LÝ LỚP HỌC
        // ===========================
        public async Task<IActionResult> LopHoc()
        {
            var dsLop = await _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .OrderByDescending(l => l.MaKhoaHocNavigation!.NgayBatDau) // ✅ lấy từ KhoaHoc
                .ToListAsync();

            ViewData["Title"] = "Quản lý lớp học";
            return View(dsLop);
        }


        [HttpGet]
        public async Task<IActionResult> ThemLopHoc()
        {
            ViewBag.DSKhoaHoc = new SelectList(
                await _context.KhoaHocs.Select(k => new { k.MaKhoaHoc, k.TenKhoaHoc }).ToListAsync(),
                "MaKhoaHoc", "TenKhoaHoc"
            );
            ViewBag.DSGiaoVien = new SelectList(
                await _context.GiaoViens.Include(g => g.NguoiDung)
                    .Select(g => new { g.MaGiaoVien, HoTen = g.NguoiDung.HoTen }).ToListAsync(),
                "MaGiaoVien", "HoTen"
            );
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemLopHoc(LopHoc model)
        {
            if (!ModelState.IsValid)
            {
                await ThemLopHoc(); // nạp lại dropdown
                return View(model);
            }

            _context.LopHocs.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "✅ Thêm lớp học thành công!";
            return RedirectToAction(nameof(LopHoc));
        }

        // ===========================
        // 📅 QUẢN LÝ LỊCH KHAI GIẢNG
        // ===========================
        [HttpGet]
        public IActionResult LichKhaiGiang()
        {
            var lichList = _context.KhoaHocs
                .AsNoTracking()
                .OrderByDescending(k => k.NgayBatDau) // thay NgayKhaiGiang -> NgayBatDau
                .ToList();

            ViewData["Title"] = "Quản lý lịch khai giảng";
            return View(lichList);
        }

        [HttpGet]
        public IActionResult ThemKhaiGiang()
        {
            ViewData["Title"] = "Thêm lịch khai giảng";
            return View(new KhoaHoc { TrangThai = "Đang mở" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemKhaiGiang(KhoaHoc model)
        {
            if (!ModelState.IsValid) return View(model);

            model.TrangThai = "Đang mở";
            _context.KhoaHocs.Add(model);
            _context.SaveChanges();
            TempData["Success"] = "✅ Đã thêm lịch khai giảng thành công!";
            return RedirectToAction(nameof(LichKhaiGiang));
        }

        [HttpGet]
        public IActionResult SuaKhaiGiang(int id)
        {
            var kh = _context.KhoaHocs.Find(id);
            if (kh == null) return NotFound();

            ViewData["Title"] = "Sửa lịch khai giảng";
            return View(kh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaKhaiGiang(KhoaHoc model)
        {
            if (!ModelState.IsValid) return View(model);

            _context.KhoaHocs.Update(model);
            _context.SaveChanges();
            TempData["Success"] = "✅ Cập nhật lịch khai giảng thành công!";
            return RedirectToAction(nameof(LichKhaiGiang));
        }

        [HttpPost]
        public IActionResult XoaKhaiGiang(int id)
        {
            var kh = _context.KhoaHocs.Find(id);
            if (kh == null) return NotFound();

            _context.KhoaHocs.Remove(kh);
            _context.SaveChanges();
            TempData["Success"] = "🗑️ Đã xóa lịch khai giảng thành công!";
            return RedirectToAction(nameof(LichKhaiGiang));
        }

        // ===========================
        // 🧠 API lấy thông tin lớp học
        // ===========================
        [HttpGet]
        public async Task<IActionResult> ThongTinLop(int maLop)
        {
            var lop = await _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .FirstOrDefaultAsync(l => l.MaLop == maLop);

            if (lop == null) return Json(null);

            var data = new
            {
                tenKhoaHoc = lop.MaKhoaHocNavigation?.TenKhoaHoc ?? "Chưa có",
                ngayKhaiGiang = lop.MaKhoaHocNavigation?.NgayBatDau?.ToString("dd/MM/yyyy") ?? "Chưa rõ"
            };
            return Json(data);
        }

        // Tạo nhanh 9 khóa học mẫu (3 chủ đề x 3 cấp độ)
        [HttpGet]
        public async Task<IActionResult> TaoKhoaHocMau()
        {
            var danhSach = new List<KhoaHoc>();
            var chuDe = new[] { "IELTS", "TOEIC", "Cambridge" };
            var trinhDo = new[] { "Cơ bản", "Trung bình", "Nâng cao" };

            foreach (var cd in chuDe)
            {
                foreach (var td in trinhDo)
                {
                    danhSach.Add(new KhoaHoc
                    {
                        TenKhoaHoc = $"{cd} {td}",
                        CapDo = td,
                        TrangThai = "Đang mở",
                        NgayBatDau = DateTime.Now.AddDays(7),
                        NgayKetThuc = DateTime.Now.AddMonths(3),
                        ThoiLuong = "12 tuần / 36 buổi",    // ✅ thay cho LoTrinhHoc
                        MoTa = $"Khóa học {cd} trình độ {td} được tổ chức hoàn toàn online."
                    });
                }
            }

            _context.KhoaHocs.AddRange(danhSach);
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Đã tạo nhanh 3 bộ khóa học mẫu (IELTS, TOEIC, Cambridge)!";
            return RedirectToAction(nameof(KhoaHoc));
        }

        // ===========================
        // ⚠️ Error page
        // ===========================
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
