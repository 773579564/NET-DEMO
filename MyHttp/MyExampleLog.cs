using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MyHelper
{
    /// <summary>
    /// 自实现日志记录类
    /// </summary>
    public class MyExampleLog
    {
        #region 保存日志文件
        static string _savePath = "";
        /// <summary>
        /// 获取保存地址
        /// </summary>
        public static string SavePaht
        {
            get
            {
                if (_savePath.Length == 0)
                {
                    _savePath = Path.Combine(AppContext.BaseDirectory, "Logs");
                    FileHelper.CreateDirectory(_savePath);
                }
                return _savePath;
            }
            set
            {
                if (_savePath != value)
                {
                    _savePath = value;
                    FileHelper.CreateDirectory(_savePath);
                }
            }
        }

        /// <summary>
        /// 保存到文件
        /// </summary>
        /// <param name="myLog"></param>
        public static void SaveToFile(MyExampleLog myLog)
        {
            string strFile = Path.Combine(SavePaht, myLog.GetFileName);
            File.WriteAllText(strFile, myLog.GetLogs());
        }
        #endregion

        #region 日志记录id
        /// <summary>
        /// 读取配置锁对象
        /// </summary>
        private static object deadlocked = new object();
        private static int m_nNextID = 1;
        public static string Auto_Name
        {
            get
            {
                lock (deadlocked)
                {
                    return (m_nNextID++).ToString() + "-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
                }
            }
        }
        #endregion
        /// <summary>
        /// 用写入日志类记录日志
        /// </summary>
        /// <param name="myLog"></param>
        /// <param name="Meg"></param>
        public static void Writelog(MyExampleLog myLog, string Meg)
        {
            if (myLog != null)
            {
                myLog.WriteLog(Meg);
            }
        }

        public MyExampleLog()
        {
            FileName = Auto_Name;
        }

        bool IsError = false;
        string FileName
        {
            get;
        }

        public string GetFileName
        {
            get
            {
                return (IsError ? "Error" : "Info") + FileName + ".txt";
            }
        }

        StringBuilder listLog = new StringBuilder();
        /// <summary>
        /// 写入一行日志
        /// </summary>
        /// <param name="strMeg"></param>
        public void WriteLog(string strMeg)
        {
            listLog.AppendLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}：{strMeg}");
        }

        /// <summary>
        /// 写入一行错误日志
        /// </summary>
        /// <param name="strMeg"></param>
        public void WriteExLog(string strMeg, Exception ex)
        {
            IsError = true;
            listLog.AppendLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}：{strMeg}" + Utility.GetMessage(ex));
        }

        /// <summary>
        /// 写入空白行
        /// </summary>
        public void WriteLogLine()
        {
            listLog.AppendLine();
        }

        /// <summary>
        /// 获取所有日志
        /// </summary>
        /// <returns></returns>
        public string GetLogs()
        {
            return listLog.ToString();
        }


        /// <summary>
        /// 保存日志到文件
        /// </summary>
        public void SaveToFile()
        {
            MyExampleLog.SaveToFile(this);
        }
    }
}
