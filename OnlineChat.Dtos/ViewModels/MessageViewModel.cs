using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineChat.Dtos.ViewModels
{
    public class MessageViewModel
    {
        public int Id { get; set; }

        public string Author { get; set; }

        public object Content { get; set; }

        public DateTime SentOn { get; set; }

        public bool HideForAuthor { get; set; }

        public ContentType ContentType { get; set; }
    }
}
