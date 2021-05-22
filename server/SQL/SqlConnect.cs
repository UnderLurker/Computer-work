using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using server.data;

namespace server.SQL
{
    /// <summary>
    /// 连接数据库
    /// </summary>
    public class SqlConnect
    {
        static string MyConString = "data source=localhost;port=3306;database=bigwork;userid=root;password=123;pooling=true;charset=utf8;sslmode=none";
        static string read = "select * from client";

        /// <summary>
        /// 读取数据库中的所有人到List中
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public int SelectMySql(List<Customer> person)
        {
            int count = 0;
            using (MySqlConnection msc=new MySqlConnection(MyConString))
            {
                MySqlCommand cmd = new MySqlCommand(read, msc);
                msc.Open();
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32("id");
                    string n = reader.GetString("name");
                    string pwd = reader.GetString("password");
                    string cp = "";
                    try
                    {
                        cp = reader.GetString("contactpersons");
                    }
                    catch(Exception ex)
                    {

                    }
                    string[] list = cp.Split(';');
                    Customer temp = new Customer();
                    temp.Name = n;
                    temp.Passwd = pwd;
                    temp.ID = id;
                    temp.OnLine = false;
                    foreach(string i in list)
                    {
                        if (i == "") continue;
                        temp.ContanctPerosnList.Add(i);
                    }
                    person.Add(temp);
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 插入到数据库
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="id"></param>
        public void insertSql(Customer customer,int id)
        {
            using (MySqlConnection msc = new MySqlConnection(MyConString))
            {
                MySqlCommand cmd = new MySqlCommand($"insert into client(id,name,password) value({id},'{customer.Name}','{customer.Passwd}')", msc);
                msc.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 为username添加联系人cpname
        /// </summary>
        /// <param name="username"></param>
        /// <param name="cpname"></param>
        public void updatePerson(string username,string cpname)
        {
            using (MySqlConnection msc = new MySqlConnection(MyConString))
            {
                MySqlCommand cmd = new MySqlCommand($"update client set contactpersons='{cpname}' where name='{username}'",msc);
                msc.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 读取联系人
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string readPerson(string username)
        {
            string result = "";
            using (MySqlConnection msc = new MySqlConnection(MyConString))
            {
                MySqlCommand cmd = new MySqlCommand($"select * from client where name='{username}'", msc);
                msc.Open();
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                try
                {
                    result = reader.GetString("contactpersons");
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// 将username的联系人列表更新为personlist
        /// </summary>
        /// <param name="username"></param>
        /// <param name="personlist"></param>
        public void deletePerson(string username,string personlist)
        {
            using (MySqlConnection msc=new MySqlConnection(MyConString))
            {
                MySqlCommand cmd1 = new MySqlCommand($"update client set contactpersons='{personlist}' where name='{username}'",msc);
                msc.Open();
                cmd1.ExecuteReader();
            }
        }
        public void ExecuteNonQuery(string sql, MySqlConnection msc)
        {
            MySqlCommand cmd = new MySqlCommand(sql, msc);
            cmd.ExecuteNonQuery();
        }
    }
}
