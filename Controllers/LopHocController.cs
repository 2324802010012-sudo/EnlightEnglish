using Microsoft.EntityFrameworkCore;
using EnlightEnglishCenter.Models;
using EnlightEnglishCenter.Data;
using Microsoft.AspNetCore.Mvc;

public class LopHocController : Controller
{
    private readonly ApplicationDbContext _context;

    public LopHocController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult ChiTiet(int id)
    {
        int? maNguoiDung = HttpContext.Session.GetInt32("MaNguoiDung");
        if (maNguoiDung == null)
            return RedirectToAction("Login", "Account");

        var hocVien = _context.HocViens.FirstOrDefault(h => h.MaNguoiDung == maNguoiDung);
        if (hocVien == null)
            return RedirectToAction("Index", "HocVien");

        // ✅ Lấy lớp học + khóa học + giáo viên + lịch học + điểm
        var lop = _context.LopHocs
            .Include(l => l.MaKhoaHocNavigation)
            .Include(l => l.MaGiaoVienNavigation)
                .ThenInclude(gv => gv.NguoiDung) // 🔥 Load luôn tên giảng viên
            .Include(l => l.LichHocs)
            .Include(l => l.DiemSos.Where(d => d.MaHocVien == hocVien.MaHocVien))
            .AsNoTracking()
            .FirstOrDefault(l => l.MaLop == id);

        if (lop == null)
            return NotFound();

        // ✅ Lấy tên giảng viên từ navigation
        string tenGv = lop.MaGiaoVienNavigation?.NguoiDung?.HoTen ?? "Chưa phân công";

        // ✅ Truyền dữ liệu ra view
        ViewBag.GiangVien = tenGv;
        ViewBag.TenLop = lop.TenLop;
        ViewBag.KhoaHoc = lop.MaKhoaHocNavigation?.TenKhoaHoc ?? "—";
        ViewBag.NgayBatDau = lop.MaKhoaHocNavigation?.NgayBatDau?.ToString("dd/MM/yyyy");
        ViewBag.NgayKetThuc = lop.MaKhoaHocNavigation?.NgayKetThuc?.ToString("dd/MM/yyyy");
        ViewBag.SiSoHienTai = lop.SiSoHienTai;
        ViewBag.SiSoToiDa = lop.SiSoToiDa;
        ViewBag.TrangThai = lop.TrangThai;

        return View(lop);
    }
}
