﻿using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using EnlightEnglishCenter.ViewModels;

namespace EnlightEnglishCenter.Controllers
{
    public class LeTanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeTanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🧾 Đăng ký học viên (từ modal)

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKyHocVien(HocVien model)
        {
            var dangKyList = (from dk in _context.DkHocVienLopHocs
                              join hv in _context.NguoiDungs on dk.MaHocVien equals hv.MaNguoiDung
                              join lop in _context.LopHocs on dk.MaLop equals lop.MaLop
                              select new
                              {
                                  dk.MaHocVien,
                                  dk.MaLop,
                                  hv.HoTen,
                                  Lop = lop.TenLop,
                                  dk.NgayDangKy,
                                  dk.TrangThai,
                                  dk.TrangThaiHoc
                              }).ToList();
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "⚠️ Dữ liệu nhập không hợp lệ.";
                return RedirectToAction("DanhSachHocVien");
            }

            model.NgayDangKy = DateTime.Now;

            _context.HocViens.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Đã đăng ký học viên mới thành công!";
            return RedirectToAction("DanhSachHocVien");
        }



        // ===============================
        // 🧾 2. Danh sách học viên (cho lễ tân)
        // ===============================
        public async Task<IActionResult> DanhSachHocVien()
        {
            var hocViens = await _context.HocViens.ToListAsync();
            return View(hocViens);
        }

        // ===============================
        // ✏️ 3. Sửa học viên
        // ===============================
        // Dùng để load dữ liệu khi bấm "Sửa"

        // ---------------------- TẠO ĐĂNG KÝ MỚI ----------------------
        [HttpGet]
        public async Task<IActionResult> GetHocVien(int id)
        {
            var hv = await _context.HocViens.FindAsync(id);
            if (hv == null) return NotFound();
            return Json(hv);
        }

        // Dùng để lưu thay đổi
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


        // ===============================
        // ❌ 4. Xóa học viên
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaHocVien(int id)
        {
            var hv = await _context.HocViens.FindAsync(id);
            if (hv == null)
            {
                TempData["Error"] = "Không tìm thấy học viên để xóa!";
                return RedirectToAction("DanhSachHocVien");
            }

            _context.HocViens.Remove(hv);
            await _context.SaveChangesAsync();

            TempData["Success"] = "🗑️ Đã xóa học viên thành công!";
            return RedirectToAction("DanhSachHocVien");
        }


        // ===============================
        // ✅ Tùy chọn: View index tổng hợp (nếu cần)
        // ===============================
        public async Task<IActionResult> Index()
        {
            var hocVienRole = await _context.VaiTros.FirstOrDefaultAsync(v => v.TenVaiTro == "Học viên");
            int? roleId = hocVienRole?.MaVaiTro;

            var ds = _context.NguoiDungs.AsQueryable();
            if (roleId != null)
                ds = ds.Where(n => n.MaVaiTro == roleId);

            var list = await ds.ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> LienHeHocVien()
        {
            var viewModel = new LienHeHocVienViewModel
            {
                HocViens = await _context.HocViens.ToListAsync(),
                KhachHangs = await _context.LienHeKhachHang.ToListAsync()
            };
            return View(viewModel);
        }
        // ===============================
        // 📞 Thêm khách hàng tiềm năng
        // ===============================
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

        // Optional: action để lấy chi tiết (AJAX)
        [HttpGet]
        public async Task<IActionResult> ChiTietLienHe(int id, string loai)
        {
            if (loai == "KhachHang")
            {
                var kh = await _context.LienHeKhachHang.FindAsync(id);
                if (kh == null) return NotFound();
                return PartialView("_PartialChiTietKhachHang", kh);
            }
            else if (loai == "HocVien")
            {
                var hv = await _context.HocViens.FindAsync(id);
                if (hv == null) return NotFound();
                return PartialView("_PartialChiTietHocVien", hv);
            }
            return BadRequest();
        }
    }
}
