using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
            var maGv = HttpContext.Session.GetInt32("MaGiaoVien");
            if (maGv == null) return RedirectToAction("Login", "Account");

            var lopQuery = _context.PhanCongGiangDays
                .Include(p => p.LopHoc)
                    .ThenInclude(l => l.MaKhoaHocNavigation)
                .Where(p => p.MaGiaoVien == maGv)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                lopQuery = lopQuery.Where(p =>
                    (p.LopHoc.TenLop ?? "").Contains(search) ||
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
            if (maGv == null) return RedirectToAction("Login", "Account");

            var lop = _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .Include(l => l.LichHocs)
                .Include(l => l.PhanCongGiangDays)
                    .ThenInclude(pc => pc.GiaoVien)
                        .ThenInclude(gv => gv.NguoiDung)
                .AsNoTracking()
                .FirstOrDefault(l => l.MaLop == id);

            if (lop == null) return NotFound();

            var phanCong = lop.PhanCongGiangDays?.FirstOrDefault();
            var giangVien = phanCong?.GiaoVien?.NguoiDung?.HoTen ?? "Chưa phân công";

            ViewBag.GiangVien = giangVien;
            ViewBag.TenLop = lop.TenLop ?? "—";
            ViewBag.KhoaHoc = lop.MaKhoaHocNavigation?.TenKhoaHoc ?? "—";
            ViewBag.NgayBatDau = lop.MaKhoaHocNavigation?.NgayBatDau?.ToString("dd/MM/yyyy") ?? "—"; // ✅ lấy từ KhoaHoc
            ViewBag.PhongHoc = lop.LichHocs?.FirstOrDefault()?.PhongHoc ?? "Chưa sắp lịch";
            ViewBag.TongBuoi = lop.LichHocs?.Count ?? 0;

            ViewData["Title"] = "Chi tiết lớp học";
            return View(lop);
        }

        // ------------------ 👨‍🎓 DANH SÁCH HỌC VIÊN ------------------
        public IActionResult DanhSachHocVien(int id)
        {
            var hocViens = _context.DkHocVienLopHocs
                .Include(d => d.MaHocVienNavigation) // Navigation tới NguoiDung (đúng theo schema)
                .Where(d => d.MaLop == id)
                .AsNoTracking()
                .ToList();

            ViewBag.MaLop = id;
            ViewData["Title"] = "Danh sách học viên";
            return View(hocViens);
        }

        // ------------------ 🧮 NHẬP ĐIỂM ------------------
        // GET: Nhập điểm
        [HttpGet]
        public async Task<IActionResult> NhapDiem(int? maLop)
        {
            var giaoVien = HttpContext.Session.GetInt32("MaGiaoVien");
            if (giaoVien == null) return RedirectToAction("Login", "Account");

            // Dropdown lớp GV đang dạy
            var dsLop = await _context.PhanCongGiangDays
                .Include(p => p.LopHoc)
                .Where(p => p.MaGiaoVien == giaoVien)
                .Select(p => new { p.LopHoc.MaLop, TenLop = p.LopHoc.TenLop ?? ("Lớp " + p.LopHoc.MaLop) })
                .Distinct()
                .AsNoTracking()
                .ToListAsync();
            ViewBag.DSLopHoc = new SelectList(dsLop, "MaLop", "TenLop", maLop);

            if (maLop == null) return View(new List<DiemSo>());

            // Lấy danh sách học viên trong lớp + ghép điểm (nếu đã có)
            var ds = await (from dk in _context.DkHocVienLopHocs
                            join nd in _context.NguoiDungs on dk.MaHocVien equals nd.MaNguoiDung
                            join d in _context.DiemSos.Where(x => x.MaLop == maLop) on dk.MaHocVien equals d.MaHocVien into gj
                            from d in gj.DefaultIfEmpty()
                            where dk.MaLop == maLop
                            select new DiemSo
                            {
                                MaDiem = d != null ? d.MaDiem : 0,       // 0 = chưa có bản ghi
                                MaHocVien = dk.MaHocVien,
                                MaLop = dk.MaLop,
                                DiemGiuaKy = d!.DiemGiuaKy,
                                DiemCuoiKy = d!.DiemCuoiKy,
                                NhanXet = d!.NhanXet,
                                HoTen = nd.HoTen,                        // NotMapped – để hiển thị
                                MaHocVienNavigation = nd,
                            })
                            .AsNoTracking()
                            .ToListAsync();

            ViewBag.MaLop = maLop;
            ViewData["Title"] = "Nhập điểm học viên";
            return View(ds);
        }

        // POST: Lưu điểm
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
                // Nếu đã có MaDiem > 0 -> cập nhật theo khóa chính
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
                    // Chưa có -> phòng ngừa kiểm tra lại theo (MaHocVien, MaLop)
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
        [HttpGet]
        public IActionResult TaiLieuGiangDay(string? search, int? lopId)
        {
            var maGv = HttpContext.Session.GetInt32("MaGiaoVien");
            if (maGv == null) return RedirectToAction("Login", "Account");

            var lopDangDay = _context.PhanCongGiangDays
                .Include(p => p.LopHoc)
                .Where(p => p.MaGiaoVien == maGv)
                .Select(p => p.LopHoc)
                .Distinct()
                .OrderBy(l => l.TenLop)
                .ToList();

            ViewBag.LopHocList = new SelectList(lopDangDay, "MaLop", "TenLop");

            var q = _context.TaiLieus.Where(t => t.MaGiaoVien == maGv);

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

        // ------------------ ⬆️ UPLOAD TÀI LIỆU ------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UploadTaiLieu(string tenTaiLieu, string? moTa, int? maLop, IFormFile file)
        {
            var maGv = HttpContext.Session.GetInt32("MaGiaoVien");
            if (maGv == null) return RedirectToAction("Login", "Account");

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

            var folder = Path.Combine(_env.WebRootPath, "uploads", "tailieu");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            var safe = Path.GetFileNameWithoutExtension(file.FileName);
            foreach (var c in Path.GetInvalidFileNameChars()) safe = safe.Replace(c, '_');
            var fileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{safe}{ext}";
            var path = Path.Combine(folder, fileName);
            using (var st = System.IO.File.Create(path)) file.CopyTo(st);

            var entity = new TaiLieu
            {
                TenTaiLieu = string.IsNullOrWhiteSpace(tenTaiLieu) ? file.FileName : tenTaiLieu.Trim(),
                MoTa = string.IsNullOrWhiteSpace(moTa) ? null : moTa.Trim(),
                DuongDan = $"/uploads/tailieu/{fileName}",
                MaGiaoVien = maGv,
                MaLop = maLop,
                NgayTaiLen = DateTime.UtcNow
            };

            _context.TaiLieus.Add(entity);
            _context.SaveChanges();
            TempData["Ok"] = "Tải lên thành công.";
            return RedirectToAction(nameof(TaiLieuGiangDay));
        }

        // ------------------ ⬇️ DOWNLOAD ------------------
        [HttpGet]
        public IActionResult DownloadTaiLieu(int id)
        {
            var maGv = HttpContext.Session.GetInt32("MaGiaoVien");
            if (maGv == null) return RedirectToAction("Login", "Account");

            var tl = _context.TaiLieus.FirstOrDefault(x => x.MaTaiLieu == id && x.MaGiaoVien == maGv);
            if (tl == null || string.IsNullOrEmpty(tl.DuongDan)) return NotFound();

            var physical = Path.Combine(_env.WebRootPath, tl.DuongDan.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (!System.IO.File.Exists(physical)) return NotFound("File không tồn tại.");

            var bytes = System.IO.File.ReadAllBytes(physical);
            var mime = GetMimeFromExt(Path.GetExtension(physical));
            return File(bytes, mime, tl.TenTaiLieu ?? Path.GetFileName(physical));
        }

        private static string GetMimeFromExt(string? ext)
        {
            ext = (ext ?? "").ToLowerInvariant();
            return ext switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".ppt" => "application/vnd.ms-powerpoint",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };
        }


        // ------------------ 🗑️ XÓA ------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XoaTaiLieu(int id)
        {
            var maGv = HttpContext.Session.GetInt32("MaGiaoVien");
            if (maGv == null) return RedirectToAction("Login", "Account");

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