using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using OnlineChat.Dtos.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Services.InstantMessaging
{
    public interface IChatManager
    {
        List<ChatMember> GetChatMembers(int chatId);
        HashSet<string> GetChatUpdateSubs(int chatId);
        HashSet<string> GetActiveChatMembers(int chatId);
        void Subscribe(string username, int chatId, string connectionId);
        void UnSubscribe(string connectionId);

        /// <returns>List of connection to notify about user status change</returns>
        HashSet<string> UserOnline(string username, string connectionId);

        /// <returns>List of connection to notify about user status change</returns>
        HashSet<string> UserOffline(string connectionId);
    }
}
