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
    public partial class Form文件遍历 : Form
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
                    text执行过程.AppendText(strText + System.Environment.NewLine);
                    text执行过程.Select(text执行过程.TextLength, 0); //将光标移动到文档结尾处
                    text执行过程.ScrollToCaret(); //将文本框滚动到光标的位置
                    text执行过程.Refresh();
                }
            ));
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


        public Form文件遍历()
        {
            WriteTextDel.degWrite = WriteText;  //设置全局日志信息
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DirectoryInfo dir = new DirectoryInfo(text目录.Text);
            if (!dir.Exists)
            {
                WriteText("目录不存在！");
                return;
            }
            MyHelper.文件数据库.IDatabase IDatabaseMdb = new DBAccess.AccessDatabase(@"F:\ZL-公司\ZL-代码\SVN\Net\清单模板编制工具\CommonDll杭钢\BidFile.mdb", "truelore", true);
            Dictionary<string, FileInfo> dic文件列表 = new Dictionary<string, FileInfo>();
            try
            {
                Get文件列表(dir, dic文件列表, "*.cs");
            }
            catch (Exception ex)
            {
                WriteText("遍历文件出现异常！" + Utility.GetMessage(ex));
                return;
            }


            string str间隔 = System.Environment.NewLine + "-------------------------------------" + System.Environment.NewLine;
            foreach (var kv in dic文件列表)
            {
                string strText = File.ReadAllText(kv.Key, Encoding.UTF8);

                string str去掉注释 = StringHelper.Get删除注释(strText);

                List<解析对象SQL语句> list对象 = 解析对象SQL语句.Get解析对象(str去掉注释);
                if (list对象.Count > 0)
                {
                    WriteText(str间隔 + kv.Key + System.Environment.NewLine + str间隔);
                    StringBuilder strBuilder = new StringBuilder();
                    foreach (解析对象SQL语句 obj对象 in list对象)
                    {
                        StringBuilder strBer1 = new StringBuilder();
                        Dictionary<string, 列对象> dic表数据列 = IDatabaseMdb.GetTableColumns(obj对象.Str表名称);
                        if (dic表数据列.Count == 0)
                        {
                            obj对象.StrError += "查询数据库表对象失败；" + System.Environment.NewLine;
                        }
                        else
                        {
                            strBuilder.Append(str间隔 + $"【{obj对象.Str表名称}】" + System.Environment.NewLine + System.Environment.NewLine);
                            strBuilder.Append("Dictionary<string, string> dic列数据 = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);" + System.Environment.NewLine);

                        }

                        foreach (var kv列对象 in obj对象.dic列对象)
                        {
                            列对象 obj列对象;
                            if (!dic表数据列.TryGetValue(kv列对象.Key, out obj列对象))
                            {
                                strBuilder.Append($"[\"{kv列对象.Key}\"] 数据库不存在！" + System.Environment.NewLine);
                            }
                            else
                            {
                                obj列对象.Is不为空 = false;
                                strBuilder.Append($"dic列数据[\"{kv列对象.Key}\"] = {Get初始赋值(kv列对象.Value)};" + System.Environment.NewLine);
                            }

                            //if (Is存在bool类型(kv列对象.Value, obj列对象))
                            //{
                            //    strBer1.Append($"[{kv列对象.Key}]----【{kv列对象.Value}】");
                            //    strBer1.Append(System.Environment.NewLine);
                            //}
                        }

                        foreach (var kv列 in dic表数据列)
                        {
                            if (kv列.Value.Is不为空)
                            {
                                strBer1.Append($"[{kv列.Key}]----不为空，没有赋值");
                            }
                        }

                        if (obj对象.StrError.Length > 0 || strBer1.Length > 0)
                        {
                            strBuilder.Append(str间隔);
                            strBuilder.Append($"【{obj对象.Str表名称}】" + System.Environment.NewLine + System.Environment.NewLine);
                        }

                        strBuilder.Append(obj对象.StrError);
                        strBuilder.Append(strBer1);
                    }

                    if (strBuilder.Length > 0)
                    {
                        WriteText(strBuilder.ToString());
                    }
                }
            }
        }

        string Get初始赋值(string strValue)
        {
            if (!strValue.Contains("Utility",StringComparison.OrdinalIgnoreCase))
            {
                return $"API.Utility.GetSQLStringForNullable({strValue})";
            }

            return strValue.Replace("Utility.GetDataBaseString", "Utility.GetSQLStringForNullable");
        }

        bool Is存在bool类型(string strText, 列对象 obj列对象)
        {
            if (strText.Contains("true", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (strText.Contains("false", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (obj列对象 != null)
            {
                if (MyGetTypeCode(obj列对象.type) == TypeCode.Boolean)
                {
                    string strNew = strText.Trim();
                    if (strNew.EndsWith("\"0\"", StringComparison.OrdinalIgnoreCase) || strNew.EndsWith("\"1\"", StringComparison.OrdinalIgnoreCase))
                    {
                        return false; 
                    }

                    if (strNew.Contains("GetSQLStringForNullable", StringComparison.OrdinalIgnoreCase) && strNew.EndsWith(")", StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取数据类型枚举
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private TypeCode MyGetTypeCode(Type type)
        {
            TypeCode code = Type.GetTypeCode(type);
            if (code == TypeCode.Object)
            {
                //对象类型 (int?) 获取类型的基础类型参数
                Type type1 = Nullable.GetUnderlyingType(type);
                code = Type.GetTypeCode(type1);
            }

            return code;
        }



        void Get文件列表(DirectoryInfo dir, Dictionary<string, FileInfo> dic文件列表, string str后缀)
        {
            FileInfo[] fileInfo = dir.GetFiles(str后缀, SearchOption.TopDirectoryOnly);
            foreach (FileInfo file in fileInfo)
            {
                dic文件列表.Add(file.FullName, file);
            }

            DirectoryInfo[] dirSubList = dir.GetDirectories();
            foreach (DirectoryInfo dirsub in dirSubList)
            {
                Get文件列表(dirsub, dic文件列表, str后缀);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DirectoryInfo dir = new DirectoryInfo(text目录.Text);
            if (!dir.Exists)
            {
                WriteText("目录不存在！");
                return;
            }
            MyHelper.文件数据库.IDatabase IDatabaseMdb = new DBAccess.AccessDatabase(@"F:\ZL-公司\ZL-代码\SVN\Net\清单模板编制工具\CommonDll杭钢\BidFile.mdb", "truelore", true);
            Dictionary<string, FileInfo> dic文件列表 = new Dictionary<string, FileInfo>();
            try
            {
                Get文件列表(dir, dic文件列表, "*.cs");
            }
            catch (Exception ex)
            {
                WriteText("遍历文件出现异常！" + Utility.GetMessage(ex));
                return;
            }


            string str间隔 = System.Environment.NewLine + "-------------------------------------" + System.Environment.NewLine;
            foreach (var kv in dic文件列表)
            {
                string strText = File.ReadAllText(kv.Key, Encoding.UTF8);

                string str去掉注释 = StringHelper.Get删除注释(strText);

                List<解析Updata对象SQL语句> list对象 = 解析Updata对象SQL语句.Get解析对象(str去掉注释, IDatabaseMdb);
                if (list对象.Count > 0)
                {
                    WriteText(str间隔 + kv.Key + System.Environment.NewLine);
                    StringBuilder strBuilder = new StringBuilder();
                    foreach (解析Updata对象SQL语句 obj对象 in list对象)
                    {
                        StringBuilder strBer1 = new StringBuilder();
                        Dictionary<string, 列对象> dic表数据列 = IDatabaseMdb.GetTableColumns(obj对象.Str表名称);
                        if (dic表数据列.Count == 0)
                        {
                            obj对象.StrError += "查询数据库表对象失败；" + System.Environment.NewLine;
                        }
                        else
                        {
                            strBuilder.Append(str间隔 + $"【{obj对象.Str表名称}】" + System.Environment.NewLine + System.Environment.NewLine);
                            strBuilder.Append("Dictionary<string, string> dic列数据 = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);" + System.Environment.NewLine);

                        }

                        foreach (var kv列对象 in obj对象.dic列对象)
                        {
                            列对象 obj列对象;
                            if (!dic表数据列.TryGetValue(kv列对象.Key, out obj列对象))
                            {
                                strBuilder.Append($"[\"{kv列对象.Key}\"] 数据库不存在！" + System.Environment.NewLine);
                            }
                            else
                            {
                                obj列对象.Is不为空 = false;
                                strBuilder.Append($"dic列数据[\"{kv列对象.Key}\"] = {Get初始赋值(kv列对象.Value)};" + System.Environment.NewLine);
                            }
                        }

                        foreach (var kv列 in dic表数据列)
                        {
                            if (kv列.Value.Is不为空)
                            {
                                strBer1.Append($"[{kv列.Key}]----不为空，没有赋值" + System.Environment.NewLine);
                            }
                        }

                        if (obj对象.StrError.Length > 0 || strBer1.Length > 0)
                        {
                            strBuilder.Append(str间隔);
                            strBuilder.Append($"【{obj对象.Str表名称}】解析存在问题：" + System.Environment.NewLine + System.Environment.NewLine);
                        }

                        strBuilder.Append(obj对象.StrError);
                        strBuilder.Append(strBer1);
                    }

                    if (strBuilder.Length > 0)
                    {
                        WriteText(strBuilder.ToString());
                    }
                }
            }

        }
    }

    public class 解析Updata对象SQL语句
    {
        public string Str表名称 { get; set; }
        public Dictionary<string, string> dic列对象 = new Dictionary<string, string>();
        public string StrError = string.Empty;

        public static List<解析Updata对象SQL语句> Get解析对象(string strSql, MyHelper.文件数据库.IDatabase IDatabaseMdb)
        {
            List<解析Updata对象SQL语句> list对象 = new List<解析Updata对象SQL语句>();

            string str截取前半段数据;
            string str截取后半段语句;

            string str查找字符 = "UPDATE";


            bool Is存在INSERT = StringHelper.Is截取字符串(strSql, str查找字符, out str截取前半段数据, out str截取后半段语句);
            while (Is存在INSERT)
            {
                string strNewSql = str截取后半段语句;
                if (str截取前半段数据.Trim().EndsWith("\"") && (str截取后半段语句.StartsWith(" ") || str截取后半段语句.StartsWith(System.Environment.NewLine)))
                {
                    解析Updata对象SQL语句 obj对象;
                    if (Sql解析INSERT数据("UPDATE" + str截取后半段语句, out obj对象, out str截取后半段语句, IDatabaseMdb))
                    {
                        list对象.Add(obj对象);
                    }
                    else
                    {
                        
                    }
                }

                Is存在INSERT = StringHelper.Is截取字符串(strNewSql, str查找字符, out str截取前半段数据, out str截取后半段语句);
            }

            return list对象;
        }

        public static bool Sql解析INSERT数据(string strSql, out 解析Updata对象SQL语句 objSql对象, out string str截取后半段语句, MyHelper.文件数据库.IDatabase IDatabaseMdb)
        {
            objSql对象 = new 解析Updata对象SQL语句();
            string str截取前半段数据;

            string str查找字符 = "UPDATE";
            if (!StringHelper.Is截取字符串(strSql, str查找字符, out str截取前半段数据, out str截取后半段语句))
            {
                //throw new Exception("获取 【INSERT】 语句失败！");
                return false;
            }

            str查找字符 = "SET";
            if (!StringHelper.Is截取字符串(str截取后半段语句, str查找字符, out str截取前半段数据, out str截取后半段语句))
            {
                //throw new Exception("获取 【UPDATE [] SET】 语句失败！");
                return false;
            }
            else
            {
                //-------------------------------------------------------
                string Str表名称语句 = str截取前半段数据.Trim().TrimStart('[').TrimEnd(']');
                objSql对象.Str表名称 = Str表名称语句;
                if (!IDatabaseMdb.TableExists(Str表名称语句))
                {
                    WriteTextDel.Write($"【{Str表名称语句}】 不存在");
                    return false;
                }
                //-------------------------------------------------------
            }

            str查找字符 = "\"";
            if (!StringHelper.Is截取字符串(str截取后半段语句, str查找字符, out str截取前半段数据, out str截取后半段语句))
            {
                // throw new Exception("获取 【INSERT INTO 表名称 (】  语句失败！");
                return false;
            }

            string strWHERE条件;
            str查找字符 = "WHERE";
            if (!StringHelper.Is截取字符串(str截取前半段数据, str查找字符, out str截取前半段数据, out strWHERE条件))
            {
            }
            //-------------------------------------------------------
            string str修改对象 = str截取前半段数据;

            //-------------------------------------------------------

            str查找字符 = ";";
            if (!StringHelper.Is截取字符串(str截取后半段语句, str查找字符, out str截取前半段数据, out str截取后半段语句))
            {
                //throw new Exception("获取 【string.Format(\"INSERT INTO 表名称 ( 字段1,字段2 ) values ( )\", 数据对象); 】  语句失败！");
                return false;
            }
            else
            {
                if (str截取前半段数据.Trim().Length == 0)
                {
                    str查找字符 = "string.Format";
                    if (!StringHelper.Is截取字符串(str截取后半段语句, str查找字符, out str截取前半段数据, out str截取后半段语句))
                    {
                        throw new Exception("获取 【\"INSERT INTO 表名称 ( 字段1,字段2 ) values ( )\", 数据对象】  语句失败！");
                        //return false;
                    }

                    str查找字符 = ",";
                    if (!StringHelper.Is截取字符串(str截取后半段语句, str查找字符, out str截取前半段数据, out str截取后半段语句))
                    {
                        throw new Exception("获取 【\"INSERT INTO 表名称 ( 字段1,字段2 ) values ( )\", 数据对象】  语句失败！");
                        //return false;
                    }

                    str查找字符 = ";";
                    if (!StringHelper.Is截取字符串(str截取后半段语句, str查找字符, out str截取前半段数据, out str截取后半段语句))
                    {
                        throw new Exception("获取 【string.Format(\"INSERT INTO 表名称 ( 字段1,字段2 ) values ( )\", 数据对象); 】  语句失败！");
                        //return false;
                    }
                }
            }
            //-------------------------------------------------------
            string str数据对象语句 = str截取前半段数据.Trim().TrimStart(',').TrimEnd(')');
            if (str截取前半段数据.EndsWith("))"))
            {
                str数据对象语句 += ")";
            }
            //-------------------------------------------------------
            Dictionary<int, string> dic变量赋值;
            string strError = Get使用表字段(str修改对象, objSql对象.dic列对象, out dic变量赋值);
            补充变量赋值(dic变量赋值, strWHERE条件);
            if (!string.IsNullOrEmpty(strError))
            {
                objSql对象.StrError += "获取表字段存在问题：" + strError + System.Environment.NewLine;
            }
            strError = Get使用数据对象(str数据对象语句, objSql对象.dic列对象, dic变量赋值);
            if (!string.IsNullOrEmpty(strError))
            {
                objSql对象.StrError += "获取数据对象存在问题：" + strError + System.Environment.NewLine;
            }


            return true;
        }

        static void 补充变量赋值(Dictionary<int, string> dic变量赋值, string strWhere条件)
        {
            if (string.IsNullOrEmpty(strWhere条件))
            {
                return;
            }
            string str截取前半段数据, str截取后半段语句;
            string strCX1 = "{";
            string strCX2 = "}";
            bool Is变量存在 = StringHelper.Is截取字符串(strWhere条件, strCX1, out str截取前半段数据, out str截取后半段语句);

            while (Is变量存在)
            {
                if (!StringHelper.Is截取字符串(str截取后半段语句, strCX2, out str截取前半段数据, out str截取后半段语句))
                {
                    return;
                }
                int index = Convert.ToInt32(str截取前半段数据);
                if (!dic变量赋值.ContainsKey(index))
                {
                    dic变量赋值.Add(index, "@@@@@@@@");
                }


                Is变量存在 = StringHelper.Is截取字符串(str截取后半段语句, strCX1, out str截取前半段数据, out str截取后半段语句);
            }
        }

        static int GetIndex值(string Values值)
        {
            string str截取前半段数据, str截取后半段语句;
            string strCX1 = "{";
            string strCX2 = "}";
            if(StringHelper.Is截取字符串(Values值, strCX1, out str截取前半段数据, out str截取后半段语句))
            {
                if (StringHelper.Is截取字符串(str截取后半段语句, strCX2, out str截取前半段数据, out str截取后半段语句))
                {
                    return Convert.ToInt32(str截取前半段数据);
                }
            }

            return -1;
        }

        static string Get使用表字段(string str表字段语句, Dictionary<string, string> dic表字段, out Dictionary<int, string> dic变量赋值)
        {
            dic表字段.Clear();
            string strError = string.Empty;

            dic变量赋值 = new Dictionary<int, string>();
            string[] strArr = str表字段语句.Split(',');
            foreach (string str修改列 in strArr)
            {
                string[] str列赋值 = str修改列.Split('=');
                if (str列赋值.Length > 1)
                {
                    string srt字段名称 = StringHelper.Get第一个单词(str列赋值[0]);
                    string Values值 = str列赋值[1].Trim();

                    dic表字段.Add(srt字段名称, Values值);
                    int index = GetIndex值(Values值);

                    if (index != -1)
                    {
                        if (dic变量赋值.ContainsKey(index))
                        {
                            dic变量赋值[index] = dic变量赋值[index] + "@@@" + srt字段名称;
                        }
                        else
                        {
                            dic变量赋值.Add(index, srt字段名称);
                        }
                    }
                    else
                    {
                        dic表字段[srt字段名称] = $"API.Utility.GetSQLStringForNullable({Values值.Replace("'", "\"")})";
                    }
                }
                else
                {
                    strError += str修改列 + "\t";
                }
            }


            return strError;
        }

        static string Get使用数据对象(string str数据对象语句, Dictionary<string, string> dic表字段, Dictionary<int, string> dic变量赋值)
        {
            string strError = string.Empty;
            string[] strArr = str数据对象语句.Split(',');
            if (strArr.Length != dic变量赋值.Count)
            {
                strError += $"表字段个数【{dic变量赋值.Count}】和数据对象个数对不上【{strArr.Length}】";
            }

            for (int index = 0; index < strArr.Length; index++)
            {
                string strSX = strArr[index].Trim();
                if (dic变量赋值.ContainsKey(index))
                {
                    string[] list对象 = dic变量赋值[index].Split("@@@");
                    foreach (string str列名称 in list对象)
                    {
                        if (dic表字段.ContainsKey(str列名称))
                        {
                            dic表字段[str列名称] = strSX;
                        }
                    }
                }
            }
            return strError;
        }

    }

    public class 解析对象SQL语句
    {
        public string Str表名称 { get; set; }
        public Dictionary<string, string> dic列对象 = new Dictionary<string, string>();
        public string StrError = string.Empty;

        public static List<解析对象SQL语句> Get解析对象(string strSql)
        {
            List<解析对象SQL语句> list对象 = new List<解析对象SQL语句>();

            string str截取前半段数据;
            string str截取后半段语句;

            string str查找字符 = "INSERT";


            bool Is存在INSERT = StringHelper.Is截取字符串(strSql, str查找字符, out str截取前半段数据, out str截取后半段语句);
            while (Is存在INSERT)
            {
                string strNewSql = str截取后半段语句;
                解析对象SQL语句 obj对象;
                if (Sql解析INSERT数据("INSERT" + str截取后半段语句, out obj对象, out str截取后半段语句))
                {
                    list对象.Add(obj对象);
                }

                Is存在INSERT = StringHelper.Is截取字符串(strNewSql, str查找字符, out str截取前半段数据, out str截取后半段语句);
            }

            return list对象;
        }

        public static bool Sql解析INSERT数据(string strSql, out 解析对象SQL语句 objSql对象, out string str截取后半段语句)
        {
            objSql对象 = new 解析对象SQL语句();
            string str截取前半段数据;

            string str查找字符 = "INSERT";
            if (!StringHelper.Is截取字符串(strSql, str查找字符, out str截取前半段数据, out str截取后半段语句))
            {
                //throw new Exception("获取 【INSERT】 语句失败！");
                return false;
            }

            str查找字符 = "INTO";
            if (!StringHelper.Is截取字符串(str截取后半段语句, str查找字符, out str截取前半段数据, out str截取后半段语句))
            {
                //throw new Exception("获取 【INSERT INTO】 语句失败！");
                return false;
            }
            else
            {
                if (str截取前半段数据.Trim().Length != 0)
                {
                    return false;
                }
            }

            str查找字符 = "(";
            if (!StringHelper.Is截取字符串(str截取后半段语句, str查找字符, out str截取前半段数据, out str截取后半段语句))
            {
                // throw new Exception("获取 【INSERT INTO 表名称 (】  语句失败！");
                return false;
            }
            else
            {
                //-------------------------------------------------------
                string Str表名称语句 = str截取前半段数据.Trim().TrimStart('[').TrimEnd(']');
                if (Str表名称语句.Length == 0 || Str表名称语句.StartsWith("\""))
                {
                    return false;
                }
                objSql对象.Str表名称 = Str表名称语句; // StringHelper.Get第一个单词(Str表名称语句);
                //-------------------------------------------------------
            }

            str查找字符 = ")";
            if (!StringHelper.Is截取字符串(str截取后半段语句, str查找字符, out str截取前半段数据, out str截取后半段语句))
            {
                //throw new Exception("获取 【INSERT INTO 表名称 ( 字段1,字段2 )】  语句失败！");
                return false;
            }
            //-------------------------------------------------------
            string str表字段语句 = str截取前半段数据;
            //-------------------------------------------------------

            str查找字符 = "(";
            if (!StringHelper.Is截取字符串(str截取后半段语句, str查找字符, out str截取前半段数据, out str截取后半段语句))
            {
                //throw new Exception("获取 【INSERT INTO 表名称 ( 字段1,字段2 ) values ( )】  语句失败！");
                return false;
            }
            else
            {
                if (!str截取前半段数据.Trim().EndsWith("values", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("获取 【INSERT INTO 表名称 ( 字段1,字段2 ) values ( )】  语句失败！");
                }
            }
            str查找字符 = ")";
            if (!StringHelper.Is截取字符串(str截取后半段语句, str查找字符, out str截取前半段数据, out str截取后半段语句))
            {
                //throw new Exception("获取 【INSERT INTO 表名称 ( 字段1,字段2 ) values ( )】  语句失败！");
                return false;
            }

            //-------------------------------------------------------
            string strVulues语句 = str截取前半段数据;
            //-------------------------------------------------------

            str查找字符 = ",";
            if (!StringHelper.Is截取字符串(str截取后半段语句, str查找字符, out str截取前半段数据, out str截取后半段语句))
            {
                //throw new Exception("获取 【\"INSERT INTO 表名称 ( 字段1,字段2 ) values ( )\", 数据对象】  语句失败！");
                return false;
            }
            else
            {
                if (str截取前半段数据.Contains("Update", StringComparison.OrdinalIgnoreCase))
                {
                    str查找字符 = "string.Format";
                    if (!StringHelper.Is截取字符串(str截取后半段语句, str查找字符, out str截取前半段数据, out str截取后半段语句))
                    {
                        throw new Exception("获取 【\"INSERT INTO 表名称 ( 字段1,字段2 ) values ( )\", 数据对象】  语句失败！");
                        //return false;
                    }

                    str查找字符 = ",";
                    if (!StringHelper.Is截取字符串(str截取后半段语句, str查找字符, out str截取前半段数据, out str截取后半段语句))
                    {
                        throw new Exception("获取 【\"INSERT INTO 表名称 ( 字段1,字段2 ) values ( )\", 数据对象】  语句失败！");
                        //return false;
                    }
                }
            }

            str查找字符 = ";";
            if (!StringHelper.Is截取字符串(str截取后半段语句, str查找字符, out str截取前半段数据, out str截取后半段语句))
            {
                //throw new Exception("获取 【string.Format(\"INSERT INTO 表名称 ( 字段1,字段2 ) values ( )\", 数据对象); 】  语句失败！");
                return false;
            }
            //-------------------------------------------------------
            string str数据对象语句 = str截取前半段数据.Trim().TrimEnd(')');
            //-------------------------------------------------------
            Dictionary<int, string> dic变量赋值;
            string strError = Get使用表字段(str表字段语句, strVulues语句, objSql对象.dic列对象, out dic变量赋值);
            if (!string.IsNullOrEmpty(strError))
            {
                objSql对象.StrError += "获取表字段存在问题：" + strError + System.Environment.NewLine;
            }
            strError = Get使用数据对象(str数据对象语句, objSql对象.dic列对象, dic变量赋值);
            if (!string.IsNullOrEmpty(strError))
            {
                objSql对象.StrError += "获取数据对象存在问题：" + strError + System.Environment.NewLine;
            }

            return true;
        }

        static string Get使用表字段(string str表字段语句, string strValues语句, Dictionary<string, string> dic表字段, out Dictionary<int, string> dic变量赋值)
        {
            dic表字段.Clear();
            string strError = string.Empty;
            List<string> listValues值 = GetValues值(strValues语句);

            dic变量赋值 = new Dictionary<int, string>();
            string[] strArr = str表字段语句.Split(',');

            for (int ctr = 0; ctr < strArr.Length; ctr++)
            {
                string srt字段名称 = StringHelper.Get第一个单词(strArr[ctr]);
                if (dic表字段.ContainsKey(srt字段名称))
                {
                    srt字段名称 = srt字段名称 + "【" + ctr.ToString() + "】";
                    strError += srt字段名称 + "\t";
                }

                dic表字段.Add(srt字段名称, listValues值[ctr]);

                if (listValues值[ctr].Contains("{"))
                {
                    int index = Convert.ToInt32(StringHelper.Get第一个单词(listValues值[ctr]));
                    if (dic变量赋值.ContainsKey(index))
                    {
                        dic变量赋值[index] = dic变量赋值[index] + "@@@" + srt字段名称;
                    }
                    else
                    {
                        dic变量赋值.Add(index, srt字段名称);
                    }
                }
                else
                {
                    dic表字段[srt字段名称] = $"API.Utility.GetSQLStringForNullable({listValues值[ctr].Replace("'", "\"")})";
                }
            }


            return strError;
        }

        static string Get使用数据对象(string str数据对象语句, Dictionary<string, string> dic表字段, Dictionary<int, string> dic变量赋值)
        {
            string strError = string.Empty;
            string[] strArr = str数据对象语句.Split(',');
            if (strArr.Length != dic变量赋值.Count)
            {
                strError += $"表字段个数【{dic变量赋值.Count}】和数据对象个数对不上【{strArr.Length}】";
            }

            for (int index = 0; index < strArr.Length; index++)
            {
                string strSX = strArr[index].Trim();
                if (dic变量赋值.ContainsKey(index))
                {
                    string[] list对象 = dic变量赋值[index].Split("@@@");
                    foreach (string str列名称 in list对象)
                    {
                        if (dic表字段.ContainsKey(str列名称))
                        {
                            dic表字段[str列名称] = strSX;
                        }
                    }
                }
            }
            return strError;
        }

        static List<string> GetValues值(string strValues语句)
        {
            List<string> listValues值 = new List<string>();

            string[] strArr = strValues语句.Split(',');
            foreach (string str in strArr)
            {
                listValues值.Add(str.Trim());
            }

            return listValues值;
        }

    }


}
