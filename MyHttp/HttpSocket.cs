using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MyHelper
{
    /// <summary>
    /// Url结构
    /// </summary>
    struct UrlInfo
    {
        public string Host;
        public int Port;
        public string File;
        public string Body;
    }

    public class WebToolkit
    {
        /// <summary>
        /// 解析URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static UrlInfo ParseURL(string strUrl)
        {
            string url = strUrl;
            UrlInfo urlInfo = new UrlInfo();
  
            urlInfo.Host = "";
            urlInfo.Port = 80;
            urlInfo.File = "/";
            urlInfo.Body = "";

            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            {
                url = url.Substring(7);
                urlInfo.Port = 80;
            }
            else if (url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                url = url.Substring(8);
                urlInfo.Port = 443;
            }

            int intIndex = url.IndexOf("/");
            if (intIndex != -1)
            {
                urlInfo.Host = url.Substring(0, intIndex);
                url = url.Substring(intIndex);
                intIndex = url.IndexOf("?");
                if (intIndex == -1)
                {
                    urlInfo.File = url;
                }
                else
                {
                    string[] strTemp = url.Split('?');
                    urlInfo.File = strTemp[0];
                    urlInfo.Body = strTemp[1];
                }
            }
            else
            {
                urlInfo.Host = url;
            }


            if (urlInfo.Host.IndexOf(":") != -1)
            {
                string[] strTemp = urlInfo.Host.Split(':');
                urlInfo.Host = strTemp[0];
                if (!int.TryParse(strTemp[1], out urlInfo.Port))
                {
                    throw new Exception("获取地址端口数据失败：" + strUrl);
                }
            }
            return urlInfo;
        }

        /// <summary>
        /// 发出请求并获取响应
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="body"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        private static string GetResponse(string server, int port, string body, Encoding encode)
        {
            string strResult = string.Empty;
            byte[] bteSend = Encoding.ASCII.GetBytes(body);
            byte[] bteReceive = new byte[1024];

            
            using (Socket socket = MySocket.ConnectSocket(server, port))
            {
                if (socket == null)
                    throw new Exception("Connection failed");

                socket.Send(bteSend, bteSend.Length, 0);

                int intLen = 0;
                do
                {
                    intLen = socket.Receive(bteReceive, bteReceive.Length, 0);
                    strResult += encode.GetString(bteReceive, 0, intLen);
                }
                while (intLen > 0);

                socket.Close();
            }

            return strResult;
        }

        /// <summary>
        /// GET请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string Get(string url, Encoding encode)
        {
            UrlInfo urlInfo = ParseURL(url);
            string strRequest = $"GET {urlInfo.File}?{urlInfo.Body} HTTP/1.1\r\nHost:{urlInfo.Host}:{urlInfo.Port}\r\nConnection:Close\r\n\r\n";
            return GetResponse(urlInfo.Host, urlInfo.Port, strRequest, encode);
        }

        /// <summary>
        /// POST请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string Post(string url, Encoding encode)
        {
            UrlInfo urlInfo = ParseURL(url);
            string strRequest = $"POST {urlInfo.File} HTTP/1.1\r\nHost:{urlInfo.Host}:{urlInfo.Port}\r\nContent-Length:{urlInfo.Body.Length}\r\nContent-Type:application/x-www-form-urlencoded\r\nConnection:Close\r\n\r\n{urlInfo.Body}";
            return GetResponse(urlInfo.Host, urlInfo.Port, strRequest, encode);
        }
    }


    public class MySocket
    {
        public static Socket ConnectSocket(string server, int port)
        {
            Socket s = null;

            // Get host related information.
            IPHostEntry hostEntry = Dns.GetHostEntry(server);

            // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
            // an exception that occurs when the host IP Address is not compatible with the address family
            // (typical in the IPv6 case).
            foreach (IPAddress address in hostEntry.AddressList)
            {
                IPEndPoint ipe = new IPEndPoint(address, port);
                Socket tempSocket =
                    new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                tempSocket.Connect(ipe);

                if (tempSocket.Connected)
                {
                    s = tempSocket;
                    break;
                }
                else
                {
                    continue;
                }
            }
            return s;
        }
    }

}
