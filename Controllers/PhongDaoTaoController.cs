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
        //public IActionResult DuyetTest()
        //{
        //    var vaiTro = HttpContext.Session.GetString("VaiTro");
        //    if (vaiTro != "Phòng đào tạo")
        //    {
        //        TempData["Error"] = "⚠️ Bạn không có quyền truy cập trang này!";
        //        return RedirectToAction("Index", "Home");
        //    }

        //    var danhSach = _context.TestDauVaos
        //         .Include(t => t.HocVien)
        //        .ToList();

        //    return View(danhSach);
        //}

        //[HttpPost]
        //public IActionResult XacNhan(int id)
        //{
        //    var vaiTro = HttpContext.Session.GetString("VaiTro");

        //    if (vaiTro != "Admin" && vaiTro != "Phòng đào tạo")
        //    {
        //        TempData["Error"] = "⚠️ Bạn không có quyền thực hiện thao tác này!";
        //        return RedirectToAction("Index", "Home");
        //    }

        //    var test = _context.TestDauVaos.FirstOrDefault(t => t.MaTest == id);
        //    if (test == null)
        //        return NotFound();

        //    test.TrangThai = "Được phép test";

        //    _context.SaveChanges();

        //    TempData["Success"] = "✅ Đã xác nhận bài Test của học viên thành công!";

        //    // ✅ Giữ lại ở trang duyệt test của Phòng đào tạo
        //    return RedirectToAction("DuyetTest");
        //}


        // ==========================
        // 🧩 Tạo lịch học tự động
        // ==========================
        [HttpPost]
        public IActionResult TaoLichTuDong(int maLop)
        {
            var lop = _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .FirstOrDefault(l => l.MaLop == maLop);

            if (lop == null)
            {
                TempData["Error"] = "Không tìm thấy lớp học.";
                return RedirectToAction("LopHoc");
            }

            if (lop.NgayBatDau == null || string.IsNullOrEmpty(lop.ThuTrongTuan) || lop.ThoiLuongTuan == null)
            {
                TempData["Error"] = "Vui lòng điền Ngày bắt đầu, Thứ trong tuần và Thời lượng tuần trước khi tạo lịch.";
                return RedirectToAction("LopHoc");
            }

            var gioHoc = (lop.LichHoc ?? "18:00-19:30").Split('-');
            TimeSpan gioBatDau = TimeSpan.Parse(gioHoc[0]);
            TimeSpan gioKetThuc = TimeSpan.Parse(gioHoc[1]);

            var cacThu = lop.ThuTrongTuan.Split(',').Select(t => int.Parse(t.Trim())).ToList();
            var ngayBatDau = lop.NgayBatDau.Value.Date;
            var soTuan = lop.ThoiLuongTuan ?? 8;

            var lichHocMoi = new List<LichHoc>();

            for (int week = 0; week < soTuan; week++)
            {
                foreach (var thu in cacThu)
                {
                    DateTime ngayHoc = ngayBatDau.AddDays((thu - (int)ngayBatDau.DayOfWeek + 7) % 7 + week * 7);

                    // 🔍 kiểm tra trùng lịch cũ
                    bool daCo = _context.LichHocs.Any(l => l.MaLop == maLop && l.NgayHoc == ngayHoc);
                    if (!daCo)
                    {
                        lichHocMoi.Add(new LichHoc
                        {
                            MaLop = maLop,
                            NgayHoc = ngayHoc,
                            GioBatDau = gioBatDau,
                            GioKetThuc = gioKetThuc,
                            PhongHoc = $"Zoom{lop.TenLop}"
                        });
                    }
                }
            }

            if (lichHocMoi.Count > 0)
            {
                _context.LichHocs.AddRange(lichHocMoi);
                _context.SaveChanges();
                TempData["Success"] = $"Đã tạo {lichHocMoi.Count} buổi học cho lớp {lop.TenLop}.";
            }
            else
            {
                TempData["Info"] = "Không có buổi mới nào được tạo (đã tồn tại đủ lịch học).";
            }

            return RedirectToAction("LopHoc");
        }

        // ==========================================================
        // 👩‍🏫 QUẢN LÝ GIẢNG VIÊN
        // ==========================================================

        [HttpGet]
        public async Task<IActionResult> GiaoVien()
        {
            var danhSach = await _context.GiaoViens
                .AsNoTracking()
                .OrderBy(g => g.MaGiaoVien)
                .ToListAsync();

            return View(danhSach);
        }

        [HttpGet]
        public async Task<IActionResult> ThemGiaoVien()
        {
            // 🧠 Lấy danh sách các khóa học đang mở để làm chuyên môn
            var dsKhoaHoc = await _context.KhoaHocs
                .Where(k => k.TrangThai == "Đang mở")
                .Select(k => new { k.MaKhoaHoc, k.TenKhoaHoc })
                .ToListAsync();

            ViewBag.DSKhoaHoc = new SelectList(dsKhoaHoc, "TenKhoaHoc", "TenKhoaHoc");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemGiaoVien(GiaoVien model)
        {
            if (!ModelState.IsValid)
            {
                // Nạp lại danh sách cho dropdown khi validation fail
                var dsKhoaHoc = await _context.KhoaHocs
                    .Where(k => k.TrangThai == "Đang mở")
                    .Select(k => new { k.MaKhoaHoc, k.TenKhoaHoc })
                    .ToListAsync();
                ViewBag.DSKhoaHoc = new SelectList(dsKhoaHoc, "TenKhoaHoc", "TenKhoaHoc");

                return View(model);
            }

            try
            {
                // ✅ Lưu giảng viên trực tiếp (không cần bảng NguoiDung)
                _context.GiaoViens.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "✅ Thêm giảng viên mới thành công!";
                return RedirectToAction(nameof(GiaoVien));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "❌ Có lỗi khi thêm giảng viên: " + ex.Message;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SuaGiaoVien(int id)
        {
            var gv = await _context.GiaoViens.FindAsync(id);
            if (gv == null)
                return NotFound();

            // Load lại danh sách khóa học đang mở
            var dsKhoaHoc = await _context.KhoaHocs
                .Where(k => k.TrangThai == "Đang mở")
                .Select(k => new { k.MaKhoaHoc, k.TenKhoaHoc })
                .ToListAsync();
            ViewBag.DSKhoaHoc = new SelectList(dsKhoaHoc, "TenKhoaHoc", "TenKhoaHoc", gv.ChuyenMon);

            return View(gv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaGiaoVien(GiaoVien model)
        {
            if (!ModelState.IsValid)
            {
                var dsKhoaHoc = await _context.KhoaHocs
                    .Where(k => k.TrangThai == "Đang mở")
                    .Select(k => new { k.MaKhoaHoc, k.TenKhoaHoc })
                    .ToListAsync();
                ViewBag.DSKhoaHoc = new SelectList(dsKhoaHoc, "TenKhoaHoc", "TenKhoaHoc", model.ChuyenMon);
                return View(model);
            }

            _context.Update(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Cập nhật thông tin giảng viên thành công!";
            return RedirectToAction(nameof(GiaoVien));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaGiaoVien(int id)
        {
            var gv = await _context.GiaoViens.FindAsync(id);
            if (gv == null)
            {
                TempData["Error"] = "❌ Không tìm thấy giảng viên cần xóa.";
                return RedirectToAction(nameof(GiaoVien));
            }

            try
            {
                _context.GiaoViens.Remove(gv);
                await _context.SaveChangesAsync();
                TempData["Success"] = "🗑️ Đã xóa giảng viên thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "❌ Lỗi khi xóa giảng viên: " + ex.Message;
            }

            return RedirectToAction(nameof(GiaoVien));
        }

        // ==========================================================
        // 🧑‍🏫 PHÂN CÔNG GIẢNG VIÊN
        // ==========================================================

        [HttpGet]
        public async Task<IActionResult> PhanCong()
        {
            var danhSach = await _context.PhanCongGiangDays
                .Include(p => p.GiaoVien)
                .Include(p => p.LopHoc).ThenInclude(l => l.MaKhoaHocNavigation)
                .OrderByDescending(p => p.NgayPhanCong)
                .AsNoTracking()
                .ToListAsync();

            return View(danhSach);
        }

        // ------------------ 🧩 HÀM DÙNG CHUNG ------------------
        private async Task BuildPhanCongViewBag()
        {
            // ✅ Lấy danh sách giảng viên
            var dsGiaoVien = await _context.GiaoViens
                .Select(g => new
                {
                    g.MaGiaoVien,
                    TenGiaoVien = g.HoTen ?? ("Giảng viên #" + g.MaGiaoVien)
                })
                .OrderBy(g => g.TenGiaoVien)
                .ToListAsync();

            // ✅ Lấy danh sách lớp học chưa được phân công
            var dsLop = await _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .Where(l => !_context.PhanCongGiangDays.Any(p => p.MaLop == l.MaLop))
                .Select(l => new
                {
                    l.MaLop,
                    Ten = (l.TenLop ?? "Lớp " + l.MaLop) + " - " + (l.MaKhoaHocNavigation.TenKhoaHoc ?? "")
                })
                .OrderBy(l => l.Ten)
                .ToListAsync();

            // ✅ Gán dữ liệu dropdown cho View
            ViewBag.GiaoVien = new SelectList(dsGiaoVien, "MaGiaoVien", "TenGiaoVien");
            ViewBag.LopHoc = new SelectList(dsLop, "MaLop", "Ten");
        }

        // ------------------ ➕ THÊM PHÂN CÔNG (GET) ------------------
        [HttpGet]
        public async Task<IActionResult> ThemPhanCong()
        {
            await BuildPhanCongViewBag();
            return View();
        }

        // ------------------ 💾 THÊM PHÂN CÔNG (POST) ------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemPhanCong(PhanCongGiangDay model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "⚠️ Vui lòng chọn đầy đủ thông tin!";
                await BuildPhanCongViewBag();
                return View(model);
            }

            try
            {
                // 🔍 Kiểm tra trùng giảng viên - lớp
                bool tonTai = await _context.PhanCongGiangDays
                    .AnyAsync(p => p.MaGiaoVien == model.MaGiaoVien && p.MaLop == model.MaLop);

                if (tonTai)
                {
                    TempData["Error"] = "⚠️ Giảng viên này đã được phân công cho lớp học này!";
                    await BuildPhanCongViewBag();
                    return View(model);
                }

                // ✅ Gán ngày phân công
                model.NgayPhanCong = DateTime.Now;
                _context.PhanCongGiangDays.Add(model);
                await _context.SaveChangesAsync();

                // ✅ Cập nhật giảng viên cho lớp học
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
            catch (Exception ex)
            {
                TempData["Error"] = "❌ Có lỗi xảy ra: " + ex.Message;
                await BuildPhanCongViewBag();
                return View(model);
            }
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
        // ✏️ SỬA BUỔI HỌC
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> SuaLichHoc(int id)
        {
            var lich = await _context.LichHocs
                .Include(l => l.LopHoc)
                .ThenInclude(x => x.MaKhoaHocNavigation)
                .FirstOrDefaultAsync(l => l.MaLich == id);

            if (lich == null)
                return NotFound();

            ViewBag.DSLopHoc = new SelectList(
                await _context.LopHocs
                    .Include(l => l.MaKhoaHocNavigation)
                    .Select(l => new
                    {
                        l.MaLop,
                        Ten = l.TenLop + " (" + (l.MaKhoaHocNavigation.TenKhoaHoc ?? "") + ")"
                    })
                    .ToListAsync(),
                "MaLop", "Ten", lich.MaLop
            );

            return View(lich);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaLichHoc(LichHoc model)
        {
            if (!ModelState.IsValid)
            {
                await SuaLichHoc(model.MaLich);
                return View(model);
            }

            var lich = await _context.LichHocs.FindAsync(model.MaLich);
            if (lich == null)
                return NotFound();

            lich.MaLop = model.MaLop;
            lich.NgayHoc = model.NgayHoc;
            lich.GioBatDau = model.GioBatDau;
            lich.GioKetThuc = model.GioKetThuc;
            lich.PhongHoc = model.PhongHoc;

            await _context.SaveChangesAsync();
            TempData["Success"] = "✅ Cập nhật lịch học thành công!";
            return RedirectToAction(nameof(LichHoc));
        }
        // ==========================================================
        // 🗑️ XÓA BUỔI HỌC
        // ==========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaLichHoc(int id)
        {
            var lich = await _context.LichHocs.FindAsync(id);
            if (lich == null)
            {
                TempData["Error"] = "❌ Không tìm thấy buổi học cần xóa.";
                return RedirectToAction(nameof(LichHoc));
            }

            _context.LichHocs.Remove(lich);
            await _context.SaveChangesAsync();

            TempData["Success"] = "🗑️ Đã xóa buổi học thành công.";
            return RedirectToAction(nameof(LichHoc));
        }


        // ==========================================================
        // 🧾 KHOÁ HỌC
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> KhoaHoc()
        {
            var data = await _context.KhoaHocs
                .Where(k => k.LoaiKhoaHoc == "ChinhThuc" || k.LoaiKhoaHoc == null)
                .OrderByDescending(x => x.NgayBatDau)
                .AsNoTracking()
                .ToListAsync();

            return View(data);
        }
        public async Task<IActionResult> KhoaHocTest()
        {
            var data = await _context.KhoaHocs
                .Where(k => k.LoaiKhoaHoc == "Test")
                .OrderBy(x => x.TenKhoaHoc)
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
            Console.WriteLine($"🧩 Debug: CapDo nhận = {model.CapDo}");
            if (!ModelState.IsValid)
            {
                BuildKhoaMauViewBag();
                return View(model);
            }

            // ✅ Phân loại khóa học
            if (model.TenKhoaHoc.ToLower().Contains("test"))
            {
                model.LoaiKhoaHoc = "Test";
                model.CapDo = "Bài test";
                model.HocPhi = 0;
                model.ThoiLuongTuan = 2;
                model.LichHoc = "246";
            }
            else
            {
                model.LoaiKhoaHoc = "ChinhThuc";
                // ✅ Xác định cấp độ tự động dựa theo tên khóa học
                if (string.IsNullOrEmpty(model.CapDo))
                {
                    if (!string.IsNullOrEmpty(model.TenKhoaHoc))
                    {
                        var ten = model.TenKhoaHoc.ToLower();

                        if (ten.Contains("foundation"))
                            model.CapDo = "Cơ bản";
                        else if (ten.Contains("intermediate"))
                            model.CapDo = "Trung bình";
                        else if (ten.Contains("advanced"))
                            model.CapDo = "Nâng cao";
                        else
                            model.CapDo = "Không xác định";
                    }
                }

                // ✅ Gán học phí tự động nếu chưa nhập
                if (model.HocPhi == null || model.HocPhi == 0)
                {
                    switch (model.CapDo)
                    {
                        case "Cơ bản":
                            model.HocPhi = 3500000;
                            break;
                        case "Trung bình":
                            model.HocPhi = 4500000;
                            break;
                        case "Nâng cao":
                            model.HocPhi = 5500000;
                            break;
                        default:
                            model.HocPhi = 3000000;
                            break;
                    }
                }

                // ✅ Gán thời lượng tuần theo cấp độ nếu chưa có
                if (model.ThoiLuongTuan == 0)
                {
                    if (model.CapDo == "Cơ bản")
                        model.ThoiLuongTuan = 8;
                    else if (model.CapDo == "Trung bình")
                        model.ThoiLuongTuan = 10;
                    else if (model.CapDo == "Nâng cao")
                        model.ThoiLuongTuan = 12;
                    else
                        model.ThoiLuongTuan = 8;
                }

                // ✅ Gán lịch học mặc định
                if (string.IsNullOrEmpty(model.LichHoc))
                {
                    if (model.CapDo == "Cơ bản")
                        model.LichHoc = "246";  // Thứ 2-4-6
                    else if (model.CapDo == "Trung bình")
                        model.LichHoc = "357";  // Thứ 3-5-7
                    else
                        model.LichHoc = "246";
                }
            }

            // ✅ Tính ngày kết thúc tự động
            if (model.NgayBatDau.HasValue && model.ThoiLuongTuan > 0)
                model.NgayKetThuc = model.NgayBatDau.Value.AddDays(model.ThoiLuongTuan * 7);

            // ✅ Trạng thái mặc định
            model.TrangThai = "Đang mở";

            // ✅ Lưu vào DB
            _context.KhoaHocs.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"✅ Tạo khóa học '{model.TenKhoaHoc}' ({model.CapDo}) thành công!";
            return RedirectToAction(nameof(KhoaHoc));
        }

        private void BuildKhoaMauViewBag()
        {
            ViewBag.KhoaMau = new List<dynamic>
    {
        // 🎯 Khóa test đầu vào
        new { Ten="IELTS Test", CapDo="Bài test", HocPhi=0},
        new { Ten="TOEIC Test", CapDo="Bài test", HocPhi=0 },
        new { Ten="Cambridge Test", CapDo="Bài test", HocPhi=0 },

        // 🎓 Khóa chính thức
        new { Ten="IELTS Foundation", CapDo="Cơ bản", HocPhi=3500000, Tuan=8 },
        new { Ten="IELTS Intermediate", CapDo="Trung bình", HocPhi=4500000, Tuan=10 },
        new { Ten="IELTS Advanced", CapDo="Nâng cao", HocPhi=5500000, Tuan=12 },

        new { Ten="TOEIC Foundation", CapDo="Cơ bản", HocPhi=2500000, Tuan=6 },
        new { Ten="TOEIC Intermediate", CapDo="Trung bình", HocPhi=3000000, Tuan=8 },
        new { Ten="TOEIC Advanced", CapDo="Nâng cao", HocPhi=3800000, Tuan=10 },

        new { Ten="Cambridge Starters", CapDo="Cơ bản", HocPhi=3000000, Tuan=10 },
        new { Ten="Cambridge Movers", CapDo="Trung bình", HocPhi=3800000, Tuan=12 },
        new { Ten="Cambridge Flyers", CapDo="Nâng cao", HocPhi=4500000, Tuan=14 }
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
                .Include(l => l.PhanCongGiangDays)          // 🔹 Lấy thông tin phân công
                    .ThenInclude(p => p.GiaoVien)           // 🔹 Lấy luôn thông tin giảng viên
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
            model.LichHoc = model.LichHoc ?? khoaHoc.LichHoc; // 👈 ưu tiên người nhập
            model.SiSoHienTai = 0;
            model.TrangThai = "Đang mở";

            // ⚠️ Đảm bảo không null
            if (string.IsNullOrEmpty(model.ThuTrongTuan))
                model.ThuTrongTuan = "2,4,6"; // mặc định nếu quên nhập

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

            // 🔍 Tìm kiếm theo tên hoặc cấp độ
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(k => k.TenKhoaHoc.Contains(q) || (k.CapDo ?? "").Contains(q));
            }

            // 🔎 Lọc theo trạng thái
            if (trangThai != "Tất cả")
            {
                query = query.Where(k => k.TrangThai == trangThai);
            }

            // 🧩 Sắp xếp theo ngày bắt đầu
            query = query.OrderBy(k => k.NgayBatDau);

            // ✅ Nếu KHÔNG tìm kiếm và trạng thái là mặc định, chỉ lấy 3 khóa học
            if (string.IsNullOrWhiteSpace(q) && (trangThai == "Tất cả"))
            {
                query = query.Take(3);
            }

            var list = await query.ToListAsync();
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


