using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Models;

[Table("LichSuTruyCap")]
[Index("MaNguoiDung", Name = "IX_LichSuTruyCap_MaNguoiDung")]
public partial class LichSuTruyCap
{
    [Key]
    public int MaLog { get; set; }

    public int MaNguoiDung { get; set; }

    public string? HanhDong { get; set; }

    public DateTime ThoiGian { get; set; }

    [Column("DiaChiIP")]
    public string? DiaChiIp { get; set; }
    [StringLength(50)]
    public string? DiaChiIP { get; set; }


    public string? MoTa { get; set; }

    [ForeignKey("MaNguoiDung")]
    [InverseProperty("LichSuTruyCaps")]
    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;
}
