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
            HttpContext.Session.Clear(); // Xóa toàn bộ session
            TempData["Success"] = "Bạn đã đăng xuất khỏi Phòng đào tạo!";
            return RedirectToAction("Login", "Account"); // Quay lại trang đăng nhập sẵn có
        }

        // ===========================
        // ✅ DUYỆT TEST ĐẦU VÀO
        // ===========================
        public async Task<IActionResult> DuyetTest()
        {
            var dsTest = await _context.TestDauVaos
                .Include(t => t.HocVien)
                .Include(t => t.KhoaHocDeXuatNavigation)
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
                .Include(t => t.KhoaHocDeXuatNavigation) // ✅ dùng navigation thật
                .FirstOrDefaultAsync(t => t.MaTest == id);

            if (test == null)
                return NotFound();

            test.TrangThai = trangThai;

            // ✅ Nếu học viên được duyệt test đầu vào cho khóa IELTS
            if (trangThai == "Được phép test" && test.KhoaHocDeXuatNavigation?.TenKhoaHoc.Contains("IELTS") == true)
            {
                // Nếu bạn có thêm cột DaLamBaiTest
                // test.DaLamBaiTest = false;

                await _context.SaveChangesAsync();

                // ✅ Lấy lớp và học phí tương ứng
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

        // ✅ Danh sách giảng viên
        public async Task<IActionResult> GiaoVien()
        {
            var danhSach = await _context.GiaoViens
                .Include(g => g.NguoiDung)
                .AsNoTracking()
                .ToListAsync();

            ViewData["Title"] = "Quản lý giảng viên";
            return View(danhSach);
        }

        // ✅ Form thêm giảng viên
        [HttpGet]
        public async Task<IActionResult> ThemGiaoVien()
        {
            ViewBag.DSNguoiDung = new SelectList(
                await _context.NguoiDungs
                    .Where(nd => nd.MaVaiTro == 3) // 3 = Giảng viên
                    .ToListAsync(),
                "MaNguoiDung", "HoTen"
            );

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemGiaoVien(GiaoVien model)
        {
            if (ModelState.IsValid)
            {
                _context.GiaoViens.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "✅ Thêm giảng viên thành công!";
                return RedirectToAction(nameof(GiaoVien));
            }

            ViewBag.DSNguoiDung = new SelectList(await _context.NguoiDungs.ToListAsync(), "MaNguoiDung", "HoTen");
            return View(model);
        }

        // ✅ Sửa giảng viên
        [HttpGet]
        public async Task<IActionResult> SuaGiaoVien(int id)
        {
            var gv = await _context.GiaoViens.FindAsync(id);
            if (gv == null) return NotFound();

            ViewBag.DSNguoiDung = new SelectList(await _context.NguoiDungs.ToListAsync(), "MaNguoiDung", "HoTen", gv.MaNguoiDung);
            return View(gv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaGiaoVien(GiaoVien model)
        {
            if (ModelState.IsValid)
            {
                _context.GiaoViens.Update(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "✅ Cập nhật giảng viên thành công!";
                return RedirectToAction(nameof(GiaoVien));
            }

            ViewBag.DSNguoiDung = new SelectList(await _context.NguoiDungs.ToListAsync(), "MaNguoiDung", "HoTen");
            return View(model);
        }

        // ✅ Xóa giảng viên
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

        // ✅ Danh sách phân công
        public async Task<IActionResult> PhanCong()
        {
            var danhSach = await _context.PhanCongGiangDays
                .Include(p => p.GiaoVien)
                    .ThenInclude(g => g.NguoiDung)
                .Include(p => p.LopHoc)
                    .ThenInclude(l => l.MaKhoaHocNavigation)
                .OrderByDescending(p => p.NgayPhanCong)
                .ToListAsync();

            ViewData["Title"] = "Phân công giảng viên";
            return View(danhSach);
        }

        // ✅ Form thêm phân công
        [HttpGet]
        public async Task<IActionResult> ThemPhanCong()
        {
            var giaoViens = await _context.GiaoViens
                .Include(g => g.NguoiDung)
                .ToListAsync();

            var lopHocs = await _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .ToListAsync();

            ViewBag.DSGiaoVien = new SelectList(giaoViens, "MaGiaoVien", "NguoiDung.HoTen");
            ViewBag.DSLopHoc = new SelectList(lopHocs, "MaLop", "TenLop");
            return View();
        }

        // ✅ Xử lý thêm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemPhanCong(PhanCongGiangDay pc)
        {
            if (ModelState.IsValid)
            {
                _context.PhanCongGiangDays.Add(pc);
                await _context.SaveChangesAsync();
                TempData["Success"] = "✅ Phân công giảng viên thành công!";
                return RedirectToAction(nameof(PhanCong));
            }

       
            return View(pc);
        }

        // ✅ Xóa phân công
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

        // Helper tải dropdown
        private async Task LoadDropdownsAsync()
        {
            var giaoViens = await _context.GiaoViens.Include(g => g.NguoiDung).ToListAsync();
            var lopHocs = await _context.LopHocs.Include(l => l.MaKhoaHocNavigation).ToListAsync();

            ViewBag.DSGiaoVien = new SelectList(giaoViens, "MaGiaoVien", "NguoiDung.HoTen");
            ViewBag.DSLopHoc = new SelectList(lopHocs, "MaLop", "TenLop");
        }

        // ===========================
        // 📅 QUẢN LÝ LỊCH HỌC
        // ===========================
        // ===========================
        // 📅 QUẢN LÝ LỊCH HỌC
        // ===========================
        public async Task<IActionResult> LichHoc()
        {
            var lichHoc = await _context.LichHocs
                .Include(l => l.LopHoc)
                    .ThenInclude(lh => lh.MaKhoaHocNavigation)
                .Include(l => l.GiaoVien)
                    .ThenInclude(gv => gv.NguoiDung)
                .AsNoTracking()
                .OrderByDescending(l => l.NgayHoc)
                .ToListAsync();

            ViewData["Title"] = "Quản lý lịch học";
            return View(lichHoc);
        }

        // ➕ GET: Thêm buổi học
        [HttpGet]
        public async Task<IActionResult> ThemLichHoc()
        {
            var giaoViens = await _context.GiaoViens
                .Include(g => g.NguoiDung)
                .AsNoTracking()
                .ToListAsync();

            ViewBag.DSLopHoc = new SelectList(await _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .Select(l => new
                {
                    l.MaLop,
                    TenLop = l.TenLop + " (" + l.MaKhoaHocNavigation.TenKhoaHoc + ")"
                })
                .ToListAsync(), "MaLop", "TenLop");

            ViewBag.DSGiaoVien = new SelectList(giaoViens, "MaGiaoVien", "NguoiDung.HoTen");

            return View();
        }

        // 💾 POST: Thêm buổi học
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemLichHoc(LichHoc model)
        {
            if (ModelState.IsValid)
            {
                _context.LichHocs.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "✅ Đã thêm buổi học thành công!";
                return RedirectToAction(nameof(LichHoc));
            }

            await LoadDropdownsAsync();
            return View(model);
        }

        // ✏️ GET: Sửa buổi học
        [HttpGet]
        public async Task<IActionResult> SuaLichHoc(int id)
        {
            var lh = await _context.LichHocs
                .Include(l => l.LopHoc)
                .Include(l => l.GiaoVien)
                .FirstOrDefaultAsync(l => l.MaLichHoc == id);

            if (lh == null) return NotFound();

            await LoadDropdownsAsync();
            return View(lh);
        }

        // 💾 POST: Sửa buổi học
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaLichHoc(LichHoc model)
        {
            if (ModelState.IsValid)
            {
                _context.LichHocs.Update(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "✅ Đã cập nhật buổi học thành công!";
                return RedirectToAction(nameof(LichHoc));
            }

            await LoadDropdownsAsync();
            return View(model);
        }

        // ❌ Xóa buổi học
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
            if (ModelState.IsValid)
            {
                model.TrangThai = "Đang mở";
                _context.KhoaHocs.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "✅ Đã thêm khóa học mới thành công!";
                return RedirectToAction(nameof(KhoaHoc));
            }
            return View(model);
        }
        // ===========================
        // ✏️ Sửa khóa học
        // ===========================
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

        // ===========================
        // ❌ Xóa khóa học
        // ===========================
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

        // ===========================
        // 🔁 Đóng / Mở khóa học
        // ===========================
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
                .OrderByDescending(l => l.NgayBatDau)
                .ToListAsync();

            ViewData["Title"] = "Quản lý lớp học";
            return View(dsLop);
        }

        // ➕ Thêm lớp học (GET)
        [HttpGet]
        public async Task<IActionResult> ThemLopHoc()
        {
            ViewBag.DSKhoaHoc = new SelectList(
                await _context.KhoaHocs.ToListAsync(),
                "MaKhoaHoc", "TenKhoaHoc"
            );
            return View();
        }

        // 💾 Thêm lớp học (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemLopHoc(LopHoc model)
        {
            if (ModelState.IsValid)
            {
                _context.LopHocs.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "✅ Thêm lớp học thành công!";
                return RedirectToAction(nameof(LopHoc));
            }

            ViewBag.DSKhoaHoc = new SelectList(
                await _context.KhoaHocs.ToListAsync(),
                "MaKhoaHoc", "TenKhoaHoc"
            );
            return View(model);
        }

        // ===========================
        // 📅 QUẢN LÝ LỊCH KHAI GIẢNG
        // ===========================

        // ✅ Hiển thị danh sách lịch khai giảng
        [HttpGet]
        public IActionResult LichKhaiGiang()
        {
            var lichList = _context.KhoaHocs
                .OrderByDescending(k => k.NgayKhaiGiang)
                .ToList();

            ViewData["Title"] = "Quản lý lịch khai giảng";
            return View(lichList);
        }

        // ✅ Form thêm mới
        [HttpGet]
        public IActionResult ThemKhaiGiang()
        {
            ViewData["Title"] = "Thêm lịch khai giảng";
            return View();
        }

        // ✅ Xử lý thêm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemKhaiGiang(KhoaHoc model)
        {
            if (ModelState.IsValid)
            {
                model.TrangThai = "Đang mở";
                _context.KhoaHocs.Add(model);
                _context.SaveChanges();
                TempData["Success"] = "✅ Đã thêm lịch khai giảng thành công!";
                return RedirectToAction(nameof(LichKhaiGiang));
            }
            return View(model);
        }

        // ✅ Sửa lịch khai giảng
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
            if (ModelState.IsValid)
            {
                _context.KhoaHocs.Update(model);
                _context.SaveChanges();
                TempData["Success"] = "✅ Cập nhật lịch khai giảng thành công!";
                return RedirectToAction(nameof(LichKhaiGiang));
            }
            return View(model);
        }

        // ✅ Xóa lịch khai giảng
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
        // ⚙️ Helpers
        // ===========================
      

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
                        NgayBatDau = DateOnly.FromDateTime(DateTime.Now.AddDays(7)), // sau 1 tuần
                        NgayKetThuc = DateOnly.FromDateTime(DateTime.Now.AddMonths(3)),
                        LoTrinhHoc = "12 tuần / 36 buổi",
                        MoTa = $"Khóa học {cd} trình độ {td} được tổ chức hoàn toàn online.",
                    });
                }
            }

            _context.KhoaHocs.AddRange(danhSach);
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Đã tạo nhanh 3 khóa học mẫu (IELTS, TOEIC, Cambridge)!";
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
