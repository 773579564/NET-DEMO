using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MyHelper
{
    public class 签名
    {
        /// <summary>
        /// 调用 HmacSHA256 签名
        /// </summary>
        /// <param name="data"></param>
        /// <param name="keyByte"></param>
        /// <returns></returns>
        public static String GetHmacSHA256(byte[] data, byte[] keyByte)
        {
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(data);
                return Convert.ToBase64String(hashmessage);
            }
        }

        public static String GetHmacSHA256_小写(byte[] data, byte[] keyByte)
        {
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(data);

                StringBuilder sb = new StringBuilder(32);
                for (int i = 0; i < hashmessage.Length; i++)
                {
                    sb.Append(hashmessage[i].ToString("x").PadLeft(2, '0'));
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// 调用 HmacSHA256 签名
        /// </summary>
        /// <param name="data">要签名的数据</param>
        /// <param name="keyByte"></param>
        /// <param name="encod">字符转换编码集，默认为utf8</param>
        /// <returns></returns>
        public static String GetHmacSHA256(string data, string keyByte, Encoding encod)
        {
            return GetHmacSHA256(encod.GetBytes(data), encod.GetBytes(keyByte));
        }
    }
}
