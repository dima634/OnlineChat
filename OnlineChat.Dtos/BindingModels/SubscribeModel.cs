using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineChat.Dtos.BindingModels
{
    public class SubscribeModel
    {
        public int Port { get; set; }
        public int SubscribeTo { get; set; }
    }
}
