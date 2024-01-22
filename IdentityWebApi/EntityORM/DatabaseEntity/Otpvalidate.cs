namespace EntityORM.DatabaseEntity;

public partial class Otpvalidate
{
    public string Username { get; set; } = null!;

    public decimal? Otp { get; set; }

    public DateTime RequestedTime { get; set; }

    public decimal? RetryAttempt { get; set; }

    public virtual Login UsernameNavigation { get; set; } = null!;
}
