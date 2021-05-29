using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Internal;
using OnlineChat.Dtos;
using OnlineChat.WebApi.Models;
using OnlineChat.WebApi.Models.Repos;
using OnlineChat.WebApi.Services.FileStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Services
{
    public class ChatService : IChatService
    {
        private IChatRepo _chatRepo;
        private IUserRepo _userRepo;
        private IMessageRepo _messageRepo;
        private IMessageReadStatusRepo _messageReadStatusRepo;
        private IFileStorage _fileStorage;

        public ChatService(IChatRepo chatRepo, IUserRepo userRepo, IMessageRepo messageRepo, IMessageReadStatusRepo messageReadStatusRepo, IFileStorage fileStorage)
        {
            _chatRepo = chatRepo;
            _userRepo = userRepo;
            _messageRepo = messageRepo;
            _messageReadStatusRepo = messageReadStatusRepo;
            _fileStorage = fileStorage;
        }

        public DirectChat CreateDirectChat(string participant1, string participant2)
        {
            var chat = GetDirectChat(participant1, participant2);

            if (chat != null) throw new ChatAlreadyExistException();

            chat = new DirectChat()
            {
                ChatMembers = new List<ChatMember>()
            };
            _chatRepo.Add(chat);

            chat.ChatMembers.Add(new ChatMember()
            {
                Chat = chat,
                User = _userRepo.GetOne(participant1)
            }); 

            chat.ChatMembers.Add(new ChatMember()
            {
                Chat = chat,
                User = _userRepo.GetOne(participant2)
            });

            _chatRepo.Save(chat);

            return chat;
        }

        public GroupChat CreateGroupChat(string owner, string name, List<string> members)
        {
            var chat = new GroupChat()
            {
                Name = name,
                Owner = _userRepo.GetOne(owner),
                ChatMembers = new List<ChatMember>()
            };

            _chatRepo.Add(chat);

            foreach (var member in members)
            {
                chat.ChatMembers.Add(new ChatMember()
                {
                    Chat = chat,
                    User = _userRepo.GetOne(member)
                });
            };

            _chatRepo.Save(chat);

            return chat;
        }

        public Message DeleteMessage(int messageId, bool deleteForAll)
        {
            var message = _messageRepo.GetOne(messageId);

            if (message == null) return null;

            if (deleteForAll)
            {
                var replies = _messageRepo.GetReplies(messageId).ToArray();
                    //_messageRepo.GetAll().OfType<ReplyMessage>().Where(m => m.ReplyTo.Id == message.Id).ToArray();

                if (replies.Length != 0)
                {
                    foreach (var replyMessage in replies)
                    {
                        replyMessage.ReplyTo = null;
                    }

                    _messageRepo.SaveRange(replies);
                }

                _messageRepo.Delete(message);
            }
            else
            {
                message.HideForAuthor = true;
                _messageRepo.Save(message);
            }

            if (deleteForAll) return null;
            else return message;
        }

        public Message EditMessage(int messageId, MessageContent newContent)
        {
            var message = _messageRepo.GetMessage(messageId);
            message.Content = newContent;
            message.IsEdited = true;
            _messageRepo.Save(message);

            return message;
        }

        public Chat GetChatByMessageId(int messageId)
            => _messageRepo.GetMessage(messageId).Chat;

        public List<User> GetChatMembers(int chatId)
            => _chatRepo.GetChatMembers(chatId);

        public List<Message> GetChatMessagesByOffset(int chatId, string username, int offset, int resultCount)
        {
            var messages = _messageRepo.GetMessages(chatId);
            var visibleMessages = messages.Where(m =>
            {
                if (m.Author.Nickname == username) return !m.HideForAuthor;
                else return true;
            }).OrderByDescending(m => m.SentOn);

            return visibleMessages.Skip(offset).Take(resultCount).ToList();
        }

        public List<Message> GetChatMessagesByPage(int chatId, string username, int page, int messagesPerPage)
        {
            var messages = _messageRepo.GetMessages(chatId);
            var skip = page * messagesPerPage;
            var visibleMessages = messages.Where(m =>
            {
                if (m.Author.Nickname == username) return !m.HideForAuthor;
                else return true;
            }).OrderByDescending(m => m.SentOn);

            return visibleMessages.Skip(skip).Take(messagesPerPage).ToList();
        }

        public List<Chat> GetChats(string username)
        {
            var user = _userRepo.GetOne(username);

            if (user == null) throw new ApplicationException($"User not found: {username}");

            var chats = _chatRepo.GetUserChats(username);

            return chats;
        }

        public DirectChat GetDirectChat(string participant1, string participant2)
            => _chatRepo.GetDirectChat(participant1, participant2);

        public bool IsChatMember(int chatId, string username)
        {
            var isMember = _chatRepo.GetChatMembers(chatId).Any(user => user.Nickname == username);

            return isMember;
        }

        public bool IsMessageAuthor(int messageId, string username)
        {
            var author = _messageRepo.GetMessageAuthor(messageId);

            return author.Nickname == username;
        }

        public bool IsMessageReadByUser(int messageId, string username)
        {
            var message = _messageRepo.GetMessage(messageId);
            var result = _messageReadStatusRepo.IsMessageReadByUser(username, messageId) ||
                         message.Author.Nickname == username;

            return result;
        }

        public void MarkMessageAsReadByUser(int messageId, string username)
        {
            _messageReadStatusRepo.MarkRead(username, messageId);
        }

        public ReplyMessage ReplyToMessage(ReplyMessage message, int chatId, int replyTo)
        {
            message.ReplyTo = _messageRepo.GetOne(replyTo);
            return SendMessage(message, chatId) as ReplyMessage;
        }

        public Message SendMessage(Message message, int chatId)
        {
            var chat = _chatRepo.GetOne(chatId);
            chat.Messages.Add(message);
            _chatRepo.Save(chat);

            return message;
        }

        public Message SendMessage(object content, ContentType type, int chatId, User author, int? replyTo)
        {
            MessageContent messageContent;
            switch (type)
            {
                case ContentType.Text:
                    messageContent = new TextContent()
                    {
                        Text = content.ToString()
                    };
                    break;

                case ContentType.File:
                    var file = content as IFormFile;
                    var guid = Guid.NewGuid();
                    var url = _fileStorage.Save(guid.ToString(), file.OpenReadStream());
                    messageContent = new FileContent()
                    {
                        FileId = guid,
                        Filename = file.FileName,
                        Url = url
                    };
                    break;

                default:
                    throw new Exception("Unsupported content");
            }

            Message message = replyTo == null ? new Message() : new ReplyMessage() { ReplyTo = _messageRepo.GetMessage(replyTo.Value) };
            message.Author = author;
            message.Content = messageContent;
            message.SentOn = DateTime.Now;
            return SendMessage(message, chatId);
        }
    }


    [Serializable]
    public class ChatAlreadyExistException : Exception
    {
        public ChatAlreadyExistException() { }
        public ChatAlreadyExistException(string message) : base(message) { }
        public ChatAlreadyExistException(string message, Exception inner) : base(message, inner) { }
        protected ChatAlreadyExistException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
