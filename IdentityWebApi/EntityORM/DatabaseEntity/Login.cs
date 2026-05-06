using System;
using System.Collections.Generic;

namespace EntityORM.DatabaseEntity
{
    public partial class Login
    {
        public Login()
        {
            Users = new HashSet<User>();
        }

        public string Username { get; set; } = null!;
        public string Key { get; set; } = null!;
        public string Salt { get; set; } = null!;
        public DateTime RegisteredDate { get; set; }
        public DateTime LastLoginTime { get; set; }
        public string? UserRole { get; set; }
        public bool IsActive { get; set; }

        public virtual Otpvalidate? Otpvalidate { get; set; }
        public virtual UserAuth? UserAuth { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
