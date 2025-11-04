
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

            if (vaiTro != "Admin" && vaiTro != "Phòng đào tạo")
            {
                TempData["Error"] = "⚠️ Bạn không có quyền thực hiện thao tác này!";
                return RedirectToAction("Index", "Home");
            }

            var test = _context.TestDauVaos.FirstOrDefault(t => t.MaTest == id);
            if (test == null)
                return NotFound();

            test.TrangThai = "Được phép test";

            _context.SaveChanges();

            TempData["Success"] = "✅ Đã xác nhận bài Test của học viên thành công!";

            // ✅ Giữ lại ở trang duyệt test của Phòng đào tạo
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
        // ------------------ 📋 DANH SÁCH PHÂN CÔNG ------------------
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

        // ------------------ ➕ THÊM PHÂN CÔNG (GET) ------------------
        [HttpGet]
        public async Task<IActionResult> ThemPhanCong()
        {
            // Danh sách giảng viên
            var dsGiaoVien = await _context.GiaoViens
                .Include(g => g.NguoiDung)
                .Select(g => new { g.MaGiaoVien, Ten = g.NguoiDung.HoTen })
                .ToListAsync();

            // Danh sách lớp chưa được phân công
            var dsLop = await _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .Where(l => !_context.PhanCongGiangDays.Any(p => p.MaLop == l.MaLop))
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

        // ------------------ 💾 LƯU PHÂN CÔNG (POST) ------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemPhanCong(PhanCongGiangDay model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "⚠️ Vui lòng chọn đầy đủ thông tin!";
                await ThemPhanCong();
                return View(model);
            }

            // Kiểm tra trùng
            bool tonTai = await _context.PhanCongGiangDays
                .AnyAsync(p => p.MaGiaoVien == model.MaGiaoVien && p.MaLop == model.MaLop);

            if (tonTai)
            {
                TempData["Error"] = "⚠️ Giảng viên này đã được phân công lớp này rồi!";
                await ThemPhanCong();
                return View(model);
            }

            model.NgayPhanCong = DateTime.Now;
            _context.PhanCongGiangDays.Add(model);
            await _context.SaveChangesAsync();

            // ✅ Cập nhật MaGiaoVien vào bảng LopHoc sau khi phân công
            if (model.MaLop.HasValue && model.MaGiaoVien.HasValue)
            {
                var lop = await _context.LopHocs.FirstOrDefaultAsync(l => l.MaLop == model.MaLop);
                if (lop != null)
                {
                    lop.MaGiaoVien = model.MaGiaoVien;
                    await _context.SaveChangesAsync();
                }
            }

            TempData["Success"] = "✅ Phân công giảng viên thành công!";
            return RedirectToAction(nameof(PhanCong));

        }
        // ===============================
        // 🗑️ XÓA PHÂN CÔNG GIẢNG DẠY
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XoaPhanCong(int id)
        {
            var phanCong = _context.PhanCongGiangDays.FirstOrDefault(p => p.MaPhanCong == id);
            if (phanCong == null)
            {
                TempData["Error"] = "Không tìm thấy bản ghi phân công!";
                return RedirectToAction("PhanCong");
            }

            _context.PhanCongGiangDays.Remove(phanCong);
            _context.SaveChanges();

            TempData["Success"] = "✅ Đã xóa phân công giảng dạy thành công!";
            return RedirectToAction("PhanCong");
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
        [HttpGet]
        public IActionResult SuaKhoaHoc(int id)
        {
            var khoaHoc = _context.KhoaHocs.FirstOrDefault(k => k.MaKhoaHoc == id);
            if (khoaHoc == null)
                return NotFound();

            return View(khoaHoc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaKhoaHoc(KhoaHoc model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.Update(model);
            _context.SaveChanges();

            TempData["Success"] = "✅ Cập nhật khóa học thành công!";
            return RedirectToAction(nameof(KhoaHoc));
        }

        // ==========================================================
        // 🏫 LỚP HỌC
        // ==========================================================
        // ✅ Danh sách lớp học
        [HttpGet]
        public async Task<IActionResult> LopHoc()
        {
            ViewBag.KhoaHocList = new SelectList(
                await _context.KhoaHocs
                    .Where(k => k.TrangThai == "Đang mở")
                    .ToListAsync(),
                "MaKhoaHoc", "TenKhoaHoc"
            );

            var dsLop = await _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .OrderByDescending(l => l.NgayBatDau)
                .ToListAsync();

            return View(dsLop);
        }


        // ✅ POST – Thêm lớp học
        [HttpPost]
        public IActionResult ThemLopHoc(LopHoc model)
        {
            var khoaHoc = _context.KhoaHocs.FirstOrDefault(x => x.MaKhoaHoc == model.MaKhoaHoc);
            if (khoaHoc == null) return RedirectToAction(nameof(LopHoc));

            // ✅ Lấy dữ liệu tự động từ khóa học
            model.HocPhi = khoaHoc.HocPhi;
            model.NgayBatDau = khoaHoc.NgayBatDau;
            model.NgayKetThuc = khoaHoc.NgayKetThuc;
            model.ThoiLuong = khoaHoc.ThoiLuong;
            model.ThoiLuongTuan = khoaHoc.ThoiLuongTuan;
            model.LichHoc = khoaHoc.LichHoc;   // 👈 THÊM DÒNG NÀY

            model.SiSoHienTai = 0;
            model.TrangThai = "Đang mở";

            _context.LopHocs.Add(model);
            _context.SaveChanges();

            TempData["Success"] = "✅ Tạo lớp học thành công!";
            return RedirectToAction(nameof(LopHoc));
        }




        // ✅ API lấy thông tin khóa học theo ID cho AJAX
        [HttpGet]
        public IActionResult LayKhoaHoc(int id)
        {
            var k = _context.KhoaHocs.FirstOrDefault(x => x.MaKhoaHoc == id);
            if (k == null) return NotFound();

            return Json(new
            {
                hocPhi = k.HocPhi,
                ngayBatDau = k.NgayBatDau?.ToString("yyyy-MM-dd"),
                ngayKetThuc = k.NgayKetThuc?.ToString("yyyy-MM-dd"),
                thoiLuong = k.ThoiLuong,
                thoiLuongTuan = k.ThoiLuongTuan
            });
        }




        // ==========================================================
        // 📅 LỊCH KHAI GIẢNG = DANH SÁCH KHOÁ HỌC
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> LichKhaiGiang(string? q, string? trangThai = "Tất cả")
        {
            ViewBag.CurrentQ = q ?? "";
            ViewBag.CurrentTrangThai = trangThai ?? "Tất cả";

            var query = _context.KhoaHocs
                .Include(k => k.LopHocs)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(k => k.TenKhoaHoc.Contains(q) || (k.CapDo ?? "").Contains(q));
            }

            if (trangThai != "Tất cả")
            {
                query = query.Where(k => k.TrangThai == trangThai);
            }

            var list = await query.OrderBy(k => k.NgayBatDau).ToListAsync();
            return View(list);
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


