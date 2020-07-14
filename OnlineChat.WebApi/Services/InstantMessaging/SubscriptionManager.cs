using OnlineChat.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Services.InstantMessaging
{
    class SubscriptionManager : ISubscriptionManager
    {
        private static Dictionary<int, HashSet<string>> _subscriptions = new Dictionary<int, HashSet<string>>();
        private static Dictionary<string, string> _activeUsers = new Dictionary<string, string>();
        private IChatService _chatService;

        public SubscriptionManager(IChatService chatService)
        {
            _chatService = chatService;
        }

        public HashSet<string> GetActiveChatMembers(int chatId)
            => _activeUsers.Where(pair => _chatService.IsChatMember(chatId, pair.Value)).Select(pair => pair.Key).ToHashSet();

        public HashSet<string> GetChatUpdateSubs(int chatId)
            => _subscriptions[chatId];

        public void Subscribe(string username, int chatId, string connectionId)
        {
            if (_chatService.IsChatMember(chatId, username))
            {
                if (_subscriptions.ContainsKey(chatId))
                {
                    _subscriptions[chatId].Add(connectionId);
                }
                else
                {
                    _subscriptions[chatId] = new HashSet<string>() { connectionId };
                }
            }
        }

        public void UnSubscribe(string connectionId)
        {
            foreach (var sub in _subscriptions.Where(sub => sub.Value.Contains(connectionId)))
            {
                sub.Value.Remove(connectionId);
            }
        }

        public void UserOffline(string connectionId)
        {
            if (_activeUsers.ContainsKey(connectionId))
            {
                _activeUsers.Remove(connectionId);
            }

            UnSubscribe(connectionId);
        }

        public void UserOnline(string username, string connectionId)
            => _activeUsers[connectionId] = username;
    }
}
