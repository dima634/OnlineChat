using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Models
{
    public class MessageReadStatus
    {
        public int MessageId { get; set; }
        public virtual Message Message { get; set; }
        
        public string Username { get; set; }
        public virtual User User { get; set; }

        public bool IsRead { get; set; }
    }
}
