using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnlightEnglishCenter.Models
{
    [Table("GiaoVien")]
    public class GiaoVien
    {
        [Key]
        public int MaGiaoVien { get; set; }

        // 🔹 Khóa ngoại trỏ tới NguoiDung
        public int? MaNguoiDung { get; set; }

        [ForeignKey("MaNguoiDung")]
        [InverseProperty("GiaoViens")]
        public virtual NguoiDung? NguoiDung { get; set; }

        // 🔹 Bổ sung lại thông tin chuyên môn
        [StringLength(100)]
        public string? TrinhDo { get; set; }

        [StringLength(200)]
        public string? KinhNghiem { get; set; }

        [StringLength(100)]
        public string? ChuyenMon { get; set; }

        [StringLength(30)]
        public string? TrangThai { get; set; } = "Đang dạy";

        // 🔹 Quan hệ với lớp học
        [InverseProperty("MaGiaoVienNavigation")]
        public virtual ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();

        // 🔹 Quan hệ với tài liệu và lương
        [InverseProperty("MaGiaoVienNavigation")]
        public virtual ICollection<TaiLieu> TaiLieus { get; set; } = new List<TaiLieu>();

        [InverseProperty("MaGiaoVienNavigation")]
        public virtual ICollection<LuongGiaoVien> LuongGiaoViens { get; set; } = new List<LuongGiaoVien>();
    }
}
