using bigwork.Modle;
using bigwork.Net;
using bigwork.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace bigwork
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Client vm = null;
        public MainWindow(string name, Client client)
        {
            vm = client;
            InitializeComponent();
            this.MouseDown += (sender, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    this.DragMove();
            };
            this.DataContext = new MainViewModel();
            Client.MV = this.DataContext as MainViewModel;
            vm = new Client();
            vm.ConnectServer("127.0.0.1", 9999);
            MainViewModel.Nname = name;
            ID.Text = name;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(MainViewModel.Nname[0]);
            avatar.Text = sb.ToString();
            vm.SendInfo($"personlist {name}");
        }

        private void input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string name = input.Text;
                if (name == "") return;

                Client.MV = this.DataContext as MainViewModel;
                vm.SendInfo($"添加 {MainViewModel.Nname} {input.Text}");
                MainViewModel.SaveContactPerson(name, name[0]);
                input.Text = string.Empty;
            }
        }

        private void context_enter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string inputValue = tb.Text;
                if (inputValue == "") return;

                var vm1 = this.DataContext as MainViewModel;
                vm.SendInfo($"add {MainViewModel.Nname} {vm1.ContactPersonModule.ContactPersonID} {tb.Text}");
                vm1.AddChat(inputValue);
                tb.Text = "";
                scroll.ScrollToVerticalOffset(scroll.ScrollableHeight);
                vm1.SaveFile(inputValue);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string inputValue = tb.Text;
            if (inputValue == "") return;

            var vm1 = this.DataContext as MainViewModel;
            vm.SendInfo($"add {MainViewModel.Nname} {vm1.ContactPersonModule.ContactPersonID} {tb.Text}");
            vm1.AddChat(inputValue);
            tb.Text = "";
            scroll.ScrollToVerticalOffset(scroll.ScrollableHeight);
            vm1.SaveFile(inputValue);
        }

        /// <summary>
        /// 垂直滚动到中央
        /// </summary>
        /// <param name="scroll"></param>
        public static void ScrollToVertical(ScrollViewer scroll)
        {
            var max = scroll.ScrollableHeight;// 最大垂直滚动范围
            scroll.ScrollToVerticalOffset(max);
        }


        private void Button_Click_Min(object sender, RoutedEventArgs e)
        {
            this.WindowState = (WindowState)System.Windows.Forms.FormWindowState.Minimized;
        }

        private void Button_Click_Max(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == (WindowState)System.Windows.Forms.FormWindowState.Maximized)
            {
                this.WindowState = (WindowState)System.Windows.Forms.FormWindowState.Normal;
                lb.Height = 395;
            }
            else
            {
                this.WindowState = (WindowState)System.Windows.Forms.FormWindowState.Maximized;
                lb.Height = 720;
            }
        }

        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            vm.SendInfo($"out {MainViewModel.Nname}");
            this.Close();
        }

        private void Change_Button_Click(object sender, RoutedEventArgs e)
        {
            var widthAnimation = new DoubleAnimation();
            widthAnimation.Duration = TimeSpan.FromSeconds(0.2);
            if (list.Width == 250)
            {
                widthAnimation.From = 250;
                widthAnimation.To = 85;
                search.Visibility = Visibility.Hidden;
                logo.Text = "Chat";
            }
            else
            {
                widthAnimation.From = 85;
                widthAnimation.To = 250;
                search.Visibility = Visibility.Visible;
                logo.Text = "Online Chat";
            }
            list.BeginAnimation(StackPanel.WidthProperty, widthAnimation);
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var widthAnimation = new DoubleAnimation();
            widthAnimation.Duration = TimeSpan.FromSeconds(0.2);
            if (list.Width == 250)
            {
                widthAnimation.From = 250;
                widthAnimation.To = 85;
                search.Visibility = Visibility.Hidden;
                logo.Text = "Chat";
            }
            else
            {
                widthAnimation.From = 85;
                widthAnimation.To = 250;
                search.Visibility = Visibility.Visible;
                logo.Text = "Online Chat";
            }
            list.BeginAnimation(StackPanel.WidthProperty, widthAnimation);
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            ContactPersonModule sendery = lb.SelectedItem as ContactPersonModule;
            vm.SendInfo($"deleteperson {MainViewModel.Nname} {sendery.ContactPersonID}");

            var vm1 = this.DataContext as MainViewModel;
            vm1.DeleteContactPerson(sendery.ContactPersonID);
        }

        private void lb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            scroll.Visibility = Visibility.Visible;
            input1.Visibility = Visibility.Visible;
        }
    }
}
