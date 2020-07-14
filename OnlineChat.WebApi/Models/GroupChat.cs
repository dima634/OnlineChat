using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Models
{
    public class GroupChat : Chat
    {
        public string Name { get; set; }

        public virtual User Owner { get; set; }
    }
}
