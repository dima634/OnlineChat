using OnlineChat.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Services.InstantMessaging
{
    class ChatManager : IChatManager
    {
        /// <summary>
        /// Chat id - connection id pairs
        /// </summary>
        private static Dictionary<int, HashSet<string>> _subscriptions = new Dictionary<int, HashSet<string>>();
        /// <summary>
        /// Connection id - username pairs
        /// </summary>
        private static Dictionary<string, string> _activeUsers = new Dictionary<string, string>();
        private IChatService _chatService;

        public ChatManager(IChatService chatService)
        {
            _chatService = chatService;
        }

        public HashSet<string> GetActiveChatMembers(int chatId)
            => _activeUsers.Where(pair => _chatService.IsChatMember(chatId, pair.Value)).Select(pair => pair.Key).ToHashSet();

        public List<Dtos.ViewModels.ChatMember> GetChatMembers(int chatId)
        {
            var chatMembers = _chatService.GetChatMembers(chatId);
            var active = GetActiveChatMembers(chatId);

            return chatMembers.Select(user => new Dtos.ViewModels.ChatMember()
            {
                Status = active.Contains(user.Nickname) ? "Online" : "Offline",
                Username = user.Nickname
            }).ToList();
        }

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

        public HashSet<string> UserOffline(string connectionId)
        {
            var username = _activeUsers[connectionId];
            var chats = _chatService.GetChats(username).Where(chat => _subscriptions.ContainsKey(chat.Id));
            var connections = _subscriptions.Where(pair => chats.Any(chat => chat.Id == pair.Key)).Select(pair => pair.Value);

            if (connections.Count() == 0)
            {
                return new HashSet<string>();
            }

            var notifyConnections = connections.Aggregate((a, b) => a.Concat(b).ToHashSet());

            if (_activeUsers.ContainsKey(connectionId))
            {
                _activeUsers.Remove(connectionId);
            }

            UnSubscribe(connectionId);

            return notifyConnections;
        }

        public HashSet<string> UserOnline(string username, string connectionId)
        {
            _activeUsers[connectionId] = username;

            var chats = _chatService.GetChats(username).Where(chat => _subscriptions.ContainsKey(chat.Id));
            var connections = _subscriptions.Where(pair => chats.Any(chat => chat.Id == pair.Key)).Select(pair => pair.Value);

            if (connections.Count() == 0) return new HashSet<string>();

            return connections.Aggregate((a, b) => a.Concat(b).ToHashSet());
        }
    }
}
