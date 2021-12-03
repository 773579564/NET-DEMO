using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Text;
using MyHelper.文件数据库;

namespace DBAccess
{
    class AccessDatabase_OLEDB_ACE : IDatabase
    {
        private OleDbConnection m_Connection = null;
        private OleDbCommand command = null;

        List<string> listTableName = new List<string>();
        private Dictionary<string, Dictionary<string, 列对象>> dic表所有列 = new Dictionary<string, Dictionary<string, 列对象>>(StringComparer.OrdinalIgnoreCase);

        public AccessDatabase_OLEDB_ACE(string strMDBPath, string strPassword)
        {
            string strConnection;
            if (strPassword == null)
                strConnection = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}",
                    strMDBPath);
            else
                strConnection = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Jet OLEDB:Database Password={1}",
                    strMDBPath,
                    strPassword);
            m_Connection = new OleDbConnection(strConnection);

            m_Connection.Open();
        }

        public override string FileName
        {
            get
            {
                return m_Connection.DataSource;
            }
        }

        public override void CloseConnection()
        {
            m_Connection.Close();
        }

        public override void ExecuteSQL(string strSQL, DbTransaction trans = null)
        {
            if (command == null)
            {
                command = new OleDbCommand(null, this.m_Connection);
                command.CommandType = System.Data.CommandType.Text;
            }
            if (command.Transaction != trans)
                command.Transaction = trans as OleDbTransaction;

            command.CommandText = strSQL;
            command.ExecuteNonQuery();
        }

        public override void ExecuteSQL(string strSQL, Dictionary<string, object> listParam, DbTransaction trans = null)
        {
            OleDbCommand command = new OleDbCommand(strSQL, this.m_Connection);
            command.Parameters.AddRange(this.GenArrayParamByListObject(listParam));

            if (trans != null)
                command.Transaction = trans as OleDbTransaction;
            command.ExecuteNonQuery();
        }

        private OleDbParameter[] GenArrayParamByListObject(Dictionary<string, object> listParam)
        {
            OleDbParameter[] parameterArray = new OleDbParameter[listParam.Count];
            int index = 0;
            foreach (KeyValuePair<string, object> pair in listParam)
            {
                parameterArray[index] = new OleDbParameter(pair.Key, pair.Value);
                index++;
            }
            return parameterArray;
        }

        public override DataTable GetTableBySQL(string strSQL, bool bAddWithKey = false)
        {
            DataTable dTable = new DataTable("MyTable");
            OleDbDataAdapter da = new OleDbDataAdapter(strSQL, m_Connection);
            if (bAddWithKey) da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            da.Fill(dTable);
            return dTable;
        }

        public override Dictionary<string, 列对象> GetTableColumns(string strTableName)
        {
            if (dic表所有列.Count == 0)
            {
                DataTable schemaTable = m_Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns,
                          new object[] { null, null, null, null });

                string strTABLE_NAME, strCOLUMN_NAME, strCOLUMN_TYPE;
                Dictionary<string, DataTable> dic数据表对象 = new Dictionary<string, DataTable>();
                foreach (DataRow row in schemaTable.Rows)
                {
                    strTABLE_NAME = row["TABLE_NAME"].ToString();
                    strCOLUMN_NAME = row["COLUMN_NAME"].ToString();
                    strCOLUMN_TYPE = row["DATA_TYPE"].ToString();

                    Dictionary<string, 列对象> dic所有列;
                    if (!dic表所有列.TryGetValue(strTABLE_NAME, out dic所有列))
                    {
                        dic所有列 = new Dictionary<string, 列对象>(StringComparer.OrdinalIgnoreCase);
                        dic表所有列[strTABLE_NAME] = dic所有列;
                    }


                    DataTable dtData;
                    if (!dic数据表对象.TryGetValue(strTABLE_NAME, out dtData))
                    {
                        dtData = GetTableBySQL("select * from [" + strTableName + "] where 1=0", true);
                        dic数据表对象[strTABLE_NAME] = dtData;
                    }

                    列对象 obj = new 列对象()
                    {
                        str列名称 = strCOLUMN_NAME,
                        Is不为空 = Convert.ToString(row["IS_NULLABLE"]) == "1",
                        Is存在默认值 = row["COLUMN_HASDEFAULT"] != DBNull.Value,
                        obj默认值对象 = row["COLUMN_HASDEFAULT"],
                    };
                    DataColumn column = dtData.Columns[obj.str列名称];
                    obj.type = column.DataType;
                    obj.Is自增长 = column.AutoIncrement;
                    if (column.DefaultValue != DBNull.Value)
                    {
                        obj.obj默认值对象 = column.DefaultValue;
                    }
                    obj.int数据长度 = column.MaxLength;
                    dic所有列[obj.str列名称] = obj;
                }
            }

            if (!dic表所有列.ContainsKey(strTableName))
            {
                return new Dictionary<string, 列对象>(StringComparer.OrdinalIgnoreCase);
            }

            return dic表所有列[strTableName];
        }

        public override List<string> GetTableName()
        {
            if (listTableName.Count == 0)
            {
                DataTable schemaTable = m_Connection.GetOleDbSchemaTable(
               OleDbSchemaGuid.Tables,
               new object[] { null, null, null, "TABLE" });

                foreach (DataRow row in schemaTable.Rows)
                {
                    listTableName.Add(row["TABLE_NAME"].ToString());
                }
            }
            return listTableName;
        }
    }
}
