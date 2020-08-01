using Newtonsoft.Json;
using OnlineChat.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using OnlineChat.Dtos.ViewModels;
using OnlineChat.WebApi.Services.InstantMessaging;
using System.Runtime.InteropServices.WindowsRuntime;
using OnlineChat.WebApi.Models.Repos;

namespace OnlineChat.WebApi.Services
{
    [Authorize]
    public class InstantMessager : Hub<IInstantMessaging>
    {
        private ISubscriptionManager _subscriptionManager;
        private IMessageReadStatusRepo _messageReadStatusRepo;

        public InstantMessager(ISubscriptionManager subscriptionManager, IMessageReadStatusRepo messageReadStatusRepo)
        {
            _subscriptionManager = subscriptionManager;
            _messageReadStatusRepo = messageReadStatusRepo;
        }

        //public void Subscribe(int chatId)
        //    => _subscriptionManager.Subscribe(Context.User.Identity.Name, chatId, Context.ConnectionId);

        //public void UnSubscribe()
        //    => _subscriptionManager.UnSubscribe(Context.ConnectionId);

        public void MarkMessageAsRead(int messageId, int chatId)
        {
            _messageReadStatusRepo.MarkRead(Context.User.Identity.Name, messageId);
            Clients.Clients(_subscriptionManager.GetActiveChatMembers(chatId).ToList()).MessageRead(messageId, chatId, Context.User.Identity.Name);
        }

        public override Task OnConnectedAsync()
        {
            _subscriptionManager.UserOnline(Context.User.Identity.Name, Context.ConnectionId);
            return Task.CompletedTask;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _subscriptionManager.UserOffline(Context.ConnectionId);
            return Task.CompletedTask;
        }
    }
}
