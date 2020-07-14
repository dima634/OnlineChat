using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineChat.Dtos.ViewModels
{
    public class ReplyMessageViewModel : MessageViewModel
    {
        public MessageViewModel ReplyTo { get; set; }
    }
}
