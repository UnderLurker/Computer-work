using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bigwork.Modle;

namespace bigwork.FileRW
{
    //使用utf-8编码
    public class FileReadWrite
    {
        public static StreamReader readStart()
        {
            StreamReader input = new StreamReader(@"F:\Visual Studio 练习\wpf\bigwork\bigwork\data\data.txt", Encoding.UTF8);
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
            StreamWriter output = new StreamWriter(@"F:\Visual Studio 练习\wpf\bigwork\bigwork\data\data.txt", true, Encoding.UTF8);
            output.WriteLine(context);
            output.Close();
        }
    }
}
