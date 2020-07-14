using OnlineChat.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Services
{
    public interface IChatService
    {
        GroupChat CreateGroupChat(string owner, string chatName, List<string> members);
        DirectChat CreateDirectChat(string participant1, string participant2);
        DirectChat GetDirectChat(string participant1, string participant2);
        Chat GetChatByMessageId(int messageId);
        Message SendMessage(Message message, int chatId);
        ReplyMessage ReplyToMessage(ReplyMessage message, int chatId, int replyTo);
        Message EditMessage(int messageId, MessageContent newContent);
        Message DeleteMessage(int messageId, bool deleteForAll);
        List<Message> GetChatMessagesByPage(int chatId, string username, int page, int messagesPerPage);
        List<Message> GetChatMessagesByOffset(int chatId, string username, int offset, int resultCount);
        List<Chat> GetChats(string username);
        bool IsChatMember(int chatId, string username);
        bool IsMessageAuthor(int messageId, string username);
    }
}
