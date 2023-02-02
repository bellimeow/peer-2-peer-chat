using System;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace p2pchat2.Models
{
    public class ChatMessage : INotifyPropertyChanged
    {
        private string ip;
        private string username;
        private string message;
        private System.DateTime time;
        private static int count;
        private bool readyToSend;
        private commandStatus command;

        public enum commandStatus
        {
            ACCEPT,
            DENY,
            BUZZ,
            CONNECTPOPUP,
            NOCOMMAND
        }

        public ChatMessage()
        {
            ip = "";
            username = "";
            message = "";

        }

        public string Ip
        {
            get { return ip; }
            set { ip = value; OnPropertyChanged("Ip"); }
        }

        public System.DateTime Time
        {
            get { return time; }
            set { time = value; OnPropertyChanged("Time"); }
        }
        public string Message
        {
            get { return message; }
            set { message = value; OnPropertyChanged("Message"); }
        }

        public string Username
        {
            get { return username; }
            set { username = value; OnPropertyChanged("Username"); }
        }

        public int Count
        {
            get { return count; }
            set { count = value; OnPropertyChanged("Count"); }
        }

        public bool ReadyToSend
        {
            get { return readyToSend; }
            set { readyToSend = value; OnPropertyChanged("ReadyToSend"); }
        }

        public commandStatus Command
        {
            get { return command; }
            set { command = value; OnPropertyChanged("Command"); }
        }

        public string getString()
        {
            return ip + ": " + username + ": " + message;
        }

        public void parseJson(JObject j)
        {
            ip = (string)j.GetValue("Ip");
            username = (string)j.GetValue("Username");
            message = (string)j.GetValue("Message");
            command = (commandStatus)Enum.Parse(typeof(commandStatus), (string)j.GetValue("Command"));
                
    
            return ;
        }

        public void parseChatMessageData(MessageData cm)
        {
            ip = cm.Ip;
            username = cm.Username;
            message = cm.Message;
            command = (ChatMessage.commandStatus)cm.Command;
            Time = cm.Time;

            return;
        }
        public string getJsonString()
        {
            var j = new JObject();
            j.Add("Ip", ip);
            j.Add("Username", username);
            j.Add("Message", message);
            j.Add("Command", command.ToString());
            return j.ToString();
        }

        public static JObject toJson(string s)
        {
            return JObject.Parse(s);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string v)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}