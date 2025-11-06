namespace EnlightEnglishCenter.ViewModels
{
    public class RevenueBucket
    {
        public string Key { get; set; } = "";   // ví dụ "2025-11-05" hoặc "IELTS Foundation"
        public decimal Total { get; set; }
        public int Count { get; set; }
    }

    public class RevenueReportVm
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PaidCount { get; set; }

        public List<RevenueBucket> ByDay { get; set; } = new();
        public List<RevenueBucket> ByMonth { get; set; } = new();
        public List<RevenueBucket> ByCourse { get; set; } = new(); // theo KhoaHoc
        public List<RevenueBucket> ByClass { get; set; } = new(); // theo LopHoc
    }
}
