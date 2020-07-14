using OnlineChat.Dtos.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Services
{
    public interface IInstantMessaging
    {
        Task MessageSent(MessageViewModel message, int chatId);
        Task MessageEdited(MessageViewModel message, int chatId);
        Task MessageDeleted(int messageId, int chatId, bool forAll, string author);
        Task ChatCreated(ChatInfo chatInfo);
    }
}
