using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineChat.Dtos.BindingModels
{
    public class EditMessageModel
    {
        public int Id { get; set; }

        public object Content { get; set; }

        public ContentType ContentType { get; set; }
    }
}
