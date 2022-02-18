using MyHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinFormsDemo
{
    public partial class Socket调用 : Form
    {
        public Socket调用()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string str返回数据 = WebToolkit.Get("https://www.baidu.com", Encoding.Default);

        }
    }
}
