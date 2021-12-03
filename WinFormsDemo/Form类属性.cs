using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace WinFormsDemo
{
    public partial class Form类属性 : Form
    {
        public Form类属性()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Type type = typeof(Node);

            string strClassName = type.FullName;
            CustomAttributeData MyAttrDBName = type.CustomAttributes.FirstOrDefault(t => t.AttributeType == typeof(MyAttrDBName));
            if (MyAttrDBName != null)
            {
                var property = MyAttrDBName.ConstructorArguments[0];
                string strName = property.Value.ToString();
            }
            PropertyInfo[] propertys = type.GetProperties();
            foreach (PropertyInfo pi in propertys)
            {
                MyAttrDBName = pi.CustomAttributes.FirstOrDefault(t => t.AttributeType == typeof(MyAttrDBName));
                if (MyAttrDBName != null)
                {
                    var property = MyAttrDBName.ConstructorArguments[0];
                    string strName = property.Value.ToString();
                }
            }

        }
    }

    [MyAttrDBName("tableName")]
    public class Node
    {
        [System.ComponentModel.DataAnnotations.Schema.Column("DISTRICT_ID")]
        public string xxxxxx
        {
            get;
            set;
        }
        [MyAttrDBName("col1")]
        public string SSSSS
        {
            get;
            set;
        }

        [MyAttrDBName("col2")]
        public int DDDD
        {
            get;
            set;
        }

        [MyAttrDBName("col3")]
        public int? FFFF
        {
            get;
            set;
        }
    }

    public class MyAttrDBName : System.Attribute
    {
        public String Name
        {
            get;
        }

        public MyAttrDBName(string _name)
        {
            Name = _name;
        }
    }
}
