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

            // Kiểm tra học viên đã có test chưa
            var testCu = _context.TestDauVaos.FirstOrDefault(t => t.MaHocVien == maHocVien);
            if (testCu != null)
            {
                TempData["Error"] = "⚠️ Bạn đã đăng ký hoặc hoàn thành Test đầu vào.";
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
                KhoaHocDeXuat = khoaHoc.MaKhoaHoc,
                NgayTest = DateTime.Now,
                TrangThai = "Chờ xác nhận"
            };

            _context.TestDauVaos.Add(test);
            _context.SaveChanges();

            TempData["Success"] = $"✅ Đăng ký Test đầu vào cho khóa '{khoaHoc.TenKhoaHoc}' thành công! Vui lòng chờ duyệt.";
            return RedirectToAction("Index", "HocVien");
        }

        // ======================================================
        // 3️⃣ Admin xem danh sách và duyệt Test
        // ======================================================
        public IActionResult DanhSach()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "Admin" && vaiTro != "Phòng đào tạo")
            {
                TempData["Error"] = "⚠️ Chỉ Admin hoặc Phòng đào tạo được phép truy cập.";
                return RedirectToAction("Index", "Home");
            }

            var ds = _context.TestDauVaos
                .Include(t => t.HocVien)
                .Include(t => t.KhoaHocDeXuatNavigation)
                .OrderByDescending(t => t.NgayTest)
                .ToList();

            return View(ds);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DuyetTest(int id)
        {
            var test = _context.TestDauVaos
                .Include(t => t.HocVien)
                .Include(t => t.KhoaHocDeXuatNavigation)
                .FirstOrDefault(t => t.MaTest == id);

            if (test == null)
            {
                TempData["Error"] = "Không tìm thấy bài test cần duyệt!";
                return RedirectToAction("DanhSach");
            }

            test.TrangThai = "Được phép test";
            _context.SaveChanges();

            GuiEmailThongBao(
                test.HocVien?.Email ?? "",
                test.HocVien?.HoTen ?? "Học viên",
                test.KhoaHocDeXuatNavigation?.TenKhoaHoc ?? "Khóa học"
            );

            TempData["Success"] = "✅ Đã duyệt học viên làm test.";
            return RedirectToAction("DanhSach");
        }

        // ======================================================
        // 4️⃣ Học viên làm bài test
        // ======================================================
        public IActionResult LamBai()
        {
            int? maHocVien = HttpContext.Session.GetInt32("MaNguoiDung");
            if (maHocVien == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập trước khi làm bài test.";
                return RedirectToAction("Login", "Account");
            }

            var test = _context.TestDauVaos
                .Include(t => t.HocVien)
                .Include(t => t.KhoaHocDeXuatNavigation)
                .FirstOrDefault(t => t.MaHocVien == maHocVien && t.TrangThai == "Được phép test");

            if (test == null)
            {
                TempData["Error"] = "❌ Bạn chưa được duyệt để làm bài test.";
                return RedirectToAction("Index", "HocVien");
            }

            string khoaHoc = test.KhoaHocDeXuatNavigation?.TenKhoaHoc?.ToLower() ?? "ielts";
            if (khoaHoc.Contains("cambridge")) khoaHoc = "cambridge";
            else if (khoaHoc.Contains("toeic")) khoaHoc = "toeic";
            else khoaHoc = "ielts";

            var grammar = ReadQuestions(khoaHoc, "questions.json", "Grammar");
            var reading = ReadQuestions(khoaHoc, "reading.json", "Reading");
            var listening = ReadQuestions(khoaHoc, "listening.json", "Listening");

            var allQuestions = grammar.Concat(reading).Concat(listening).ToList();

            ViewBag.Course = test.KhoaHocDeXuatNavigation?.TenKhoaHoc ?? "";
            ViewBag.HoTen = test.HocVien?.HoTen ?? "";
            ViewBag.TestId = test.MaTest;

            return View(allQuestions);
        }


        // ======================================================
        // 5️⃣ Nộp bài & tính điểm
        // ======================================================
        // ======================================================
        // 5️⃣ Nộp bài & tính điểm (học viên làm test online)
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

            // Quy về thang 10
            double score10 = count > 0 ? (correct / (double)count) * 10 : 0;

            string deXuat = score10 < 4 ? "Cấp độ Cơ bản"
                          : score10 < 7 ? "Cấp độ Trung bình"
                          : "Cấp độ Nâng cao";

            int? maHocVien = HttpContext.Session.GetInt32("MaNguoiDung");
            if (maHocVien == null)
                return RedirectToAction("Login", "Account");

            var test = _context.TestDauVaos
                .Include(t => t.HocVien)
                .Include(t => t.KhoaHocDeXuatNavigation)
                .FirstOrDefault(t => t.MaHocVien == maHocVien && t.TrangThai == "Được phép test");

            if (test != null)
            {
                // Ghi điểm: có thể dồn vào DiemNguPhap hoặc chia ra các kỹ năng nếu bạn tách bài
                test.DiemNguPhap = (decimal)score10;
                test.TongDiem = (decimal)score10;     // ✅ lưu tổng điểm đúng cột có thật
                test.TrangThai = "Hoàn thành";
                test.NgayTest = DateTime.Now;
                test.LoTrinhHoc = deXuat;             // ✅ gợi ý lộ trình

                _context.SaveChanges();

                GuiEmailKetQua(
                    test.HocVien?.Email ?? "",
                    test.HocVien?.HoTen ?? "Học viên",
                    score10,
                    deXuat
                );
            }

            ViewBag.Score = score10;
            ViewBag.DeXuat = deXuat;
            ViewBag.Course = course;

            return View("KetQua");
        }



        // ======================================================
        // 5️⃣-bis. Cập nhật điểm test (Admin cập nhật hoặc chấm tay)
        // ======================================================
        [HttpPost]
        [Route("TestDauVao/CapNhatDiem")]
        public async Task<IActionResult> CapNhatDiem(int id, double diem)
        {
            var test = await _context.TestDauVaos
                .Include(t => t.KhoaHocDeXuatNavigation)
                .FirstOrDefaultAsync(t => t.MaTest == id);

            if (test == null) return NotFound();

            test.TongDiem = (decimal)diem;  // ✅ cột có thật
            test.TrangThai = "Hoàn thành";

            // ✅ Gợi ý lộ trình theo khóa đề xuất & điểm
            string loTrinh = "Cấp độ Cơ bản";
            var ten = test.KhoaHocDeXuatNavigation?.TenKhoaHoc?.ToUpperInvariant() ?? "";

            if (ten.Contains("IELTS"))
                loTrinh = diem >= 8 ? "IELTS Nâng cao" : diem >= 6 ? "IELTS Trung bình" : "IELTS Cơ bản";
            else if (ten.Contains("TOEIC"))
                loTrinh = diem >= 8 ? "TOEIC Nâng cao" : diem >= 6 ? "TOEIC Trung bình" : "TOEIC Cơ bản";
            else if (ten.Contains("CAMBRIDGE"))
                loTrinh = diem >= 8 ? "Cambridge Nâng cao" : diem >= 6 ? "Cambridge Trung bình" : "Cambridge Cơ bản";

            test.LoTrinhHoc = loTrinh;       // ✅ cột có thật

            await _context.SaveChangesAsync();
            TempData["Success"] = $"✅ Đã cập nhật điểm và lộ trình: {loTrinh}";
            return RedirectToAction("KetQua", new { id = test.MaTest });
        }



        // ======================================================
        // 6️⃣ Trang Kết quả
        // ======================================================
        [HttpGet]
        public IActionResult KetQua(int? id)
        {
            int? maHocVien = HttpContext.Session.GetInt32("MaNguoiDung");
            if (maHocVien == null) return RedirectToAction("Login", "Account");

            var query = _context.TestDauVaos
                .Include(t => t.HocVien)
                .Include(t => t.KhoaHocDeXuatNavigation)
                .AsQueryable();

            var test = id.HasValue
                ? query.FirstOrDefault(t => t.MaTest == id.Value && t.MaHocVien == maHocVien)
                : query.Where(t => t.MaHocVien == maHocVien)
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
