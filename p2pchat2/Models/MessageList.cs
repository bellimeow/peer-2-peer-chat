using System.Collections.ObjectModel;

namespace p2pchat2.Models
{
    public class MessageList : ObservableCollection<ChatMessage>
    {
        public MessageList() : base()
        {}
    }
}

