using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChatB0t
{
    public enum ChatHistoryRecordType
    {
        User,
        Bot
    }

    public class ChatHistoryRecord : INotifyPropertyChanged
    {
        private string? message;

        public ChatHistoryRecordType RecordType { get; set; }

        public string? Message 
        {   
            get => message; 
            set { message = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
