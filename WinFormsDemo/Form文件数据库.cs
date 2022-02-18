using MyHelper;
using MyHelper.文件数据库;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace WinFormsDemo
{
    public partial class Form文件数据库 : Form
    {
        /// <summary>
        /// 输出日志到控件
        /// </summary>
        /// <param name="strText"></param>
        void WriteText(string strText)
        {
            this.Invoke(
                new MethodInvoker(delegate
                {
                    text执行过程.AppendText(string.Format("【{0}】{1}{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"), strText, System.Environment.NewLine));
                    text执行过程.Select(text执行过程.TextLength, 0); //将光标移动到文档结尾处
                    text执行过程.ScrollToCaret(); //将文本框滚动到光标的位置
                    text执行过程.Refresh();
                }
            ));
            System.Threading.Thread.Sleep(20);
        }

        /// <summary>
        /// 清空控件文本框
        /// </summary>
        void clean清理文本框()
        {
            this.Invoke(
               new MethodInvoker(delegate
               {
                   text执行过程.Text = "";
                   text执行过程.Refresh();
               }
           ));
        }


        public Form文件数据库()
        {
            WriteTextDel.degWrite = WriteText;  //设置全局日志信息
            InitializeComponent();
        }
        MyHelper.文件数据库.IDatabase IDatabaseMdb = null;
        private void button2_Click(object sender, EventArgs e)
        {
            clean清理文本框();

            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string strMdbFile = openFile.FileName;
                try
                {
                    IDatabaseMdb = new DBAccess.AccessDatabase(strMdbFile, "truelore", true);
                    WriteText("打开成功：" + System.Environment.NewLine + strMdbFile);
                    List<string> list表 =  IDatabaseMdb.GetTableName();
                    foreach (var strTable in list表)
                    {
                        Dictionary<string, 列对象> dic列 = IDatabaseMdb.GetTableColumns(strTable);
                        if (dic列.ContainsKey("ID"))
                        {
                            IDatabaseMdb.GetNextID(strTable);
                        }
                        //string str列 = "";
                        //foreach (var kv in dic列)
                        //{
                        //    str列 += kv.Key + "\t" + kv.Value.type.ToString() + "\t" + (kv.Value.Is不为空 ? "不为空" : "") + System.Environment.NewLine;
                        //}

                        //WriteText("【" + strTable + "】获取所有列：" + System.Environment.NewLine + str列 + System.Environment.NewLine);
                    }
                    WriteText("获取所有表完成!" + System.Environment.NewLine);
                }
                catch(Exception ex)
                {
                    WriteText("打开失败：" + Utility.GetMessage(ex));
                }

            }
        }

        MyHelper.文件数据库.IDatabase IDatabaseSqlite = null;
        private void button3_Click(object sender, EventArgs e)
        {

            clean清理文本框();
            if (IDatabaseMdb == null)
            {
                WriteText("请先打开mdb数据库！");
                return;
            }

            SaveFileDialog openFile = new SaveFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string strFile = openFile.FileName;
                if (System.IO.File.Exists(strFile))
                {
                    WriteText("文件存在，删除文件...");
                    if (FileHelper.DelFile(strFile))
                    {
                        WriteText("删除成功！");
                    }
                    else
                    {
                        WriteText("无法失败！");
                        return;
                    }
                }
                FileStream fileStream = File.Create(strFile);
                fileStream.Close();

                if (!System.IO.File.Exists(strFile))
                {
                    WriteText("创建失败！");
                    return;
                }

                IDatabaseSqlite = new DBSqlite.SqliteDatabase(strFile, "", false);


                WriteText("开始建表！");
                List<string> list表 = IDatabaseMdb.GetTableName();
                foreach (var strTableName in list表)
                {
                    string strSql = DBSqlite.Sqlite建表语句.Get创建表语句(IDatabaseMdb, strTableName);

                    WriteText(strSql);
                    IDatabaseSqlite.ExecuteSQL(strSql);

                    WriteText(strSql);
                    DBSqlite.Sqlite插入数据.表数据导入(IDatabaseSqlite, strTableName, IDatabaseMdb);
                }

                IDatabaseSqlite.CloseConnection();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            
        }

    }
}
