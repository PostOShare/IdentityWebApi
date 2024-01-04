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

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\MSSQL2;Database=IdentityPM;User=sa;Password=Sq1231;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Login>(entity =>
        {
            entity.HasKey(e => e.Username).HasName("PK_IdetityPM_Login");

            entity.ToTable("Login");

            entity.HasIndex(e => e.Username, "IX_Login_Username");

            entity.Property(e => e.Username).HasMaxLength(10);
            entity.Property(e => e.Key).HasMaxLength(300);
            entity.Property(e => e.LastLoginTime).HasColumnType("datetime");
            entity.Property(e => e.RegisteredDate).HasColumnType("datetime");
            entity.Property(e => e.Salt).HasMaxLength(300);
            entity.Property(e => e.UserRole).HasMaxLength(10);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_IdetityPM_User");

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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
