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
            //���ļ���ȡ������Ϣ����ϵ��
            //��ϵ��
            //StreamReader input = ContactPersonRW.readStart();
            //string context = string.Empty;
            //while ((context= ContactPersonRW.readLine(input))!=string.Empty)
            //{
            //    string[] info = context.Split(' ');
            //    //�ļ�
            //    StreamReader i = FileReadWrite.readStart();
            //    string contexti = string.Empty;
            //    ObservableCollection<Chat> chatContext = new ObservableCollection<Chat>() { };
            //    while ((contexti = FileReadWrite.readLine(i)) != string.Empty)
            //    {
            //        string[] chatInfo = contexti.Split(' ');
            //        if (chatInfo[2] == info[1])
            //        {
            //            //ͬһ��
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

        //ѡ����ϵ��
        public RelayCommand<ContactPersonModule> SelectedCommand { get; set; }
        private void Select(ContactPersonModule model)
        {
            ContactPersonModule = model;
        }

        //��ϵ���б�
        private static ObservableCollection<ContactPersonModule> personList = new ObservableCollection<ContactPersonModule>();
        public ObservableCollection<ContactPersonModule> PersonList
        {
            get { return personList; }
            set { personList = value; RaisePropertyChanged(); }
        }

        //�����ϵ��
        public void AddContactPerson(string name)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                personList.Add(new ContactPersonModule() { ContactPersonAvatar = name[0], OnLine = true, ContactPersonID = name });
            }));

            //RaisePropertyChanged();
        }
        //ɾ����ϵ��
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


        //��ҳ��Ϣ
        private ContactPersonModule contactPersonModule;
        public ContactPersonModule ContactPersonModule
        {
            get { return contactPersonModule; }
            set { contactPersonModule = value; RaisePropertyChanged(); }
        }

        //�����Ϣ
        public void AddChat(string ct)
        {
            ContactPersonModule.ChatContext.Add(new Chat()
            {
                context = ct,
                ContactPersonAvatar = Nname[0],
                name = Nname
            });
        }

        //�洢��Ϣ
        public void SaveFile(string context)
        {
            //�洢��Ϣ ��ʽ ʱ��+ͷ��+����+������Ϣ
            string message = string.Empty;
            string time = string.Format("{0:M}", DateTime.Now);
            string[] weeks = { "������", "����һ", "���ڶ�", "������", "������", "������", "������" };
            string week = weeks[System.Convert.ToInt32(System.DateTime.Now.DayOfWeek)];
            message += $"{time},{week}" + " ";
            message += contactPersonModule.ContactPersonAvatar + " ";
            message += contactPersonModule.ContactPersonID + " ";
            //foreach (Chat i in contactPersonModule.ChatContext)
            //{
            //    message += i.context + " ";
            //}
            message += context;
            //message += "$";//������

            FileReadWrite.write(message);
        }
        //������ϵ��
        public static void SaveContactPerson(string name, char avator)
        {
            //��ʽ ͷ��+����
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

        //��ϵ������true yes
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

        //��ϵ������
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

        //�����Ϣ
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