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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
namespace WpfApp2
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        //建立mysql連線
        MySqlConnection con = new MySqlConnection("server = localhost; userid = root; password = ;database =calculator");
        //定義命令物件
        MySqlCommand cmd;
        string sql;

        double value = 0.0, ans = 0.0;
        string operate = "";
        string inputString = "";




        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (resultTextbox.Text == "0")
            {
                resultTextbox.Clear();
            }
            Button btn = sender as Button;
            resultTextbox.Text = resultTextbox.Text + btn.Content.ToString();
            inputString += btn.Content.ToString();
        }

        private void Operater_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            operate = btn.Content.ToString();
            //value = Double.Parse(resultTextbox.Text);
            resultTextbox.Text = resultTextbox.Text + btn.Content.ToString();
            inputString += btn.Content.ToString();
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            resultTextbox.Text = "0";
            inputString = "";
            operate = "";
            value = 0.0;
            ans = 0.0;
            posLabel.Content = "0";
            perLabel.Content = "0";
            decimalLable.Content = "0";
            binaryLabel.Content = "0";

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)  //deleteButton click event
        {
            int resultTextboxLength = resultTextbox.Text.Length - 1;
            string text = resultTextbox.Text;
            resultTextbox.Clear();
            inputString = "";
            for (int i = 0; i < resultTextboxLength; i++)
            {
                resultTextbox.Text = resultTextbox.Text + text[i];
                inputString = inputString + text[i];
            }
        }

        public string toPostfix(string inp)
        {
            char[] input = inp.ToCharArray();
            char[] stack = new char[input.Length];
            char[] postfix = new char[input.Length];
            char op;
            int top = 0;
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                op = input[i];
                switch (op)
                {
                    case '+':
                    case '-':
                    case '/':
                    case '*':
                        while (priority(stack[top]) >= priority(op))
                        {
                            sb.Append(stack[top]);
                            top--;
                        }
                        if (top < stack.Length)
                        {
                            top++;
                            stack[top] = op;
                        }
                        break;
                    default:
                        sb.Append(op);
                        break;
                }
            }
            while (top > 0)
            {
                sb.Append(stack[top]);
                top--;
            }
            return sb.ToString();
        }

        public string toPerfix(string inp)
        {
            char[] input = inp.ToCharArray();
            char[] stack = new char[input.Length];
            char op;
            int top = 0;
            StringBuilder sb = new StringBuilder();

            for (int i = input.Length - 1; i >= 0; i--)
            {
                op = input[i];
                switch (op)
                {
                    case '+':
                    case '-':
                    case '/':
                    case '*':
                        while (priority(stack[top]) >= priority(op))
                        {
                            sb.Append(stack[top]);
                            top--;
                        }
                        if (top < stack.Length)
                        {
                            top++;
                            stack[top] = op;
                        }
                        break;
                    default:
                        sb.Append(op);
                        break;
                }
            }
            while (top > 0)
            {
                sb.Append(stack[top]);
                top--;
            }
            string rev = "";
            char[] array = sb.ToString().ToCharArray();
            for (int i = array.Length - 1; i >= 0; i--)
            {
                rev += array[i];
            }
            return rev;
        }

        public double adder(string posinp)
        {
            double addAns = 0;
            double n1 = 0, n2 = 0, num = 0;
            Stack<double> st = new Stack<double>();
            char[] input = posinp.ToCharArray();
            char op;

            for (int i = 0; i < input.Length; i++)
            {
                op = input[i];
                switch (op)
                {
                    case '+':
                        n1 = st.Pop();
                        n2 = st.Pop();
                        num = n1 + n2;
                        st.Push(num);
                        break;
                    case '-':
                        n1 = st.Pop();
                        n2 = st.Pop();
                        num = n2 - n1;
                        st.Push(num);
                        break;
                    case '/':
                        n1 = st.Pop();
                        n2 = st.Pop();
                        num = n2 / n1;
                        st.Push(num);
                        break;
                    case '*':
                        n1 = st.Pop();
                        n2 = st.Pop();
                        num = n1 * n2;
                        st.Push(num);
                        break;
                    default:
                        double doubleOp = Convert.ToDouble(op.ToString());
                        st.Push(doubleOp);
                        break;
                }

            }
            addAns = st.Pop();
            return addAns;
        }



        private int priority(char op)
        {
            switch (op)
            {
                case '*':
                case '/':
                    return 2;
                case '+':
                case '-':
                    return 1;
                default:
                    return 0;
            }
        }

        private void insertButton_Click(object sender, RoutedEventArgs e)
        {
            con.Open();
            cmd = new MySqlCommand();
            cmd.Connection = con;

            sql = $"SELECT * FROM calculatortable WHERE infix = '{inputString}'";

            cmd.CommandText = sql;

            object existsCount = cmd.ExecuteScalar();

            if (existsCount == null)
            {
                sql = "Insert into `calculatortable`(`infix`,`postfix`,`perfix`,`bin`,`dec`) values " +
                    "('" + resultTextbox.Text + "' " +
                    ",'" + posLabel.Content + "' " +
                    ",'" + perLabel.Content + "' " +
                    ",'" + binaryLabel.Content + "' " +
                    ",'" + decimalLable.Content + "')";

                cmd.CommandText = sql;
                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("insert成功");
                }
                else
                {
                    MessageBox.Show("insert失敗");
                }
            }
            else
                MessageBox.Show("資料庫已有該筆資料");

            con.Close();
            //myMethod(sql);

        }

        //private void myMethod(string sql)
        //{
        //    //開啟資料庫連線
        //    con.Open();
        //    //建立命令物件
        //    cmd = new MySqlCommand();
        //    //連線MySQL伺服器資料庫
        //    cmd.Connection = con;
        //    //取得或設定在資料源執行的SQL語句
        //    cmd.CommandText = sql;
        //    try
        //    {
        //        int result = cmd.ExecuteNonQuery();
        //        if (result > 0)
        //        {
        //            MessageBox.Show("insert成功");
        //        }
        //        else
        //        {
        //            MessageBox.Show("insert失敗");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(e.ToString());
        //    }


        //    con.Close();

        //}



        private void checkDbButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window1();
            window.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MySqlCommand cmd = new MySqlCommand("ALTER TABLE calculatortable AUTO_INCREMENT=1", con);
            cmd.Connection.Open();

            cmd.ExecuteNonQuery();

            cmd.Connection.Close();

        }

        private void calculateButton_Click(object sender, RoutedEventArgs e)
        {
            string binaryAns = Convert.ToString((int)adder(toPostfix(inputString)), 2);
            double decimalAns = adder(toPostfix(inputString));
            //MessageBox.Show("計算中序式:"+inputString);
            posLabel.Content = toPostfix(inputString);
            perLabel.Content = toPerfix(inputString);
            decimalLable.Content = decimalAns;
            binaryLabel.Content = binaryAns;
            insertButton.IsEnabled = true;
        }


    }
}
