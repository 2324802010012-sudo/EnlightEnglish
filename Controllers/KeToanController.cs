using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EnlightEnglishCenter.Controllers
{
    public class KeToanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KeToanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------------------- TRANG CHÍNH ----------------------
        public IActionResult Index()
        {
            ViewData["Title"] = "Trang kế toán";
            return View();
        }

        // ---------------------- HỌC PHÍ ----------------------
        public IActionResult HocPhi()
        {
            var hocPhi = (from hp in _context.HocPhis
                          join hv in _context.NguoiDungs on hp.MaHocVien equals hv.MaNguoiDung
                          join lop in _context.LopHocs on hp.MaLop equals lop.MaLop
                          select new
                          {
                              hp.MaHocPhi,
                              HocVien = hv.HoTen,
                              Lop = lop.TenLop,
                              hp.SoTienPhaiDong,
                              hp.SoTienDaDong,
                              hp.TrangThai,
                              hp.NgayDongCuoi
                          }).ToList();

            return View(hocPhi);
        }

        // ---------------------- CHI TIẾT HỌC PHÍ ----------------------
        public IActionResult Details(int id)
        {
            var hocPhi = (from hp in _context.HocPhis
                          join hv in _context.NguoiDungs on hp.MaHocVien equals hv.MaNguoiDung
                          join lop in _context.LopHocs on hp.MaLop equals lop.MaLop
                          where hp.MaHocPhi == id
                          select new
                          {
                              hp.MaHocPhi,
                              HocVien = hv.HoTen,
                              Lop = lop.TenLop,
                              hp.SoTienPhaiDong,
                              hp.SoTienDaDong,
                              hp.TrangThai,
                              hp.NgayDongCuoi
                          }).FirstOrDefault();

            if (hocPhi == null)
                return NotFound();

            return View(hocPhi);
        }

        // ---------------------- LƯƠNG GIÁO VIÊN ----------------------
        public IActionResult LuongGiaoVien()
        {
            var luong = (from l in _context.LuongGiaoViens
                         join gv in _context.NguoiDungs on l.MaGiaoVien equals gv.MaNguoiDung
                         select new
                         {
                             l.MaLuong,
                             GiaoVien = gv.HoTen,
                             l.Thang,
                             l.Nam,
                             l.SoBuoiDay,
                             l.LuongMoiBuoi,
                             l.TongLuong
                         }).ToList();

            return View(luong);
        }

        // ---------------------- TRANG NHẬP DỮ LIỆU TÍNH LƯƠNG ----------------------
        public IActionResult TinhLuong()
        {
            var giaoVienList = (from gv in _context.NguoiDungs
                                join vtro in _context.VaiTros on gv.MaVaiTro equals vtro.MaVaiTro
                                where vtro.TenVaiTro == "Giáo viên"
                                select new
                                {
                                    gv.MaNguoiDung,
                                    gv.HoTen
                                }).ToList();

            ViewBag.GiaoVienList = giaoVienList;

            return View();
        }

        // ---------------------- XỬ LÝ TÍNH LƯƠNG ----------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TinhLuong(int MaGiaoVien, int Thang, int Nam, decimal LuongMoiBuoi)
        {
            // ✅ Đếm số buổi dạy trong tháng (dựa theo LichHoc)
            var soBuoiDay = (from lich in _context.LichHocs
                             join lop in _context.LopHocs on lich.MaLop equals lop.MaLop
                             where lop.MaGiaoVien == MaGiaoVien
                             select lich)
                             .AsEnumerable()
                             .Where(lich =>
                                lich.NgayHoc.Value.Month == Thang &&
                                lich.NgayHoc.Value.Year == Nam

                             )
                             .Count();

            // ✅ Tạo bản ghi lương mới
            var luong = new LuongGiaoVien
            {
                MaGiaoVien = MaGiaoVien,
                Thang = Thang,
                Nam = Nam,
                SoBuoiDay = soBuoiDay,
                LuongMoiBuoi = LuongMoiBuoi
            };

            _context.LuongGiaoViens.Add(luong);
            _context.SaveChanges();

            TempData["Message"] = $"✅ Đã tính lương cho giáo viên #{MaGiaoVien} ({soBuoiDay} buổi, tổng: {(soBuoiDay * LuongMoiBuoi):N0} đ)";
            return RedirectToAction(nameof(LuongGiaoVien));
        }
    }
}
