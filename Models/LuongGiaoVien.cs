using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Models;

[Table("LuongGiaoVien")]
public partial class LuongGiaoVien
{
    [Key]
    public int MaLuong { get; set; }

    public int? MaGiaoVien { get; set; }

    public int? Thang { get; set; }

    public int? Nam { get; set; }

    public int? SoBuoiDay { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? LuongMoiBuoi { get; set; }

    [Column(TypeName = "decimal(21, 2)")]
    public decimal? TongLuong { get; set; }

    [ForeignKey("MaGiaoVien")]
    [InverseProperty("LuongGiaoViens")]
    public virtual NguoiDung? MaGiaoVienNavigation { get; set; }
}
