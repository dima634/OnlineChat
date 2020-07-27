using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace OnlineChat.Dtos.ViewModels
{
    public class MessageViewModel : INotifyPropertyChanged
    {
        private bool isRead;

        public int Id { get; set; }

        public string Author { get; set; }

        public object Content { get; set; }

        public DateTime SentOn { get; set; }

        public bool IsEdited { get; set; }

        public bool IsRead
        {
            get => isRead;
            set
            {
                if (IsRead != value)
                {
                    isRead = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(isRead)));
                }
            }
        }

        public ContentType ContentType { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
            => Content.ToString();
    }
}
