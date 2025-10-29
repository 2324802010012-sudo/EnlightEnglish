using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnlightEnglishCenter.Models
{
    [Table("DonHocPhi")]
    public class DonHocPhi
    {
        [Key]
        public int MaDon { get; set; }

        public int MaHocVien { get; set; }
        public int MaLop { get; set; }

        public decimal TongTien { get; set; }
        public string TrangThai { get; set; } = "Chờ thanh toán";
        public DateTime NgayTao { get; set; }
        public DateTime? NgayThanhToan { get; set; }
        [ForeignKey(nameof(MaHocVien))]
        public HocVien HocVien { get; set; }

        [ForeignKey(nameof(MaLop))]
        public LopHoc LopHoc { get; set; }
    }
}
