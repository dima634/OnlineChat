using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Models
{
    public class ReplyMessage : Message
    {
        public virtual Message ReplyTo { get; set; }
    }
}
