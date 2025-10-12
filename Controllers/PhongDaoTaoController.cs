using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Controllers
{
    public class PhongDaoTaoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhongDaoTaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ GET: /PhongDaoTao/DanhSach
        public IActionResult DanhSach()
        {
            // Lấy toàn bộ danh sách test đầu vào, bao gồm thông tin học viên
            var ds = _context.TestDauVaos
                .Include(t => t.MaHocVien)
                .ToList();

            return View(ds); // ✅ Trả về đúng biến
        }
    }
}
