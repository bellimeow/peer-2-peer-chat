using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.IO;

namespace p2pchat2.Models
{
    class Server: INotifyPropertyChanged
    {
        private string myUsername;
        private string myIp;
        private static string yourUsername;
        private static string yourIp;
        private static ChatData receivedServerMessages = new ChatData();
        private static bool isNewConnection = true;
        private bool sendMessage;
        private ChatMessage sendServerMessage = new ChatMessage();
        private int connectionClosed;
        private int clientConnectError;
        private string receivedServerMessage;

        public event PropertyChangedEventHandler PropertyChanged;

        public static List<TcpClient> Clients = new List<TcpClient>();
        string clientIP = null;
       

        public Server(string myIpNew, string myUsernameNew)
        {
            myIp = Server.GetIp();
            myUsername = myUsernameNew;

        }

        public bool getSendMessage()
        {
            return sendMessage;
        }


        public string MyUsername
        {
            get { return myUsername; }
            set { myUsername = value; sendServerMessage.Username = value; }
        }

        public string MyIP
        {
            get { return myIp; }
            set { myIp = value; sendServerMessage.Ip = value; }
        }

        public string YourIp
        {
            get { return yourIp; }
            set { yourIp = value; }
        }

        public string YourUsername
        {
            get { return yourUsername; }
            set { yourUsername = value; }
        }

        public string ReceivedServerMessage
        {
            get { return receivedServerMessage; }
            set
            {
                receivedServerMessage = value;
                OnPropertyChanged("ReceivedServerMessage");
            }
        }

        public ChatMessage SendServerMessage
        {
            get { return sendServerMessage; }
            set
            {
                sendServerMessage = value;
                OnPropertyChanged("SendServerMessage");
            }
        }

        public int ConnectionClosed
        {
            get { return connectionClosed; }
            set
            {
                connectionClosed = value;
                OnPropertyChanged("ConnectionClosed");
            }
        }

        public int ClientConnectError
        {
            get { return clientConnectError; }
            set
            {
                clientConnectError = value;
                OnPropertyChanged("ClientConnectError");
            }
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public void newClient()
        {

        }

        public void runListen(int port_in)
        {
            awaitClient(port_in);
        }

        public void awaitClient(int port_in)
        {
            
            Trace.WriteLine("Local address: " + myIp);
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    TcpListener server = null;
                  

                    Trace.WriteLine("Local address: " + GetLocalIPAddress() + "port: " + port_in);
                    IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                    server = new TcpListener(localAddr, (int)port_in);
                    server.Start();
                   
                    Trace.Write("Waiting for a connection... ");
                    Clients.Add(await server.AcceptTcpClientAsync());
                    Trace.WriteLine("Connected!");
                    clientIP = Clients[0].Client.RemoteEndPoint.AddressFamily.ToString();

                    Listen();
                }
                catch
                {

                }
            });

        }

        public void getClient(int port_in, string serverIp)
        {

            Task.Factory.StartNew( () =>
            {
                try
                {                
                    Clients.Add(new TcpClient(serverIp, port_in));
                    sendServerMessage.Ip = myIp;
                    sendServerMessage.Username = myUsername;
                    SendMessage();
                }
                catch (ArgumentOutOfRangeException x)
                {
                    ClientConnectError++;
                }
                catch (SocketException x)
                {
                    ClientConnectError++;
                }
                Listen();
               
            });

        }

        public void Listen()
        {
            // Get a stream object for reading and writing
            
            Task.Factory.StartNew(async () =>
           {
               try
               {
                   NetworkStream stream = Clients[0].GetStream();
                   Byte[] bytes = new Byte[256];
                   String data = "";
                   while (true)
                   {
                       // Read to client
                       int i;
                       i = await stream.ReadAsync(bytes, 0, bytes.Length);
                       if (i > System.Text.Encoding.ASCII.GetBytes("").Length)
                       {
                           data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                           Trace.WriteLine("Received: " + data);

                           JObject jsonMessage = ChatMessage.toJson(data);
                  
                           YourIp = jsonMessage.GetValue("Ip").ToString();
                         
                           YourUsername = jsonMessage.GetValue("Username").ToString();
          
                           ReceivedServerMessage = data;
                         
                           ChatMessage x = new ChatMessage();
                           x.parseJson(JObject.Parse(data));
                           DataControls.makeHistory(x.Ip, x.Username);
                           if (x.Command == ChatMessage.commandStatus.NOCOMMAND)
                           {
                               DataControls.addHistoryToMe(YourIp, MyUsername, x.Username, x.Message);
                           }
                           stream.Flush();
                          
                       }
                   }
               }
               catch (IOException e)
               {
                   System.Diagnostics.Debug.WriteLine(e);
                   ConnectionClosed++;
                   Trace.WriteLine("IOException: {0}");
               }
               catch (IndexOutOfRangeException e)
               {
                   System.Diagnostics.Debug.WriteLine(e);
                 
                   Trace.WriteLine("IOException: {0}");
               }
           });
        }


        public static string GetIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public void SendMessage()
        {
            // Send to client
            byte[] msg;
         
            msg = System.Text.Encoding.ASCII.GetBytes(sendServerMessage.getJsonString());
            Trace.WriteLine("Sent: " + sendServerMessage.getJsonString());

            var stream = Clients[0].GetStream();
            stream.Write(msg, 0, msg.Length);
            if(sendServerMessage.Command == ChatMessage.commandStatus.NOCOMMAND) {
                DataControls.makeHistory(YourIp, YourUsername);
                DataControls.addHistoryFromMe(sendServerMessage.Ip, YourUsername, myUsername, sendServerMessage.Message);
            }
           

        }

        // Set commando to be sent with next message to client
        internal void setCommand(ChatMessage.commandStatus command)
        {
            sendServerMessage.Command = command;
        }

        internal void setReadyToSend()
        {
            sendServerMessage.ReadyToSend = true;
        }

        internal void setMessage(string s)
        {
            sendServerMessage.Message = s;
        }

        internal void clearMessage()
        {
            sendServerMessage = new ChatMessage();
        }

        internal MessageList getCurrentHistory()
        {
            var ml = DataControls.GetChatMessages(YourIp, yourUsername);
            return ml;
        }
        private void OnPropertyChanged(string v)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }
    }
}
