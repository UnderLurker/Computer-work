using server.data;
using server.SQL;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace server
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        static SqlConnect sql = new SqlConnect();
        static int count = 0;
        Dictionary<string, Customer> Person = new Dictionary<string, Customer>() { };

        public MainWindow()
        {
            count = sql.SelectMySql(Person);
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
                        try
                        {
                            if (Person[str[2]].OnLine)
                            {
                                Person[str[2]].SendInfo($"please {str[1]} {str[2]}");
                            }
                            else
                            {
                                Person[str[1]].SendInfo($"noline {str[2]}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Person[str[1]].SendInfo($"none");
                        }
                    }
                    else if (str[0] == "yes")
                    {
                        string contactpersons = "";
                        foreach (string i in Person[str[2]].ContanctPerosnList.Values)
                        {
                            if (i == "") continue;
                            contactpersons += i + ";";
                        }
                        contactpersons += str[1] + ";";
                        sql.updatePerson(str[2], contactpersons);
                        Person[str[2]].SendInfo($"yes {str[1]}");
                        Person[str[2]].ContanctPerosnList.Add(str[1], str[1]);

                        contactpersons = "";
                        foreach (string i in Person[str[1]].ContanctPerosnList.Values)
                        {
                            if (i == "") continue;
                            contactpersons += i + ";";
                        }
                        contactpersons += str[2] + ";";
                        sql.updatePerson(str[1], contactpersons);
                        Person[str[1]].ContanctPerosnList.Add(str[2], str[2]);
                    }
                    else if (str[0] == "register")
                    {
                        Customer temp = new Customer() { Name = str[1], Passwd = str[2], ID = count + 1, OnLine = false };
                        sql.insertSql(temp, ++count);
                        Person.Add(temp.Name, temp);
                    }
                    else if (str[0] == "signin")
                    {
                        bool isy = false;
                        if (Person[str[1]].Passwd == str[2])
                        {
                            ClientSocket.Send(Encoding.Default.GetBytes($"successful {str[1]}"));
                            Person[str[1]].Socket = ClientSocket;
                            Person[str[1]].OnLine = true;
                            isy = true;
                        }
                        else
                        {
                            ClientSocket.Send(Encoding.Default.GetBytes($"failed 密码错误"));
                            isy = true;
                        }
                        if (!isy)
                        {
                            ClientSocket.Send(Encoding.Default.GetBytes($"fail {str[1]}未注册"));
                        }
                    }
                    else if (str[0] == "deleteperson")
                    {
                        string list1 = "", list2 = "";
                        Person[str[1]].ContanctPerosnList.Remove(str[2]);
                        foreach (string i in Person[str[1]].ContanctPerosnList.Values)
                        {
                            list1 += i + ";";
                        }
                        Person[str[2]].SendInfo($"deleteperson {str[1]}");
                        Person[str[2]].ContanctPerosnList.Remove(str[1]);
                        foreach (string i in Person[str[2]].ContanctPerosnList.Values)
                        {
                            list2 += i + ";";
                        }
                        sql.deletePerson(str[1], list1);
                        sql.deletePerson(str[2], list2);
                    }
                }
                else if (str.Length == 2)
                {
                    if (str[0] == "no")
                    {
                        Person[str[1]].SendInfo("no");
                    }
                }
                else if (str.Length == 4)
                {
                    if (str[0] == "add")
                    {
                        if (Person[str[2]].OnLine)
                        {
                            Person[str[2]].SendInfo($"info {str[1]} {str[3]}");
                        }
                        else
                        {
                            Person[str[1]].SendInfo($"noline {str[2]}");
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
                    string msgStr = $"{DateTime.Now}" + "\n" + $"[接收{client.RemoteEndPoint.ToString()}]\n{msg}";
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        listBox2.Items.Add(msgStr + Environment.NewLine);
                    }));
                    if (str[0] == "out")
                    {
                        Person[str[1]].OnLine = false;
                        foreach (string i in Person[str[1]].ContanctPerosnList.Values)
                        {
                            if (Person[i].OnLine)
                            {
                                Person[i].SendInfo($"out {str[1]}");
                            }
                        }
                    }
                }
            }
        }

    }
}
