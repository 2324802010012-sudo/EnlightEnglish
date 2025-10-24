using System.Collections.Generic;
using EnlightEnglishCenter.Models;

namespace EnlightEnglishCenter.ViewModels
{
    public class LienHeHocVienViewModel
    {
        public IEnumerable<HocVien> HocViens { get; set; } = new List<HocVien>();
        public IEnumerable<LienHeKhachHang> KhachHangs { get; set; } = new List<LienHeKhachHang>();
    }
}
