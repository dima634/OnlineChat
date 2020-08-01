using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineChat.Dtos.ViewModels
{
    public class ChatInfo
    {
        public int Id { get; set; }

        public string[] Members { get; set; }

        public int UnreadByCurrentUserMessagesCount { get; set; }
    }

    public class GroupChatInfo : ChatInfo
    {
        public string Owner { get; set; }

        public string Name { get; set; }
    }

    public class DirectChatInfo : ChatInfo
    {

    }
}
