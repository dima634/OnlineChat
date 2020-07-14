using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineChat.Dtos.BindingModels
{
    public class DeleteMessageModel
    {
        public int Id { get; set; }
        public bool DeleteForAll { get; set; }
    }
}
