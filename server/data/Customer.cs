using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace server.data
{
    public class Customer
    {
        //private string name;
        public string Name { get; set; }

        //private int id;
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

        //private readonly string passwd;
        public string Passwd { get; set; }

        private bool online = false;
        public bool OnLine
        {
            get { return online; }
            set { online = value; }
        }

        private Dictionary<string,string> contactPersonList = new Dictionary<string, string>() { };
        public Dictionary<string, string> ContanctPerosnList
        {
            get { return contactPersonList; }
            set { contactPersonList = value; }
        }


    }
}
