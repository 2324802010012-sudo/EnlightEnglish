using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace EnlightEnglishCenter.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ======================= ĐĂNG NHẬP =======================
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string Username, string Password)
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ViewBag.Error = "⚠️ Vui lòng nhập đầy đủ thông tin!";
                return View();
            }

            var user = _context.NguoiDungs.FirstOrDefault(u =>
                u.TenDangNhap == Username || u.Email == Username);

            if (user == null)
            {
                ViewBag.Error = "❌ Tên đăng nhập hoặc mật khẩu không đúng!";
                return View();
            }

            // ✅ Nếu tài khoản bị khóa tạm thời
            if (user.KhoaDenNgay != null && user.KhoaDenNgay > DateTime.Now)
            {
                var phutConLai = (user.KhoaDenNgay.Value - DateTime.Now).Minutes;
                ViewBag.Error = $"🚫 Tài khoản bị khóa. Vui lòng thử lại sau {phutConLai} phút.";
                return View();
            }

            // ✅ So sánh mật khẩu (loại bỏ khoảng trắng 2 bên)
            if (user.MatKhau.Trim() != Password.Trim())
            {
                user.SoLanSaiMatKhau++;

                if (user.SoLanSaiMatKhau >= 3)
                {
                    user.KhoaDenNgay = DateTime.Now.AddMinutes(5);
                    user.SoLanSaiMatKhau = 0;
                    ViewBag.Error = "🚫 Bạn đã nhập sai 3 lần. Tài khoản bị khóa 5 phút.";
                }
                else
                {
                    ViewBag.Error = $"❌ Sai mật khẩu! Còn {3 - user.SoLanSaiMatKhau} lần thử.";
                }

                _context.SaveChanges();
                return View();
            }

            // ✅ Nếu mật khẩu đúng
            user.SoLanSaiMatKhau = 0;
            user.KhoaDenNgay = null;
            _context.SaveChanges();

            // ✅ Lưu session chung
            HttpContext.Session.SetInt32("MaNguoiDung", user.MaNguoiDung);
            HttpContext.Session.SetString("TenDangNhap", user.TenDangNhap);
            var role = _context.VaiTros.FirstOrDefault(v => v.MaVaiTro == user.MaVaiTro)?.TenVaiTro ?? "Học viên";
            HttpContext.Session.SetString("VaiTro", role);

            TempData["LoginSuccess"] = $"🎉 Xin chào {user.HoTen}! ({role})";

            // ✅ Nếu là giáo viên thì lưu thêm Session MaGiaoVien
            if (role == "Giáo viên")
            {
                var giaoVien = _context.GiaoViens.FirstOrDefault(g => g.MaNguoiDung == user.MaNguoiDung);
                if (giaoVien != null)
                    HttpContext.Session.SetInt32("MaGiaoVien", giaoVien.MaGiaoVien);
            }

            // ✅ Điều hướng theo vai trò
            return role switch
            {
                "Admin" => RedirectToAction("Index", "Admin"),
                "Giáo viên" => RedirectToAction("Index", "GiaoVien"),
                "Kế toán" => RedirectToAction("Index", "KeToan"),
                "Phòng đào tạo" => RedirectToAction("Index", "PhongDaoTao"),
                "Lễ tân" => RedirectToAction("Index", "LeTan"),
                _ => RedirectToAction("Index", "HocVien")
            };
        }

            // ======================= ĐĂNG KÝ =======================
            [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(string FullName, string Email, string Password)
        {
            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ViewBag.Error = "⚠️ Vui lòng nhập đầy đủ thông tin!";
                return View();
            }

            if (_context.NguoiDungs.Any(u => u.Email == Email))
            {
                ViewBag.Error = "⚠️ Email này đã được sử dụng!";
                return View();
            }

            var newUser = new NguoiDung
            {
                HoTen = FullName.Trim(),
                Email = Email.Trim(),
                TenDangNhap = Email.Split('@')[0],
                MatKhau = Password.Trim(),
                MaVaiTro = 4, // Học viên
                TrangThai = "Hoạt động"
            };

            _context.NguoiDungs.Add(newUser);
            _context.SaveChanges();

            TempData["RegisterSuccess"] = "🎉 Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

        // ======================= ĐĂNG XUẤT =======================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Logout"] = "👋 Bạn đã đăng xuất thành công.";
            return RedirectToAction("Login");
        }

        // ======================= HỒ SƠ CÁ NHÂN =======================
        public IActionResult Profile()
        {
            int? maNguoiDung = HttpContext.Session.GetInt32("MaNguoiDung");
            if (maNguoiDung == null)
                return RedirectToAction("Login");

            var nguoiDung = _context.NguoiDungs
                .Include(nd => nd.MaVaiTroNavigation)
                .FirstOrDefault(nd => nd.MaNguoiDung == maNguoiDung);

            return nguoiDung != null ? View(nguoiDung) : RedirectToAction("Login");
        }

        // ======================= CHỈNH SỬA HỒ SƠ =======================
        [HttpGet]
        public IActionResult EditProfile()
        {
            int? maNguoiDung = HttpContext.Session.GetInt32("MaNguoiDung");
            if (maNguoiDung == null)
                return RedirectToAction("Login");

            var user = _context.NguoiDungs.Find(maNguoiDung);
            return user != null ? View(user) : RedirectToAction("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(NguoiDung model)
        {
            int? maNguoiDung = HttpContext.Session.GetInt32("MaNguoiDung");
            if (maNguoiDung == null)
                return RedirectToAction("Login");

            var user = _context.NguoiDungs.Find(maNguoiDung);
            if (user == null)
                return RedirectToAction("Profile");

            user.HoTen = model.HoTen;
            user.Email = model.Email;
            user.SoDienThoai = model.SoDienThoai;
            user.DiaChi = model.DiaChi;
            user.GioiTinh = model.GioiTinh;
            user.NgaySinh = model.NgaySinh;
            _context.SaveChanges();

            TempData["Success"] = "✅ Hồ sơ đã được cập nhật!";
            return RedirectToAction("Profile");
        }
        // ================== ĐỔI MẬT KHẨU ==================
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var userName = HttpContext.Session.GetString("TenDangNhap");

            if (string.IsNullOrEmpty(userName))
            {
                TempData["Error"] = "Bạn cần đăng nhập để đổi mật khẩu.";
                return RedirectToAction("Login");
            }

            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu mới và xác nhận không khớp.";
                return View();
            }

            var user = _context.NguoiDungs.FirstOrDefault(u => u.TenDangNhap == userName);
            if (user == null)
            {
                ViewBag.Error = "Không tìm thấy tài khoản.";
                return View();
            }

            // Giả sử bạn chưa mã hóa mật khẩu
            if (user.MatKhau != oldPassword)
            {
                ViewBag.Error = "Mật khẩu cũ không đúng.";
                return View();
            }

            // Cập nhật mật khẩu mới
            user.MatKhau = newPassword;
            _context.Update(user);
            _context.SaveChanges();

            ViewBag.Success = "✅ Đổi mật khẩu thành công!";
            return View();
        }

        // ======================= QUÊN MẬT KHẨU =======================
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(string Email)
        {
            var user = _context.NguoiDungs.FirstOrDefault(u => u.Email == Email);
            if (user == null)
            {
                TempData["Error"] = "❌ Email không tồn tại trong hệ thống!";
                return RedirectToAction("ForgotPassword");
            }

            string token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            user.ResetToken = token;
            user.ResetTokenExpiry = DateTime.Now.AddMinutes(15);
            _context.SaveChanges();

            string resetLink = Url.Action("ResetPassword", "Account", new { token = token }, Request.Scheme);

            // ✅ Hiển thị link trực tiếp cho môi trường demo
            TempData["Message"] = "📩 Link đặt lại mật khẩu (chỉ hiển thị trong demo):";
            TempData["ResetLink"] = resetLink;

            return RedirectToAction("ForgotPassword");
        }

        // ======================= ĐẶT LẠI MẬT KHẨU =======================
        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            var user = _context.NguoiDungs.FirstOrDefault(u =>
                u.ResetToken == token && u.ResetTokenExpiry > DateTime.Now);

            if (user == null)
            {
                ViewBag.Error = "❌ Liên kết không hợp lệ hoặc đã hết hạn.";
                return View();
            }

            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(string token, string NewPassword, string ConfirmPassword)
        {
            var user = _context.NguoiDungs.FirstOrDefault(u =>
                u.ResetToken == token && u.ResetTokenExpiry > DateTime.Now);

            if (user == null)
            {
                ViewBag.Error = "❌ Liên kết không hợp lệ hoặc đã hết hạn.";
                ViewBag.Token = token;
                return View();
            }

            if (NewPassword != ConfirmPassword)
            {
                ViewBag.Error = "⚠️ Mật khẩu xác nhận không trùng khớp!";
                ViewBag.Token = token;
                return View();
            }

            user.MatKhau = NewPassword.Trim();
            user.ResetToken = null;
            user.ResetTokenExpiry = null;
            _context.SaveChanges();

            TempData["ResetSuccess"] = "✅ Mật khẩu đã được thay đổi thành công!";
            return RedirectToAction("Login");
        }
    }
}
