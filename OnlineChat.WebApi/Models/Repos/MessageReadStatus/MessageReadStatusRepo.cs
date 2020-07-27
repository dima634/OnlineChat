using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Models.Repos
{
    public class MessageReadStatusRepo : BaseRepo<MessageReadStatus>, IMessageReadStatusRepo
    {
        public MessageReadStatusRepo(OnlineChatDatabaseContext context) : base(context)
        {
            Table = Context.MessagesReadStatuse;
        }

        public bool IsMessageRead(string author, int messageId)
            => Table.Any(readStatus => readStatus.MessageId == messageId && readStatus.Username != author);

        public bool IsMessageReadByUser(string username, int messageId)
            => Table.Find(new { messageId, username })?.IsRead ?? false;

        public void MarkRead(string username, int messageId)
        {
            var status = Table.Find(messageId, username);

            if (status == null)
            {
                status = new MessageReadStatus()
                {
                    IsRead = true,
                    MessageId = messageId,
                    Username = username
                };

                Add(status);
            }
            else
            {
                status.IsRead = true;
                Save(status);
            }
        }
    }
}
