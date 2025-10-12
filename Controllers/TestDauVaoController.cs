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
        public IActionResult Index()
        {
            var khoaHoc = _context.KhoaHocs.ToList();
            return View(khoaHoc);
        }

        // ======================================================
        // 2️⃣ Học viên đăng ký Test đầu vào
        // ======================================================
        [HttpGet]
        public IActionResult DangKy(int maKhoaHoc = 1)
        {
            int? maHocVien = HttpContext.Session.GetInt32("MaNguoiDung");
            if (maHocVien == null)
            {
                TempData["Error"] = "⚠️ Bạn cần đăng nhập trước khi đăng ký Test đầu vào.";
                return RedirectToAction("Login", "Account");
            }

            // ✅ Kiểm tra học viên đã có bài test nào chưa
            var testCu = _context.TestDauVaos.FirstOrDefault(t => t.MaHocVien == maHocVien);
            if (testCu != null)
            {
                if (testCu.TrangThai == "Hoàn thành")
                    TempData["Error"] = "⚠️ Bạn đã hoàn thành bài Test đầu vào. Mỗi học viên chỉ được làm một lần.";
                else if (testCu.TrangThai == "Được phép test")
                    TempData["Error"] = "⚠️ Bạn đã được duyệt để làm Test, không thể đăng ký lại.";
                else
                    TempData["Error"] = "⚠️ Bạn đã đăng ký Test đầu vào và đang chờ xác nhận.";

                return RedirectToAction("Index", "HocVien");
            }

            var khoaHoc = _context.KhoaHocs.Find(maKhoaHoc);
            if (khoaHoc == null)
            {
                TempData["Error"] = "❌ Không tìm thấy khóa học.";
                return RedirectToAction("Index", "HocVien");
            }

            var test = new TestDauVao
            {
                MaHocVien = maHocVien.Value,
                KhoaHocDeXuat = khoaHoc.TenKhoaHoc,
                NgayTest = DateTime.Now,
                TrangThai = "Chờ xác nhận"
            };

            _context.TestDauVaos.Add(test);
            _context.SaveChanges();

            TempData["Success"] = "✅ Đăng ký Test đầu vào thành công! Vui lòng chờ Admin duyệt.";
            return RedirectToAction("Index", "HocVien");
        }

        // ======================================================
        // 3️⃣ Admin xem danh sách và duyệt Test
        // ======================================================
        public IActionResult DanhSach()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "Admin")
            {
                TempData["Error"] = "⚠️ Chỉ Admin mới được phép truy cập.";
                return RedirectToAction("Index", "Home");
            }

            var ds = _context.TestDauVaos
                .Include(t => t.HocVien)
                .OrderByDescending(t => t.NgayTest)
                .ToList();

            return View(ds);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DuyetTest(int id)
        {
            var test = _context.TestDauVaos.Include(t => t.HocVien).FirstOrDefault(t => t.MaTest == id);
            if (test == null)
            {
                TempData["Error"] = "Không tìm thấy bài test cần duyệt!";
                return RedirectToAction("DanhSach");
            }

            test.TrangThai = "Được phép test";
            _context.SaveChanges();

            GuiEmailThongBao(test.HocVien?.Email ?? "", test.HocVien?.HoTen ?? "Học viên", test.KhoaHocDeXuat ?? "Khóa học");
            TempData["Success"] = "✅ Đã duyệt học viên làm test.";
            return RedirectToAction("DanhSach");
        }

        // ======================================================
        // 4️⃣ Đọc file JSON câu hỏi
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
        // 5️⃣ Học viên làm bài test
        // ======================================================
        public IActionResult LamBai()
        {
            int? maHocVien = HttpContext.Session.GetInt32("MaNguoiDung");
            if (maHocVien == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập trước khi làm bài test.";
                return RedirectToAction("Login", "Account");
            }

            // ✅ Nếu học viên đã hoàn thành thì không được làm lại
            var daLamXong = _context.TestDauVaos.Any(t => t.MaHocVien == maHocVien && t.TrangThai == "Hoàn thành");
            if (daLamXong)
            {
                TempData["Error"] = "⚠️ Bạn đã hoàn thành bài test đầu vào rồi, không thể làm lại!";
                return RedirectToAction("Index", "HocVien");
            }

            var test = _context.TestDauVaos.Include(t => t.HocVien)
                .FirstOrDefault(t => t.MaHocVien == maHocVien && t.TrangThai == "Được phép test");

            if (test == null)
            {
                TempData["Error"] = "❌ Bạn chưa được duyệt để làm bài test.";
                return RedirectToAction("Index", "HocVien");
            }

            // ✅ Xác định thư mục câu hỏi tương ứng với khóa học
            string khoaHoc = test.KhoaHocDeXuat?.Trim().ToLower() ?? "ielts";
            if (khoaHoc.Contains("cambridge")) khoaHoc = "cambridge";
            else if (khoaHoc.Contains("toeic")) khoaHoc = "toeic";
            else khoaHoc = "ielts";

            // ✅ Đọc dữ liệu câu hỏi
            var grammar = ReadQuestions(khoaHoc, "questions.json", "Grammar");
            var reading = ReadQuestions(khoaHoc, "reading.json", "Reading");
            var listening = ReadQuestions(khoaHoc, "listening.json", "Listening");

            var allQuestions = grammar.Concat(reading).Concat(listening).ToList();

            ViewBag.Course = khoaHoc;
            ViewBag.HoTen = test.HocVien?.HoTen ?? "";
            ViewBag.TestId = test.MaTest;

            return View(allQuestions);
        }

        // ======================================================
        // 6️⃣ Nộp bài & tính điểm
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
            {
                if (answers[i] == correctAnswers[i])
                    correct++;
            }

            double score = count > 0 ? (correct / (double)count) * 10 : 0;
            if (double.IsNaN(score) || double.IsInfinity(score)) score = 0;

            string deXuat = score < 4 ? "Cấp độ Cơ bản (Beginner)" :
                            score < 7 ? "Cấp độ Trung bình (Intermediate)" :
                                        "Cấp độ Nâng cao (Advanced)";

            int? maHocVien = HttpContext.Session.GetInt32("MaNguoiDung");
            if (maHocVien == null)
                return RedirectToAction("Login", "Account");

            var test = _context.TestDauVaos.Include(t => t.HocVien)
                .FirstOrDefault(t => t.MaHocVien == maHocVien && t.TrangThai == "Được phép test");

            if (test != null)
            {
                test.DiemNguPhap = (decimal)score;
                test.KhoaHocDeXuat = deXuat;
                test.TrangThai = "Hoàn thành";
                test.NgayTest = DateTime.Now;
                _context.SaveChanges();

                GuiEmailKetQua(test.HocVien?.Email ?? "", test.HocVien?.HoTen ?? "Học viên", score, deXuat);
            }

            ViewBag.Score = score;
            ViewBag.DeXuat = deXuat;
            ViewBag.Course = course;

            return View("KetQua");
        }

        // ======================================================
        // 7️⃣ Trang kết quả test
        // ======================================================
        public IActionResult KetQua()
        {
            int? maHocVien = HttpContext.Session.GetInt32("MaNguoiDung");
            if (maHocVien == null)
                return RedirectToAction("Login", "Account");

            var test = _context.TestDauVaos
                .Where(t => t.MaHocVien == maHocVien)
                .OrderByDescending(t => t.NgayTest)
                .FirstOrDefault();

            if (test == null)
            {
                TempData["Error"] = "❌ Bạn chưa làm bài test nào!";
                return RedirectToAction("Index", "HocVien");
            }

            ViewBag.Score = test.DiemNguPhap ?? 0;
            ViewBag.DeXuat = test.KhoaHocDeXuat ?? "Chưa có lộ trình";

            return View();
        }

        // ======================================================
        // 8️⃣ Gửi email kết quả test
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

        // ======================================================
        // 9️⃣ Gửi email khi Admin duyệt test
        // ======================================================
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
                        <p>Bạn có thể vào hệ thống để bắt đầu làm bài test.</p>",
                    IsBodyHtml = true
                };

                smtp.Send(message);
            }
            catch { }
        }

        // ======================================================
        // 🔟 Cấu trúc câu hỏi
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
