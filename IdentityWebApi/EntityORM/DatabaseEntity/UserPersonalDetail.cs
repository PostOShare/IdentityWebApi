using System;
using System.Collections.Generic;

namespace EntityORM.DatabaseEntity
{
    public partial class UserPersonalDetail
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Location { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Status { get; set; }
        public string? Gender { get; set; }
        public string? LanguageOne { get; set; }
        public string? LanguageTwo { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
