using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using MyHelper.文件数据库;

namespace DBAccess
{
    internal enum AccessType
    {
        None,
        ODBC,
        OLEDB_JET,
        OLEDB_ACE
    }

    public class AccessDatabase : IDatabase
    {
        private static AccessType m_AccessType = AccessType.None;
        private IDatabase m_IAccess = null;//IFileDatabase

		public AccessDatabase(string strMDBPath, string strPassword, bool bReadOnly = false)
		{
			if (m_AccessType == AccessType.None)
			{
				//自动检测
				StringBuilder sb = new StringBuilder();
				try
				{
					m_IAccess = new AccessDatabase_OLEDB_JET(strMDBPath, strPassword, bReadOnly);
					m_AccessType = AccessType.OLEDB_JET;
				}
				catch (Exception e)
				{
					sb.AppendLine("JET:");
					sb.AppendLine(e.Message);
					sb.AppendLine(string.Empty);
				}

				if (m_IAccess == null)
				{
					try
					{
						m_IAccess = new AccessDatabase_OLEDB_ACE(strMDBPath, strPassword);
						m_AccessType = AccessType.OLEDB_ACE;
					}
					catch (Exception e)
					{
						sb.AppendLine("ACE:");
						sb.AppendLine(e.Message);
						sb.AppendLine(string.Empty);
					}
				}

				if (m_IAccess == null)
				{
					try
					{
						m_IAccess = new AccessDatabase_ODBC(strMDBPath, strPassword);
						m_AccessType = AccessType.ODBC;
					}
					catch (Exception e)
					{
						sb.AppendLine("ODBC:");
						sb.AppendLine(e.Message);
						sb.AppendLine(string.Empty);

						throw new Exception(sb.ToString());
					}
				}
			}
			else
			{
				switch (m_AccessType)
				{
					case AccessType.ODBC:
						m_IAccess = new AccessDatabase_ODBC(strMDBPath, strPassword);
						break;
					case AccessType.OLEDB_ACE:
						m_IAccess = new AccessDatabase_OLEDB_ACE(strMDBPath, strPassword);
						break;
					case AccessType.OLEDB_JET:
						m_IAccess = new AccessDatabase_OLEDB_JET(strMDBPath, strPassword, bReadOnly);
						break;
					default:
						throw new Exception(this.GetType().FullName + "初始化异常！AccessType枚举初始化未实现： " + m_AccessType.ToString());
				}
			}
		}

		public override string FileName
		{
			get
			{
				return m_IAccess.FileName;
			}
		}

        public override void CloseConnection()
        {
			m_IAccess.CloseConnection();

		}

        public override void ExecuteSQL(string strSQL, DbTransaction trans = null)
        {
			m_IAccess.ExecuteSQL(strSQL, trans);

		}

        public override void ExecuteSQL(string strSQL, Dictionary<string, object> listParam, DbTransaction trans = null)
        {
			m_IAccess.ExecuteSQL(strSQL, listParam, trans);

		}

        public override DataTable GetTableBySQL(string strSQL, bool bAddWithKey = false)
        {
			string strTableName = Do计时开始(strSQL);

			DataTable dataTable = m_IAccess.GetTableBySQL(strSQL, bAddWithKey);

			Do计时暂停(strTableName);
			return dataTable;

		}

        public override Dictionary<string, 列对象> GetTableColumns(string strTableName)
        {
			return m_IAccess.GetTableColumns(strTableName);

		}

        public override List<string> GetTableName()
        {
			return m_IAccess.GetTableName();
		}
    }
}
