using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Models
{
    public partial class User
    {
        [NotMapped]
        public List<Chat> Chats
        {
            get
            {
                return ChatMembers.Where(cm => cm.Username == Nickname).Select(cm => cm.Chat).ToList();
            }
        }
    }
}
