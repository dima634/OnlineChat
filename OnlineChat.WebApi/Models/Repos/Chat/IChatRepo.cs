using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Models.Repos
{
    public interface IChatRepo : IRepo<Chat>
    {
        List<Chat> GetUserChats(string username);
        List<User> GetChatMembers(int chatId);
        DirectChat GetDirectChat(string participant1, string participant2);
    }
}
