using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
namespace WpfApp2
{
    /// <summary>
    /// Window1.xaml 的互動邏輯
    /// </summary>
    public partial class Window1 : Window
    {
        String stdDetails = "{0,-15}{1,-25}{2,-25}{3,-25}{4,-25}{5,-20}";
        String stdDetails2 = "{0,-14}{1,-21}{2,-24}{3,-23}{4,-28}{5,-20}";
        //建立mysql連線
        MySqlConnection con = new MySqlConnection("server = localhost; userid = root; password = ;database =calculator");
        //定義命令物件
        MySqlCommand cmd;
        MySqlDataReader dr;
        public Window1()
        {
            InitializeComponent();
        }

        private void showButton_Click(object sender, RoutedEventArgs e)
        {
            dbListBox.Items.Clear();
            cmd = new MySqlCommand();
            con.Open();
            cmd.Connection = con;
            cmd.CommandText = "select * from calculatortable";
            dr = cmd.ExecuteReader();
            String id, infix, postfix, perfix, binary, decim;
            dbListBox.Items.Add(String.Format(stdDetails, "id", "infix", "postfix", "perfix", "binary", "decimal"));
            while (dr.Read())
            {
                id = dr["id"].ToString();
                //MessageBox.Show(id);
                infix = dr["infix"].ToString();
                postfix = dr["postfix"].ToString();
                perfix = dr["perfix"].ToString();
                binary = dr["bin"].ToString();
                decim = dr["dec"].ToString();
                dbListBox.Items.Add(String.Format(stdDetails2, id, infix, postfix, perfix, binary, decim));
            }
            con.Close();
        }

        private void dbListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void deleteDataButton_Click(object sender, RoutedEventArgs e)
        {
            if (dbListBox.SelectedIndex == -1)
            {
                MessageBox.Show("請選擇資料");
                return;
            }

            if (dbListBox.Items.Count > 1)
            {
                string targetString = dbListBox.SelectedItem.ToString();
                string targetId = targetString.Split(new char[] { ' ' }, 2)[0];//遇到空格 分割最大數量2
                //  [100,          200]
                string sql;
                con.Open();
                cmd = new MySqlCommand();
                cmd.Connection = con;

                sql = $"DELETE FROM calculatortable WHERE id = '{targetId}'";

                cmd.CommandText = sql;

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    dbListBox.Items.Remove(dbListBox.SelectedItem);
                    MessageBox.Show("刪除第"+ targetId+"筆資料成功");
                }
                else
                {
                    MessageBox.Show("刪除失敗");
                }

                //for (int i = 0; i <= dbListBox.Items.Count - 1; i++)
                //{
                //    if (dbListBox.)
                //    {
                //        //sql = $"DELETE * FROM calculatortable WHERE id = '[i]'"
                //    }
                //}


                con.Close();


            }
            else
            {
                MessageBox.Show("db is null");
            }
            

        }
    }
}
