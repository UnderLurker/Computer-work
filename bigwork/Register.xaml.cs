using bigwork.Net;
using System.Windows;

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
            string passwd_again = user_passwd_again.Password.Trim();
            if (name == "" || passwd == "" || passwd_again == "" || passwd != passwd_again) //不正确
            {
                error.Visibility = Visibility.Visible;
                return;
            }
            vm.SendInfo($"register {name} {passwd}");

            SignIn signIn = new SignIn();
            signIn.Show();
            this.Close();
        }

        private void sregister_Click(object sender, RoutedEventArgs e)
        {
            SignIn signIn = new SignIn();
            signIn.Show();
            this.Close();
        }
    }
}
