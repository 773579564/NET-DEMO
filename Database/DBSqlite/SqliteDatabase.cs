using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using MyHelper.文件数据库;

namespace DBSqlite
{
    public class SqliteDatabase : IDatabase
    {
       
        private Dictionary<string, Dictionary<string, 列对象>> dic表所有列 = new Dictionary<string, Dictionary<string, 列对象>>(StringComparer.OrdinalIgnoreCase);
       
        private List<string> list所有表 = new List<string>();
        private SQLiteConnection m_Connection;
        private SQLiteCommand m_cmd = null;
        private string m_strFileName;
        // Methods
        public SqliteDatabase(string strFileName, string strPassword, bool bReadOnly = false)
        {
            m_strFileName = strFileName;
            if (bReadOnly)
            {
                try
                {
                    this.OpenReadOnly(strFileName, strPassword);
                }
                catch
                {
                    this.OpenReadWrite(strFileName, strPassword);
                }
            }
            else
            {
                this.OpenReadWrite(strFileName, strPassword);
            }
            //获取或设置临时数据库文件所使用的存储模式 2 或 MEMORY	使用基于内存的存储
            ExecuteSQL("PRAGMA TEMP_STORE=2", new Dictionary<string, object>());
            //获取或设置控制日志文件如何存储和处理的日志模式:OFF	不保留任何日志记录
            ExecuteSQL("PRAGMA JOURNAL_MODE=OFF", new Dictionary<string, object>());
            //获取或设置当前磁盘的同步模式:0 或 OFF	不进行同步
            ExecuteSQL("PRAGMA SYNCHRONOUS=OFF", new Dictionary<string, object>());
            //ExecuteSQL("PRAGMA LOCKING_MODE=EXCLUSIVE", new Dictionary<string, object>());
        }

        private void OpenReadOnly(string strFileName, string strPassword)
        {
            m_Connection = new System.Data.SQLite.SQLiteConnection();
            System.Data.SQLite.SQLiteConnectionStringBuilder connstr = new System.Data.SQLite.SQLiteConnectionStringBuilder();
            connstr.DataSource = strFileName;
            if (!String.IsNullOrEmpty(strPassword)) connstr.Password = strPassword;//设置密码，SQLite ADO.NET实现了数据库密码保护
            connstr.ReadOnly = true;
            m_Connection.ConnectionString = connstr.ToString();
            m_Connection.Open();
        }

        private void OpenReadWrite(string strFileName, string strPassword)
        {
            m_Connection = new System.Data.SQLite.SQLiteConnection();
            System.Data.SQLite.SQLiteConnectionStringBuilder connstr = new System.Data.SQLite.SQLiteConnectionStringBuilder();
            connstr.DataSource = strFileName;

            if (!String.IsNullOrEmpty(strPassword)) connstr.Password = strPassword;//设置密码，SQLite ADO.NET实现了数据库密码保护
            connstr.ReadOnly = false;
            m_Connection.ConnectionString = connstr.ToString();
            m_Connection.Open();
        }

        public override string FileName
        {
            get
            {
                return "";
            }
        }

        public override void CloseConnection()
        {
            m_Connection.ReleaseMemory();
            m_Connection.Close();
            //GC.WaitForPendingFinalizers();
            System.Data.SQLite.SQLiteConnection.ClearAllPools();
            GC.Collect();
        }

        


        public override DataTable GetTableBySQL(string strSQL, bool bAddWithKey = false)
        {

            string strTableName = Do计时开始(strSQL);

            System.Data.SQLite.SQLiteCommand selectCommand = new System.Data.SQLite.SQLiteCommand(null, m_Connection)
            {
                CommandType = CommandType.Text,
                CommandText = strSQL
            };
            System.Data.SQLite.SQLiteDataAdapter adapter = new System.Data.SQLite.SQLiteDataAdapter(selectCommand);
            if (bAddWithKey)
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            Do计时暂停(strTableName);

            return dataTable;
        }

        public DataTable GetAllFields(string strTableName)
        {
            DataTable table = this.GetTableBySQL(String.Format("PRAGMA table_info({0})", strTableName));

            return table;
        }


        public override Dictionary<string, 列对象> GetTableColumns(string strTableName)
        {
            if (!dic表所有列.ContainsKey(strTableName))
            {
                DataTable table所有列 = GetAllFields(strTableName);
                DataTable dtData = GetTableBySQL("select * from [" + strTableName + "] where 1=0", true);

                Dictionary<string, 列对象> dic所有列 = new Dictionary<string, 列对象>(StringComparer.OrdinalIgnoreCase);
                foreach (DataRow dataRow in table所有列.Rows)
                {
                    列对象 obj = new 列对象()
                    {
                        str列名称 = dataRow["name"].ToString(),
                        Is不为空 = Convert.ToString(dataRow["notnull"]) == "1",
                        Is存在默认值 = dataRow["dflt_value"] != DBNull.Value,
                        obj默认值对象 = dataRow["dflt_value"],
                    };

                    DataColumn column = dtData.Columns[obj.str列名称];
                    obj.type = column.DataType;
                    obj.Is自增长 = column.AutoIncrement;
                    obj.obj默认值对象 = column.DefaultValue;
                    obj.int数据长度 = column.MaxLength;

                    dic所有列[obj.str列名称] = obj;
                }

                dic表所有列[strTableName] = dic所有列;
            }
            return dic表所有列[strTableName];
        }

        public override List<string> GetTableName()
        {
            if (list所有表.Count == 0)
            {
                foreach (DataRow row in this.GetTableBySQL("select * from sqlite_master where type='table'").Rows)
                {
                    list所有表.Add(row["name"].ToString());
                }
            }
            return list所有表;
        }


        public override void ExecuteSQL(string strSQL, DbTransaction trans = null)
        {
            if (m_cmd == null)
            {
                m_cmd = new System.Data.SQLite.SQLiteCommand(null, m_Connection);
                m_cmd.CommandType = System.Data.CommandType.Text;
            }
            if (m_cmd.Transaction != trans)
                m_cmd.Transaction = trans as SQLiteTransaction;
            m_cmd.CommandText = strSQL;

            m_cmd.ExecuteNonQuery();

        }

        public override void ExecuteSQL(string strSQL, Dictionary<string, object> listParam, DbTransaction trans = null)
        {
            System.Data.SQLite.SQLiteCommand command = new System.Data.SQLite.SQLiteCommand(strSQL, this.m_Connection);
            command.Parameters.AddRange(this.GenArrayParamByListObject(listParam));
            command.Transaction = trans as SQLiteTransaction;
            command.ExecuteNonQuery();
        }

        private System.Data.SQLite.SQLiteParameter[] GenArrayParamByListObject(Dictionary<string, object> listParam)
        {
            System.Data.SQLite.SQLiteParameter[] parameterArray = new System.Data.SQLite.SQLiteParameter[listParam.Count];
            int index = 0;
            foreach (KeyValuePair<string, object> pair in listParam)
            {
                parameterArray[index] = new System.Data.SQLite.SQLiteParameter(pair.Key, pair.Value);
                index++;
            }
            return parameterArray;
        }

        public override void FlushPool2(List<SqlStringObj> listSQL, bool bClearAll = false)
        {
            if (listSQL.Count == 0)
                return;

            if (listSQL.Count < 1000 && !bClearAll) return;


            SQLiteTransaction tran = m_Connection.BeginTransaction();// this.BeginTransaction();
            try
            {
                foreach (SqlStringObj sql in listSQL)
                {
                    if (sql.parms == null || sql.parms.Count == 0)
                        this.ExecuteSQL(sql.Sql, tran);
                    else
                        this.ExecuteSQL(sql.Sql, sql.parms, tran);
                }
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw ex;
            }
            listSQL.Clear();
        }

        public override void FlushPool2(List<string> listSQL, bool bClearAll = false)
        {
            if (listSQL.Count == 0)
                return;

            if (listSQL.Count < 1000 && !bClearAll) return;


            SQLiteTransaction tran = m_Connection.BeginTransaction();// this.BeginTransaction();
            try
            {
                foreach (string sql in listSQL)
                {
                    this.ExecuteSQL(sql, tran);

                }
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw ex;
            }
            listSQL.Clear();
        }
    }
}
