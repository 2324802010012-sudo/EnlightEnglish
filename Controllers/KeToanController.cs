using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using EnlightEnglishCenter.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EnlightEnglishCenter.Controllers
{
    public class KeToanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KeToanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================================
        // 🏠 TRANG CHÍNH
        // ==========================================================
       
        // =========== HỌC PHÍ ===========
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var first = new DateTime(today.Year, today.Month, 1);
            var next = first.AddMonths(1);

            ViewBag.PendingCount = await _context.DonHocPhis
                .CountAsync(d => d.TrangThai != "Đã thanh toán");

            ViewBag.MonthPaidCount = await _context.DonHocPhis
                .CountAsync(d => d.TrangThai == "Đã thanh toán"
                              && d.NgayThanhToan >= first
                              && d.NgayThanhToan < next);

            ViewBag.MonthRevenue = await _context.DonHocPhis
                .Where(d => d.TrangThai == "Đã thanh toán"
                         && d.NgayThanhToan >= first
                         && d.NgayThanhToan < next)
                .SumAsync(d => (decimal?)d.TongTien) ?? 0m;

            return View(); // trả về Views/KeToan/Index.cshtml
        }

        [HttpGet]
        public async Task<IActionResult> HocPhi([FromQuery] FinanceFilterVm f)
        {
            var q = _context.DonHocPhis
                .AsNoTracking()
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc).ThenInclude(l => l.MaKhoaHocNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(f.Keyword))
            {
                var kw = f.Keyword.Trim();
                q = q.Where(d =>
                    d.MaDon.ToString().Contains(kw) ||
                    EF.Functions.Like(d.HocVien!.HoTen ?? "", $"%{kw}%") ||
                    EF.Functions.Like(d.LopHoc!.TenLop ?? "", $"%{kw}%") ||
                    EF.Functions.Like(d.LopHoc!.MaKhoaHocNavigation!.TenKhoaHoc ?? "", $"%{kw}%"));
            }

            if (f.From.HasValue) q = q.Where(d => d.NgayTao >= f.From.Value.Date);
            if (f.To.HasValue) q = q.Where(d => d.NgayTao < f.To.Value.Date.AddDays(1));

            if (string.Equals(f.TrangThai, "ChoXacNhan", StringComparison.OrdinalIgnoreCase))
                q = q.Where(d => d.TrangThai != "Đã thanh toán");
            else if (string.Equals(f.TrangThai, "DaThanhToan", StringComparison.OrdinalIgnoreCase))
                q = q.Where(d => d.TrangThai == "Đã thanh toán");

            var items = await q.OrderByDescending(d => d.NgayTao).ToListAsync();
            return View(new FeeListVm { Items = items, Filter = f, Total = items.Count });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanHocPhi(int id)
        {
            var don = await _context.DonHocPhis
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc).ThenInclude(l => l.MaKhoaHocNavigation)
                .FirstOrDefaultAsync(d => d.MaDon == id);

            if (don == null)
            {
                TempData["Error"] = "Không tìm thấy đơn học phí!";
                return RedirectToAction(nameof(HocPhi));
            }
            if (don.TrangThai == "Đã thanh toán")
            {
                TempData["Success"] = "Đơn này đã xác nhận.";
                return RedirectToAction(nameof(HocPhi));
            }

            using var tx = await _context.Database.BeginTransactionAsync();

            // cập nhật đơn
            don.TrangThai = "Đã thanh toán";
            don.NgayThanhToan = DateTime.Now;

            // cập nhật sĩ số lớp (nếu có)
            var lop = await _context.LopHocs.FirstOrDefaultAsync(l => l.MaLop == don.MaLop);
            if (lop != null)
            {
                var cur = lop.SiSoHienTai ?? 0;
                var max = lop.SiSoToiDa ?? int.MaxValue;
                if (cur < max) lop.SiSoHienTai = cur + 1;
            }

            // thêm đăng ký lớp nếu chưa có


            // ghi báo cáo (DbSet tên BaoCaos – KHÔNG phải BaoCaoes)
            _context.BaoCaos.Add(new BaoCao
            {
                LoaiBaoCao = "Doanh thu học phí",
                NoiDung = $"HV {don.HocVien?.HoTen} thanh toán {don.TongTien:N0} đ cho lớp {don.LopHoc?.TenLop} ({don.LopHoc?.MaKhoaHocNavigation?.TenKhoaHoc}).",
                NguoiLap = HttpContext.Session.GetInt32("MaNguoiDung"),
                NgayLap = DateTime.Now
            });

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            TempData["Success"] = "Đã xác nhận thanh toán.";
            return RedirectToAction(nameof(HocPhi));
        }
        // ===============================
        // 📊 BÁO CÁO TÀI CHÍNH - KẾ TOÁN
        // ===============================
        public IActionResult BaoCaoTaiChinh()
        {
            // === 1️⃣ Học phí thu từ lễ tân ===
            var tongHocPhi = _context.HocPhis
                .Where(h => h.TrangThai == "Đã thanh toán")
                .Sum(h => (decimal?)h.SoTienPhaiDong) ?? 0;

            var soHocVienDaDong = _context.HocPhis
                .Count(h => h.TrangThai == "Đã thanh toán");

            // === 2️⃣ Lương giáo viên ===
            var tongLuong = _context.LuongGiaoViens
                .Sum(l => (decimal?)l.TongLuong) ?? 0;

            var soGiaoVien = _context.LuongGiaoViens
                .Select(l => l.MaGiaoVien)
                .Distinct()
                .Count();

            // === 3️⃣ Lợi nhuận = Doanh thu - Chi phí lương ===
            var loiNhuan = tongHocPhi - tongLuong;

            // === 4️⃣ Trả về model động ===
            ViewBag.TongHocPhi = tongHocPhi;
            ViewBag.SoHocVienDaDong = soHocVienDaDong;
            ViewBag.TongLuong = tongLuong;
            ViewBag.SoGiaoVien = soGiaoVien;
            ViewBag.LoiNhuan = loiNhuan;

            // === 5️⃣ Danh sách chi tiết gần đây (tùy chọn) ===
            ViewBag.DanhSachHocPhi = _context.HocPhis
                .OrderByDescending(x => x.NgayDongCuoi)
                .Take(5)
                .ToList();

            ViewBag.DanhSachLuong = _context.LuongGiaoViens
                .OrderByDescending(x => x.Thang)
                .ThenByDescending(x => x.Nam)
                .Take(5)
                .ToList();

            return View();
        }


        // ==========================================================
        // 🧾 DANH SÁCH LƯƠNG GIÁO VIÊN (CÓ LỌC THEO TÊN, THÁNG, NĂM)
        // ==========================================================
        public IActionResult LuongGiaoVien(string? keyword, int? thang, int? nam)
        {
            // ✅ Truy vấn dữ liệu lương
            var luongQuery = from l in _context.LuongGiaoViens
                             join gv in _context.GiaoViens on l.MaGiaoVien equals gv.MaGiaoVien
                             select new
                             {
                                 l.MaLuong,
                                 GiaoVien = gv.HoTen,
                                 l.Thang,
                                 l.Nam,
                                 l.SoBuoiDay,
                                 l.LuongMoiBuoi,
                                 l.TongLuong,
                                 l.TrangThai
                             };

            // ✅ Lọc theo tên
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.Trim().ToLower();
                luongQuery = luongQuery.Where(x => x.GiaoVien.ToLower().Contains(keyword));
                ViewBag.FilterName = keyword;
            }

            // ✅ Lọc theo tháng
            if (thang.HasValue && thang.Value > 0)
            {
                luongQuery = luongQuery.Where(x => x.Thang == thang.Value);
                ViewBag.FilterThang = thang.Value;
            }

            // ✅ Lọc theo năm
            if (nam.HasValue && nam.Value > 0)
            {
                luongQuery = luongQuery.Where(x => x.Nam == nam.Value);
                ViewBag.FilterNam = nam.Value;
            }

            // ✅ Lấy danh sách
            var luongList = luongQuery
                .OrderByDescending(x => x.Nam)
                .ThenByDescending(x => x.Thang)
                .ToList();

            // ✅ Tính tổng lương
            decimal tongLuong = luongList.Sum(x => x.TongLuong ?? 0);
            ViewBag.TongLuong = tongLuong;

            // ✅ Dữ liệu cho dropdown
            ViewBag.ThangList = Enumerable.Range(1, 12).ToList();
            ViewBag.NamList = _context.LuongGiaoViens
                .Select(x => x.Nam)
                .Distinct()
                .OrderByDescending(x => x)
                .ToList();

            return View(luongList);
        }
        // ==========================================================
        // 💰 DOANH THU TRUNG TÂM (LẤY TỪ BẢNG HỌC PHÍ)
        // ==========================================================
        public IActionResult DoanhThu(string? keyword, string? trangThai)
        {
            // 🔹 Lấy danh sách học phí
            var hocPhiQuery = from hp in _context.HocPhis
                              join hv in _context.HocViens on hp.MaHocVien equals hv.MaHocVien
                              join nd in _context.NguoiDungs on hv.MaNguoiDung equals nd.MaNguoiDung
                              join lop in _context.LopHocs on hp.MaLop equals lop.MaLop
                              select new
                              {
                                  HocVien = nd.HoTen,
                                  Lop = lop.TenLop,
                                  hp.SoTienPhaiDong,
                                  hp.SoTienDaDong,
                                  hp.TrangThai,
                                  hp.NgayDongCuoi
                              };

            // 🔎 Tìm kiếm học viên hoặc lớp
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.Trim().ToLower();
                hocPhiQuery = hocPhiQuery.Where(x =>
                    x.HocVien.ToLower().Contains(keyword) ||
                    x.Lop.ToLower().Contains(keyword));
                ViewBag.Keyword = keyword;
            }

            // 🔎 Lọc theo trạng thái
            if (!string.IsNullOrEmpty(trangThai))
            {
                hocPhiQuery = hocPhiQuery.Where(x => x.TrangThai == trangThai);
                ViewBag.TrangThai = trangThai;
            }

            var list = hocPhiQuery.ToList();

            // ✅ Tổng hợp
            var tongPhaiDong = list.Sum(x => x.SoTienPhaiDong);
            var tongDaDong = list.Sum(x => x.SoTienDaDong);
            var tongConNo = tongPhaiDong - tongDaDong;


            ViewBag.TongPhaiDong = tongPhaiDong;
            ViewBag.TongDaDong = tongDaDong;
            ViewBag.TongConNo = tongConNo;

            return View(list);
        }


        [HttpGet]
        public async Task<IActionResult> ChiTietDonHocPhi(int id)
        {
            var don = await _context.DonHocPhis
                .AsNoTracking()
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc).ThenInclude(l => l.MaKhoaHocNavigation)
                .FirstOrDefaultAsync(d => d.MaDon == id);

            if (don == null)
            {
                TempData["Error"] = $"Không tìm thấy đơn {id}.";
                return RedirectToAction(nameof(HocPhi));
            }

            var vm = new FeeDetailVm
            {
                MaDon = don.MaDon.ToString(),
                NgayTao = don.NgayTao,
                NgayThanhToan = don.NgayThanhToan,
                TrangThai = don.TrangThai ?? "Chờ xác nhận",

                HocVien = new FeeStudentVm
                {
                    HoTen = don.HocVien?.HoTen ?? "(Không rõ)",
                    Email = don.HocVien?.Email,
                    SoDienThoai = don.HocVien?.SoDienThoai,
                    // ✅ MaHocVien là int => KHÔNG dùng "?."
                    MaHocVien = don.HocVien != null ? don.HocVien.MaHocVien.ToString() : null
                },
                LopHoc = new FeeClassVm
                {
                    TenLop = don.LopHoc?.TenLop,
                    TenKhoaHoc = don.LopHoc?.MaKhoaHocNavigation?.TenKhoaHoc,
                    // ✅ DB không có CaHoc → dùng LichHoc (hoặc ThuTrongTuan)
                    CaHoc = don.LopHoc?.LichHoc
                },
                // ✅ DB không có bảng chi tiết khoản phí → tạo 1 dòng mặc định
                Items = new List<FeeItemVm>
        {
            new FeeItemVm {
                NoiDung = $"Học phí lớp {don.LopHoc?.TenLop} ({don.LopHoc?.MaKhoaHocNavigation?.TenKhoaHoc})",
                SoLuong = 1,
                DonGia = don.TongTien
            }
        },
                GiamTru = 0,
                GhiChu = null,
                NhanVienLap = null
            };

            // ✅ Không có bảng BiênLai → nếu đã thanh toán thì tạo 1 payment từ NgayThanhToan
            if (don.NgayThanhToan.HasValue)
            {
                vm.Payments.Add(new FeePaymentVm
                {
                    ThoiGian = don.NgayThanhToan.Value,
                    SoTien = don.TongTien,
                    HinhThuc = "Thanh toán 1 lần",
                    GhiChu = null
                });
            }

            ViewData["Title"] = $"Đơn học phí #{vm.MaDon}";
            return View(vm);
        }
        // GET: /KeToan/TaoDonHocPhi
        [HttpGet]
        public IActionResult TaoDonHocPhi()
        {
            // TODO: trả về form tạo đơn
            return View();
        }

        // POST: /KeToan/TaoDonHocPhi
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoDonHocPhi(DonHocPhi model)
        {
            if (!ModelState.IsValid) return View(model);
            model.NgayTao = DateTime.Now;
            model.TrangThai = "Chờ xác nhận";
            _context.DonHocPhis.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã thêm đơn học phí.";
            return RedirectToAction(nameof(HocPhi));
        }

        // GET: /KeToan/SuaDonHocPhi/{id}
        [HttpGet]
        public async Task<IActionResult> SuaDonHocPhi(int id)
        {
            var don = await _context.DonHocPhis.FindAsync(id);
            if (don == null) { TempData["Error"] = "Không tìm thấy đơn."; return RedirectToAction(nameof(HocPhi)); }
            if (string.Equals(don.TrangThai, "Đã thanh toán", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Đơn đã thanh toán, không thể sửa.";
                return RedirectToAction(nameof(HocPhi));
            }
            return View(don);
        }

        // POST: /KeToan/SuaDonHocPhi/{id}
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaDonHocPhi(int id, DonHocPhi model)
        {
            if (id != model.MaDon) { return BadRequest(); }
            var don = await _context.DonHocPhis.FindAsync(id);
            if (don == null) { TempData["Error"] = "Không tìm thấy đơn."; return RedirectToAction(nameof(HocPhi)); }
            if (string.Equals(don.TrangThai, "Đã thanh toán", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Đơn đã thanh toán, không thể sửa.";
                return RedirectToAction(nameof(HocPhi));
            }

            // TODO: cập nhật các trường cho phép sửa (ví dụ TongTien, MaLop, ghi chú…)
            don.TongTien = model.TongTien;
            don.MaLop = model.MaLop;
           

            _context.Update(don);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã cập nhật đơn học phí.";
            return RedirectToAction(nameof(HocPhi));
        }

        // POST: /KeToan/XoaDonHocPhi
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaDonHocPhi(int id)
        {
            var don = await _context.DonHocPhis.FindAsync(id);
            if (don == null) { TempData["Error"] = "Không tìm thấy đơn."; return RedirectToAction(nameof(HocPhi)); }
            if (string.Equals(don.TrangThai, "Đã thanh toán", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Đơn đã thanh toán, không thể xóa.";
                return RedirectToAction(nameof(HocPhi));
            }

            _context.DonHocPhis.Remove(don);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã xóa đơn học phí.";
            return RedirectToAction(nameof(HocPhi));
        }

        // ==========================================================
        // ⚙️ TRANG TÍNH LƯƠNG TỰ ĐỘNG
        // ==========================================================
        public IActionResult TinhLuong()
        {
            return View();
        }

        // ==========================================================
        // 💰 TÍNH LƯƠNG TOÀN BỘ GIÁO VIÊN (POST)
        // ==========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TinhLuongAll()
        {
            int tongBanGhiMoi = 0;
            int tongBanGhiCapNhat = 0;

            var giaoVienList = _context.GiaoViens
                .Where(gv => _context.PhanCongGiangDays.Any(pc => pc.MaGiaoVien == gv.MaGiaoVien))
                .ToList();

            foreach (var gv in giaoVienList)
            {
                // ✅ Nếu chưa có lương mỗi buổi, set mặc định = 30.000 và lưu ngay
                if (gv.LuongMoiBuoi == null || gv.LuongMoiBuoi == 0)
                {
                    gv.LuongMoiBuoi = 30000;
                    _context.GiaoViens.Update(gv);
                    _context.SaveChanges();
                }

                var maLops = _context.PhanCongGiangDays
                    .Where(pc => pc.MaGiaoVien == gv.MaGiaoVien)
                    .Select(pc => pc.MaLop)
                    .Distinct()
                    .ToList();

                var lichHocs = _context.LichHocs
                    .Where(lh => lh.NgayHoc.HasValue && maLops.Contains(lh.MaLop))
                    .ToList();

                var nhomTheoThangNam = lichHocs
                    .GroupBy(lh => new { Thang = lh.NgayHoc!.Value.Month, Nam = lh.NgayHoc!.Value.Year })
                    .Select(g => new
                    {
                        Thang = g.Key.Thang,
                        Nam = g.Key.Nam,
                        SoBuoi = g.Count()
                    })
                    .ToList();

                foreach (var item in nhomTheoThangNam)
                {
                    var existing = _context.LuongGiaoViens
                        .FirstOrDefault(x => x.MaGiaoVien == gv.MaGiaoVien &&
                                             x.Thang == item.Thang &&
                                             x.Nam == item.Nam);

                    decimal luongMoiBuoi = gv.LuongMoiBuoi ?? 30000;
                    decimal tongLuong = item.SoBuoi * luongMoiBuoi;

                    // ✅ Nếu chưa có -> thêm mới
                    if (existing == null)
                    {
                        var luong = new LuongGiaoVien
                        {
                            MaGiaoVien = gv.MaGiaoVien,
                            Thang = item.Thang,
                            Nam = item.Nam,
                            SoBuoiDay = item.SoBuoi,
                            LuongMoiBuoi = luongMoiBuoi,
                            TongLuong = tongLuong,
                            TrangThai = "Đã tính"
                        };

                        _context.LuongGiaoViens.Add(luong);
                        tongBanGhiMoi++;
                    }
                    else
                    {
                        // ✅ Nếu đã có nhưng lương hoặc tổng lương = 0 → cập nhật lại
                        if (existing.LuongMoiBuoi == 0 || existing.TongLuong == 0)
                        {
                            existing.SoBuoiDay = item.SoBuoi;
                            existing.LuongMoiBuoi = luongMoiBuoi;
                            existing.TongLuong = tongLuong;
                            existing.TrangThai = "Cập nhật lại";
                            _context.LuongGiaoViens.Update(existing);
                            tongBanGhiCapNhat++;
                        }
                    }
                }
            }


            _context.SaveChanges();

            TempData["Success"] = $"✅ Đã tự động tính/cập nhật lương cho {tongBanGhiMoi + tongBanGhiCapNhat} bản ghi (30,000đ/buổi mặc định). " +
                                  $"(Thêm mới: {tongBanGhiMoi}, Cập nhật: {tongBanGhiCapNhat})";

            return RedirectToAction(nameof(LuongGiaoVien));
        }
    }
}