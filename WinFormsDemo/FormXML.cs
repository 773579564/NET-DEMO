using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using MyHelper;

namespace WinFormsDemo
{
    public partial class FormXML : Form
    {
        public FormXML()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {         
 

            SaveFileDialog openFile = new SaveFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string strSaveFile = openFile.FileName;
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = new UTF8Encoding(false);
                settings.NewLineChars = Environment.NewLine;

                XmlWriter xmlWriter = XmlWriter.Create(strSaveFile, settings);
       

                try
                {
                    xmlWriter.WriteStartDocument();

                    StringBuilder strb = new StringBuilder();
              
                    strb.Append((char)0xE000);
                    strb.Append((char)0xD800);
                    strb.Append((char)0xD800);
                    strb.Append((char)0xE000);
                    string strb2 = XmlHelper.SanitizeXmlString(strb.ToString());
                    string str = "<xxxx&x/t/r/n\r\n\t0x20xx0xFFFD>";
                    string str2 = XmlHelper.SanitizeXmlString(str);

                    //创建根元素 
                    xmlWriter.WriteStartElement("ConstructProject");
                    xmlWriter.WriteAttributeString("strb", strb.ToString());
                    xmlWriter.WriteAttributeString("strb2", strb2.ToString());
                    xmlWriter.WriteAttributeString("xxxxx", str);
                    xmlWriter.WriteAttributeString("xxxxx2", str2);


                    xmlWriter.WriteEndElement();



                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    xmlWriter.Close();
                }
            }
        }

       

        

    }
}
