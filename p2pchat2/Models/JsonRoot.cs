using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace p2pchat2.Models
{
    public class MessageData
    {
        public string Username { get; set; }
        public string Message { get; set; }
        public string Ip { get; set; }
        public DateTime Time { get; set; }
        public bool ReadyToSend { get; set; }
        public int Command { get; set; }
    }

    public class JRoot
    {
        public List<MessageData> messages { get; set; }
    }
}
