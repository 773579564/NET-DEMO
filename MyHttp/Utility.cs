using System;
using System.Collections.Generic;
using System.Text;

namespace MyHelper
{
    public static class Utility
    {
        /// <summary>
        /// 读取配置锁对象
        /// </summary>
        private static object deadlocked = new object();

        /// <summary>
        /// 获取Guid,加锁
        /// </summary>
        /// <returns>string类型：32的数字，由连字符分隔：00000000-0000-0000-0000-000000000000</returns>
        public static string GetGuid()
        {
            lock (deadlocked)
            {
                return Guid.NewGuid().ToString();
            }
        }

        /// <summary>
        /// 获取Guid,加锁, 
        /// </summary>
        /// <returns>string类型：32位数：00000000000000000000000000000000</returns>
        public static string GetNGuid()
        {
            lock (deadlocked)
            {
                return Guid.NewGuid().ToString("N");
            }
        }

        /// <summary>
        /// 获取Guid,加锁, 
        /// </summary>
        /// <returns>string类型：32位，用连字符隔开，括在大括号中：{00000000-0000-0000-0000-000000000000}</returns>
        public static string GetBGuid()
        {
            lock (deadlocked)
            {
                return Guid.NewGuid().ToString("B");
            }
        }

        /// <summary>
        /// 获取错误日志
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetMessage(Exception ex)
        {
            StringBuilder strMeg = new StringBuilder();
            strMeg.AppendLine();
            strMeg.AppendLine("**------------------Exception.Message--------------------**");
            strMeg.AppendLine(ex.Message);
            strMeg.AppendLine("*-------------------Exception.StackTrace------------------*");
            strMeg.AppendLine(ex.StackTrace);
            strMeg.AppendLine("**------------------Exception.End------------------------**");
            strMeg.AppendLine();
            if (ex.InnerException != null)
            {
                strMeg.AppendLine(GetMessage(ex.InnerException));
            }

            return strMeg.ToString();
        }

        /// <summary>
        /// 获取exe、dll文件数字签名
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static Boolean alreadySigned(string file)
        {
            try
            {
                var cert = System.Security.Cryptography.X509Certificates.X509Certificate.CreateFromSignedFile(file);
                var cert2 = new System.Security.Cryptography.X509Certificates.X509Certificate2(cert.Handle);
                bool valid = cert2.Verify();
                return valid;
            }
            catch (Exception e)
            {
            }
            return false;
        }
    }
}
