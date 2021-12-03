using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinFormsDemo
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormFile fm = new FormFile();
            fm.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormXML fm = new FormXML();
            fm.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form类属性 fm = new Form类属性();
            fm.ShowDialog();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            FormClass处理 fm = new FormClass处理();
            fm.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form文件数据库 fm = new Form文件数据库();
            fm.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form文件遍历 fm = new Form文件遍历();
            fm.ShowDialog();
        }
    }

}
