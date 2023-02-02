using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using System.Diagnostics;
using p2pchat2.Models;
using p2pchat2.ViewModels;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace p2pchat2.ViewModels
{
    class ChatViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ChatViewModel chatVM;
        private string sendMessageBox;
        private string username;
        private string receiverUsername;
        private static readonly string CHATACCEPTEDTEXT = "Chat Accepted";

        public ObservableCollection<ChatMessage> chatBoxMessages { get; set; } = 
            new ObservableCollection<ChatMessage>();

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            Console.WriteLine("onpropertychanged {0}");

        }

        public ChatViewModel()
        {
            ChatMessage acceptedChatMessage = new ChatMessage();
            acceptedChatMessage.Message = CHATACCEPTEDTEXT;
            chatBoxMessages.Add(acceptedChatMessage);
        }

        public string SendMessageBox
        {
            get { return sendMessageBox; }
            set { sendMessageBox = value; OnPropertyChanged("SendMessageBox"); }
        }

        public string Username
        {
            get { return username; }
            set { username = value; OnPropertyChanged("Username"); }
        }

        public string Receiverusername
        {
            get { return receiverUsername; }
            set { receiverUsername = value; /*OnPropertyChanged("Username"); */}
        }

    }
}