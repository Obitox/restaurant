using System;
using System.Collections.Generic;

namespace Restaurant.DAL.MySQL.Models
{
    public partial class User
    {
        public ulong UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string RequestAntiForgeryToken { get; set; }
        public byte IsDeleted { get; set; }
        public byte IsEmailConfirmed { get; set; }
        public string Salt { get; set; }

        public virtual Cart Cart { get; set; }
        public virtual Order Order { get; set; }
    }
}
