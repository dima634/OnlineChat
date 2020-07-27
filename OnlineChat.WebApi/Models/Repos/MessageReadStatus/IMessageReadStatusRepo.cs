using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Models.Repos
{
    public interface IMessageReadStatusRepo
    {
        bool IsMessageRead(string author, int messageId);
        bool IsMessageReadByUser(string username, int messageId);
        void MarkRead(string username, int messageId);
    }
}
