using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bigwork.FileRW
{
    public class ContactPersonRW
    {
        public static StreamReader readStart()
        {
            StreamReader input = new StreamReader(@"F:\Visual Studio 练习\wpf\bigwork\bigwork\data\ContactPerson.txt", Encoding.UTF8);
            return input;
        }
        //一次读一行
        public static string readLine(StreamReader input)
        {
            //string result[3];
            string ans = input.ReadLine();
            if (ans == null) return string.Empty;
            return ans;
        }
        public static void readEnd(StreamReader input)
        {
            input.Close();
        }

        public static void write(string context)
        {
            StreamWriter output = new StreamWriter(@"F:\Visual Studio 练习\wpf\bigwork\bigwork\data\ContactPerson.txt", true, Encoding.UTF8);
            output.WriteLine(context);
            output.Close();
        }
    }
}
