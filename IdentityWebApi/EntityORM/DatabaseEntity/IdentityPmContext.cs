using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EntityORM.DatabaseEntity
{
    public partial class IdentityPMContext : DbContext
    {
        public IdentityPMContext()
        {
        }

        public IdentityPMContext(DbContextOptions<IdentityPMContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Login> Logins { get; set; } = null!;
        public virtual DbSet<Otpvalidate> Otpvalidates { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserAuth> UserAuths { get; set; } = null!;
        public virtual DbSet<UserEmploymentDetail> UserEmploymentDetails { get; set; } = null!;
        public virtual DbSet<UserLearnDetail> UserLearnDetails { get; set; } = null!;
        public virtual DbSet<UserPersonalDetail> UserPersonalDetails { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            optionsBuilder.UseSqlServer(builder.Build().GetSection("ConnectionDB").Value);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Login>(entity =>
            {
                entity.HasKey(e => e.Username)
                    .HasName("PK_IdentityPM_Login");

                entity.ToTable("Login");

                entity.HasIndex(e => e.Username, "IX_Login_Username");

                entity.Property(e => e.Username).HasMaxLength(10);

                entity.Property(e => e.Key).HasMaxLength(300);

                entity.Property(e => e.LastLoginTime).HasColumnType("datetime");

                entity.Property(e => e.RegisteredDate).HasColumnType("datetime");

                entity.Property(e => e.Salt).HasMaxLength(300);

                entity.Property(e => e.UserRole).HasMaxLength(10);
            });

            modelBuilder.Entity<Otpvalidate>(entity =>
            {
                entity.HasKey(e => e.Username)
                    .HasName("PK_IdentityPM_OTPValidate");

                entity.ToTable("OTPValidate");

                entity.HasIndex(e => e.Username, "IX_OTPValidate_Username");

                entity.Property(e => e.Username).HasMaxLength(10);

                entity.Property(e => e.Otp)
                    .HasColumnType("numeric(6, 0)")
                    .HasColumnName("OTP");

                entity.Property(e => e.RequestedTime).HasColumnType("datetime");

                entity.Property(e => e.RetryAttempt).HasColumnType("numeric(2, 0)");

                entity.HasOne(d => d.UsernameNavigation)
                    .WithOne(p => p.Otpvalidate)
                    .HasForeignKey<Otpvalidate>(d => d.Username)
                    .HasConstraintName("FK_OTPValidate_Login");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasIndex(e => e.Username, "IX_User_Username");

                entity.Property(e => e.EmailAddress).HasMaxLength(30);

                entity.Property(e => e.FirstName).HasMaxLength(30);

                entity.Property(e => e.LastName).HasMaxLength(30);

                entity.Property(e => e.Phone).HasMaxLength(10);

                entity.Property(e => e.Suffix).HasMaxLength(10);

                entity.Property(e => e.Title).HasMaxLength(5);

                entity.Property(e => e.Username).HasMaxLength(10);

                entity.HasOne(d => d.UsernameNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.Username)
                    .HasConstraintName("FK_Login_User");
            });

            modelBuilder.Entity<UserAuth>(entity =>
            {
                entity.HasKey(e => e.Username)
                    .HasName("PK_IdentityPM_UserAuth");

                entity.ToTable("UserAuth");

                entity.HasIndex(e => e.Username, "IX_UserAuth_Username");

                entity.Property(e => e.Username).HasMaxLength(10);

                entity.Property(e => e.CreatedTime).HasColumnType("datetime");

                entity.HasOne(d => d.UsernameNavigation)
                    .WithOne(p => p.UserAuth)
                    .HasForeignKey<UserAuth>(d => d.Username)
                    .HasConstraintName("FK_UserAuth_Login");
            });

            modelBuilder.Entity<UserEmploymentDetail>(entity =>
            {
                entity.ToTable("User_EmploymentDetails");

                entity.HasIndex(e => e.UserId, "IX_User_EmploymentDetails_UserId");

                entity.Property(e => e.EmployerCity).HasMaxLength(50);

                entity.Property(e => e.EmployerName).HasMaxLength(300);

                entity.Property(e => e.IsCurrentEmployer).HasDefaultValueSql("((0))");

                entity.Property(e => e.Role).HasMaxLength(20);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserEmploymentDetails)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserEmploymentDetails_User");
            });

            modelBuilder.Entity<UserLearnDetail>(entity =>
            {
                entity.ToTable("User_LearnDetails");

                entity.HasIndex(e => e.UserId, "IX_User_LearnDetails_UserId");

                entity.Property(e => e.Award).HasMaxLength(50);

                entity.Property(e => e.InstitutionName).HasMaxLength(300);

                entity.Property(e => e.Major).HasMaxLength(100);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserLearnDetails)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserLearnDetails_User");
            });

            modelBuilder.Entity<UserPersonalDetail>(entity =>
            {
                entity.ToTable("User_PersonalDetails");

                entity.HasIndex(e => e.UserId, "IX_User_PersonalDetails_UserId");

                entity.Property(e => e.BirthDate).HasColumnType("datetime");

                entity.Property(e => e.Gender).HasMaxLength(10);

                entity.Property(e => e.LanguageOne).HasMaxLength(10);

                entity.Property(e => e.LanguageTwo).HasMaxLength(10);

                entity.Property(e => e.Location).HasMaxLength(300);

                entity.Property(e => e.Status).HasMaxLength(10);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserPersonalDetails)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserPersonalDetails_User");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
