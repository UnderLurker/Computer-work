using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace bigwork.Modle
{
    public class ContactPersonModule : ViewModelBase
    {
        //public string name { get; set; }
        //联系人头像文字
        public char ContactPersonAvatar { get; set; }

        //是否在线
        public bool OnLine { get; set; } = false;

        //联系人ID
        public string ContactPersonID { get; set; } = string.Empty;

        //聊天信息
        private ObservableCollection<Chat> chatContext = new ObservableCollection<Chat>() { };
        public ObservableCollection<Chat> ChatContext
        {
            get { return chatContext; }
            set { chatContext = value; RaisePropertyChanged(); }
        }

    }
    public class Chat
    {
        public string context { get; set; }
        public char ContactPersonAvatar { get; set; }
        public string name { get; set; }
    }
}
