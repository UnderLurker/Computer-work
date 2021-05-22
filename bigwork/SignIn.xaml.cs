using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using bigwork.Net;
using bigwork.ViewModel;

namespace bigwork
{
    /// <summary>
    /// SignIn.xaml 的交互逻辑
    /// </summary>
    public partial class SignIn : Window
    {
        Client vm = new Client();
        public SignIn()
        {
            vm.ConnectServer("127.0.0.1", 9999);
            InitializeComponent();
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            while (true)
            {
                string name = user_name.Text.Trim();
                string passwd = user_passwd.Password.Trim();
                if (isVictory)
                {
                    MainWindow mainWindow = new MainWindow(name, vm);
                    mainWindow.Show();
                    this.Close();
                    break;
                }

                if (name == "" || passwd == "")//不正确
                {
                    error.Visibility = Visibility.Visible;
                    return;
                }
                //发送给服务器消息
                vm.SendInfo($"signin {name} {passwd}");
            }
        }
        private void SignUp_Click1(object sender, RoutedEventArgs e)
        {
            Register register = new Register();
            register.Show();
            this.Close();
        }

        public void jump(string name)
        {
            Dispatcher.Invoke(
                new Action(
                        delegate
                        {
                            MainWindow mainWindow = new MainWindow(name, this.vm);
                            mainWindow.Show();
                            this.Close();
                        }
                    ));
        }

        private static bool isVictory = false;
        //登录是否成功
        public static string isSuccessful(string name)
        {
            isVictory = true;
            return name;
        }

    }
}
