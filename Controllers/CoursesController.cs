using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnlightEnglishCenter.Data;
using Microsoft.AspNetCore.Http; // ✅ Thêm dòng này để dùng HttpContext.Session

namespace EnlightEnglishCenter.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Trang IELTS
        public IActionResult IELTS()
        {
            ViewData["Title"] = "Khóa học IELTS";
            ViewBag.Description = "Khóa học IELTS giúp học viên rèn luyện 4 kỹ năng Nghe – Nói – Đọc – Viết, " +
                "tập trung vào kỹ năng học thuật và bài thi thực tế. Lộ trình được chia theo từng band điểm: " +
                "từ nền tảng (3.0–4.5), trung cấp (5.0–6.0), đến nâng cao (6.5+).";

            ViewBag.LoTrinh = new[]
            {
                "Band 3.0–4.5: Ôn ngữ pháp nền tảng, luyện nghe – đọc cơ bản.",
                "Band 5.0–6.0: Mở rộng từ vựng học thuật, luyện viết Task 1 & 2.",
                "Band 6.5+: Luyện đề, phản xạ nói và chiến thuật thi nâng cao."
            };

            return View();
        }

        // Trang TOEIC
        public IActionResult TOEIC()
        {
            ViewData["Title"] = "Khóa học TOEIC";
            ViewBag.Description = "Khóa TOEIC hướng đến sinh viên và người đi làm, " +
                "tập trung vào tiếng Anh giao tiếp và môi trường công sở. " +
                "Lộ trình được chia theo mục tiêu điểm thi.";

            ViewBag.LoTrinh = new[]
            {
                "Mục tiêu 450–550: Ngữ pháp, từ vựng và nghe cơ bản.",
                "Mục tiêu 600–750: Tăng tốc luyện đề, phản xạ nghe nhanh.",
                "Mục tiêu 800+: Chuyên sâu kỹ năng đọc hiểu và phân tích câu phức."
            };

            return View();
        }

        // Trang Cambridge
        public IActionResult Cambridge()
        {
            ViewData["Title"] = "Khóa học Cambridge";
            ViewBag.Description = "Khóa Cambridge dành cho thiếu nhi và thiếu niên, " +
                "giúp học sinh tiếp cận tiếng Anh tự nhiên, học qua trò chơi và tương tác. " +
                "Lộ trình học theo chuẩn Cambridge (Starters, Movers, Flyers, KET, PET).";

            ViewBag.LoTrinh = new[]
            {
                "Starters–Movers: Làm quen phát âm, từ vựng, hội thoại cơ bản.",
                "Flyers–KET: Ngữ pháp nâng cao, viết câu hoàn chỉnh.",
                "PET: Chuẩn bị chứng chỉ Cambridge và nâng cao phản xạ giao tiếp."
            };

            return View();
        }

        // ---------------------- ĐĂNG KÝ TEST ----------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangKyTest(TestDangKyViewModel model)
        {
            int? maHocVien = HttpContext.Session.GetInt32("MaNguoiDung");

            // ✅ Nếu chưa đăng nhập
            if (maHocVien == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập trước khi đăng ký Test đầu vào.";
                return RedirectToAction("Login", "Account");
            }

            // ✅ Kiểm tra học viên đã đăng ký test cho khóa này chưa
            bool daDangKy = _context.TestDauVaos.Any(t =>
                t.MaHocVien == maHocVien.Value &&
                t.KhoaHocDeXuat == model.KhoaHocDeXuat &&
                t.TrangThai == "Chờ xác nhận"
            );

            if (daDangKy)
            {
                TempData["Error"] = $"⚠️ Bạn đã đăng ký Test đầu vào cho khóa '{model.KhoaHocDeXuat}' rồi. Vui lòng chờ xác nhận.";
                return RedirectToAction("Index", "TestDauVao");
            }

            // ✅ Tạo bản ghi mới
            var test = new TestDauVao
            {
                MaHocVien = maHocVien.Value,
                NgayTest = DateTime.Now,
                KhoaHocDeXuat = model.KhoaHocDeXuat,
                TrangThai = "Chờ xác nhận"
            };

            _context.TestDauVaos.Add(test);
            _context.SaveChanges();

            TempData["Success"] = $"🎉 Đăng ký Test đầu vào cho khóa '{model.KhoaHocDeXuat}' thành công! Vui lòng chờ phòng đào tạo xác nhận.";

            return RedirectToAction("Index", "Home");
        }
    }
}
