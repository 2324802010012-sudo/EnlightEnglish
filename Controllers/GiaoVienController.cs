using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.StaticFiles;

namespace EnlightEnglishCenter.Controllers
{
    public class GiaoVienController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public GiaoVienController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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
            var lopQuery = _context.PhanCongGiangDays
                .Include(p => p.LopHoc)
                    .ThenInclude(l => l.MaKhoaHocNavigation)
                .Include(p => p.GiaoVien)
                    .ThenInclude(gv => gv.NguoiDung)
                .AsNoTracking()
                // ✅ Lọc bỏ những bản ghi chưa có lớp hoặc mã lớp
                .Where(p => p.LopHoc != null && p.LopHoc.MaLop != null);

            // 🔍 Nếu có tìm kiếm theo tên lớp, mã lớp, hoặc tên giảng viên
            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.Trim().ToLower();
                lopQuery = lopQuery.Where(p =>
                    (p.LopHoc.TenLop ?? "").ToLower().Contains(s) ||
                    p.LopHoc.MaLop.ToString().Contains(s) ||
                    (p.GiaoVien.NguoiDung.HoTen ?? "").ToLower().Contains(s)
                );
            }

            var lopDangDay = lopQuery
                .OrderBy(p => p.LopHoc.TenLop)
                .ToList();

            ViewBag.Search = search;
            ViewData["Title"] = "Lớp đang dạy";
            return View(lopDangDay);
        }


        // ------------------ 📖 CHI TIẾT LỚP HỌC ------------------
        [HttpGet]
        public IActionResult ChiTietLop(int id)
        {
            var lop = _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .Include(l => l.LichHocs)
                .Include(l => l.PhanCongGiangDays)
                    .ThenInclude(pc => pc.GiaoVien)
                        .ThenInclude(gv => gv.NguoiDung)
                .AsNoTracking()
                .FirstOrDefault(l => l.MaLop == id);

            if (lop == null)
                return NotFound();

            // ✅ Lấy giảng viên được phân công (nếu có)
            var phanCong = lop.PhanCongGiangDays?.FirstOrDefault();
            string tenGiangVien = phanCong?.GiaoVien?.NguoiDung?.HoTen ?? "Chưa phân công";

            // ✅ Lấy phòng học: nếu có nhiều lịch -> lấy danh sách duy nhất
            string phongHoc = (lop.LichHocs != null && lop.LichHocs.Any())
                ? string.Join(", ", lop.LichHocs
                    .Where(l => !string.IsNullOrEmpty(l.PhongHoc))
                    .Select(l => l.PhongHoc)
                    .Distinct())
                : "Chưa có lịch học";

            // ✅ Gán thông tin cho ViewBag
            ViewBag.TenLop = lop.TenLop ?? "—";
            ViewBag.KhoaHoc = lop.MaKhoaHocNavigation?.TenKhoaHoc ?? "—";
            ViewBag.GiangVien = tenGiangVien;
            ViewBag.PhongHoc = phongHoc;
            ViewBag.NgayBatDau = lop.MaKhoaHocNavigation?.NgayBatDau?.ToString("dd/MM/yyyy") ?? "—";
            ViewBag.TongBuoi = lop.LichHocs?.Count ?? 0;

            ViewData["Title"] = $"Chi tiết lớp - {lop.TenLop}";
            return View(lop);
        }

        // ------------------ 👨‍🎓 DANH SÁCH HỌC VIÊN ------------------
        public IActionResult DanhSachHocVien(int id)
        {
            var lop = _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .FirstOrDefault(l => l.MaLop == id);

            if (lop == null)
            {
                TempData["Error"] = "Không tìm thấy lớp học!";
                return RedirectToAction("LopDangDay");
            }

            var hocViens = _context.DkHocVienLopHocs
                .Include(d => d.MaHocVienNavigation)
                .Where(d => d.MaLop == id)
                .AsNoTracking()
                .ToList();

            ViewBag.TenLop = lop.TenLop;
            ViewBag.MaLop = lop.MaLop;
            ViewBag.KhoaHoc = lop.MaKhoaHocNavigation?.TenKhoaHoc;
            ViewData["Title"] = "Danh sách học viên";

            return View(hocViens);
        }


        // ------------------ 🧮 NHẬP ĐIỂM ------------------
        // =============================
        // 🧮 NHẬP ĐIỂM (CHO TẤT CẢ GIẢNG VIÊN)
        // =============================
        [HttpGet]
        public async Task<IActionResult> NhapDiem(int? maLop)
        {
            // ❌ Bỏ kiểm tra Session (nếu muốn xem tất cả)
            // var giaoVien = HttpContext.Session.GetInt32("MaGiaoVien");
            // if (giaoVien == null) return RedirectToAction("Login", "Account");

            // ✅ Lấy tất cả lớp có trong bảng PhanCongGiangDay
            var dsLop = await _context.PhanCongGiangDays
                .Include(p => p.LopHoc)
                    .ThenInclude(l => l.MaKhoaHocNavigation)
                .Where(p => p.LopHoc != null) // tránh lỗi null
                .Select(p => new
                {
                    MaLop = p.LopHoc!.MaLop,
                    TenLop = string.IsNullOrEmpty(p.LopHoc.TenLop)
                        ? ("Lớp " + p.LopHoc.MaLop)
                        : p.LopHoc.TenLop + " (" + (p.LopHoc.MaKhoaHocNavigation != null
                            ? p.LopHoc.MaKhoaHocNavigation.TenKhoaHoc
                            : "Không rõ khóa") + ")"
                })
                .Distinct()
                .AsNoTracking()
                .ToListAsync();

            ViewBag.DSLopHoc = new SelectList(dsLop, "MaLop", "TenLop", maLop);

            if (maLop == null)
                return View(new List<DiemSo>());

            // ✅ Lấy danh sách học viên trong lớp + điểm (nếu có)
            var ds = await (from dk in _context.DkHocVienLopHocs
                            join nd in _context.NguoiDungs on dk.MaHocVien equals nd.MaNguoiDung
                            join d in _context.DiemSos.Where(x => x.MaLop == maLop)
                                on dk.MaHocVien equals d.MaHocVien into gj
                            from d in gj.DefaultIfEmpty()
                            where dk.MaLop == maLop
                            select new DiemSo
                            {
                                MaDiem = d != null ? d.MaDiem : 0,
                                MaHocVien = dk.MaHocVien,
                                MaLop = dk.MaLop,
                                DiemGiuaKy = d != null ? d.DiemGiuaKy : null,
                                DiemCuoiKy = d != null ? d.DiemCuoiKy : null,
                                NhanXet = d != null ? d.NhanXet : null,
                                HoTen = nd.HoTen,
                            })
                            .AsNoTracking()
                            .ToListAsync();

            ViewBag.MaLop = maLop;
            ViewData["Title"] = "Nhập điểm học viên";
            return View(ds);
        }


        // =============================
        // 💾 LƯU ĐIỂM
        // =============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LuuDiem(int maLop, List<DiemSo> diemList)
        {
            if (diemList == null || diemList.Count == 0)
            {
                TempData["Error"] = "Không có dữ liệu điểm để lưu.";
                return RedirectToAction(nameof(NhapDiem), new { maLop });
            }

            foreach (var item in diemList)
            {
                if (item.MaDiem > 0)
                {
                    var diem = await _context.DiemSos.FirstOrDefaultAsync(d => d.MaDiem == item.MaDiem);
                    if (diem != null)
                    {
                        diem.DiemGiuaKy = item.DiemGiuaKy;
                        diem.DiemCuoiKy = item.DiemCuoiKy;
                        diem.NhanXet = item.NhanXet;
                    }
                }
                else
                {
                    var existed = await _context.DiemSos
                        .FirstOrDefaultAsync(d => d.MaHocVien == item.MaHocVien && d.MaLop == maLop);

                    if (existed != null)
                    {
                        existed.DiemGiuaKy = item.DiemGiuaKy;
                        existed.DiemCuoiKy = item.DiemCuoiKy;
                        existed.NhanXet = item.NhanXet;
                    }
                    else
                    {
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
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "✅ Đã lưu điểm thành công!";
            return RedirectToAction(nameof(NhapDiem), new { maLop });
        }

        // ------------------ 📁 TÀI LIỆU GIẢNG DẠY: DANH SÁCH + TÌM KIẾM ------------------


        // ------------------ ⬆️ UPLOAD TÀI LIỆU ------------------
        [HttpGet]
        [AllowAnonymous] // ⬅️ Cho phép truy cập không cần login
        public IActionResult TaiLieuGiangDay(string? search, int? lopId)
        {
            var maGv = HttpContext.Session.GetInt32("MaGiaoVien");

            // Danh sách lớp để filter (nếu có đăng nhập GV thì chỉ lớp của GV, nếu không thì tất cả lớp)
            IQueryable<LopHoc> lopQuery;
            if (maGv.HasValue)
            {
                lopQuery = _context.PhanCongGiangDays
                            .Include(p => p.LopHoc)
                            .Where(p => p.MaGiaoVien == maGv)
                            .Select(p => p.LopHoc!)
                            .Distinct();
            }
            else
            {
                // Ẩn danh: cho phép chọn tất cả lớp (hoặc bạn có thể chỉ lấy những lớp có tài liệu)
                lopQuery = _context.LopHocs.AsQueryable();
            }

            var lopDangDay = lopQuery
                .Where(l => l != null)
                .OrderBy(l => l.TenLop)
                .ToList();

            ViewBag.LopHocList = new SelectList(lopDangDay, "MaLop", "TenLop");

            // Query tài liệu
            IQueryable<TaiLieu> q = _context.TaiLieus;

            // Nếu có đăng nhập giáo viên thì chỉ xem tài liệu của GV đó
            if (maGv.HasValue)
                q = q.Where(t => t.MaGiaoVien == maGv);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                q = q.Where(t =>
                    (t.TenTaiLieu ?? "").Contains(s) ||
                    (t.MoTa ?? "").Contains(s) ||
                    (t.DuongDan ?? "").Contains(s));
            }

            if (lopId.HasValue)
                q = q.Where(t => t.MaLop == lopId.Value);

            var list = q.OrderByDescending(t => t.NgayTaiLen ?? DateTime.MinValue).ToList();
            return View(list);
        }

        // Upload/Download/Delete vẫn như cũ (bắt buộc login), KHÔNG đổi


        // ------------------ ⬆️ UPLOAD TÀI LIỆU ------------------
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult UploadTaiLieu(string tenTaiLieu, string? moTa, int? maLop, IFormFile file)
        {
            // 1) Lấy MaGiaoVien từ session nếu có
            var maGvSession = HttpContext.Session.GetInt32("MaGiaoVien");

            // 2) Nếu chưa đăng nhập, thử suy ra GV từ lớp được chọn
            int? maGvFromLop = null;
            if (!maGvSession.HasValue && maLop.HasValue)
            {
                maGvFromLop = _context.PhanCongGiangDays
                                      .Where(p => p.MaLop == maLop.Value)
                                      .Select(p => (int?)p.MaGiaoVien)
                                      .FirstOrDefault();
            }

            // 3) Chọn chủ sở hữu tài liệu: ưu tiên session, fallback theo lớp
            var ownerGv = maGvSession ?? maGvFromLop;

            // 4) Nếu vẫn không xác định được GV -> báo lỗi nhẹ nhàng (không redirect Login)
            if (ownerGv == null)
            {
                TempData["Err"] = "Không xác định được giảng viên. Hãy đăng nhập hoặc chọn lớp hợp lệ.";
                return RedirectToAction(nameof(TaiLieuGiangDay));
            }

            // 5) Validate file
            if (file == null || file.Length == 0)
            {
                TempData["Err"] = "Vui lòng chọn file.";
                return RedirectToAction(nameof(TaiLieuGiangDay));
            }

            var choPhep = new[] { ".pdf", ".doc", ".docx", ".ppt", ".pptx", ".xlsx", ".xls" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!choPhep.Contains(ext))
            {
                TempData["Err"] = "Chỉ nhận: PDF, DOC/DOCX, PPT/PPTX, XLS/XLSX.";
                return RedirectToAction(nameof(TaiLieuGiangDay));
            }
            if (file.Length > 20 * 1024 * 1024)
            {
                TempData["Err"] = "File quá lớn. Tối đa 20MB.";
                return RedirectToAction(nameof(TaiLieuGiangDay));
            }

            // 6) Lưu file
            var folder = Path.Combine(_env.WebRootPath, "uploads", "tailieu");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            var safe = Path.GetFileNameWithoutExtension(file.FileName);
            foreach (var c in Path.GetInvalidFileNameChars()) safe = safe.Replace(c, '_');
            var fileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{safe}{ext}";
            var path = Path.Combine(folder, fileName);
            using (var st = System.IO.File.Create(path)) file.CopyTo(st);

            // 7) Lưu DB
            var entity = new TaiLieu
            {
                TenTaiLieu = string.IsNullOrWhiteSpace(tenTaiLieu) ? file.FileName : tenTaiLieu.Trim(),
                MoTa = string.IsNullOrWhiteSpace(moTa) ? null : moTa.Trim(),
                DuongDan = $"/uploads/tailieu/{fileName}",
                MaGiaoVien = ownerGv, // <— dùng GV đã xác định
                MaLop = maLop,
                NgayTaiLen = DateTime.UtcNow
            };

            _context.TaiLieus.Add(entity);
            _context.SaveChanges();

            TempData["Ok"] = "Tải lên thành công.";
            return RedirectToAction(nameof(TaiLieuGiangDay));
        }

        // ------------------ ⬇️ DOWNLOAD ------------------
        // (Tuỳ chọn) Cho phép tải không cần login để tránh bị đá:
        // Nếu muốn giữ ràng buộc theo GV, bỏ [AllowAnonymous] và lọc theo MaGiaoVien như cũ.
        [HttpGet]
        [AllowAnonymous]
        public IActionResult DownloadTaiLieu(int id)
        {
            var tl = _context.TaiLieus.FirstOrDefault(x => x.MaTaiLieu == id);
            if (tl == null || string.IsNullOrEmpty(tl.DuongDan)) return NotFound();

            var physical = Path.Combine(_env.WebRootPath, tl.DuongDan.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (!System.IO.File.Exists(physical)) return NotFound("File không tồn tại.");

            var bytes = System.IO.File.ReadAllBytes(physical);

            // Lấy MIME theo đuôi file
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(physical, out var mime))
                mime = "application/octet-stream";

            return File(bytes, mime, tl.TenTaiLieu ?? Path.GetFileName(physical));
        }

        // ------------------ 🗑️ XÓA ------------------
        // Giữ bảo mật: chỉ GV của tài liệu (trong session) mới được xóa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XoaTaiLieu(int id)
        {
            var maGv = HttpContext.Session.GetInt32("MaGiaoVien");
            if (maGv == null)
            {
                TempData["Err"] = "Bạn cần đăng nhập bằng tài khoản giảng viên để xóa tài liệu.";
                return RedirectToAction(nameof(TaiLieuGiangDay));
            }

            var tl = _context.TaiLieus.FirstOrDefault(x => x.MaTaiLieu == id && x.MaGiaoVien == maGv);
            if (tl == null) return NotFound();

            if (!string.IsNullOrEmpty(tl.DuongDan))
            {
                var physical = Path.Combine(_env.WebRootPath, tl.DuongDan.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(physical)) System.IO.File.Delete(physical);
            }

            _context.TaiLieus.Remove(tl);
            _context.SaveChanges();
            TempData["Ok"] = "Đã xóa tài liệu.";
            return RedirectToAction(nameof(TaiLieuGiangDay));
        }
    }
}