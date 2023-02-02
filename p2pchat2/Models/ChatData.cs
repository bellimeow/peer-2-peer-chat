using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace p2pchat2.Models
{
    public class ChatData : ObservableCollection<ChatMessage>
    {
        public ChatData() : base(){}

        public override event NotifyCollectionChangedEventHandler CollectionChanged;
        protected override event PropertyChangedEventHandler PropertyChanged;

        protected override void InsertItem(int index, ChatMessage item)
        {
            base.InsertItem(index, item);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }
    }
}
