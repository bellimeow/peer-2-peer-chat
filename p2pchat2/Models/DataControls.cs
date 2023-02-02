using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Text.Json;

namespace p2pchat2.Models
{
    class DataControls
    {
        public static string username;
        public static string ip;
        private static string projFolder = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        private static string modelfolder = projFolder;
        private static readonly int basicFileCharSize = 20;

        private static string getFileName(string ip, string recieverName)
        {
            return string.Concat(modelfolder, $@"\Models\History\{recieverName}-{ip}.txt");
        }

        public static string gethistory(string ip, string recieverName)
        {
             string fileName = getFileName(ip, recieverName);
            return File.ReadAllText(fileName);
        }

        public static MessageList GetChatMessages(string ip, string username)
        {
            string fileName = getFileName(ip, username);
            string jString = File.ReadAllText(fileName);
            if (jString.Length < basicFileCharSize) { return new MessageList(); }
            var jsonList = JsonConvert.DeserializeObject<JRoot>(jString);
            var retList = new MessageList();
            if (jsonList != null )
            {
                foreach (MessageData data in jsonList.messages)
                {
                    ChatMessage newMessage = new ChatMessage();
                    newMessage.parseChatMessageData(data);
                    retList.Add(newMessage);
                }
            }
            return retList;
        }

        private static void killLastLine(string filename)
        {
            var lines = System.IO.File.ReadAllLines(filename);
            System.IO.File.WriteAllLines(filename, lines.Take(lines.Length - 1).ToArray());
        }

        public static void addHistoryFromMe(string receiverIp, string recieverName, string sender, string message)
        {

            string fileName = getFileName(receiverIp, recieverName);
            killLastLine(fileName);
            try
            {
                int filesize = System.IO.File.ReadAllLines(fileName).Length;
                ChatMessage messageObj = new ChatMessage();
                messageObj.Ip = ip;
                messageObj.Username = sender;
                messageObj.Message = message;
                messageObj.Time = DateTime.Now;

                using (StreamWriter ws = File.AppendText(fileName))
                {


                    if (filesize > 3) { ws.WriteLine(","); }
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    ws.WriteLine(System.Text.Json.JsonSerializer.Serialize(messageObj, options ));
                    ws.WriteLine("]}");
                    ws.Close();
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }

        }
    
        public static void addHistoryToMe(string senderIp, string recieverName, string sender, string message)
        {
            
            
            string fileName = getFileName(senderIp, sender);
            killLastLine(fileName);
            try
            {
                int filesize = System.IO.File.ReadAllLines(fileName).Length;
                ChatMessage messageObj = new ChatMessage();
                messageObj.Ip = ip;
                messageObj.Username = sender;
                messageObj.Message = message;
                messageObj.Time = DateTime.Now;
                WaitForFile(fileName);
                using (StreamWriter ws = File.AppendText(fileName))
                {
                    if (filesize > 3) { ws.WriteLine(","); }
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    ws.WriteLine(System.Text.Json.JsonSerializer.Serialize(messageObj, options));
                    ws.WriteLine("]}");
                    ws.Close();
                }
                
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }

        }
        public static bool IsFileReady(string filename)
        {
            try
            {
                using (FileStream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                    return inputStream.Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void WaitForFile(string filename)
        {
            while (!IsFileReady(filename)) { }
        }

        public static void makeHistory(string ip, string recieverName)
        {
            string fileName = getFileName(ip, recieverName);
            Trace.WriteLine(fileName);
            FileInfo fi = new FileInfo(fileName);

            if (fi.Exists)
            {
                return;
            }
            
            using (FileStream fs = fi.Create())
            {
                Byte[] txt = new UTF8Encoding(true).GetBytes("{" + "\"messages\":" + "[" + "\n" + "]}");
                fs.Write(txt);
            }
        }

        public static List<string> fileNamesToList()
        {
            DirectoryInfo directory = new DirectoryInfo(projFolder + "\\Models\\History");

            List<string> retList = new List<string>();

            var sortedFiles = new DirectoryInfo(projFolder + "\\Models\\History").GetFiles()
                                      .OrderByDescending(f => f.LastWriteTime)
                                      .ToList();

            foreach (var sortedFile in sortedFiles)
            {
                retList.Add(sortedFile.Name);
            }

            return retList;
        }
    }
}
