using bigwork.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace bigwork.Net
{
    public class Client
    {
        private SignIn si = null;
        public SignIn Si
        {
            get { return si; }
            set { si = value; }
        }
        private static MainViewModel mv;
        public static MainViewModel MV
        {
            get { return mv; }
            set { mv = value; }
        }
        static Socket clientSocket = null;
        static Thread clientThread = null;
        //连接服务器
        public void ConnectServer(string address, int port)
        {
            //创建套接字
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //设置IP地址和端口号
            IPAddress address1 = IPAddress.Parse(address.Trim());
            IPEndPoint endPoint = new IPEndPoint(address1, port);
            try
            {
                //与服务器建立连接
                clientSocket.Connect(endPoint);
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接失败：" + ex.Message, "友情提示");
                Messenger.Default.Send("Close");
            }
            //接收或发送消息 使用线程来实现
            clientThread = new Thread(ReceiveMsg);
            clientThread.IsBackground = true; //开启后台线程
            clientThread.Start();
        }

        public void ReceiveMsg()
        {
            while (true)
            {
                byte[] recBuffer = new byte[1024 * 1024 * 2];//声明最大字符内存
                int length = -1; //字节长度
                try
                {
                    length = clientSocket.Receive(recBuffer);//返回接收到的实际的字节数量
                }
                catch (SocketException ex)
                {
                    System.Windows.Forms.MessageBox.Show("连接失败：" + ex.Message, "友情提示");
                    break;
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("连接失败：" + ex.Message, "友情提示");
                    break;
                }
                //接收到消息
                if (length > 0)
                {
                    string msg = Encoding.Default.GetString(recBuffer, 0, length);//转译字符串(字符串，开始的索引，字符串长度)
                    string[] info = msg.Split(' ');
                    if (info[0] == "personlist")
                    {
                        if (info.Length > 1) 
                        {
                            string[] temp = info[1].Split(';');
                            for (int i = 0; i < temp.Length; i++)
                            {
                                if (temp[i] == "") break;
                                MV.AddContactPerson(temp[i]);
                            }

                        }
                    }
                    if (info.Length == 2)
                    {
                        if (info[0] == "null")
                        {
                            System.Windows.MessageBox.Show($"没有 {info[1]}");
                        }
                        else if (info[0] == "successful")
                        {
                            //Si.jump(info[1]);
                            SignIn.isSuccessful(info[1]);

                        }
                        else if (info[0] == "failed")
                        {
                            MessageBox.Show($"{info[1]}");
                        }
                        else if (info[0] == "noline")
                        {
                            MessageBox.Show($"{info[1]} 不在线，请稍后重试。");
                        }
                        else if (info[0] == "yes")
                        {
                            MV.AddContactPerson(info[1]);
                        }
                        else if (info[0] == "out")
                        {
                            MV.outLine(info[1]);
                        }
                        else if (info[0] == "deleteperson")
                        {
                            MV.DeleteContactPerson(info[1]);
                        }
                    }
                    else if (info.Length == 3)
                    {
                        if (info[0] == "online")
                        {
                            bool.TryParse(info[1], out bool result);
                            MV.ContactPersonOnline(info[2], result);
                        }
                        else if (info[0] == "please")
                        {
                            if (MessageBox.Show($"{info[1]} 请求添加你为好友", "添加好友", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                MV.AddContactPerson(info[1]);
                                SendInfo($"yes {info[2]} {info[1]}");
                            }
                            else
                            {
                                SendInfo($"no {info[1]}");
                            }
                        }
                        else if (info[0] == "info")
                        {
                            MainViewModel.AddInfo(info[1], info[2]);
                        }
                    }
                    else if (info.Length == 1)
                    {
                        if (info[0] == "no")
                        {
                            MessageBox.Show("对方拒绝添加您为好友", "添加好友");
                        }
                    }

                }
            }
        }

        //发送消息
        public void SendInfo(string context)
        {
            string str = context.Trim();
            byte[] buffer = Encoding.Default.GetBytes(str);
            clientSocket.Send(buffer);
        }
    }
}
