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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Login>(entity =>
        {
            entity.HasKey(e => e.Username);

            entity.ToTable("Login");

            entity.Property(e => e.Username).HasMaxLength(10);
            entity.Property(e => e.LastLoginTime).HasColumnType("datetime");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.RegisteredDate).HasColumnType("datetime");
            entity.Property(e => e.UserRole).HasMaxLength(10);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.EmailAddress).HasMaxLength(30);
            entity.Property(e => e.FirstName).HasMaxLength(30);
            entity.Property(e => e.LastName).HasMaxLength(30);
            entity.Property(e => e.Phone).HasMaxLength(10);
            entity.Property(e => e.Suffix).HasMaxLength(10);
            entity.Property(e => e.Title).HasMaxLength(5);
            entity.Property(e => e.Username).HasMaxLength(10);            
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
