using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models; // Đảm bảo bạn có namespace chứa model HocPhi
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
        // Danh sách học phí
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

        // Chi tiết học phí
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

        // GET: Tạo mới
        public IActionResult Create()
        {
            ViewBag.HocVienList = _context.NguoiDungs.ToList();
            ViewBag.LopList = _context.LopHocs.ToList();
            return View();
        }

        // POST: Tạo mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(HocPhi hocPhi)
        {
            if (ModelState.IsValid)
            {
                _context.HocPhis.Add(hocPhi);
                _context.SaveChanges();
                return RedirectToAction(nameof(HocPhi));
            }

            ViewBag.HocVienList = _context.NguoiDungs.ToList();
            ViewBag.LopList = _context.LopHocs.ToList();
            return View(hocPhi);
        }

        // GET: Sửa học phí
        public IActionResult Edit(int id)
        {
            var hocPhi = _context.HocPhis.Find(id);
            if (hocPhi == null)
                return NotFound();

            ViewBag.HocVienList = _context.NguoiDungs.ToList();
            ViewBag.LopList = _context.LopHocs.ToList();
            return View(hocPhi);
        }

        // POST: Sửa học phí
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(HocPhi hocPhi)
        {
            if (ModelState.IsValid)
            {
                _context.HocPhis.Update(hocPhi);
                _context.SaveChanges();
                return RedirectToAction(nameof(HocPhi));
            }

            ViewBag.HocVienList = _context.NguoiDungs.ToList();
            ViewBag.LopList = _context.LopHocs.ToList();
            return View(hocPhi);
        }

        // GET: Xóa học phí
        public IActionResult Delete(int id)
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


        // POST: Xác nhận xóa
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var hocPhi = _context.HocPhis.Find(id);
            if (hocPhi != null)
            {
                _context.HocPhis.Remove(hocPhi);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(HocPhi));
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
        // ---------------------- TÍNH LƯƠNG GIÁO VIÊN ----------------------
        public IActionResult TinhLuong()
        {
            // Lấy danh sách giáo viên
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
                     lich.NgayHoc.HasValue &&
                     lich.NgayHoc.Value.ToDateTime(TimeOnly.MinValue).Month == Thang &&
                     lich.NgayHoc.Value.ToDateTime(TimeOnly.MinValue).Year == Nam)
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
    