using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Models.Repos
{
    public interface IMessageRepo : IRepo<Message>
    {
        User GetMessageAuthor(int messageId);
        List<Message> GetMessages(int chatId);
        Message GetMessage(int messageId);
        List<ReplyMessage> GetReplies(int messageId);
    }
}
