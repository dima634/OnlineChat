using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Models
{
    public partial class Message
    {
        [NotMapped]
        public List<MessageReadStatus> ReadByUsers => MessagesReadStatus?.Where(readStatus => readStatus.MessageId == Id).ToList() ?? new List<MessageReadStatus>();

        public bool IsRead => ReadByUsers.Any(readStatus => readStatus.Username != Author.Nickname);
    }
}
