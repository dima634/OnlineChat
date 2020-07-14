using OnlineChat.Site.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineChat.Dtos.BindingModels;
using OnlineChat.Dtos.ViewModels;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using Extensions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OnlineChat.Site.InstantMessaging
{
    public class MessageReceivedEventArgs
    {
        public MessageViewModel Message { get; set; }
        public int ChatId { get; set; }
    }

    public class MessageEditedEventArgs
    {
        public MessageViewModel Message { get; set; }
        public int ChatId { get; set; }
    }

    public class MessageDeletedEventArgs
    {
        public bool DeletedForAll { get; set; }
        public int MessageId { get; set; }
        public int ChatId { get; set; }
        public string Author { get; set; }
    }

    public class ChatCreatedEventArgs
    {
        public ChatInfo ChatInfo { get; set; }
    }

    public class InstantMessager
    {
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<MessageDeletedEventArgs> MessageDeleted;
        public event EventHandler<MessageEditedEventArgs> MessageEdited;
        public event EventHandler<ChatCreatedEventArgs> ChatCreated;

        private HubConnection _connection;
        private string _hubUrl;
        private WebApiClient _webApi;

        public InstantMessager(IConfiguration configuration, WebApiClient webApi)
        {
#if DEBUG
            _hubUrl = configuration["ChatHubUrlDevelopment"];
#else
            _hubUrl = configuration["ChatHubUrl"];
#endif
            _webApi = webApi;
            _connection = new HubConnectionBuilder()
                .WithUrl($"{_hubUrl}/?", opt =>
                {
                    opt.AccessTokenProvider = () => Task.FromResult(_webApi.AccessToken);
                })
                .WithAutomaticReconnect()
                .AddNewtonsoftJsonProtocol(opt => opt.PayloadSerializerSettings.TypeNameHandling = TypeNameHandling.Objects)
                .Build();

            _connection.On<MessageViewModel, int>("MessageSent", OnMessageReceived);
            _connection.On<MessageViewModel, int>("MessageEdited", OnMessageEdited);
            _connection.On<int, int, bool, string>("MessageDeleted", OnMessageDeleted);
            _connection.On<ChatInfo>("ChatCreated", OnChatCreated);
        }

        public async Task GoOnline()
        {
            if (_connection.State != HubConnectionState.Connected) await _connection.StartAsync();
        }

        public async Task SubscribeAsync(int chatId)
        {
            if(_connection.State == HubConnectionState.Disconnected) await _connection.StartAsync();

            try
            {
                await _connection.SendAsync("Subscribe", chatId);
            }
            catch (TaskCanceledException)
            {
                await UnsubscribeAsync();
            }
        }

        public async Task UnsubscribeAsync()
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                await _connection.SendAsync("UnSubscribe");
            }
        }

        private void OnMessageReceived(MessageViewModel message, int chatId)
        {
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs()
            {
                Message = message,
                ChatId = chatId
            });
        }

        private void OnMessageEdited(MessageViewModel message, int chatId)
        {
            MessageEdited?.Invoke(this, new MessageEditedEventArgs()
            {
                Message = message,
                ChatId = chatId
            });
        }

        private void OnMessageDeleted(int messageId, int chatId, bool forAll, string author)
        {
            MessageDeleted?.Invoke(this, new MessageDeletedEventArgs()
            {
                MessageId = messageId,
                ChatId = chatId,
                DeletedForAll = forAll,
                Author = author
            });
        }

        private void OnChatCreated(ChatInfo chatInfo)
        {
            ChatCreated?.Invoke(this, new ChatCreatedEventArgs()
            {
                ChatInfo = chatInfo
            });
        }
    }
}
