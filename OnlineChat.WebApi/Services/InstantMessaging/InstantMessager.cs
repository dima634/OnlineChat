using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OnlineChat.WebApi.Models.Repos;
using OnlineChat.WebApi.Services.InstantMessaging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Services
{
    [Authorize]
    public class InstantMessager : Hub<IInstantMessaging>
    {
        private IChatManager _chatManager;
        private IChatService _chatService;
        private IMessageReadStatusRepo _messageReadStatusRepo;

        public InstantMessager(IChatManager chatManager, IChatService chatService, IMessageReadStatusRepo messageReadStatusRepo)
        {
            _chatManager = chatManager;
            _messageReadStatusRepo = messageReadStatusRepo;
            _chatService = chatService;
        }

        public void MarkMessageAsRead(int messageId, int chatId)
        {
            _messageReadStatusRepo.MarkRead(Context.User.Identity.Name, messageId);
            Clients.Clients(_chatManager.GetActiveChatMembers(chatId).ToList()).MessageRead(messageId, chatId, Context.User.Identity.Name);
        }

        public void GetChatMembers(int chatId)
        {
            if (_chatService.IsChatMember(chatId, Context.User.Identity.Name))
            {
                var members = _chatManager.GetChatMembers(chatId);
                Clients.Caller.ChatMembers(members);
            }
        }

        public override Task OnConnectedAsync()
        {
            var notify = _chatManager.UserOnline(Context.User.Identity.Name, Context.ConnectionId);
            Clients.Clients(notify.ToList()).UserStatusChanged(Context.User.Identity.Name, "Online");

            return Task.CompletedTask;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var notify = _chatManager.UserOffline(Context.ConnectionId);
            Clients.Clients(notify.ToList()).UserStatusChanged(Context.User.Identity.Name, "Offline");
            return Task.CompletedTask;
        }
    }
}
