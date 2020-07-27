using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Models.Repos
{
    public class MessageRepo : BaseRepo<Message>, IMessageRepo
    {
        public MessageRepo(OnlineChatDatabaseContext db) : base(db)
        {
            Table = Context.Messages;
        }

        public Message GetMessage(int messageId)
        {
            //TO DO
            //Optimize query
            Context.MessageContents.Load();
            Context.Users.Load();
            Context.Chats.Load();
            var message = Table.Include(message => message.MessagesReadStatus).First(m => m.Id == messageId);
            return message;
        }

        public User GetMessageAuthor(int messageId)
            => Table.Include(m => m.Author).First(m => m.Id == messageId).Author;

        public List<Message> GetMessages(int chatId)
        {
            //TO DO
            //Optimize query
            Context.MessageContents.Load();
            Context.Users.Load();
            Context.Chats.Load();
            var messages = Table.Include(message => message.MessagesReadStatus).Where(m => m.Chat.Id == chatId);
            return messages.ToList();
        }

        public List<ReplyMessage> GetReplies(int messageId)
            => Table.OfType<ReplyMessage>().Include(m => m.ReplyTo).Where(m => m.ReplyTo.Id == messageId).ToList();
    }
}
