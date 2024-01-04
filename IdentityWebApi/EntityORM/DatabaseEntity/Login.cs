namespace EntityORM.DatabaseEntity;

public partial class Login
{
    public string Username { get; set; } = null!;

    public string Key { get; set; } = null!;

    public string Salt { get; set; } = null!;

    public DateTime RegisteredDate { get; set; }

    public DateTime LastLoginTime { get; set; }

    public string? UserRole { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
