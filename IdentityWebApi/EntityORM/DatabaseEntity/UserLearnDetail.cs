using System.Text.Json.Serialization;

namespace EntityORM.DatabaseEntity
{
    public partial class UserLearnDetail
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public int UserId { get; set; }
        public string InstitutionName { get; set; } = null!;
        public string Award { get; set; } = null!;
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public string? Major { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; } = null!;
    }
}