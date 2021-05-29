using OnlineChat.Dtos;
using OnlineChat.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Services
{
    public interface IChatService
    {
        GroupChat CreateGroupChat(string owner, string chatName, List<string> members);
        DirectChat CreateDirectChat(string participant1, string participant2);
        DirectChat GetDirectChat(string participant1, string participant2);
        List<Chat> GetChats(string username);
        List<User> GetChatMembers(int chatId);
        Chat GetChatByMessageId(int messageId);
        Message SendMessage(Message message, int chatId);
        Message SendMessage(object content, ContentType type, int chatId, User author, int? replyTo);
        ReplyMessage ReplyToMessage(ReplyMessage message, int chatId, int replyTo);
        Message EditMessage(int messageId, MessageContent newContent);
        Message DeleteMessage(int messageId, bool deleteForAll);
        List<Message> GetChatMessagesByPage(int chatId, string username, int page, int messagesPerPage);
        List<Message> GetChatMessagesByOffset(int chatId, string username, int offset, int resultCount);
        bool IsChatMember(int chatId, string username);
        bool IsMessageAuthor(int messageId, string username);
        bool IsMessageReadByUser(int messageId, string username);
        void MarkMessageAsReadByUser(int messageId, string username);
    }
}
