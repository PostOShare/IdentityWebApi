using System;
using System.Collections.Generic;

namespace EntityORM.DatabaseEntity
{
    public partial class UserLearnDetail
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string InstitutionName { get; set; } = null!;
        public string Award { get; set; } = null!;
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public string? Major { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
