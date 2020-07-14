using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Models.Repos
{
    public class ChatRepo : BaseRepo<Chat>, IChatRepo
    {
        public ChatRepo(OnlineChatDatabaseContext db) : base(db)
        {
            Table = Context.Chats;
        }

        public List<User> GetChatMembers(int chatId)
            => Table.Include(chat => chat.ChatMembers).ThenInclude(cm => cm.User).First(chat => chat.Id == chatId).Members;

        public List<Chat> GetUserChats(string username)
            => Table.Include(chat => chat.ChatMembers).ThenInclude(cm => cm.User).Where(chat => chat.ChatMembers.Any(cm => cm.Username == username)).ToList();

        public DirectChat GetDirectChat(string participant1, string participant2)
        {
            var userChats = GetUserChats(participant1);
            var direct = userChats.OfType<DirectChat>().FirstOrDefault(chat => chat.Members.Any(user => user.Nickname == participant2));
            return direct;
        }
    }
}
