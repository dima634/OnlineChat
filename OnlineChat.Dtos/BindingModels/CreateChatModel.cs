using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineChat.Dtos.BindingModels
{
    public class CreateChatModel
    {

        /// <summary>
        /// Don`t include chat creator
        /// </summary>
        public List<string> Members { get; set; } = new List<string>();

        public string Name { get; set; }

        public ChatType ChatType { get; set; }
    }
}
