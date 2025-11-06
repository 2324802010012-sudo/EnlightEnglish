namespace EnlightEnglishCenter.ViewModels
{
    public class FinanceFilterVm
    {
        public string? Keyword { get; set; }
        public string? TrangThai { get; set; }   // "", "ChoXacNhan", "DaThanhToan"
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
