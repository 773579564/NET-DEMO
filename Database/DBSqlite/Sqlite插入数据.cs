using MyHelper.文件数据库;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DBSqlite
{
    public class Sqlite插入数据
    {
        public static void 表数据导入(IDatabase dbSqlite, string strTableName, IDatabase db数据源)
        {
            string str查询语句 = string.Format("select * from [{0}]", strTableName);
            DataTable dt数据源 = db数据源.GetTableBySQL(str查询语句);
            int intRow = dt数据源.Rows.Count;
            if (intRow == 0)
            {
                return;
            }
            else
            {
                MyHelper.WriteTextDel.Write($"【{strTableName}】总记录数量：" + intRow);
            }

            Dictionary<string, int> dic数据源列索引 = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (DataColumn column in dt数据源.Columns)
            {
                dic数据源列索引.Add(column.ColumnName, column.Ordinal);
            }

            Dictionary<列对象, int> dic对应列 = new Dictionary<列对象, int>();
            Dictionary<string, 列对象> dic列对象 = dbSqlite.GetTableColumns(strTableName);
            StringBuilder str添加语句 = new StringBuilder();
            foreach (var kv in dic列对象)
            {
                string str列名称 = kv.Key;
                if (str添加语句.Length > 0)
                {
                    str添加语句.Append(",");
                }

                str添加语句.Append("[" + str列名称 + "]");
                if (dic数据源列索引.ContainsKey(str列名称))
                {
                    dic对应列[kv.Value] = dic数据源列索引[str列名称];
                }
                else
                {
                    dic对应列[kv.Value] = -1;
                }
            }

            string str添加数据语句 = string.Format("INSERT INTO [{0}] ({1})", strTableName, str添加语句.ToString());
            List<string> list插入语句 = Get插入语句(dt数据源, dic对应列, str添加数据语句);

            foreach (string str插入语句 in list插入语句)
            {
                dbSqlite.ExecuteSQL(str插入语句);
            }

        }
        static List<string> Get插入语句(DataTable dt数据源, Dictionary<列对象, int> dic对应列, string str添加语句)
        {
            List<string> list插入语句 = new List<string>();

            int intRow = dt数据源.Rows.Count;
            StringBuilder str查询数据 = new StringBuilder();
            int MaxRow = 5000; //每次插入记录数量
            int thisRow = 0;
            for (int index = 0; index < intRow; index++)
            {
                if (thisRow > MaxRow)
                {
                    thisRow = 0;
                    list插入语句.Add(str添加语句 + str查询数据.ToString());
                    str查询数据.Clear();
                }

                str查询数据.Append(System.Environment.NewLine);
                str查询数据.Append(Get插入数据语句(dt数据源, dic对应列, thisRow == 0));
                
                thisRow++;
            }

            list插入语句.Add(str添加语句 + str查询数据.ToString());

            return list插入语句;
        }


        static string Get插入数据语句(DataTable dt数据源, Dictionary<列对象, int> dic对应列, bool Is首行)
        {
            StringBuilder str子数据 = new StringBuilder();
            if (Is首行)
            {

                foreach (var kv in dic对应列)
                {
                    if (str子数据.Length > 0)
                    {
                        str子数据.Append(",");
                    }

                    if (kv.Value == -1)
                    {
                        str子数据.Append(string.Format("null [{0}]", kv.Key.str列名称));
                    }
                    else
                    {
                        str子数据.Append(string.Format("{1} [{0}]", kv.Key.str列名称, Get数据(kv.Key, dt数据源.Rows[0][kv.Value])));
                    }
                }

                return "SELECT " + str子数据.ToString();
            }
            else
            {
                foreach (var kv in dic对应列)
                {
                    if (str子数据.Length > 0)
                    {
                        str子数据.Append(",");
                    }

                    if (kv.Value == -1)
                    {
                        str子数据.Append("null");
                    }
                    else
                    {
                        str子数据.Append(Get数据(kv.Key, dt数据源.Rows[0][kv.Value]));
                    }
                }
                return "UNION ALL SELECT " + str子数据.ToString();
            }
        }

        static string Get数据(列对象 obj列对象, object objdata)
        {
            if (objdata == DBNull.Value)
            {
                if (!obj列对象.Is不为空)
                {
                    return "null";
                }
                objdata = obj列对象.obj默认值对象;
            }

            bool Is默认值对象为空 = false;
            if (objdata == DBNull.Value || objdata == null)
            {
                Is默认值对象为空 = true;
            }
            TypeCode code = Type.GetTypeCode(obj列对象.type);
            switch (code)
            {
                case TypeCode.Boolean:
                    bool IsTrue;
                    if (Boolean.TryParse(Convert.ToString(objdata), out IsTrue))
                    {
                        return IsTrue ? "1" : "0";
                    }
                    return "0";
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    if (Is默认值对象为空)
                    {
                        return "0";
                    }
                    else
                    {
                        return Convert.ToString(objdata);
                    }
                default:
                    return "'" + Convert.ToString(objdata) + "'";
            }
        }


        /// <summary>
        /// 获取统一拼接语句
        /// </summary>
        /// <param name="strTableName"></param>
        /// <param name="listDic待写入"></param>
        /// <returns></returns>
        public static List<string> GetInsertSql(String strTableName, List<Dictionary<string, string>> listDic待写入, int int执行总数 = 5000)
        {
            List<string> list插入语句 = new List<string>();
            if (listDic待写入 == null || listDic待写入.Count == 0)
            {
                return list插入语句;
            }

            if (!strTableName.StartsWith("["))
            {
                strTableName = "[" + strTableName + "]";
            }


            StringBuilder str添加语句 = new StringBuilder();
            StringBuilder str查询数据 = new StringBuilder();
            int thisRow = 0;
            foreach (Dictionary<string, string> dic待写入 in listDic待写入)
            {

                if (thisRow > int执行总数)
                {
                    thisRow = 0;
                    list插入语句.Add(str添加语句.ToString() + str查询数据.ToString());
                    str查询数据 = new StringBuilder();
                    str添加语句 = new StringBuilder();
                }

                str查询数据.Append(System.Environment.NewLine);
                if (thisRow == 0)
                {
                    str添加语句.Append("INSERT INTO ").Append(strTableName).Append(" (");
                    str查询数据.Append("SELECT ");
                    foreach (var kv in dic待写入)
                    {
                        str添加语句.Append("[");
                        str添加语句.Append(kv.Key);
                        str添加语句.Append("]");

                        str查询数据.Append(kv.Value);
                        str查询数据.Append("[");
                        str查询数据.Append(kv.Key);
                        str查询数据.Append("]");
                        str查询数据.Append(",");
                    }

                    str添加语句.Append(")");
                    str查询数据 = str查询数据.Remove(str查询数据.Length - 1, 1);
                }
                else
                {
                    str查询数据.Append("UNION ALL SELECT ");
                    foreach (var kv in dic待写入)
                    {
                        str查询数据.Append(kv.Value);
                        str查询数据.Append(",");
                    }
                    str查询数据 = str查询数据.Remove(str查询数据.Length - 1, 1);
                }

                thisRow++;
            }

            if (thisRow > 0)
            {
                list插入语句.Add(str添加语句.ToString() + str查询数据.ToString());
            }
            return list插入语句;
        }

    }
}
