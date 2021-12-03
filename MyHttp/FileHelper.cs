using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyHelper
{
    public class FileHelper
    {
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="strFile">文件地址</param>
        /// <returns>bool类型：true-删除成功；false-删除失败</returns>
        public static bool DelFile(string strFile)
        {
            try
            {
                if (File.Exists(strFile))
                {
                    //去掉只读属性
                    File.SetAttributes(strFile, FileAttributes.Normal);

                    File.Delete(strFile);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="strPath">目录地址</param>
        /// <returns>bool类型：true-删除成功；false-删除失败</returns>
        public static bool DelDirectory(string strPath)
        {
            try
            {
                if (Directory.Exists(strPath))
                {
                    Directory.Delete(strPath, true);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="strFile"></param>
        /// <returns></returns>
        public static bool FileExists(string strFile)
        {
            return File.Exists(strFile);
        }

        /// <summary>
        /// 判断目录是否存在
        /// </summary>
        /// <param name="strPath"></param>
        /// <returns></returns>
        public static bool DirectoryExists(string strPath)
        {
            return Directory.Exists(strPath);
        }

        /// <summary>
        /// 判断目录是否存在
        /// </summary>
        /// <param name="strPath"></param>
        /// <returns></returns>
        public static void CreateDirectory(string strPath)
        {
            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }
        }

        /// <summary>
        /// 获取临时文件
        /// </summary>
        /// <param name="str临时文件后缀">如：.rar</param>
        /// <returns>临时文件地址：C:\Users\Administrator\AppData\Local\Temp\e2dc1b74-f99a-4c89-966f-6c0d0e58a45f.rar</returns>
        public static string GetTemp临时文件(string str临时文件后缀)
        {
            return Path.Combine(Path.GetTempPath(), Utility.GetNGuid() + str临时文件后缀);
        }

        /// <summary>
        /// 获取临时目录
        /// </summary>
        /// <param name="str临时文件后缀">如：.rar</param>
        /// <returns>临时目录地址：C:\Users\Administrator\AppData\Local\Temp\e2dc1b74-f99a-4c89-966f-6c0d0e58a45f</returns>
        public static string GetTemp临时目录()
        {
            return Path.Combine(Path.GetTempPath(), Utility.GetNGuid());
        }


    }
}
