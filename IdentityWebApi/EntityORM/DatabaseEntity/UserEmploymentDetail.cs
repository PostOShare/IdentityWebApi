using System.Text.Json.Serialization;

namespace EntityORM.DatabaseEntity
{
    public partial class UserEmploymentDetail
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public int UserId { get; set; }
        public string EmployerName { get; set; } = null!;
        public string? EmployerCity { get; set; }
        public string Role { get; set; } = null!;
        public string? Responsibilities { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsCurrentEmployer { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; } = null!;
    }
}
