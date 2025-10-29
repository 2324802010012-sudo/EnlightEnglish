using EnlightEnglishCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace EnlightEnglishCenter.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ==========================
        // DbSet
        // ==========================
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<VaiTro> VaiTros { get; set; }
        public DbSet<KhoaHoc> KhoaHocs { get; set; }
        public DbSet<LopHoc> LopHocs { get; set; }
        public DbSet<HocVien> HocViens { get; set; }
        public DbSet<LienHeKhachHang> LienHeKhachHang { get; set; }
        public DbSet<TestDauVao> TestDauVaos { get; set; }
        public DbSet<HocPhi> HocPhis { get; set; }
        public DbSet<LuongGiaoVien> LuongGiaoViens { get; set; }
        public DbSet<LichHoc> LichHocs { get; set; }
        public DbSet<LichThi> LichThis { get; set; }
        public DbSet<DiemSo> DiemSos { get; set; }
        public DbSet<TaiLieu> TaiLieus { get; set; }
        public DbSet<DiemDanh> DiemDanhs { get; set; }
        public DbSet<LichSuTruyCap> LichSuTruyCaps { get; set; }
        public DbSet<BaoCao> BaoCaos { get; set; }
        public DbSet<DkHocVienLopHoc> DkHocVienLopHocs { get; set; }
        public DbSet<DangKyTuVan> DangKyTuVan { get; set; }
        public DbSet<GiaoVien> GiaoViens { get; set; }
        public DbSet<PhanCongGiangDay> PhanCongGiangDays { get; set; }
        public DbSet<PhongDaoTao> PhongDaoTaos { get; set; }
        public DbSet<NhanVienLeTan> NhanVienLeTans { get; set; }
        public DbSet<PhongHoc> PhongHocs { get; set; }
        public DbSet<DonHocPhi> DonHocPhis { get; set; }


        // ==========================
        // Fluent config
        // ==========================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DiemSo>(e =>
            {
                e.ToTable("DiemSo");
                e.HasKey(x => x.MaDiem);

                e.Property(x => x.DiemGiuaKy).HasColumnType("decimal(18,2)");
                e.Property(x => x.DiemCuoiKy).HasColumnType("decimal(18,2)");
                e.Property(x => x.NhanXet).HasMaxLength(255);

                e.HasIndex(x => x.MaHocVien);
                e.HasIndex(x => x.MaLop);

                e.HasOne(x => x.MaHocVienNavigation)
                    .WithMany(nd => nd.DiemSos)   // đúng theo InverseProperty trong model
                    .HasForeignKey(x => x.MaHocVien)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.MaLopNavigation)
                    .WithMany(l => l.DiemSos)     // đúng theo InverseProperty trong model
                    .HasForeignKey(x => x.MaLop)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // --- KhoaHoc ---
            modelBuilder.Entity<KhoaHoc>(e =>
            {
                e.ToTable("KhoaHoc");
                e.Property(x => x.TenKhoaHoc).HasMaxLength(100).IsRequired();
                e.Property(x => x.MoTa).HasMaxLength(255);
                e.Property(x => x.ThoiLuong).HasMaxLength(50);
                e.Property(x => x.TrangThai).HasMaxLength(20).HasDefaultValue("Đang mở");

                // DB KHÔNG có các cột này -> bỏ qua để EF không query
                e.Ignore(x => x.CapDo);
                e.Ignore("TrinhDo");
                e.Ignore("GiangVien");
                e.Ignore("HinhThuc");
                e.Ignore("LoTrinhHoc");
                e.Ignore("NgayKhaiGiang");
                e.Ignore("ThoiGianHoc");

                // 1-n: KhoaHoc - LopHoc
                e.HasMany(k => k.LopHocs)
                 .WithOne(l => l.MaKhoaHocNavigation)
                 .HasForeignKey(l => l.MaKhoaHoc)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // --- LopHoc ---
            modelBuilder.Entity<LopHoc>(e =>
            {
                e.ToTable("LopHoc");
                e.HasKey(x => x.MaLop);
                e.Property(x => x.TenLop).HasMaxLength(100);
                e.Property(x => x.TrangThai).HasMaxLength(100).HasDefaultValue("Đang học");

                e.Property(x => x.SiSoToiDa).HasDefaultValue(20);
                e.Property(x => x.SiSoHienTai).HasDefaultValue(0);

                // KHÔNG cấu hình NgayBatDau/NgayKetThuc ở LopHoc vì bảng không có
                // (Ngày bắt đầu/kết thúc lấy từ bảng KhoaHoc)
            });

            // --- NguoiDung - VaiTro ---
            modelBuilder.Entity<NguoiDung>()
                .HasOne(u => u.MaVaiTroNavigation)
                .WithMany(v => v.NguoiDungs)
                .HasForeignKey(u => u.MaVaiTro)
                .OnDelete(DeleteBehavior.Restrict);

            // --- HocVien ---
            modelBuilder.Entity<HocVien>()
                .HasKey(h => h.MaHocVien);

            // --- DkHocVienLopHoc (composite key) ---
            modelBuilder.Entity<DkHocVienLopHoc>()
                .HasKey(d => new { d.MaHocVien, d.MaLop });

            // --- DiemDanh (composite key) ---
            modelBuilder.Entity<DiemDanh>()
                .HasKey(d => new { d.MaHocVien, d.MaLich });

            // --- TestDauVao ---
            modelBuilder.Entity<TestDauVao>(e =>
            {
                e.ToTable("TestDauVao");
                e.HasKey(t => t.MaTest);

                e.Property(t => t.DiemNghe).HasColumnType("decimal(4,1)");
                e.Property(t => t.DiemDoc).HasColumnType("decimal(4,1)");
                e.Property(t => t.DiemViet).HasColumnType("decimal(4,1)");
                e.Property(t => t.DiemNguPhap).HasColumnType("decimal(4,1)");
                e.Property(t => t.TongDiem).HasColumnType("decimal(4,1)");
                e.Property(t => t.TrangThai).HasMaxLength(30).HasDefaultValue("Chờ xác nhận");
                e.Property(t => t.Email).HasMaxLength(100);
                e.Property(t => t.HoTen).HasMaxLength(100);

                // FK: TestDauVao.MaHocVien -> NguoiDung.MaNguoiDung
                // DÙNG .WithMany() (không tham số) để KHÔNG sinh cột ảo HocVienMaHocVien/NguoiDungMaNguoiDung
                e.HasOne(t => t.HocVien)
                 .WithMany()
                 .HasForeignKey(t => t.MaHocVien)
                 .OnDelete(DeleteBehavior.Restrict);

                // FK: TestDauVao.KhoaHocDeXuat -> KhoaHoc.MaKhoaHoc
                // DÙNG .WithMany() để KHÔNG sinh cột ảo KhoaHocMaKhoaHoc/TestDauVaoMaTest
                e.HasOne(t => t.KhoaHocDeXuatNavigation)
                 .WithMany()
                 .HasForeignKey(t => t.KhoaHocDeXuat)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // --- TaiLieu ---
            modelBuilder.Entity<TaiLieu>()
                .HasOne(t => t.MaGiaoVienNavigation)
                .WithMany(u => u.TaiLieus)
                .HasForeignKey(t => t.MaGiaoVien)
                .OnDelete(DeleteBehavior.Restrict);

           
            // --- BaoCao ---
            modelBuilder.Entity<BaoCao>()
                .HasOne(b => b.NguoiLapNavigation)
                .WithMany(u => u.BaoCaos)
                .HasForeignKey(b => b.NguoiLap)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
