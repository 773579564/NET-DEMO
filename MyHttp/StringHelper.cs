using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MyHelper
{
    public class StringHelper
    {
        public static string GetMessage(Exception ex)
        {
            string strMeg = System.Environment.NewLine + "*----------------------------------------------------*" + System.Environment.NewLine;
            strMeg += ex.Message + System.Environment.NewLine;
            strMeg += ex.StackTrace + System.Environment.NewLine + System.Environment.NewLine;
            if (ex.InnerException != null)
            {
                strMeg += GetMessage(ex.InnerException);
            }
            strMeg += System.Environment.NewLine + "*----------------------------------------------------*" + System.Environment.NewLine;
            return strMeg;
        }

        public static string Get删除注释(string input)
        {
            return Regex.Replace(input, @"//.+", "");
        }

        public static string Get第一个单词(string str语句)
        {
            return Regex.Match(str语句, @"\b\w+\b").Value;
        }


        public static bool Is截取字符串(string str字符串, string str查询字符, out string str截取前半段数据, out string str截取后半段数据)
        {
            int index = str字符串.IndexOf(str查询字符, StringComparison.OrdinalIgnoreCase);
            if (index > -1)
            {
                int int开始截取 = index + str查询字符.Length;
                int int截取长度 = str字符串.Length - int开始截取;
                str截取前半段数据 = str字符串.Substring(0, index);
                str截取后半段数据 = str字符串.Substring(int开始截取, int截取长度);
                return true;
            }
            str截取前半段数据 = str字符串;
            str截取后半段数据 = "";
            return false;
        }

    }
}
