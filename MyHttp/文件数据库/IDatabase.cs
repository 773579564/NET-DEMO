using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Text;

namespace MyHelper.文件数据库
{
    /// <summary>
    /// 数据库实现抽象类
    /// </summary>
    public abstract class IDatabase
    {
        protected IDatabase()
        {
        }

        /// <summary>
        /// 数据库文件名称
        /// </summary>
        public abstract string FileName { get; }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public abstract void CloseConnection();

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="strSQL">sql执行语句：增、删、改</param>
        /// <param name="trans">数据库事务</param>
        public abstract void ExecuteSQL(string strSQL, DbTransaction trans = null);
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="strSQL">sql执行语句：增、删、改</param>
        /// <param name="listParam">参数</param>
        /// <param name="trans">数据库事务</param>
        public abstract void ExecuteSQL(string strSQL, Dictionary<string, object> listParam, DbTransaction trans = null);

        /// <summary>
        /// 批量执行数据库语句，数据库事务执行
        /// </summary>
        /// <param name="listSQL"></param>
        /// <param name="bClearAll"></param>
        public virtual void FlushPool2(List<SqlStringObj> listSQL, bool bClearAll = false)
        {
            throw new Exception("【IDatabase.FlushPool2(List<SqlStringObj> listSQL, bool bClearAll = false)】方法未实现！");
        }

        /// <summary>
        /// 批量执行数据库语句，数据库事务执行
        /// </summary>
        /// <param name="listSQL"></param>
        /// <param name="bClearAll">是否立即执行所有数据</param>
        public virtual void FlushPool2(List<string> listSQL, bool bClearAll = false)
        {
            throw new Exception("【IDatabase.FlushPool2(List<string> listSQL, bool bClearAll = false)】方法未实现！");
        }

        /// <summary>
        /// 查询数据库,返回第一行
        /// </summary>
        /// <param name="strSQL">sql执行语句：查询</param>
        /// <returns></returns>
        public DataRow GetRowBySQL(string strSQL)
        {
            DataTable tableBySQL = this.GetTableBySQL(strSQL);
            if (tableBySQL.Rows.Count == 0)
            {
                return null;
            }
            return tableBySQL.Rows[0];
        }

        /// <summary>
        /// 查询数据库
        /// </summary>
        /// <param name="strSQL">sql执行语句：查询</param>
        /// <param name="bAddWithKey">是否添加必需的列和主键信息以完成架构</param>
        /// <returns>DataTable类型：查询结果集</returns>
        public abstract DataTable GetTableBySQL(string strSQL, bool bAddWithKey = false);

        /// <summary>
        /// 获取数据库所有表名称
        /// </summary>
        /// <returns></returns>
        public abstract List<string> GetTableName();

        /// <summary>
        /// 获取数据库表格所有列、列属性
        /// </summary>
        /// <param name="strTableName">表名称</param>
        /// <returns>Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) 类型：列-列数据属性键值对，表所有列，表不存在返回空键值对</returns>
        public abstract Dictionary<string, 列对象> GetTableColumns(string strTableName);

        /// <summary>
        /// 判断数据库表对应列是否存在
        /// </summary>
        /// <param name="strTableName"></param>
        /// <param name="strFieldName"></param>
        /// <returns>bool类型：true-存在；false-不存在</returns>
        public virtual bool FieldExists(string strTableName, string strFieldName)
        {
            return GetTableColumns(strTableName).ContainsKey(strFieldName);
        }

        /// <summary>
        /// 判断数据库表是否存在
        /// </summary>
        /// <param name="strTableName"></param>
        /// <returns>bool类型：true-存在；false-不存在</returns>
        public virtual bool TableExists(string strTableName)
        {
            List<string> tableName = this.GetTableName();
            foreach (string str in tableName)
            {
                if (strTableName.Equals(str, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }


        private Dictionary<String, int> m_dicTableName2NextId = new Dictionary<String, int>(StringComparer.CurrentCultureIgnoreCase);
        public int GetNextID(String tableName)
        {
            tableName = tableName.TrimStart('[').TrimEnd(']');

            int MaxID;
            if (!m_dicTableName2NextId.TryGetValue(tableName, out MaxID))
            {
                String sql = "select Max(ID) as maxID from " + tableName;
                DataRow row = GetRowBySQL(sql);
                if (row.IsNull("maxID"))
                    MaxID = 1;
                else
                    MaxID = System.Convert.ToInt32(row["maxID"]) + 1;
            }

            m_dicTableName2NextId[tableName] = MaxID + 1;
            return MaxID;
        }

        #region 统计查询用时

        public Stopwatch sp取数据计时 = new Stopwatch();
        public Dictionary<string, Stopwatch> dic表查询用时统计 = new Dictionary<string, Stopwatch>(StringComparer.OrdinalIgnoreCase);
        protected string Do计时开始(string strSql)
        {
            string strTableName = Get查询表(strSql);


            Stopwatch sp表取数据计时;
            if (!dic表查询用时统计.TryGetValue(strTableName, out sp表取数据计时))
            {
                sp表取数据计时 = new Stopwatch();
                dic表查询用时统计.Add(strTableName, sp表取数据计时);
            }

            sp表取数据计时.Start();
            sp取数据计时.Start();
            return strTableName;
        }

        protected void Do计时暂停(string strTableName)
        {
            sp取数据计时.Stop();
            dic表查询用时统计[strTableName].Stop();
        }

        public string Get查询表(string strSQL)
        {
            string str截取前半段数据;
            string str截取后半段数据;
            if (false == MyHelper.StringHelper.Is截取字符串(strSQL, "From", out str截取前半段数据, out str截取后半段数据))
            {
                return strSQL;  //没有from的直接返回语句
            }
            if (true == MyHelper.StringHelper.Is截取字符串(str截取后半段数据, "Where", out str截取前半段数据, out str截取后半段数据))
            {
                return str截取前半段数据.Trim().ToUpper();
            }
            MyHelper.StringHelper.Is截取字符串(str截取前半段数据, "ORDER", out str截取前半段数据, out str截取后半段数据);

            return str截取前半段数据.Trim().ToUpper();

        }
        #endregion

    }

    public class SqlStringObj 
    {
        public String Sql { get; set; }
        public Dictionary<string, object> parms { get; set; }

        public SqlStringObj()
        {
        }
    }

    public class 列对象
    {
        public string str列名称 { get; set; }
        public Type type { get; set; }

        public bool Is自增长 = false;
        public bool Is存在默认值 = false;
        public bool Is不为空 = false;
        public bool Is主键 = false;
        public int int数据长度 = -1;
        public int int小数位 = -1;
        public object obj默认值对象 = null;

    }
}
