namespace EntityORM.DatabaseEntity;

public partial class UserAuth
{
    public string Username { get; set; } = null!;

    public string? Token { get; set; }

    public DateTime CreatedTime { get; set; }

    public bool? Enabled { get; set; }

    public virtual Login UsernameNavigation { get; set; } = null!;
}
