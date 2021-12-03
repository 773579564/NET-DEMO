using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TrueLore.标书查看.Main
{
    public class DownloadFile
    {
        private static bool RemoteCertificateValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            //对SSL证书无条件接受通过
            return true;
        }

        #region 下载相关
        public delegate void delegateDownloadProcessHandler(long downSize);
        public static delegateDownloadProcessHandler DownloadProcessHandler = null;

        public delegate void delegateDownloadFinishHandler(string strMeg);
        public static delegateDownloadFinishHandler DownloadFinishHandler = null;

        public delegate void DownloadStartEvent(string fileName, long fileSize);
        public static DownloadStartEvent DownloadStartHandler = null;
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="downFileUrl"></param>
        /// <param name="localFullFileName"></param>
        /// <returns>下载成功返回true，否则返回false</returns>
        public static string Download(String downFileUrl, string localFilePath, string localFileName, out string str下载地址, bool 显示进度条 = true)
        {
            string strMeg = "";
            HttpWebResponse rspesponse = null;
            Stream stream = null;
            str下载地址 = string.Empty;

            try
            {

                HttpWebRequest request = null;
                if (downFileUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    //忽略SSL检查
                    ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidate;
                }
                request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(downFileUrl));
                request.Method = "GET";
                ServicePointManager.DefaultConnectionLimit = 500;

                //获取返回结果
                rspesponse = (HttpWebResponse)request.GetResponse();
                String str_Content_Disposition = rspesponse.GetResponseHeader("Content-Disposition");

                //获取文件下载名称
                GetFileName(str_Content_Disposition, ref localFileName); //获取文件名称
                if (!localFileName.Contains("."))
                {
                    throw new Exception("标书类型获取失败，请确认！");
                }
                str下载地址 = Path.Combine(localFilePath, localFileName);
                if (File.Exists(str下载地址))
                {
                    File.Delete(str下载地址);
                }
                else if (!Directory.Exists(localFilePath))
                {
                    Directory.CreateDirectory(localFilePath);
                }

                long fileSize = rspesponse.ContentLength;
                long downSize = 0;
                if (DownloadStartHandler != null && 显示进度条) DownloadStartHandler(str下载地址, fileSize);
                stream = rspesponse.GetResponseStream();
                using (var fs = File.Open(str下载地址, FileMode.Create))
                {
                    int length = 0;
                    int bufLength = 1024 * 4;
                    var buf = new byte[bufLength];
                    do
                    {
                        length = stream.Read(buf, 0, bufLength);
                        fs.Write(buf, 0, length);

                        downSize += length;
                        if (DownloadFinishHandler != null && 显示进度条) DownloadProcessHandler(downSize);


                    } while (length != 0);
                }
            }
            catch (Exception ex)
            {
                strMeg = ex.Message;
            }
            finally
            {
                // 释放资源
                if (stream != null) stream.Close();
                if (rspesponse != null) rspesponse.Close();
                if (DownloadFinishHandler != null && 显示进度条) DownloadFinishHandler(strMeg);
            }
            return strMeg;
        }

        public static void GetFileName(string str_Content_Disposition, ref string strFileName)
        {
            if (!strFileName.Contains("."))
            {
                //获取服务端文件名
                int index = str_Content_Disposition.IndexOf("filename=");
                if (index >= 0)
                {
                    int indexEND = str_Content_Disposition.IndexOf(";", index + 9);
                    if (indexEND < 0)
                    {
                        indexEND = str_Content_Disposition.Length;
                    }
                    int len = indexEND - index - 9;
                    String strServerFileName = System.Web.HttpUtility.UrlDecode(str_Content_Disposition.Substring(index + 9, len));
                    strFileName = strServerFileName;
                }
            }
        }
        #endregion

        public static String 字节单位(long 字节)
        {
            if (字节 > 1024 * 1024) //1M
                return (字节 / 1024 / 1024).ToString() + "MB";
            else if (字节 > 1024) //1K
                return (字节 / 1024).ToString() + "KB";
            else
                return 字节 + "B";
        }
    }
}
