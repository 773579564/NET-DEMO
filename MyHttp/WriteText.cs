using System;
using System.Collections.Generic;
using System.Text;

namespace MyHelper
{
    public class WriteTextDel
    {
        //下载过程执行委托
        public delegate void degWriteText(string strText);
        public static degWriteText degWrite = null;
        public static void Write(string strText)
        {
            if (degWrite != null)
            {
                degWrite(strText);
            }
        }
    }
}
