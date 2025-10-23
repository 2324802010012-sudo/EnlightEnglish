using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Models;

[Table("BaoCao")]
[Index("NguoiLap", Name = "IX_BaoCao_NguoiLap")]
public partial class BaoCao
{
    [Key]
    public int MaBaoCao { get; set; }

    [StringLength(100)]
    public string? LoaiBaoCao { get; set; }

    public string? NoiDung { get; set; }

    public int? NguoiLap { get; set; }

    public DateTime NgayLap { get; set; }

    [ForeignKey("NguoiLap")]
    [InverseProperty("BaoCaos")]
    public virtual NguoiDung? NguoiLapNavigation { get; set; }
}
