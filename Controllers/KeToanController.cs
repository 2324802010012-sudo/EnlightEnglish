using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
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
        public IActionResult Index()
        {
            ViewData["Title"] = "Trang kế toán";
            return View();
        }
        // ======================= 💵 DANH SÁCH HỌC PHÍ =======================
        public IActionResult HocPhi()
        {
            var hocPhi = (from hp in _context.HocPhis
                          join hv in _context.HocViens on hp.MaHocVien equals hv.MaHocVien
                          join lop in _context.LopHocs on hp.MaLop equals lop.MaLop
                          select new
                          {
                              hp.MaHocPhi,
                              HocVien = hv.HoTen,
                              Lop = lop.TenLop,
                              hp.SoTienPhaiDong,
                              hp.TrangThai,
                              hp.NgayDongCuoi
                          })
                          .OrderByDescending(x => x.MaHocPhi)
                          .ToList();

            return View(hocPhi);
        }

        // ======================= 💰 XÁC NHẬN ĐÃ THANH TOÁN =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CapNhatHocPhi(int id)
        {
            var hocPhi = _context.HocPhis.FirstOrDefault(x => x.MaHocPhi == id);
            if (hocPhi == null)
            {
                TempData["Error"] = "Không tìm thấy bản ghi học phí!";
                return RedirectToAction(nameof(HocPhi));
            }

            // ✅ Lấy tên học viên & lớp từ các bảng liên quan
            var hocVien = _context.HocViens.FirstOrDefault(h => h.MaHocVien == hocPhi.MaHocVien);
            var lop = _context.LopHocs.FirstOrDefault(l => l.MaLop == hocPhi.MaLop);

            // ✅ Cập nhật học phí
            hocPhi.TrangThai = "Đã thanh toán";
            hocPhi.NgayDongCuoi = DateTime.Now;

            _context.SaveChanges();

            // ✅ Ghi vào báo cáo doanh thu
            var baoCao = new BaoCao
            {
                LoaiBaoCao = "Doanh thu học phí",
                NoiDung = $"Kế toán xác nhận học viên {hocVien?.HoTen ?? "(Không rõ)"} " +
                          $"đã thanh toán {hocPhi.SoTienPhaiDong:N0} đ cho lớp {lop?.TenLop ?? "(Không rõ)"}",
                NgayLap = DateTime.Now,
                NguoiLap = HttpContext.Session.GetInt32("MaNguoiDung")
            };

            _context.BaoCaos.Add(baoCao);
            _context.SaveChanges();

            TempData["Success"] = $"✅ Đã xác nhận thanh toán cho học viên {hocVien?.HoTen ?? "(Không rõ)"}!";
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