using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Internal;
using OnlineChat.WebApi.Models;
using OnlineChat.WebApi.Models.Repos;
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

        public ChatService(IChatRepo chatRepo, IUserRepo userRepo, IMessageRepo messageRepo, IMessageReadStatusRepo messageReadStatusRepo)
        {
            _chatRepo = chatRepo;
            _userRepo = userRepo;
            _messageRepo = messageRepo;
            _messageReadStatusRepo = messageReadStatusRepo;
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
            => _messageReadStatusRepo.IsMessageReadByUser(username, messageId);

        public void MarkMessageAsReadByUser(int messageId, string username)
        {
            _messageReadStatusRepo.MarkRead(username, messageId);
        }

        public ReplyMessage ReplyToMessage(ReplyMessage message, int chatId, int replyTo)
        {
            var chat = _chatRepo.GetOne(chatId);
            message.ReplyTo = _messageRepo.GetOne(replyTo);
            chat.Messages.Add(message);
            _chatRepo.Save(chat);
            message = _messageRepo.GetMessage(message.Id) as ReplyMessage;
            return message;
        }

        public Message SendMessage(Message message, int chatId)
        {
            var chat = _chatRepo.GetOne(chatId);
            chat.Messages.Add(message);
            _chatRepo.Save(chat);

            return message;
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
