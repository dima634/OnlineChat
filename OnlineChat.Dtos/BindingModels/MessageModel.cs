﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineChat.Dtos.BindingModels
{
    public class MessageModel
    {
        public object Content { get; set; }

        public string ContentType { get; set; }

        public int? ReplyTo { get; set; }
    }
}
