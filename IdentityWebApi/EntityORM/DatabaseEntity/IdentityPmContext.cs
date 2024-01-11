using Microsoft.EntityFrameworkCore;

namespace EntityORM.DatabaseEntity;

public partial class IdentityPmContext : DbContext
{
    public IdentityPmContext()
    {
    }

    public IdentityPmContext(DbContextOptions<IdentityPmContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Login> Logins { get; set; }

    public virtual DbSet<Otpvalidate> Otpvalidates { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAuth> UserAuths { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\MSSQL2;User=sa;Password=Sq1231;Database=IdentityPM;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Login>(entity =>
        {
            entity.HasKey(e => e.Username).HasName("PK_IdentityPM_Login");

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
            entity.HasKey(e => e.Username).HasName("PK_IdentityPM_OTPValidate");

            entity.ToTable("OTPValidate");

            entity.HasIndex(e => e.Username, "IX_OTPValidate_Username");

            entity.Property(e => e.Username).HasMaxLength(10);
            entity.Property(e => e.Otp)
                .HasColumnType("numeric(6, 0)")
                .HasColumnName("OTP");
            entity.Property(e => e.RequestedTime).HasColumnType("datetime");
            entity.Property(e => e.RetryAttempt).HasColumnType("numeric(2, 0)");

            entity.HasOne(d => d.UsernameNavigation).WithOne(p => p.Otpvalidate)
                .HasForeignKey<Otpvalidate>(d => d.Username)
                .HasConstraintName("FK_OTPValidate_Login");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_IdentityPM_User");

            entity.ToTable("User");

            entity.HasIndex(e => e.Username, "IX_User_Username");

            entity.Property(e => e.EmailAddress).HasMaxLength(30);
            entity.Property(e => e.FirstName).HasMaxLength(30);
            entity.Property(e => e.LastName).HasMaxLength(30);
            entity.Property(e => e.Phone).HasMaxLength(10);
            entity.Property(e => e.Suffix).HasMaxLength(10);
            entity.Property(e => e.Title).HasMaxLength(5);
            entity.Property(e => e.Username).HasMaxLength(10);

            entity.HasOne(d => d.UsernameNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.Username)
                .HasConstraintName("FK_Login_User");
        });

        modelBuilder.Entity<UserAuth>(entity =>
        {
            entity.HasKey(e => e.Username).HasName("PK_IdentityPM_UserAuth");

            entity.ToTable("UserAuth");

            entity.HasIndex(e => e.Username, "IX_UserAuth_Username");

            entity.Property(e => e.Username).HasMaxLength(10);
            entity.Property(e => e.CreatedTime).HasColumnType("datetime");

            entity.HasOne(d => d.UsernameNavigation).WithOne(p => p.UserAuth)
                .HasForeignKey<UserAuth>(d => d.Username)
                .HasConstraintName("FK_UserAuth_Login");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
