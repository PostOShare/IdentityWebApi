using System.ComponentModel.DataAnnotations.Schema;

namespace EntityORM.DatabaseEntity;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string? Title { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Suffix { get; set; }

    public string EmailAddress { get; set; } = null!;

    public string? Phone { get; set; }

    [ForeignKey("Username")]
    public virtual Login UsernameNavigation { get; set; } = null!;
}
