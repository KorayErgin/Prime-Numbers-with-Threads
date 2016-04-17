using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Yazlab_2
{
    public partial class Form1 : Form
    {
        SqlConnection sql = new SqlConnection("Data Source=pc\\ms_sql_2014;Initial Catalog=Yazlab;Integrated Security=True");
        SqlCommand cmd;
        int sayac=0;
        float ara;
        volatile bool isStop = false;
        public int current = 15, thread_number, top_limit, under_limit;
        public List<int> Datas = new List<int>();
        public List<int> Datas2 = new List<int>();
        public List<int> Havuz = new List<int>();
        
        // ListViewItem listvieww = new ListViewItem();
        Thread[] thread_array;
        // under_limit= 2; // temporary line
        //bool stop = false;

        public Form1()
        {
            System.Windows.Forms.Form.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
          //  Thread th = Thread.CurrentThread;

        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        void sql_oku()
        {
            Datas.Clear();
            cmd = new SqlCommand("select * from asal", sql);
            sql.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Datas.Add(Convert.ToInt32(reader["Sayi"]));
            }
            sql.Close();
        }

        void isPrime()
        {
            sql_oku();

            current = Datas[Datas.Count - 1];
           // current = 822650;
         //   label3.Text = current.ToString();
            while(!isStop)
            {
                listView1.Items.Clear();
                sayac = 0;
                current++;
                label2.Text = "Araştırılan Sayı : " + current;
               // current =15;
                sql_oku();
                top_limit = (int)Math.Sqrt(current);
               /* if (Datas.Count < 100) thread_number = 2;
                else thread_number = Datas.Count / 100 + 1;*/
                
                
                if (current %2 == 1) {

                    ///////////////////////////////////
                    Havuz.Clear();
                    cmd = new SqlCommand("select * from asal where Sayi<=" +top_limit + "", sql);
                    sql.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Havuz.Add(Convert.ToInt32(reader["Sayi"]));
                    }
                    sql.Close();
                    if (Havuz.Count < 100) thread_number = 2;
                    else thread_number = Havuz.Count / 100 + 1;
                   // label4.Text = Havuz.Count.ToString();
                    thread_array = new Thread[thread_number];
                    float temp = (float)Havuz.Count()/thread_number;
                 //  label2.Text = temp.ToString(); 
                    for (int i = 0; i < thread_number-1; i++)
                {
                       
                thread_array[i] = new Thread(() => ThreadJob1(i*temp,(i+1)*temp));
                thread_array[i].Name = "Thread " + i;
                thread_array[i].Start();
                thread_array[i].Join();
                ara = temp * (i + 1);

                }

                thread_array[thread_number - 1] = new Thread(() => ThreadJob2(top_limit,ara));
                thread_array[thread_number - 1].Name = "Thread " + (thread_number - 1);
                thread_array[thread_number - 1].Start();
                thread_array[thread_number - 1].Join();

                    if (sayac == 0)
                    {
                        Thread sql_add = new Thread(() => sql_ekle(current));
                        sql_add.Start();
                        sql_add.Join();
                    }
                }
                Thread.Sleep(1);
             
            }

             
            

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //sql_oku();
            //  isPrime();
            isStop = false;
            
            Thread th = new Thread(() => isPrime());
            th.Name = "MainThread";
            //label1.Text = th.Name;
            th.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            isStop = true;
        }

        void ThreadJob1(float start, float end)
        {
            int bas =(int) start;
            int son = (int)end;
            int s = 0;
            bool b = false;
           // Datas2.Clear();
            for(int i=bas;i<=son;i++)
            {
                if (current % Havuz[i] == 0)
                {
                    b = true;
                    s = Havuz[i];
                    sayac++;
                  
                }
            }


            string[] arr = new string[5];
            ListViewItem item;
            arr[0] = Thread.CurrentThread.Name;
            arr[1] = Havuz[bas].ToString();                    // arayüz 
            arr[2] = Havuz[son].ToString();
            arr[3] = b.ToString();
            arr[4] = current.ToString();

           // label1.Text = Havuz.Count.ToString();
            item = new ListViewItem(arr);
            listView1.Items.Add(item);
            if (b == true) label1.Text = "Sayı asal değildir" + Thread.CurrentThread.Name + " da bulunan\n" + s + " sayısına tam bölünmektedir.";
            if (b == false) label1.Text = "Sayı asaldır";

            if (b==true) Thread.CurrentThread.Abort();


            Thread.Sleep(1);

        }
        void ThreadJob2(int end, float stop)
        {
            int dur = (int)stop;
            int a=0;
           
       
       
            bool b = false;
            // Datas2.Clear();
            for (int i = Havuz.Count-1; i >= dur; --i)
            {
                if (current % Havuz[i] == 0)
                {
                    b = true;
                    a = Havuz[i];
                    sayac++;
                    
                }
            }



            string[] arr = new string[5];
            ListViewItem item;
       
            arr[0] = Thread.CurrentThread.Name;
            arr[1] = end.ToString();                    // arayüz 
            arr[2] = Havuz[dur].ToString();
            arr[3] = b.ToString();
            arr[4] = current.ToString();
            item = new ListViewItem(arr);
            
            listView1.Items.Add(item);
            if (b == true) label1.Text = "Sayı asal değildir" + Thread.CurrentThread.Name + " da bulunan\n " + a + " sayısına tam bölünmektedir.";
            if (b == false) label1.Text = "Sayı asaldır";

            if (b==true)Thread.CurrentThread.Abort();
            //label1.Text = "" + k;
            //listBox1.Items.Add(k);
            //listBox1.Items.Add(Thread.CurrentThread.Name);
            Thread.Sleep(1);

        }
        void sql_ekle(int veri)
        {
              // listView2.Items.Add(veri.ToString());
              sql.Open();
              cmd = new SqlCommand("insert into asal (Sayi)values(" + veri + ")", sql);
              cmd.ExecuteNonQuery();
              sql.Close();
              Thread.Sleep(1);
          //  listView1.Items.Clear();

        }

    }

}
