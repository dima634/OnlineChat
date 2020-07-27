using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Models
{
    public partial class Message
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public virtual User Author { get; set; }

        public virtual MessageContent Content { get; set; }

        public DateTime SentOn { get; set; }

        public bool HideForAuthor { get; set; }

        public bool IsEdited { get; set; }

        public virtual Chat Chat { get; set; }

        public virtual ICollection<MessageReadStatus> MessagesReadStatus { get; set; }
    }
}
