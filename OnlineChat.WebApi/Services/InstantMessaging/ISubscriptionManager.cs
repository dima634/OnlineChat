using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Services.InstantMessaging
{
    public interface ISubscriptionManager
    {
        HashSet<string> GetChatUpdateSubs(int chatId);
        HashSet<string> GetActiveChatMembers(int chatId);
        void Subscribe(string username, int chatId, string connectionId);
        void UnSubscribe(string connectionId);
        void UserOnline(string username, string connectionId);
        void UserOffline(string connectionId);
    }
}
