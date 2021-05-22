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
            get { return socket; }
            set { socket = value; }
        }

        //private readonly string passwd;
        public string Passwd { get; set; }

        private bool online = false;
        public bool OnLine
        {
            get { return online; }
            set { online = value; }
        }

        private List<string> contactPersonList = new List<string>() { };
        public List<string> ContanctPerosnList
        {
            get { return contactPersonList; }
            set { contactPersonList = value; }
        }
    }
}
