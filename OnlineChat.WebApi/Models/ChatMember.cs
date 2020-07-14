using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Models
{
    public class ChatMember
    {
        public int ChatId { get; set; }
        public virtual Chat Chat { get; set; }

        public string Username { get; set; }
        public virtual User User { get; set; }
    }
}
