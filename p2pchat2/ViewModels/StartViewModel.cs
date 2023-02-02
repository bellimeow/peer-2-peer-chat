using System;
using System.Linq;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using p2pchat2.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace p2pchat2.ViewModels
{
    public class StartViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private static string ip;
        private static string port;
        private string personalIp;
        private static string username;
        private static string receiverUsername;

        private string acceptPopupText;
        private string popupVisible;
        private string popupText;
        private string errorText;
        private string errorTextVisible;
        private static string startViewVisible = "Visible";
        private static string historyViewVisible = "Hidden";
        private static string chatViewVisible = "Hidden";
        private static string backButtonVisible = "Hidden";
        private static string searchHistoryBoxVisibility = "Hidden";
        private string searchHistoryInput;

        private string selectedUserHistoryName;
        public string SelectedUserHistoryName
        {
            get { return selectedUserHistoryName; }
            set
            {
                selectedUserHistoryName = value;
                if (selectedUserHistoryName != null)
                { getSelectedHistory(); }
            }
        }
        private Server server;
        private string selectedItem;

        private string sendMessageBox;

        private ObservableCollection<ChatMessage> chatBoxMessages;
        public ObservableCollection<ChatMessage> ChatBoxMessages
        {
            get { return chatBoxMessages; }
            set
            {
                chatBoxMessages = value;
                OnPropertyChanged("ChatBoxMessages");
            }
        }

        private ObservableCollection<string> chatHistoryList;
        public ObservableCollection<string> ChatHistoryList
        {
            get { return chatHistoryList; }
            set
            {
                chatHistoryList = value;
                OnPropertyChanged("ChatHistoryList");
            }

        }

        private ObservableCollection<ChatMessage> chatBoxHistoryMessages;
        public ObservableCollection<ChatMessage> ChatBoxHistoryMessages
        {
            get { return chatBoxHistoryMessages; }
            set
            {
                chatBoxHistoryMessages = value;
                OnPropertyChanged("ChatBoxHistoryMessages");
            }
        }

        public string SelectedUser { get; set; }


        public StartViewModel()
        {
            ip = "127.0.0.1";
            port = "11000";
            personalIp = Server.GetIp();
            receiverUsername = "HARDCODED RECIPIENTNAME";
            popupVisible = "False";
            errorTextVisible = "Hidden";
            sendMessageBox = "Write your message here!";
            username = "Anonymous Penguin";
            this.chatHistoryList = new ObservableCollection<string>();
            this.chatBoxHistoryMessages = new ObservableCollection<ChatMessage>();

            this.server = new Server(personalIp, username);
            this.server.PropertyChanged += new PropertyChangedEventHandler(server_PropertyChanged);
            this.chatBoxMessages = new MessageList();
            this.chatBoxMessages.CollectionChanged += this.OnCollectionChanged;
        }

        private void collectHistory()
        {
            var looplist = server.getCurrentHistory();
            if (looplist != null) { 
                foreach (ChatMessage msg in looplist)
                {
                    chatBoxMessages.Add(msg);
                }
            }
        }

        private void collectChatHistoryMessages(string ip_in, string user_in)
        {
            var looplist = DataControls.GetChatMessages(ip_in, user_in);
            if (looplist != null)
            {
                foreach (ChatMessage msg in looplist)
                {
                    chatBoxHistoryMessages.Add(msg);
                }
            }
        }

        private void collectChatHistory()
        {
            chatHistoryList.Clear();

            foreach (string name in DataControls.fileNamesToList())
            {
                chatHistoryList.Add(name);
            }
        }

        private void client_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ClientConnectError")
            {
                RaiseProperyChanged(e.PropertyName);
            }
            else if (e.PropertyName == "ReceivedClientMessage")
            {
                RaiseProperyChanged(e.PropertyName);
            }
            else if (e.PropertyName == "ConnectionClosed")
            {
                RaiseProperyChanged(e.PropertyName);
            }
        }

        private void server_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ReceivedServerMessage")
            {
                RaiseProperyChanged(e.PropertyName);
            }
            else if (e.PropertyName == "ConnectionClosed")
            {
                RaiseProperyChanged(e.PropertyName);
            }
            else if (e.PropertyName == "ClientConnectError")
            {
                RaiseProperyChanged(e.PropertyName);
            }
        }

        private void RaiseProperyChanged(string propertyName)
        {
            if (propertyName == "ReceivedServerMessage")
            {
                handleMessage(server.ReceivedServerMessage);
            }
            else if (propertyName == "ClientConnectError")
            {
                ConnectionErrorPopup();
            }
            else if (propertyName == "ConnectionClosed")
            {
                ConnectionClosedErrorPopup();
            }
        }

        private void rendermymessage(ChatMessage msg)
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                insertmessage(msg);
            });
           
        }

        private void renderYOURmessage(ChatMessage msg)
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                insertmessage(msg);
            });

        }

        private void insertmessage(ChatMessage msg)
        {
            ChatBoxMessages.Add(msg);
        }

        public string IP
        {
            get { return ip; }
            set { ip = value; OnPropertyChanged("IP"); }
        }

        public string Port
        {
            get { return port; }
            set { port = value; OnPropertyChanged("Port"); }
        }

        public string Username
        {
            get { return username; }
            set { username = value; OnPropertyChanged("Username");}
        }

        public string ReceiverUsername
        {
            get { return receiverUsername; }
            set { receiverUsername = value; OnPropertyChanged("ReceiverUsername"); }
        }

        public string StartViewVisible
        {
            get { return startViewVisible; }
            set { startViewVisible = value; OnPropertyChanged("StartViewVisible"); }
        }

        public string HistoryViewVisible
        {
            get { return historyViewVisible; }
            set { historyViewVisible = value; OnPropertyChanged("HistoryViewVisible"); }
        }

        public string ChatViewVisible
        {
            get { return chatViewVisible; }
            set { chatViewVisible = value; OnPropertyChanged("ChatViewVisible"); }
        }

        public string BackButtonVisible
        {
            get { return backButtonVisible; }
            set { backButtonVisible = value; OnPropertyChanged("BackButtonVisible"); }
        }

        public string SearchHistoryBoxVisibility
        {
            get { return searchHistoryBoxVisibility; }
            set { searchHistoryBoxVisibility = value; OnPropertyChanged("SearchHistoryBoxVisibility"); }
        }

        public string SearchHistoryInput
        {
            get { return searchHistoryInput; }
            set { searchHistoryInput = value; OnPropertyChanged("SearchHistoryInput"); }
        }

        public string PersonalIp
        {
            get { return personalIp; }
            set { personalIp = value; OnPropertyChanged("PersonalIp"); }
        }

        public string AcceptConnectionPopupText
        {
            get { return acceptPopupText; }
            set { acceptPopupText = value; OnPropertyChanged("AcceptPopupText"); }
        }

        public string PopupVisible
        {
            get { return popupVisible; }
            set { popupVisible = value; OnPropertyChanged("PopupVisible"); }
        }

        public string PopupText
        {
            get { return popupText; }
            set { popupText = value; OnPropertyChanged("PopupText"); }
        }

        public string ErrorText {
            get { return errorText; }
            set { errorText = value; OnPropertyChanged("ErrorText"); }
            }

        public string ErrorTextVisible
        {
            get { return errorTextVisible; }
            set { errorTextVisible = value; OnPropertyChanged("ErrorTextVisible"); }
        }

        public string SendMessageBox
        {
            get { return sendMessageBox; }
            set { sendMessageBox = value; OnPropertyChanged("SendMessageBox"); }
        }

        public string SelectedItem
        {
            get { return selectedItem; }
            set { selectedItem = value; OnPropertyChanged("SelectedItem"); }
        }

        private StartViewModel startVM;

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            Console.WriteLine("onpropertychanged {0}");

        }

        void Source_CollectionChanged(object aSender, NotifyCollectionChangedEventArgs aArgs)
        {
            switch (aArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Remove:
                    break;

                case NotifyCollectionChangedAction.Replace:
                    break;

                case NotifyCollectionChangedAction.Reset:
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        internal void Connect()
        {
            server.MyUsername = username;
            server.MyIP = personalIp;
            server.setCommand(ChatMessage.commandStatus.CONNECTPOPUP);
            Trace.WriteLine("THISPORT CONNECT: " + port);
            server.getClient(Int32.Parse(port), ip);
        }

        internal void Listen()
        {
            server.MyUsername = username;
            server.MyIP = ip;
            Trace.WriteLine("THISPORT LISTEN: " + port);
            server.runListen(Int32.Parse(port));
            
        }

        public ICommand ConnectButtonCommand
        {
            get
            {
                Trace.WriteLine("Button \"Connect\" CLICKED! " + port);
                return new RelayCommand(Connect);
            }
        }

        public ICommand ListenButtonCommand
        {
            get
            {
                Trace.WriteLine("Button \"Listen\" CLICKED! ");
                return new RelayCommand(Listen);
            }
        }

        public ICommand ChatHistoryButtonCommand
        {
            get
            {
                Trace.WriteLine("Button \"SwitchToChatHistoryView\" CLICKED! ");
                return new RelayCommand(SwitchToChatHistoryView);
            }
        }

        public ICommand BackToStartButtonCommand
        {
            get
            {
                Trace.WriteLine("Button \"SwitchToChatHistoryView\" CLICKED! ");
                return new RelayCommand(BackToStartView);
            }
        }

        public ICommand SearchChatHistoryCommand
        {
            get
            {
                Trace.WriteLine("Button \"SearchChatHistoryCommand\" CLICKED! ");
                return new RelayCommand(SearchChatHistory);
            }
        }

        public ICommand AcceptButtonCommand
        {
            get
            {
                Trace.WriteLine("Button \"Accept\" CLICKED! ");

                return new RelayCommand(Accept);
            }
        }

        public ICommand DenyButtonCommand
        {
            get
            {
                Trace.WriteLine("Button \"Deny\" CLICKED! ");
                return new RelayCommand(Deny);
            }
        }

        public ICommand SendCommand

        {
            get
            {
                Trace.WriteLine("Button \"Send message\" CLICKED! ");
                return new RelayCommand(Send);
            }
        }

        public ICommand BuzzCommand
        {
            get
            {
                Trace.WriteLine("Button \"Buzz\" CLICKED! ");
                return new RelayCommand(sendBuzz);
            }
        }

        public event NotifyCollectionChangedEventHandler OnCollectionChanged;

        internal void requestChatPopup()
        {
            PopupText = "User " + server.YourUsername + " wants to start a chat with you. Accept?";
            PopupVisible = "True";
        }

        private void Accept()
        {
            Trace.WriteLine("Button \"Accept\" CLICKED! ");
            PopupVisible = "False";
            server.setCommand(ChatMessage.commandStatus.ACCEPT);
            server.SendMessage();
            ChatViewVisible = "Visible";
        }

        private void Deny()
        {
            Trace.WriteLine("Button \"Deny\" CLICKED! ");
            PopupVisible = "False";
            server.setCommand(ChatMessage.commandStatus.DENY);
            server.SendMessage();
        }

        private void Send()
        {
            Trace.WriteLine("Button \"Send Message\" CLICKED! ");
            server.clearMessage();
            server.MyUsername = username;
            server.MyIP = personalIp;
            server.setCommand(ChatMessage.commandStatus.NOCOMMAND);
            server.setMessage(sendMessageBox);
            server.SendMessage();
            rendermymessage(server.SendServerMessage);
        }

        private void SwitchToChatHistoryView()
        {
            HistoryViewVisible = "Visible";
            BackButtonVisible = "Visible";
            SearchHistoryBoxVisibility = "Visible";
            collectChatHistory();
        }

        private void BackToStartView()
        {
            HistoryViewVisible = "Collapsed";
            StartViewVisible = "Visible";
            BackButtonVisible = "Hidden";
            SearchHistoryBoxVisibility = "Hidden";
        }

        private void sendBuzz() {
            server.setCommand(ChatMessage.commandStatus.BUZZ);
            server.SendMessage();  
        }

        private void Buzz()
        {
            Trace.WriteLine("Button \"Buzz\" CLICKED! ");

            Action<object> buzz = (o) =>
            {
                Action a = () => App.Current.MainWindow.Left += 24;
                App.Current.Dispatcher.Invoke(a);
                System.Threading.Thread.Sleep(50);

                a = () => App.Current.MainWindow.Top -= 24;
                App.Current.Dispatcher.Invoke(a);
                System.Threading.Thread.Sleep(50);

                a = () => App.Current.MainWindow.Left -= 12;
                App.Current.Dispatcher.Invoke(a);
                System.Threading.Thread.Sleep(20);

                a = () => App.Current.MainWindow.Top += 24;
                App.Current.Dispatcher.Invoke(a);
                System.Threading.Thread.Sleep(50);

                a = () => App.Current.MainWindow.Left -= 12;
                App.Current.Dispatcher.Invoke(a);
                System.Threading.Thread.Sleep(20);
            };

            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(buzz));
        }

        public  static ChatMessage newMessage = new ChatMessage();
        
        public void handleMessage(string jsonData)
        {
                Trace.WriteLine("JSON DATA: " + jsonData);
                JObject data = ChatMessage.toJson(jsonData);

            switch ((ChatMessage.commandStatus)Enum.Parse(typeof(ChatMessage.commandStatus),
                    (string)data.GetValue("Command")))
            {
                case ChatMessage.commandStatus.ACCEPT:
                    ChatViewVisible = "Visible";
                    break;
                case ChatMessage.commandStatus.DENY:
                    ConnectionDeniedPopup((string)data.GetValue("Username"));
                    break;
                case ChatMessage.commandStatus.BUZZ:
                    Buzz();
                    break;
                case ChatMessage.commandStatus.CONNECTPOPUP:
                    requestChatPopup();
                    break;
                case ChatMessage.commandStatus.NOCOMMAND:
                    ChatMessage x = new ChatMessage();
                    x.parseJson(JObject.Parse(server.ReceivedServerMessage));
                    ReceiverUsername = x.Username;
                    renderYOURmessage(x);

                    break;
                default:
                    Trace.WriteLine("no command " + (ChatMessage.commandStatus)Enum.Parse(typeof(ChatMessage.commandStatus),
                    (string)data.GetValue("Command")) + "------------------------------------------------------------------");
                    break;
            }
        }

        private void SearchChatHistory()
        {
            if (searchHistoryInput == null || searchHistoryInput == "")
            {
                collectChatHistory();
            }
            else
            {
                ChatHistoryList.Clear();
                collectChatHistory();
                ObservableCollection<string> temp = new ObservableCollection<string>();
                var x = ChatHistoryList.Where(x => !x.Contains(searchHistoryInput)).ToList();
                foreach (string name in x)
                {
                    ChatHistoryList.Remove(name);
                }
            }
        }

        private void getSelectedHistory()
        {
            Trace.WriteLine("SelectedItem: " + selectedUserHistoryName);

            String str = SelectedUserHistoryName;

            String[] separator = { "-" };
            Int32 count = 2;

            String[] strlist = str.Split(separator, count,
            StringSplitOptions.RemoveEmptyEntries);

            string split_string_user = strlist[0];
            Trace.WriteLine("SPLITEEEED USER: " + split_string_user);
            string split_string_ip = strlist[1].Remove(strlist[1].Length - 4);
            Trace.WriteLine("SPLITEEEED IP: " + split_string_ip);

            ChatBoxHistoryMessages.Clear();
            collectChatHistoryMessages(split_string_ip, split_string_user);
        }

        private void ConnectionErrorPopup()
        {
            ErrorText = "Error: No user listening at " + ip + ":" + port + ".";
            ErrorTextVisible = "Visible";
        }

        private void ConnectionDeniedPopup(string user)
        {
            ErrorText = "Error: " + user + " has denied your chat request!";
            ErrorTextVisible = "Visible";
        }

        private void ConnectionClosedErrorPopup()
        {
            ErrorText = "Error: User has closed the chat.";
            ErrorTextVisible = "Visible";
            ChatViewVisible = "Hidden";
        }

    }
}
