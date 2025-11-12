using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;

namespace EnlightEnglishCenter.Controllers
{
    public class TestDauVaoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public TestDauVaoController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ======================================================
        // 1️⃣ Học viên xem danh sách khóa học
        // ======================================================
        [HttpGet]
        public IActionResult Index()
        {
            int? maNguoiDung = HttpContext.Session.GetInt32("MaNguoiDung");
            if (maNguoiDung == null)
            {
                TempData["Error"] = "⚠️ Bạn cần đăng nhập để đăng ký test.";
                return RedirectToAction("Login", "Account");
            }

            // ✅ FIX: tìm đúng học viên theo mã người dùng
            var hocVien = _context.HocViens.FirstOrDefault(h => h.MaNguoiDung == maNguoiDung);
            if (hocVien == null)
            {
                TempData["Error"] = "❌ Không tìm thấy thông tin học viên.";
                return RedirectToAction("Login", "Account");
            }

            // 🔹 Lấy danh sách khóa học đang mở
            var khoaHoc = _context.KhoaHocs
                .Where(k => k.TrangThai == "Đang mở")
                .OrderBy(k => k.NgayBatDau)
                .ToList();

            // 🔹 Lấy danh sách test của học viên (đúng mã học viên)
            var dsTest = _context.TestDauVaos
                .Where(t => t.MaHocVien == hocVien.MaHocVien)
                .ToList();

            ViewBag.DanhSachTest = dsTest;

            return View(khoaHoc);
        }

        // ======================================================
        // 2️⃣ Học viên đăng ký Test đầu vào
        // ======================================================
        // 2️⃣ Học viên đăng ký Test đầu vào
        [HttpGet]
        public IActionResult DangKy(int? maKhoaHoc = null)
        {
            int? maNguoiDung = HttpContext.Session.GetInt32("MaNguoiDung");
            if (maNguoiDung == null)
            {
                TempData["Error"] = "⚠️ Bạn cần đăng nhập trước khi bắt đầu Test.";
                return RedirectToAction("Login", "Account");
            }

            var hocVien = _context.HocViens.FirstOrDefault(h => h.MaNguoiDung == maNguoiDung);
            if (hocVien == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin học viên.";
                return RedirectToAction("Index", "TestDauVao");
            }

            // 1) Nếu đã có test đang mở -> dùng lại và vào LamBai
            var testDangMo = _context.TestDauVaos
                .Where(t => t.MaHocVien == hocVien.MaHocVien && t.TrangThai != "Hoàn thành")
                .OrderByDescending(t => t.NgayTest)
                .FirstOrDefault();

            if (testDangMo != null)
            {
                if (!string.Equals(testDangMo.TrangThai?.Trim(), "Được phép test", StringComparison.OrdinalIgnoreCase))
                {
                    testDangMo.TrangThai = "Được phép test";
                    testDangMo.NgayTest = DateTime.Now;
                    _context.SaveChanges();
                }
                return RedirectToAction("LamBai", "TestDauVao");
            }

            // 2) Chưa có -> tạo mới, KHÔNG ràng buộc phải đúng mã khóa học
            var khoaHocMacDinh =
                (maKhoaHoc.HasValue ? _context.KhoaHocs.Find(maKhoaHoc.Value) : null)
                ?? _context.KhoaHocs.FirstOrDefault(k => k.TrangThai == "Đang mở")
                ?? _context.KhoaHocs.FirstOrDefault(); // fallback bất kỳ

            if (khoaHocMacDinh == null)
            {
                TempData["Error"] = "❌ Hiện chưa có khóa học nào trong hệ thống.";
                return RedirectToAction("Index", "TestDauVao");
            }

            var test = new TestDauVao
            {
                MaHocVien = hocVien.MaHocVien,
                KhoaHocDeXuat = khoaHocMacDinh.MaKhoaHoc, // dùng khóa mặc định, không cần “đúng”
                NgayTest = DateTime.Now,
                TrangThai = "Được phép test",
                LoTrinhHoc = null
            };

            _context.TestDauVaos.Add(test);
            _context.SaveChanges();

            // Vào làm bài ngay
            return RedirectToAction("LamBai", "TestDauVao");
        }



        // ==========================
        // 🧠 LỄ TÂN ĐĂNG KÝ TEST CHO HỌC VIÊN
        // ==========================
        [HttpGet]
        public IActionResult DangKyTestHocVien(int id)
        {
            var hocVien = _context.HocViens.Find(id);
            if (hocVien == null)
            {
                TempData["Error"] = "Không tìm thấy học viên.";
                return RedirectToAction("DanhSachHocVien", "LeTan");
            }

            ViewBag.HocVien = hocVien;
            ViewBag.KhoaHocList = _context.KhoaHocs.ToList();

            return View("~/Views/LeTan/DangKyTestHocVien.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangKyTestHocVien(int MaHocVien, int MaKhoaHoc)
        {
            var hocVien = _context.HocViens.Find(MaHocVien);
            if (hocVien == null)
            {
                TempData["Error"] = "Không tìm thấy học viên để đăng ký test.";
                return RedirectToAction("DanhSachHocVien", "LeTan");
            }

            var khoaHoc = _context.KhoaHocs.Find(MaKhoaHoc);
            if (khoaHoc == null)
            {
                TempData["Error"] = "Khóa học không tồn tại.";
                return RedirectToAction("DanhSachHocVien", "LeTan");
            }

            var testTonTai = _context.TestDauVaos
                .FirstOrDefault(t => t.MaHocVien == MaHocVien && t.KhoaHocDeXuat == MaKhoaHoc);

            if (testTonTai != null)
            {
                TempData["Error"] = $"Học viên '{hocVien.HoTen}' đã đăng ký test cho khóa '{khoaHoc.TenKhoaHoc}' rồi.";
                return RedirectToAction("DanhSachHocVien", "LeTan");
            }

            var test = new TestDauVao
            {
                MaHocVien = MaHocVien,
                KhoaHocDeXuat = MaKhoaHoc,
                NgayTest = DateTime.Now,
                TrangThai = "Chờ xác nhận"
            };

            _context.TestDauVaos.Add(test);
            _context.SaveChanges();

            TempData["Success"] = $"Đã đăng ký test đầu vào cho học viên '{hocVien.HoTen}' - Khóa '{khoaHoc.TenKhoaHoc}'.";
            return RedirectToAction("DanhSachHocVien", "LeTan");
        }

        // ======================================================
        // 3️⃣ Admin xem danh sách và duyệt Test
        // ======================================================






        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult DuyetTest(int id)
        //{
        //    var test = _context.TestDauVaos
        //        .Include(t => t.HocVien)
        //        .Include(t => t.KhoaHocDeXuatNavigation)
        //        .FirstOrDefault(t => t.MaTest == id);

        //    if (test == null)
        //    {
        //        TempData["Error"] = "Không tìm thấy bài test cần duyệt!";
        //        return RedirectToAction("DanhSach");
        //    }

        //    test.TrangThai = "Được phép test";
        //    _context.SaveChanges();

        //    GuiEmailThongBao(
        //        test.HocVien?.Email ?? "",
        //        test.HocVien?.HoTen ?? "Học viên",
        //        test.KhoaHocDeXuatNavigation?.TenKhoaHoc ?? "Khóa học"
        //    );

        //    TempData["Success"] = "✅ Đã duyệt học viên làm test.";

        //    var vaiTro = HttpContext.Session.GetString("VaiTro")?.Trim();

        //    if (!string.IsNullOrEmpty(vaiTro) &&
        //        vaiTro.Equals("Phòng đào tạo", StringComparison.OrdinalIgnoreCase))
        //    {
        //        return RedirectToAction("DuyetTest", "PhongDaoTao");
        //    }
        //    else
        //    {
        //        return RedirectToAction("DanhSach", "TestDauVao");
        //    }
        //}

        // ======================================================
        // 4️⃣ Học viên làm bài test
        // ======================================================
        public IActionResult LamBai()
        {
            int? maNguoiDung = HttpContext.Session.GetInt32("MaNguoiDung");
            if (maNguoiDung == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập trước khi làm bài test.";
                return RedirectToAction("Login", "Account");
            }

            // Tìm đúng học viên theo MaNguoiDung
            var hocVien = _context.HocViens.FirstOrDefault(h => h.MaNguoiDung == maNguoiDung);
            if (hocVien == null)
            {
                TempData["Error"] = "Không tìm thấy học viên.";
                return RedirectToAction("Login", "Account");
            }

            // Lấy bài test MỚI NHẤT chưa hoàn thành
            var test = _context.TestDauVaos
                .Include(t => t.HocVien)
                .Include(t => t.KhoaHocDeXuatNavigation)
                .Where(t => t.MaHocVien == hocVien.MaHocVien && t.TrangThai != "Hoàn thành")
                .OrderByDescending(t => t.NgayTest)
                .FirstOrDefault();

            if (test == null)
            {
                TempData["Error"] = "❌ Bạn chưa có bài test đang mở. Hãy chọn khóa học để bắt đầu test.";
                return RedirectToAction("Index", "TestDauVao");
            }

            // Nếu còn trạng thái cũ “Chờ xác nhận” → nâng lên “Được phép test” để thi ngay
            if (string.Equals(test.TrangThai?.Trim(), "Chờ xác nhận", StringComparison.OrdinalIgnoreCase))
            {
                test.TrangThai = "Được phép test";
                _context.SaveChanges();
            }

            // Xác định thư mục câu hỏi theo tên khóa học
            string khoaHoc = test.KhoaHocDeXuatNavigation?.TenKhoaHoc?.ToLower() ?? "ielts";
            if (khoaHoc.Contains("cambridge")) khoaHoc = "cambridge";
            else if (khoaHoc.Contains("toeic")) khoaHoc = "toeic";
            else khoaHoc = "ielts";

            var grammar = ReadQuestions(khoaHoc, "questions.json", "Grammar");
            var reading = ReadQuestions(khoaHoc, "reading.json", "Reading");
            var listening = ReadQuestions(khoaHoc, "listening.json", "Listening");

            var allQuestions = grammar.Concat(reading).Concat(listening).ToList();

            if (allQuestions.Count == 0)
            {
                TempData["Error"] = "❌ Chưa có dữ liệu câu hỏi cho khóa học này. Vui lòng liên hệ quản trị.";
                return RedirectToAction("Index", "TestDauVao");
            }

            ViewBag.Course = test.KhoaHocDeXuatNavigation?.TenKhoaHoc ?? "";
            ViewBag.HoTen = test.HocVien?.HoTen ?? "";
            ViewBag.TestId = test.MaTest;

            return View(allQuestions);
        }


        // ======================================================
        // 5️⃣ Nộp bài & tính điểm
        // ======================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NopBai(List<int> answers, List<int> correctAnswers, string course)
        {
            if (answers == null || correctAnswers == null)
                return Content("⚠️ Lỗi khi nhận dữ liệu bài làm.");

            int correct = 0;
            int count = Math.Min(answers.Count, correctAnswers.Count);
            for (int i = 0; i < count; i++)
                if (answers[i] == correctAnswers[i]) correct++;

            double score10 = count > 0 ? (correct / (double)count) * 10 : 0;

            string deXuat = score10 < 4 ? "Cấp độ Cơ bản"
                          : score10 < 7 ? "Cấp độ Trung bình"
                          : "Cấp độ Nâng cao";

            int? maNguoiDung = HttpContext.Session.GetInt32("MaNguoiDung");
            if (maNguoiDung == null)
                return RedirectToAction("Login", "Account");

            var hocVien = _context.HocViens.FirstOrDefault(h => h.MaNguoiDung == maNguoiDung);
            if (hocVien == null)
                return RedirectToAction("Login", "Account");

            var test = _context.TestDauVaos
                .Include(t => t.HocVien)
                .Include(t => t.KhoaHocDeXuatNavigation)
                .FirstOrDefault(t => t.MaHocVien == hocVien.MaHocVien && t.TrangThai == "Được phép test");

            if (test != null)
            {
                test.DiemNguPhap = (decimal)score10;
                test.TongDiem = (decimal)score10;
                test.TrangThai = "Hoàn thành";
                test.NgayTest = DateTime.Now;
                test.LoTrinhHoc = deXuat;
                _context.SaveChanges();

                GuiEmailKetQua(
                    test.HocVien?.Email ?? "",
                    test.HocVien?.HoTen ?? "Học viên",
                    score10,
                    deXuat
                );
            }

            return RedirectToAction("KetQua");
        }

        // ======================================================
        // 6️⃣ Trang Kết quả
        // ======================================================
        [HttpGet]
        public IActionResult KetQua(int? id)
        {
            int? maNguoiDung = HttpContext.Session.GetInt32("MaNguoiDung");
            if (maNguoiDung == null) return RedirectToAction("Login", "Account");

            var hocVien = _context.HocViens.FirstOrDefault(h => h.MaNguoiDung == maNguoiDung);
            if (hocVien == null) return RedirectToAction("Login", "Account");

            var query = _context.TestDauVaos
                .Include(t => t.HocVien)
                .Include(t => t.KhoaHocDeXuatNavigation)
                .AsQueryable();

            var test = id.HasValue
                ? query.FirstOrDefault(t => t.MaTest == id.Value && t.MaHocVien == hocVien.MaHocVien)
                : query.Where(t => t.MaHocVien == hocVien.MaHocVien)
                       .OrderByDescending(t => t.NgayTest)
                       .FirstOrDefault();

            if (test == null)
            {
                TempData["Error"] = "❌ Bạn chưa có bài test nào!";
                return RedirectToAction("Index", "HocVien");
            }

            return View(test);
        }

        // ======================================================
        // 🔟 Gửi email kết quả và thông báo
        // ======================================================
        private void GuiEmailKetQua(string email, string ten, double diem, string loTrinh)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email)) return;
                var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential("trungtamenlight@gmail.com", "app-password-gmail")
                };
                var message = new MailMessage(
                    new MailAddress("trungtamenlight@gmail.com", "Enlight English Center"),
                    new MailAddress(email))
                {
                    Subject = "Kết quả Test đầu vào - Enlight English Center",
                    Body = $@"
                        <h3>Chào {ten},</h3>
                        <p>Bạn vừa hoàn thành bài Test đầu vào.</p>
                        <p><b>Điểm tổng:</b> {diem:F1}/10</p>
                        <p><b>Lộ trình đề xuất:</b> {loTrinh}</p>
                        <p>Chúc bạn học tốt cùng ENLIGHT 🌟</p>",
                    IsBodyHtml = true
                };
                smtp.Send(message);
            }
            catch { }
        }

        private void GuiEmailThongBao(string email, string ten, string khoaHoc)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email)) return;
                var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential("trungtamenlight@gmail.com", "app-password-gmail")
                };
                var message = new MailMessage(
                    new MailAddress("trungtamenlight@gmail.com", "Enlight English Center"),
                    new MailAddress(email))
                {
                    Subject = "Thông báo duyệt Test đầu vào",
                    Body = $@"
                        <h3>Chào {ten},</h3>
                        <p>Yêu cầu Test đầu vào cho khóa học <b>{khoaHoc}</b> của bạn đã được phê duyệt 🎯.</p>
                        <p>Bạn có thể vào hệ thống để bắt đầu làm bài test tại: 
                        <a href='https://localhost:7153/TestDauVao/LamBai'>Làm bài Test ngay</a></p>",
                    IsBodyHtml = true
                };
                smtp.Send(message);
            }
            catch { }
        }

        // ======================================================
        // Đọc file câu hỏi JSON
        // ======================================================
        private List<Question> ReadQuestions(string folder, string file, string skill)
        {
            string path = Path.Combine(_env.WebRootPath, "data", folder, file);
            if (!System.IO.File.Exists(path))
                return new List<Question>();

            var json = System.IO.File.ReadAllText(path);
            var list = JsonConvert.DeserializeObject<List<Question>>(json) ?? new List<Question>();

            foreach (var q in list)
            {
                q.Skill = skill;
                if (skill == "Listening")
                    q.Audio = $"/data/{folder}/audio.mp3";
            }

            return list;
        }

        // ======================================================
        // Cấu trúc câu hỏi
        // ======================================================
        public class Question
        {
            public int Id { get; set; }
            public string QuestionText { get; set; } = string.Empty;
            public List<string> Options { get; set; } = new();
            public int Answer { get; set; }
            public string Skill { get; set; } = "";
            public string? Audio { get; set; }
            public string? Passage { get; set; }
        }
    }
}
