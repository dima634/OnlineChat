using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Models
{
    public abstract class MessageContent
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }

    public class TextContent : MessageContent
    {
        public string Text { get; set; }

        public override string ToString() => Text;
    }

    public class FileContent : MessageContent
    {
        public string Filename { get; set; }

        public Guid FileId { get; set; }

        public string Url { get; set; }
    }
}
