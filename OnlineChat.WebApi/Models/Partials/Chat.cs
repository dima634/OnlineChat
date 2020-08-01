using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Models
{
    public partial class Chat
    {
        [NotMapped]
        public List<User> Members
            => ChatMembers.Where(cm => cm.ChatId == Id).Select(cm => cm.User).ToList();

        [NotMapped]
        public int UnreadByCurrentUserMessagesCount { get; set; }
    }
}
 