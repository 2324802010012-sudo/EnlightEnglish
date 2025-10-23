using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnlightEnglishCenter.Models
{
    [Table("HocVien")] // ✅ Ánh xạ đúng tên bảng trong SQL
    public class HocVien
    {
        [Key]
        [Display(Name = "Mã học viên")]
        public int MaHocVien { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty; // ✅ Không nullable vì luôn phải có

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; } // ✅ Có thể để trống

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string? DienThoai { get; set; } // ✅ Có thể để trống

        public string? DiaChi { get; set; }

        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        [Display(Name = "Trình độ hiện tại")]
        public string? TrinhDo { get; set; }

        [Display(Name = "Khóa học đăng ký")]
        public string? KhoaHoc { get; set; }

        [Display(Name = "Lớp học")]
        public string? LopHoc { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày bắt đầu học")]
        public DateTime? NgayBatDauHoc { get; set; }

        [Display(Name = "Tình trạng học")]
        public string? TinhTrangHoc { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày đăng ký")]
        public DateTime NgayDangKy { get; set; } = DateTime.Now; // ✅ Gán mặc định để tránh null

        [Display(Name = "Ghi chú thêm")]
        public string? GhiChu { get; set; }
    }
}
