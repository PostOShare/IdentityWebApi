namespace EntityORM.DatabaseEntity;

public partial class Login
{
    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime RegisteredDate { get; set; }

    public DateTime LastLoginTime { get; set; }

    public string? UserRole { get; set; }

    public bool IsActive { get; set; }
}
