using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;

namespace EnlightEnglishCenter.Controllers
{
    public class NguoiDungController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NguoiDungController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🟦 Danh sách người dùng
        public async Task<IActionResult> Index()
        {
            var list = await _context.NguoiDungs
                .Include(n => n.MaVaiTroNavigation)
                .ToListAsync();
            return View(list);
        }

        // 🟩 Hiển thị form thêm
        public IActionResult Create()
        {
            ViewBag.VaiTros = _context.VaiTros.ToList();
            return View();
        }

        // 🟩 Xử lý thêm người dùng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NguoiDung model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm người dùng thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.VaiTros = _context.VaiTros.ToList();
            return View(model);
        }

        // 🟨 Hiển thị form sửa
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.NguoiDungs.FindAsync(id);
            if (user == null) return NotFound();

            ViewBag.VaiTros = _context.VaiTros.ToList();
            return View(user);
        }

        // 🟨 Xử lý cập nhật người dùng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NguoiDung model)
        {
            if (id != model.MaNguoiDung) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật thông tin thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
            }
            ViewBag.VaiTros = _context.VaiTros.ToList();
            return View(model);
        }

        // 🟥 Xác nhận xóa
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.NguoiDungs
                .Include(n => n.MaVaiTroNavigation)
                .FirstOrDefaultAsync(n => n.MaNguoiDung == id);
            if (user == null) return NotFound();

            return View(user);
        }

        // 🟥 Xử lý xóa
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.NguoiDungs.FindAsync(id);
            if (user != null)
            {
                _context.NguoiDungs.Remove(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa người dùng thành công!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
