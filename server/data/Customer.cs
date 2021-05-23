using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace server.data
{
    public class Customer
    {
        public string Name { get; set; }

        public int ID { get; set; }

        private Socket socket = null;
        public Socket Socket
        {
            set { socket = value; }
        }

        public void SendInfo(string context)
        {
            string str = context.Trim();
            byte[] buffer = Encoding.Default.GetBytes(str);
            socket.Send(buffer);
        }

        public string Passwd { get; set; }

        private bool online = false;
        public bool OnLine
        {
            get { return online; }
            set { online = value; }
        }

        private Dictionary<string, string> contactPersonList = new Dictionary<string, string>() { };
        public Dictionary<string, string> ContanctPerosnList
        {
            get { return contactPersonList; }
            set { contactPersonList = value; }
        }


    }
}
