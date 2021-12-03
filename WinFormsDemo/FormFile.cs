using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyHelper;

namespace WinFormsDemo
{
    public partial class FormFile : Form
    {
        public FormFile()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime dt = Convert.ToDateTime("2021-06-01");
            txtShow.Text = "";

            int index = 0;
            string strText = "";
            foreach (var kv in dic文件列表)
            {
                if (kv.Value.LastWriteTime > dt)
                {
                    strText += kv.Key;
                    strText += System.Environment.NewLine;
                    index++;
                }
            }

            txtShow.Text = strText;

        }
        Dictionary<string, FileInfo> dic文件列表 = new Dictionary<string, FileInfo>();
        void Show()
        {
            txtShow.Text = "";

            string strText = "";
            foreach (var kv in dic文件列表)
            {
                strText += kv.Key;
                strText += System.Environment.NewLine;
            }

            txtShow.Text = strText;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<String> lst排除目录 = new List<string> {
                "obj"
            };

            List<String> lst排除文件 = new List<string> {
                ".dll", ".pdb", ".cache", ".resources",".bak"
            };
            //dic文件列表 = FileHelper.GetAllFiles(textBox2.Text, lst排除目录, lst排除文件);
            //label1.Text = "总数据：" + dic文件列表.Count;
            Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Show();
        }
    }
}
