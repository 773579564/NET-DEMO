using MyHelper.文件数据库;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DBSqlite
{
    public class Sqlite建表语句
    {
        public static string Get创建表语句(IDatabase dbAccess, string strTableName)
        {
            //取主键                               
            Dictionary<string, 列对象> dic列对象 = dbAccess.GetTableColumns(strTableName);
            StringBuilder strSql = new StringBuilder();

            foreach (var kv in dic列对象)
            {
                if (strSql.Length > 0)
                {
                    strSql.Append("," + System.Environment.NewLine);
                }

                列对象 obj = kv.Value;
                string str列语句 = Get创建列语句(obj);


                strSql.Append("    " + str列语句);
            }


            string str创建表语句 = "Create Table " + strTableName + " (" + System.Environment.NewLine + strSql.ToString() + System.Environment.NewLine + ");";
            return str创建表语句;
        }

        private static string Get创建列语句(列对象 obj列对象)
        {
            StringBuilder strSql = new StringBuilder();


            TypeCode code = Type.GetTypeCode(obj列对象.type);
            switch (code)
            {
                case TypeCode.Boolean:
                    strSql.Append(string.Format("[{0}] BOOLEAN", obj列对象.str列名称));
                    break;
                case TypeCode.DateTime:
                    strSql.Append(string.Format("[{0}] DateTime", obj列对象.str列名称));
                    break;
                case TypeCode.Decimal:
                    strSql.Append(string.Format("[{0}] DECIMAL(30,10)", obj列对象.str列名称));
                    break;
                case TypeCode.Double:
                    strSql.Append(string.Format("[{0}] DECIMAL(30,10)", obj列对象.str列名称));
                    break;
                case TypeCode.Byte:
                case TypeCode.SByte:
                    strSql.Append(string.Format("[{0}] TINYINT", obj列对象.str列名称));
                    break;
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                    if (!obj列对象.Is自增长)
                    {
                        strSql.Append(string.Format("[{0}] INT", obj列对象.str列名称));
                    }
                    else
                    {
                        strSql.Append(string.Format("[{0}] INTEGER PRIMARY KEY  AUTOINCREMENT", obj列对象.str列名称));
                    }
                    break;
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    if (!obj列对象.Is自增长)
                    {
                        strSql.Append(string.Format("[{0}] BIGINT", obj列对象.str列名称));
                    }
                    else
                    {
                        strSql.Append(string.Format("[{0}] INTEGER PRIMARY KEY  AUTOINCREMENT", obj列对象.str列名称));
                    }
                    break;
                case TypeCode.String:
                    if (obj列对象.int数据长度 > 2000)
                    {
                        strSql.Append(string.Format("[{0}] ntext", obj列对象.str列名称));
                    }
                    else
                    {
                        strSql.Append(string.Format("[{0}] nvarchar({1})", obj列对象.str列名称, obj列对象.int数据长度));
                    }
                    break;
                default:
                    strSql.Append(Get特殊类型列(obj列对象));
                    break;

            }

            if (!obj列对象.Is自增长)
            {
                if (obj列对象.Is主键)
                {
                    strSql.Append(" PRIMARY KEY");
                }
            }

            strSql.Append(Get不为NULL和默认值(obj列对象));

            return strSql.ToString();
        }


        static string Get不为NULL和默认值(列对象 obj列对象)
        {
            StringBuilder strSql = new StringBuilder();
            if (obj列对象.Is不为空)
            {
                strSql.Append(" NOT NULL");

                TypeCode code = Type.GetTypeCode(obj列对象.type);

                strSql.Append(" Default ");
                switch (code)
                {
                    case TypeCode.Boolean:
                        if (obj列对象.Is存在默认值)
                        {
                            strSql.Append(Convert.ToBoolean(obj列对象.obj默认值对象) ? "1" : "0");
                        }
                        else
                        {
                            strSql.Append("0");
                        }
                        break;
                    case TypeCode.String:
                        strSql.Append("'" + Convert.ToString(obj列对象.obj默认值对象) + "'");
                        break;
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Double:
                        strSql.Append(Convert.ToString(obj列对象.obj默认值对象));
                        break;
                    default:
                        throw new Exception("未实现当前默认值对象：" + obj列对象.type.ToString());
                }
            }
            return strSql.ToString();
        }



        public static string Get特殊类型列(列对象 obj列对象)
        {

            switch (obj列对象.type.ToString().ToLower())
            {
                case "system.byte[]":
                    return obj列对象.str列名称 + " Blob";
                default:
                    throw new Exception("未实现当前类型列创建：" + obj列对象.type.ToString());
            }
        }
    }
}
