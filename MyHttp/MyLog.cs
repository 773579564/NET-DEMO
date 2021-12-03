using System;
using System.Collections.Generic;
using System.Text;

namespace MyHelper
{
    class MyLog
    {
        /// <summary>
        /// 用写入日志类记录日志
        /// </summary>
        /// <param name="myLog"></param>
        /// <param name="Meg"></param>
        public static void Writelog(MyLog myLog, string Meg)
        {
            if (myLog != null)
            {
                myLog.WriteLog(Meg);
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
    }
}
