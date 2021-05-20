using bigwork.FileRW;
using bigwork.Modle;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;

namespace bigwork.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            //PersonList = new ObservableCollection<ContactPersonModule>();
            //从文件读取聊天信息和联系人
            //联系人
            //StreamReader input = ContactPersonRW.readStart();
            //string context = string.Empty;
            //while ((context= ContactPersonRW.readLine(input))!=string.Empty)
            //{
            //    string[] info = context.Split(' ');
            //    //文件
            //    StreamReader i = FileReadWrite.readStart();
            //    string contexti = string.Empty;
            //    ObservableCollection<Chat> chatContext = new ObservableCollection<Chat>() { };
            //    while ((contexti = FileReadWrite.readLine(i)) != string.Empty)
            //    {
            //        string[] chatInfo = contexti.Split(' ');
            //        if (chatInfo[2] == info[1])
            //        {
            //            //同一人
            //            chatContext.Add(new Chat() { context = chatInfo[3] });
            //        }
            //    }
            //    FileReadWrite.readEnd(i);
            //    personList.Add(new ContactPersonModule() { ContactPersonID = info[1], ContactPersonAvatar = info[0][0], ChatContext=chatContext});
            //}
            //ContactPersonRW.readEnd(input);


            SelectedCommand = new RelayCommand<ContactPersonModule>(t => Select(t));
        }

        private bool online = true;
        public bool OnLine
        {
            get { return online; }
            set { online = value; RaisePropertyChanged(); }
        }
        private static string name = string.Empty;
        public static string Nname
        {
            get { return name; }
            set { name = value; }
        }

        //选择联系人
        public RelayCommand<ContactPersonModule> SelectedCommand { get; set; }
        private void Select(ContactPersonModule model)
        {
            ContactPersonModule = model;
        }

        //联系人列表
        private static ObservableCollection<ContactPersonModule> personList = new ObservableCollection<ContactPersonModule>();
        public ObservableCollection<ContactPersonModule> PersonList
        {
            get { return personList; }
            set { personList = value; RaisePropertyChanged(); }
        }

        //添加联系人
        public void AddContactPerson(string name)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                personList.Add(new ContactPersonModule() { ContactPersonAvatar = name[0], OnLine = true, ContactPersonID = name });
            }));

            //RaisePropertyChanged();
        }
        //删除联系人
        public void DeleteContactPerson(String name)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                foreach (ContactPersonModule i in personList)
                {
                    if (i.ContactPersonID == name)
                    {
                        personList.Remove(i);
                        break;
                    }
                }
            }));
        }


        //本页信息
        private ContactPersonModule contactPersonModule;
        public ContactPersonModule ContactPersonModule
        {
            get { return contactPersonModule; }
            set { contactPersonModule = value; RaisePropertyChanged(); }
        }

        //添加消息
        public void AddChat(string ct)
        {
            ContactPersonModule.ChatContext.Add(new Chat()
            {
                context = ct,
                ContactPersonAvatar = Nname[0],
                name = Nname
            });
        }

        //存储信息
        public void SaveFile(string context)
        {
            //存储信息 格式 时间+头像+名字+聊天信息
            string message = string.Empty;
            string time = string.Format("{0:M}", DateTime.Now);
            string[] weeks = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            string week = weeks[System.Convert.ToInt32(System.DateTime.Now.DayOfWeek)];
            message += $"{time},{week}" + " ";
            message += contactPersonModule.ContactPersonAvatar + " ";
            message += contactPersonModule.ContactPersonID + " ";
            //foreach (Chat i in contactPersonModule.ChatContext)
            //{
            //    message += i.context + " ";
            //}
            message += context;
            //message += "$";//结束符

            FileReadWrite.write(message);
        }
        //保存联系人
        public static void SaveContactPerson(string name, char avator)
        {
            //格式 头像+名字
            string message = string.Empty;
            message += avator + " " + name;
            ContactPersonRW.write(message);
        }

        public void change(bool info, string id)
        {
            foreach (ContactPersonModule i in personList)
            {
                if (i.ContactPersonID == id)
                {
                    i.OnLine = info;
                }
            }
        }

        //联系人上线true yes
        public void ContactPersonOnline(string name, bool onLine)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                foreach (ContactPersonModule i in personList)
                {
                    if (i.ContactPersonID == name)
                    {
                        i.OnLine = onLine;
                        break;
                    }
                }
                RaisePropertyChanged();
            }));
        }

        //联系人下线
        public void outLine(string name)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                foreach (ContactPersonModule i in personList)
                {
                    if (i.ContactPersonID == name)
                    {
                        i.OnLine = false;
                        break;
                    }
                }
                RaisePropertyChanged();
            }));
        }

        //添加消息
        public static void AddInfo(string name, string context)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                foreach (ContactPersonModule i in personList)
                {
                    if (i.ContactPersonID == name)
                    {
                        i.ChatContext.Add(new Chat() { context = context, ContactPersonAvatar = name[0],name=name});
                    }
                }
            }));
        }
    }
}