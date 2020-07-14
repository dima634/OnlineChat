using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Models
{
    public partial class User
    {
        [Key]
        public string Nickname { get; set; }

        public string PasswordHash { get; set; }

        public virtual ICollection<ChatMember> ChatMembers { get; set; }
    }
}
