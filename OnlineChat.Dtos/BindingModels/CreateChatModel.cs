using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OnlineChat.Dtos.BindingModels
{

    public class CreateGroupModel
    {
        [Required]
        [MinLength(1, ErrorMessage = "Group must have at least 2 members")]
        public List<string> Members { get; set; } = new List<string>();

        [Required]
        [MinLength(4, ErrorMessage = "Group name must contain at least 4 symbols")]
        [RegularExpression("[A-z]+", ErrorMessage = "Group name must contain only latin letters")]
        public string Name { get; set; }
    }

    public class CreateDirectChatModel
    {
        [Required]
        [MinLength(4, ErrorMessage = "Username must contain at least 4 symbols")]
        [RegularExpression("[A-z0-9]+", ErrorMessage = "Username must contain only latin letters")]
        public string WithUser { get; set; }
    }
}
