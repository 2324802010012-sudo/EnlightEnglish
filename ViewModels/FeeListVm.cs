using EnlightEnglishCenter.Models;

namespace EnlightEnglishCenter.ViewModels
{
    public class FeeListVm
    {
        public required List<DonHocPhi> Items { get; set; }
        public required FinanceFilterVm Filter { get; set; }
        public int Total { get; set; }
    }
}
