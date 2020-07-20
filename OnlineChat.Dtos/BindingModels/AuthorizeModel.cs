using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OnlineChat.Dtos.BindingModels
{
    public class AuthorizeModel
    {
        [Required]
        [MinLength(4, ErrorMessage = "Username must contain at least 4 symbols")]
        [RegularExpression("[A-z0-9]+", ErrorMessage = "Username must contain only latin letters and digits")]
        public string Username { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "Password must contain at least 6 symbols")]
        public string Password { get; set; }
    }
}
