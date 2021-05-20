using bigwork.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace bigwork
{
    /// <summary>
    /// Register.xaml 的交互逻辑
    /// </summary>
    public partial class Register : Window
    {
        Client vm = new Client();
        public Register()
        {
            vm.ConnectServer("127.0.0.1", 9999);
            InitializeComponent();
        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            string name = user_name.Text.Trim();
            string passwd = user_passwd.Password.Trim();
            if (name == "" || passwd == "")//不正确
            {
                error.Visibility = Visibility.Visible;
                return;
            }
            vm.SendInfo($"register {name} {passwd}");

            SignIn signIn = new SignIn();
            signIn.Show();
            this.Close();
        }
    }
}
