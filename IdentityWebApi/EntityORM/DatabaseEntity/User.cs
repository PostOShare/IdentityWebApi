using System;
using System.Collections.Generic;

namespace EntityORM.DatabaseEntity
{
    public partial class User
    {
        public User()
        {
            UserEmploymentDetails = new HashSet<UserEmploymentDetail>();
            UserLearnDetails = new HashSet<UserLearnDetail>();
            UserPersonalDetails = new HashSet<UserPersonalDetail>();
        }

        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string? Title { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Suffix { get; set; }
        public string EmailAddress { get; set; } = null!;
        public string? Phone { get; set; }

        public virtual Login UsernameNavigation { get; set; } = null!;
        public virtual ICollection<UserEmploymentDetail> UserEmploymentDetails { get; set; }
        public virtual ICollection<UserLearnDetail> UserLearnDetails { get; set; }
        public virtual ICollection<UserPersonalDetail> UserPersonalDetails { get; set; }
    }
}
