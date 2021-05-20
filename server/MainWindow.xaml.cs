using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows;
using server.data;
using server.SQL;

namespace server
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        static SqlConnect sql = new SqlConnect();
        static int count = 0;
        List<Customer> Person = new List<Customer>() { };

        public MainWindow()
        {
            count=sql.SelectMySql(Person);
            InitializeComponent();
            server_info.Text = "服务器已关闭";
        }
        //1.1声明套接字
        Socket serverSocket = null;
        //3.2 创建用来专门作为监听来电等待工作的线程
        Thread listenThread = null;
        private void btnStartServer_Click(object sender, EventArgs e)
        {
            if (btnStartServer.Tag.ToString() == "open")
            {
                btnStartServer.Tag = "close";
                btnStartServer.Content = "关闭服务器";
                //1.2调用Socket()函数 用于通信的套接字               
                //第一个参数为寻找地址的方式,此时选定为IPV4的地址; 第二个参数为数据传输的方式，此时选择的是Stream传输(能够准确无误的将数据传输到)；第三个参数为执行的协议，此时选择的是TCP协议；
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //2.套接字绑定端口号，设置套接字的地址调用bind()因为此函数需要EndPoint 所以创建2.1和2.2
                //2.1 设置地址  IPaddress 在using System.Net;下此时需引入
                IPAddress address = IPAddress.Parse(txtIP.Text.Trim());
                //2.2 设置地址和端口
                IPEndPoint endPoint = new IPEndPoint(address, int.Parse(txtPort.Text.Trim())); //第一个参数为要设置的IP地址，第二参数为端口号
                try
                {
                    //2.套接字绑定端口号和IP
                    serverSocket.Bind(endPoint);
                    server_info.Text = "服务已开启！";
                    System.Windows.MessageBox.Show("开启服务成功", "开启服务");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("开启服务失败：" + ex.Message, "开启服务失败");//ex.Message为出现异常的消息
                    return;
                }
                //3.1监听套接字，等待
                serverSocket.Listen(10); //参数为最大监听的用户数
                listenThread = new Thread(ListenConnectSocket);
                listenThread.IsBackground = true; //关闭后天线程
                listenThread.Start();
            }
            else if (btnStartServer.Tag.ToString() == "close")
            {
                server_info.Text = "服务器已关闭";
                btnStartServer.Tag = "open";
                btnStartServer.Content = "开启服务器";
                isOpen = false;
                serverSocket.Close();
                serverSocket.Dispose();
            }
        }
        //3.3 用于判断用户是否链到服务器
        bool isOpen = true;
        Socket ClientSocket = null;
        //3.4 监听用户来电 等待
        void ListenConnectSocket()
        {
            while (isOpen)
            {
                try
                {
                    ClientSocket = serverSocket.Accept();
                    //byte[] buffer = Encoding.Default.GetBytes("成功连接到服务器！");
                    //ClientSocket.Send(buffer);
                    //string client = ClientSocket.RemoteEndPoint.ToString();
                    //this.Dispatcher.Invoke(new Action<string>((msg) =>
                    //{
                    //    listBox1.Items.Add(DateTime.Now + ": " + msg);
                    //}), client);

                    Thread thr = new Thread(ReceiveCkientMsg);
                    thr.IsBackground = true;
                    thr.Start(ClientSocket);
                }
                catch (Exception ex)
                {
                    listenThread.Abort(ex.Message);
                }
            }
        }

        /// <summary>
        /// 服务器解释用户消息
        /// </summary>
        /// <param name="clientSocket"></param>
        private void ReceiveCkientMsg(object clientSocket)
        {
            Socket client = clientSocket as Socket;
            while (true)
            {
                byte[] recBuffer = new byte[1024 * 1024 * 2];
                int length = -1;
                try
                {
                    length = client.Receive(recBuffer);
                }
                catch (Exception ex)
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        string msg = client.RemoteEndPoint.ToString();
                        listBox1.Items.Add($"{msg}:{msg}");
                        listBox1.Items.Remove(msg);
                    }));
                    break;
                }
                string[] str = Encoding.Default.GetString(recBuffer, 0, length).Split(' ');
                if (str.Length == 2)
                {
                    if (str[0] == "personlist")
                    {
                        string contactperson = sql.readPerson(str[1]);
                        ClientSocket.Send(Encoding.Default.GetBytes($"personlist {contactperson}"));
                    }
                }
                else if (str.Length == 3)
                {
                    if (str[0] == "添加")
                    {
                        foreach (Customer i in Person)
                        {
                            if (i.Name == str[2])
                            {
                                if (i.OnLine == false)
                                {
                                    foreach (Customer j in Person)
                                    {
                                        if (j.Name == str[1])
                                        {
                                            j.Socket.Send(Encoding.Default.GetBytes($"noline {str[2]}"));
                                        }
                                    }
                                    break;
                                }
                                i.Socket.Send(Encoding.Default.GetBytes($"please {str[1]} {str[2]}"));
                                break;
                            }
                        }
                    }
                    else if (str[0] == "yes")
                    {
                        foreach (Customer i in Person)
                        {
                            if (i.Name == str[2])
                            {
                                string contactpersons = "";
                                foreach(string j in i.ContanctPerosnList)
                                {
                                    if (j == "") continue;
                                    contactpersons += j + ";";
                                }
                                contactpersons += str[1] + ";";
                                sql.updatePerson(i.Name, contactpersons);
                                i.Socket.Send(Encoding.Default.GetBytes($"yes {str[1]}"));
                                i.ContanctPerosnList.Add(str[1]);
                            }
                            if (i.Name == str[1])
                            {
                                string contactpersons = "";
                                foreach (string j in i.ContanctPerosnList)
                                {
                                    if (j == "") continue;
                                    contactpersons += j + ";";
                                }
                                contactpersons += str[2] + ";";
                                sql.updatePerson(i.Name, contactpersons);
                                i.ContanctPerosnList.Add(str[2]);
                            }
                        }
                    }
                    else if (str[0] == "register")
                    {
                        Customer temp = new Customer() { Name = str[1], Passwd = str[2], ID=count+1, OnLine=false };
                        sql.insertSql(temp, ++count);
                        Person.Add(temp);
                    }
                    else if (str[0] == "signin")
                    {
                        bool isy = false;
                        foreach (Customer i in Person)
                        {
                            if (i.Name == str[1])
                            {
                                if (i.Passwd == str[2])
                                {
                                    ClientSocket.Send(Encoding.Default.GetBytes($"successful {str[1]}"));
                                    i.Socket = ClientSocket;
                                    i.OnLine = true;
                                    isy = true;
                                    //string contactperson = sql.readPerson(i.Name);
                                    //ClientSocket.Send(Encoding.Default.GetBytes($"personlist {contactperson}"));
                                    break;
                                }
                                ClientSocket.Send(Encoding.Default.GetBytes($"failed 密码错误"));
                                isy = true;
                                break;
                            }
                        }
                        if (!isy)
                        {
                            ClientSocket.Send(Encoding.Default.GetBytes($"fail {str[1]}未注册"));
                        }
                    }
                    else if(str[0]== "deleteperson")
                    {
                        string list1 = "", list2 = "";
                        foreach (Customer i in Person)
                        {
                            if (i.Name == str[1])
                            {
                                for (int j = 0; j < i.ContanctPerosnList.Count; j++)
                                {
                                    if (i.ContanctPerosnList[j] == str[2])
                                    {
                                        i.ContanctPerosnList.Remove(i.ContanctPerosnList[j]);
                                        j--;
                                        continue;
                                    }
                                    list1 += i.ContanctPerosnList[j] + ';';
                                }
                            }
                            if (i.Name == str[2])
                            {
                                i.Socket.Send(Encoding.Default.GetBytes($"deleteperson {str[1]}"));
                                for (int j = 0; j < i.ContanctPerosnList.Count; j++)
                                {
                                    if (i.ContanctPerosnList[j] == str[1])
                                    {
                                        i.ContanctPerosnList.Remove(i.ContanctPerosnList[j]);
                                        j--;
                                        continue;
                                    }
                                    list2 += i.ContanctPerosnList[j] + ';';
                                }
                            }
                        }
                        sql.deletePerson(str[1], list1);
                        sql.deletePerson(str[2], list2);
                    }
                }
                else if (str.Length == 2)
                {
                    if (str[0] == "no")
                    {
                        foreach (Customer i in Person)
                        {
                            if (i.Name == str[1])
                            {
                                i.Socket.Send(Encoding.Default.GetBytes("no"));
                            }
                        }
                    }
                    //else if (str[0] == "out")
                    //{
                    //    foreach (Customer i in Person)
                    //    {
                    //        if (i.Name == str[1])
                    //        {
                    //            i.OnLine = false;
                    //            continue;
                    //        }
                    //        else if (i.OnLine == false)
                    //        {
                    //            continue;
                    //        }
                    //        foreach (string j in i.ContanctPerosnList)
                    //        {
                    //            i.Socket.Send(Encoding.Default.GetBytes($"out {str[1]}"));
                    //        }
                    //        Person.Remove(i);
                    //    }
                    //}
                }
                else if (str.Length == 4)
                {
                    if (str[0] == "add")
                    {
                        foreach (Customer i in Person)
                        {
                            if (i.Name == str[2] && i.OnLine)
                            {
                                i.Socket.Send(Encoding.Default.GetBytes($"info {str[1]} {str[3]}"));
                                break;
                            }
                        }
                    }
                }

                if (length == 0)
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        string msg = client.RemoteEndPoint.ToString();
                        listBox1.Items.Add($"{msg}:下线了！");
                        listBox1.Items.Remove(msg);
                    }));
                    break;
                }
                else
                {
                    string msg = Encoding.Default.GetString(recBuffer, 0, length);
                    string msgStr = $"{DateTime.Now}" + "\n" + $"[接收{client.RemoteEndPoint.ToString()}]{msg}";
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        listBox2.Items.Add(msgStr + Environment.NewLine);
                    })); 
                    //这错了
                    if (str[0] == "out")
                    {
                        foreach (Customer i in Person)
                        {
                            if (i.Name == str[1])
                            {
                                i.OnLine = false;
                                foreach(string name in i.ContanctPerosnList)
                                {
                                    foreach(Customer j in Person)
                                    {
                                        if (j.Name == name && j.OnLine == true)
                                        {
                                            j.Socket.Send(Encoding.Default.GetBytes($"out {str[1]}"));
                                            break;
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

    }
}
