using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using EnlightEnglishCenter.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace EnlightEnglishCenter.Controllers
{
    public class LeTanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeTanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ======================================
        // 🧍‍♂️ 1. Đăng ký học viên mới → Mở form RegisterFromLeTan
        // ======================================
        [HttpGet]
        public IActionResult DangKyHocVien()
        {
            // ✅ Điều hướng sang trang đăng ký của AccountController
            return RedirectToAction("RegisterFromLeTan", "Account");
        }

        // ===============================
        // 🔹 GET: Form đăng ký học viên từ lễ tân
        // ===============================
        [HttpGet]
        public IActionResult RegisterFromLeTan()
        {
            ViewBag.IsFromLeTan = true; // đánh dấu là đang vào từ lễ tân
            return View("Register");
        }

        // ===============================
        // 🔹 POST: Xử lý đăng ký học viên từ lễ tân
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterFromLeTan(NguoiDung model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IsFromLeTan = true;
                return View("Register", model);
            }

            // Kiểm tra trùng email hoặc username
            if (_context.NguoiDungs.Any(x => x.Email == model.Email || x.TenDangNhap == model.TenDangNhap))
            {
                ModelState.AddModelError("Email", "Email hoặc tên đăng nhập đã tồn tại.");
                ViewBag.IsFromLeTan = true;
                return View("Register", model);
            }

            // Tìm vai trò học viên
            var vaiTro = await _context.VaiTros.FirstOrDefaultAsync(v => v.TenVaiTro == "Học viên");
            if (vaiTro == null)
            {
                ModelState.AddModelError("", "Không tìm thấy vai trò Học viên.");
                ViewBag.IsFromLeTan = true;
                return View("Register", model);
            }

            // Tạo người dùng
            var nguoiDung = new NguoiDung
            {
                HoTen = model.HoTen,
                Email = model.Email,
                SoDienThoai = model.SoDienThoai,
                TenDangNhap = model.Email ?? Guid.NewGuid().ToString(),
             //   MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau ?? "123456"),
                MaVaiTro = vaiTro.MaVaiTro,
                TrangThai = "Hoạt động",
                SoLanSaiMatKhau = 0
            };

            _context.NguoiDungs.Add(nguoiDung);
            await _context.SaveChangesAsync();

            // Tạo học viên tương ứng
            var hocVien = new HocVien
            {
                HoTen = nguoiDung.HoTen,
                Email = nguoiDung.Email,
                SoDienThoai = nguoiDung.SoDienThoai,
                MaNguoiDung = nguoiDung.MaNguoiDung,
                NgayDangKy = DateTime.Now,
                TrangThai = "Mới đăng ký"
            };

            _context.HocViens.Add(hocVien);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"✅ Đã tạo học viên {hocVien.HoTen} thành công (tài khoản: {hocVien.Email}, mật khẩu mặc định: 123456)";
            return RedirectToAction("Index", "LeTan");
        }

        // ======================================
        // 🧾 4. Danh sách học viên
        // ======================================
        public async Task<IActionResult> DanhSachHocVien()
        {
            var hocViens = await _context.HocViens
              
                .OrderByDescending(h => h.NgayDangKy)
                .ToListAsync();
            return View(hocViens);
        }

        // ======================================
        // ✏️ 5. Sửa học viên
        // ======================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHocVien(HocVien model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("DanhSachHocVien");

            _context.Update(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Đã cập nhật thông tin học viên!";
            return RedirectToAction("DanhSachHocVien");
        }




        // ======================================
        // 📞 7. Liên hệ học viên & khách hàng
        // ======================================
        public async Task<IActionResult> LienHeHocVien()
        {
            var viewModel = new LienHeHocVienViewModel
            {
                HocViens = await _context.HocViens.ToListAsync(),
                KhachHangs = await _context.LienHeKhachHang.ToListAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemLienHe(LienHeKhachHang model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "⚠️ Vui lòng nhập đầy đủ thông tin liên hệ.";
                return RedirectToAction("LienHeHocVien");
            }

            model.NgayLienHe = DateTime.Now;
            _context.LienHeKhachHang.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Đã thêm khách hàng tiềm năng mới!";
            return RedirectToAction("LienHeHocVien");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaLienHe(int id)
        {
            var lienHe = await _context.LienHeKhachHang.FindAsync(id);
            if (lienHe == null)
            {
                TempData["Error"] = "Không tìm thấy liên hệ để xóa!";
                return RedirectToAction("LienHeHocVien");
            }

            _context.LienHeKhachHang.Remove(lienHe);
            await _context.SaveChangesAsync();

            TempData["Success"] = "🗑️ Đã xóa liên hệ khách hàng thành công!";
            return RedirectToAction("LienHeHocVien");
        }

        // ======================================
        // 💰 8. Quản lý thu học phí
        // ======================================
        // =======================================================
        // 💰  QUẢN LÝ THU HỌC PHÍ
        // =======================================================
        public IActionResult QuanLyThuHocPhi()
        {
            var danhSach = _context.DonHocPhis
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l.MaKhoaHocNavigation)
                .OrderByDescending(d => d.NgayTao)
                .ToList();

            return View(danhSach);
        }
        // =======================================================
        // ✅ XÁC NHẬN THANH TOÁN HỌC PHÍ
        // =======================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XacNhanHocPhi(int id)
        {
            var don = _context.DonHocPhis
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l.MaKhoaHocNavigation)
                .FirstOrDefault(d => d.MaDon == id);

            if (don == null)
            {
                TempData["Error"] = "❌ Không tìm thấy đơn học phí!";
                return RedirectToAction(nameof(QuanLyThuHocPhi));
            }

            if (don.TrangThai == "Đã thanh toán")
            {
                TempData["Info"] = "⚠️ Đơn học phí này đã được xác nhận trước đó.";
                return RedirectToAction(nameof(QuanLyThuHocPhi));
            }

            // 🔹 Cập nhật trạng thái thanh toán
            don.TrangThai = "Đã thanh toán";
            don.NgayThanhToan = DateTime.Now;

            // 🔹 Cập nhật sĩ số lớp
            var lop = _context.LopHocs.FirstOrDefault(l => l.MaLop == don.MaLop);
            if (lop != null)
            {
                lop.SiSoHienTai = (lop.SiSoHienTai ?? 0) + 1;
            }

            // 🔹 Ghi học viên vào danh sách lớp (nếu chưa có)
            var daTonTai = _context.DkHocVienLopHocs
                .Any(x => x.MaHocVien == don.MaHocVien && x.MaLop == don.MaLop);

            if (!daTonTai)
            {
                var dk = new DkHocVienLopHoc
                {
                    MaHocVien = don.MaHocVien,
                    MaLop = don.MaLop,
                    NgayDangKy = DateTime.Now,
                    TrangThai = "Đã thanh toán",
                    TrangThaiHoc = "Chưa bắt đầu"
                };
                _context.DkHocVienLopHocs.Add(dk);
            }

            // 🔹 Ghi nhận vào bảng BÁO CÁO DOANH THU
            var baoCao = new BaoCao
            {
                LoaiBaoCao = "Doanh thu học phí",
                NoiDung = $"💰 Học viên {don.HocVien?.HoTen} đã thanh toán {don.TongTien:N0} đ cho lớp {don.LopHoc?.TenLop} ({don.LopHoc?.MaKhoaHocNavigation?.TenKhoaHoc}).",
                NguoiLap = HttpContext.Session.GetInt32("MaNguoiDung"),
                NgayLap = DateTime.Now
            };
            _context.BaoCaos.Add(baoCao);

            _context.SaveChanges();

            TempData["Success"] = $"✅ Đã xác nhận thanh toán học phí cho học viên {don.HocVien?.HoTen}.";
            return RedirectToAction(nameof(QuanLyThuHocPhi));
        }
        public IActionResult BaoCao()
        {
            var dsBaoCao = _context.BaoCaos
                .Include(b => b.NguoiLapNavigation)
                .OrderByDescending(b => b.NgayLap)
                .ToList();

            return View(dsBaoCao);
        }
        // ======================================
        // 📋 DANH SÁCH HỌC VIÊN (CÓ TÌM KIẾM)
        // ======================================
        [HttpGet]
        public IActionResult DanhSachHocVien(string? search)
        {
            var hocViens = _context.HocViens
                .OrderByDescending(h => h.NgayDangKy)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string keyword = search.Trim().ToLower();
                hocViens = hocViens.Where(h =>
                    h.HoTen.ToLower().Contains(keyword) ||
                    (h.Email ?? "").ToLower().Contains(keyword) ||
                    (h.SoDienThoai ?? "").Contains(keyword));
            }

            ViewBag.CurrentSearch = search;
            return View(hocViens.ToList());
        }


        [HttpGet]
        public IActionResult DangKyTestHocVien(int maHocVien)
        {
            var hocVien = _context.HocViens.Find(maHocVien);
            if (hocVien == null)
            {
                TempData["Error"] = "Không tìm thấy học viên.";
                return RedirectToAction("DanhSachHocVien");
            }

            ViewBag.HocVien = hocVien;
            ViewBag.KhoaHocList = _context.KhoaHocs
                .Where(k => k.TrangThai == "Đang mở")
                .ToList();

            return View("~/Views/LeTan/DangKyTestHocVien.cshtml");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangKyTestHocVien(int MaHocVien, int MaKhoaHoc)
        {
            var hv = _context.HocViens.Find(MaHocVien);
            if (hv == null)
            {
                TempData["Error"] = "Không tìm thấy học viên.";
                return RedirectToAction("DanhSachHocVien");
            }

            var tonTai = _context.TestDauVaos
                .FirstOrDefault(t => t.MaHocVien == MaHocVien && t.KhoaHocDeXuat == MaKhoaHoc);
            if (tonTai != null)
            {
                TempData["Error"] = "Học viên này đã đăng ký test cho khóa học này.";
                return RedirectToAction("DanhSachHocVien");
            }

            var test = new TestDauVao
            {
                MaHocVien = MaHocVien,
                KhoaHocDeXuat = MaKhoaHoc,
                NgayDangKy = DateTime.Now,
                TrangThai = "Chờ xác nhận"
            };

            _context.TestDauVaos.Add(test);
            _context.SaveChanges();

            TempData["Success"] = $"✅ Đã đăng ký test đầu vào cho học viên {hv.HoTen}.";
            return RedirectToAction("DanhSachHocVien");
        }
        // ===============================
        // 📘 XEM THÔNG TIN LỚP HỌC (lấy từ Phòng Đào Tạo)
        // ===============================
        public IActionResult ThongTinLopHoc()
        {
            // Lấy dữ liệu từ bảng LopHoc, có bao gồm thông tin khóa học và giáo viên
            var dsLop = _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .Include(l => l.MaGiaoVienNavigation)
                    .ThenInclude(gv => gv.NguoiDung)
                .OrderByDescending(l => l.NgayBatDau)
                .ToList();

            return View(dsLop);
        }
        // ======================================
        // 🚪 9. Đăng xuất
        // ======================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        // ======================================
        // 🏠 10. Trang chủ Lễ tân
        // ======================================
        public IActionResult Index()
        {
            return View();
        }
    }
}
