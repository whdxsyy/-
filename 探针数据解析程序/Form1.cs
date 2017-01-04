using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace 探针数据解析程序
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class wifiJsonData
        {
            public string id { get; set; }
            public string time { get; set; }
            public string lat { get; set; }
            public string lon { get; set; }
            public List<wifiData> data { get; set; }
            public wifiJsonData strtojson(string jsonstr)
            {
                return JsonConvert.DeserializeObject<wifiJsonData>(jsonstr);
            }
        }
        public class wifiData
        {
            public string mac { get; set; }
            public string rssi { get; set; }
            public string ts { get; set; }
            public string tmc { get; set; }
            public string tc { get; set; }
            public string ds { get; set; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //DB db = new DB();
            //this.dataGridView1.DataSource = db.getPsize().Tables["psize"];
            //db.closeConn();

            DirectoryInfo TheFolder = new DirectoryInfo("D:\\wifijson");
            ////遍历文件夹
            //foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
            //    this.listBox1.Items.Add(NextFolder.Name);

            //遍历文件
            string filename = "";
            int mm = 0;
            string fileContent = "";
            foreach (FileInfo NextFile in TheFolder.GetFiles())
            {
                filename = NextFile.FullName;
                // fileContent=ReadFile(filename);
                StreamReader sr = new StreamReader(filename);
                string s = sr.ReadLine();
                DB db = new DB();
                try
                {
                    while (s != null)
                    {
                        wifiJsonData json = new wifiJsonData();
                        json = json.strtojson(s.Trim());
                        foreach (wifiData wfd in json.data)
                        {
                            bool res = db.insertData(json.id, wfd.mac, wfd.rssi, json.time, json.lat, json.lon, wfd.ts, wfd.tmc, wfd.tc, wfd.ds);
                        }
                        s = sr.ReadLine();//读取每行
                    }
                }
                catch (Exception e1)
                { }
                finally
                {
                    db.closeConn();
                }

            }
        }

        //读取文件
        private static string ReadFile(string filepath)
        {
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                StreamReader sr = new StreamReader(fs, Encoding.Default);
                string content = sr.ReadToEnd();
                sr.Close();
                return content;
            }
        }

        //写文件
        private static void WriteFile()
        {
            using (FileStream fs = new FileStream("c:\\CreateText.txt", FileMode.Create, FileAccess.Write))
            {
                StreamWriter sr = new StreamWriter(fs);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < 10000; i++)
                    sb.Append(i.ToString("0000") + ",");
                sr.Write(sb.ToString(0, sb.Length - 1));
                sr.Close();
            }
        }

        class DB
        {
            MySqlConnection conn = null;
            //构造函数，设置数据库连接 设置数据库编码 
            protected static String dbServer = "localhost";
            protected static String dbUser = "root";
            protected static String dbPwd = "4027853";
            protected static String dbName = "wifijson";
            public DB()
            {
                conn = new MySqlConnection("Server=" + dbServer + ";User Id=" + dbUser + ";Password=" + dbPwd + ";Persist Security Info=True;Database=" + dbName);//构造连接字符串，连接数据库  
                conn.Open();//打开连接  

            }
            public DataSet getPsize()
            {
                string sql = "select * from wifijson";
                MySqlDataAdapter myadp = new MySqlDataAdapter(sql, this.conn);
                //声明数据适配器，执行数据查询  
                DataSet ds = new DataSet();//声明数据集  
                myadp.Fill(ds, "psize");//把查到的结果填充到数据集中  
                conn.Close();//关闭连接  
                return ds;//返回数据集，用于绑定控件作为数据源  
            }
            public bool insertData(string tanzhenid, string macid, string rssi, string time, string lat, string lon, string ts, string tmc, string tc, string ds)
            {
                string sql = @"insert into wifijson (tanzhenid,macid,rssi,time,lat,lon,ts,tmc,tc,ds) values (?tanzhenid,?macid,?rssi,?time,?lat,?lon,?ts,?tmc,?tc,?ds)";

                try
                {
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlParameter[] pms = new MySqlParameter[] {
                    new MySqlParameter("?tanzhenid", tanzhenid),
                    new MySqlParameter("?macid", macid),
                     new MySqlParameter("?rssi", int.Parse(rssi.Trim())),
                      new MySqlParameter("?time", time),
                       new MySqlParameter("?lat", double.Parse(lat.Trim())),
                        new MySqlParameter("?lon", double.Parse(lon.Trim())),
                         new MySqlParameter("?ts", ts),
                          new MySqlParameter("?tmc", tmc),
                           new MySqlParameter("?tc", tc),
                            new MySqlParameter("?ds", ds)
                    };
                    cmd.Parameters.AddRange(pms);
                    int i = cmd.ExecuteNonQuery();
                    return (i>=1);
                }
                catch (Exception e)
                {
                    string mm = e.ToString();
                    return false;
                }
                finally
                {

                }
            }

            public void closeConn()
            {
                if (this.conn != null)
                {
                    conn.Close();
                    int mm = 00;
                }
            }

        }
    }
}
