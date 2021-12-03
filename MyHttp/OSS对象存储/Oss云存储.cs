using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Model.Internal.MarshallTransformations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace 京东云demo
{
    public class Oss云存储
    {
        const int MULTIPLE_OBJECT_DELETE_LIMIT = 1000;
        IAmazonS3 s3Client = null;
        string bucketName;
        public Oss云存储(string _accessKeyId, string _accessKeySecret, string _ServiceURL, string _strBucketName)
        {
            s3Client = new AmazonS3Client(_accessKeyId, _accessKeySecret, new AmazonS3Config{ServiceURL = _ServiceURL,SignatureVersion = "4",UseHttp = true,});

            bucketName = _strBucketName;
        }

        /// <summary>
        /// 获取对应前缀使用文件记录
        /// </summary>
        /// <param name="strPrefix"></param>
        /// <returns></returns>
        public class目录列表Z Get文件列表(string strPrefix)
        {
            class目录列表Z class列表 = new class目录列表Z(strPrefix);
            var flag = true;
            string maker = string.Empty;
            do
            {
                var listObjectsRequest = new Amazon.S3.Model.ListObjectsRequest();
                listObjectsRequest.BucketName = bucketName; //桶目录
                listObjectsRequest.Marker = maker; //获取下一页的起始点，它的下一项
                listObjectsRequest.MaxKeys = 1000;//设置分页的页容量
                if (strPrefix.Length > 0)
                {
                    listObjectsRequest.Prefix = strPrefix;
                }
                listObjectsRequest.Delimiter = "/";

                var result = s3Client.ListObjectsAsync(listObjectsRequest);

                foreach (var obj in result.Result.CommonPrefixes)
                {
                    class列表.Set目录(obj, null);
                }

                foreach (var obj in result.Result.S3Objects)
                {
                    class列表.add文件(obj);
                }

                maker = result.Result.NextMarker;
                flag = result.Result.IsTruncated;//全部执行完后，为false

            }
            while (flag);

            return class列表;
        }

        /// <summary>
        /// 获取对应前缀使用文件记录
        /// </summary>
        /// <param name="strPrefix"></param>
        /// <returns></returns>
        public List<string> Get所有文件列表(string strPrefix)
        {
            GC.Collect();

            List<string> list文件列表 = new List<string>();

            var flag = true;
            string maker = string.Empty;
            do
            {
                var listObjectsRequest = new Amazon.S3.Model.ListObjectsRequest();
                listObjectsRequest.BucketName = bucketName; //桶目录
                listObjectsRequest.Marker = maker; //获取下一页的起始点，它的下一项
                listObjectsRequest.MaxKeys = 1000;//设置分页的页容量
                if (strPrefix.Length > 0)
                {
                    listObjectsRequest.Prefix = strPrefix;
                }

                var result = s3Client.ListObjectsAsync(listObjectsRequest);

                foreach (var obj in result.Result.S3Objects)
                {
                    list文件列表.Add(obj.Key);
                }

                maker = result.Result.NextMarker;
                flag = result.Result.IsTruncated;//全部执行完后，为false
            }
            while (flag);

            return list文件列表;
        }
        public void DeleteObject(string objectKey)
        {
            string url = GeneratePreSignedURL(objectKey, HttpVerb.DELETE, DateTime.Now.AddMinutes(60));
           // DeleteObjectResponse deleteObject = s3Client.DeleteObjectAsync(bucketName, objectKey);
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="objectKey"></param>
        public void DeleteDirectory(string objectKey)
        {
            if (objectKey.EndsWith("/"))
            {
                objectKey += "/";
            }

            List<string> list文件列表 = Get所有文件列表(objectKey);
            if (list文件列表.Count == 0)
            {
                return;
            }
            //删除目录
            DeleteObjectsRequest deleteRequest = new DeleteObjectsRequest
            {
                BucketName = bucketName,
            };

            foreach (string key in list文件列表)
            {
                deleteRequest.AddKey(key);
                //最大删除数量
                if (1000 == deleteRequest.Objects.Count)
                {
                    s3Client.DeleteObjectsAsync(deleteRequest);
                    deleteRequest.Objects.Clear();
                }
            }

            if (deleteRequest.Objects.Count > 0)
            {
                s3Client.DeleteObjectsAsync(deleteRequest);
            }
        }



        /// <summary>
        /// 获取http链接
        /// </summary>
        /// <param name="objectKey"></param>
        /// <param name="httpVerb"></param>
        /// <param name="dt到期时间"></param>
        /// <returns></returns>
        public string GeneratePreSignedURL(string objectKey, HttpVerb httpVerb, DateTime dt到期时间)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = objectKey,
                Verb = httpVerb,
                Expires = dt到期时间 
            };

            string url = s3Client.GetPreSignedURL(request);
            return url;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="objectKey"></param>
        /// <param name="filePath"></param>
        public void UploadObject(string objectKey, string filePath)
        {
            string url = GeneratePreSignedURL(objectKey, HttpVerb.PUT, DateTime.Now.AddMinutes(60));
            HttpWebRequest httpRequest = WebRequest.Create(url) as HttpWebRequest;
            httpRequest.Method = "PUT";
            using (Stream dataStream = httpRequest.GetRequestStream())
            {
                var buffer = new byte[8000];
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    int bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        dataStream.Write(buffer, 0, bytesRead);
                    }
                }
            }
            HttpWebResponse response = httpRequest.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// 复制对象
        /// </summary>
        /// <param name="objectKey"></param>
        /// <param name="destObjectKey"></param>
        public void CopyObject(string objectKey, string destObjectKey)
        {
            CopyObjectRequest request = new CopyObjectRequest
            {
                SourceBucket = bucketName,
                SourceKey = objectKey,
                DestinationBucket = bucketName,
                DestinationKey = destObjectKey
            };
            //CopyObjectResponse response = s3Client.CopyObjectAsync(request);
        }

        /// <summary>
        /// 复制目录
        /// </summary>
        /// <param name="objectKey"></param>
        /// <param name="destObjectKey"></param>
        public void CopyDirectory(string objectKey, string destObjectKey)
        {
            if (!objectKey.EndsWith("/"))
            {
                objectKey += "/";
            }
            if (!destObjectKey.EndsWith("/"))
            {
                destObjectKey += "/";
            }

            List<string> list文件列表 = Get所有文件列表(objectKey);
      
            foreach (string objKey in list文件列表)
            {
                CopyObject(objKey, destObjectKey + objKey.Remove(0, objectKey.Length));
            }
        }

        /// <summary>
        /// 获取目录大小
        /// </summary>
        /// <param name="objectKey"></param>
        /// <returns></returns>
        public long GetDirectorySize(string objectKey)
        {
            if (!objectKey.EndsWith("/"))
            {
                objectKey += "/";
            }

            long dirSize = 0;
            var flag = true;
            string maker = string.Empty;
            do
            {
                var listObjectsRequest = new Amazon.S3.Model.ListObjectsRequest();
                listObjectsRequest.BucketName = bucketName; //桶目录,
                listObjectsRequest.Marker = maker; //获取下一页的起始点，它的下一项
                listObjectsRequest.MaxKeys = 1000;//设置分页的页容量
                listObjectsRequest.Prefix = objectKey;

                //listObjectsRequest.Delimiter = "/"; //设置查询当前目录下的文件，不包括子目录

                var result = s3Client.ListObjectsAsync(listObjectsRequest);

                foreach (var obj in result.Result.S3Objects)
                {
                    dirSize += obj.Size;
                }

                maker = result.Result.NextMarker;
                flag = result.Result.IsTruncated;//全部执行完后，为false
            }
            while (flag);

            return dirSize;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="objectKey"></param>
        /// <param name="ToFile"></param>
        public void Download(string objectKey,string ToFile)
        {
            GetObjectRequest request = new GetObjectRequest()
            {
                BucketName = bucketName,
                Key = objectKey
            };

            //GetObjectResponse response = s3Client.GetObjectAsync(request);
            //response.WriteResponseStreamToFileAsync(ToFile);
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="objectKey"></param>
        public void CreateDirectory(string objectKey)
        {
            if (!objectKey.EndsWith("/"))
            {
                objectKey += "/";
            }
            PutObjectRequest request = new PutObjectRequest 
            {
                BucketName = bucketName, 
                Key = objectKey, 
            };
            s3Client.PutObjectAsync(request);
        }
        #region 判断文件、目录 是否存在

        public bool FileExists(string file)
        {
            return ObjectExists(file);
        }

        public bool DirectoryExists(string directory)
        {
            if (!directory.EndsWith("/"))
            {
                directory += "/";
            }

            return ObjectExists(directory);
        }

        public bool ObjectExists(string objkey)
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = bucketName,
                    Key = objkey
                };
                // If the object doesn't exist then a "NotFound" will be thrown
                s3Client.GetObjectMetadataAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public dynamic GetFileList(string directory)
        {
            if (!directory.EndsWith("/") && directory.Length > 0)
            {
                directory += "/";
            }

            dynamic response_Obj = new System.Dynamic.ExpandoObject();
            //使用XDocument创建xml
            System.Xml.Linq.XDocument xdoc = new XDocument();
            //添加根节点
            XElement rootEle = new XElement("Result");
            xdoc.Add(rootEle);

            class目录列表 CML = Get文件列表XX(directory);
            rootEle.Add(CML.GetXElement(null));

            response_Obj =(xdoc.Root.ToString()).TrimStart("<Result>".ToArray()).TrimEnd("</Result>".ToArray());

            return response_Obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strPrefix"></param>
        /// <returns></returns>
        public class目录列表 Get文件列表XX(string strPrefix)
        {
            class目录列表 class列表 = new class目录列表(strPrefix);
            var flag = true;
            string maker = string.Empty;
            do
            {
                var listObjectsRequest = new Amazon.S3.Model.ListObjectsRequest();
                listObjectsRequest.BucketName = bucketName; //桶目录
                listObjectsRequest.Marker = maker; //获取下一页的起始点，它的下一项
                listObjectsRequest.MaxKeys = 1000;//设置分页的页容量
                listObjectsRequest.Prefix = strPrefix;

                //listObjectsRequest.Delimiter = "/";

                var result = s3Client.ListObjectsAsync(listObjectsRequest);

                foreach (var obj in result.Result.S3Objects)
                {
                    class列表.AddObject(obj);
                }

                maker = result.Result.NextMarker;
                flag = result.Result.IsTruncated;//全部执行完后，为false
            }
            while (flag);

            return class列表;
        }

    }

    public class class目录列表
    {
        string strPrefix;
        string MLname = string.Empty;
        public class目录列表(string _strPrefix)
        {
            strPrefix = _strPrefix;
            int indexE = strPrefix.LastIndexOf('/');
            if (indexE > 0)
            {
                int indexS = strPrefix.LastIndexOf('/', indexE - 1) + 1;
                MLname = strPrefix.Substring(indexS, indexE - indexS);
            }
        }
        Dictionary<string, class目录列表> dic目录 = new Dictionary<string, class目录列表>();
        List<XElement> list文件列表 = new List<XElement>();

        public void AddObject(S3Object obj)
        {
            if (obj.Key.EndsWith("/"))
            {
                InitML(obj.Key);
            }
            else
            {
                XElement itemEle = new XElement("FileInfo");
                string strML = InitWJxml(obj, itemEle);
                InitML(strML, itemEle);


            }
        }
        public void InitML(string strML, XElement itemEle = null)
        {
            if (strML.Length > strPrefix.Length)
            {
                int index = strML.IndexOf('/', strPrefix.Length) + 1;
                string str子目录 = strML.Substring(0, index);
                if (dic目录.Keys.Contains(str子目录))
                {
                    dic目录[str子目录].InitML(strML, itemEle);
                }
                else
                {
                    class目录列表 classML = new class目录列表(str子目录);
                    dic目录[str子目录] = classML;
                    classML.InitML(strML, itemEle);
                }
            }
            else
            {
                if (strML == strPrefix && itemEle != null)
                {
                    list文件列表.Add(itemEle);
                }
            }
        }


        string InitWJxml(S3Object obj, XElement itemEle)
        {
            int index = obj.Key.LastIndexOf('/') + 1;
            string strML = obj.Key.Substring(0, index);
            string fileName = obj.Key.Remove(0, index);
            //给根节点添加子节点
            itemEle.Add(new XElement("FileName", fileName));
            itemEle.Add(new XElement("Length", obj.Size.ToString()));
            itemEle.Add(new XElement("CreateTime", obj.LastModified.ToString("yyyy-MM-dd HH:mm:ss")));
            itemEle.Add(new XElement("LastModifyTime", obj.LastModified.ToString("yyyy-MM-dd HH:mm:ss")));

            return strML;
        }

        public XElement GetXElement(XElement directoryEle)
        {
            if (directoryEle == null)
            {
                directoryEle = new XElement("Directory");
                XAttribute attrValue = new XAttribute("Value", MLname);
                directoryEle.Add(attrValue);
            }

            foreach (var obj in dic目录)
            {
                directoryEle.Add(obj.Value.GetXElement(null));
            }

            foreach (XElement obj in list文件列表)
            {
                directoryEle.Add(obj);
            }

            return directoryEle;
        }

        public List<String> list目录()
        {
            List<String> list目录 = new List<string>();
            foreach (var obj in dic目录)
            {
                list目录.Add(obj.Key);
            }

            foreach (XElement obj in list文件列表)
            {
                list目录.Add(obj.Element("FileName").Value);
            }

            return list目录;
        }
    }

    /**/
    public class class目录列表Z
    {
        string strPrefix;
        public class目录列表Z(string _strPrefix)
        {
            strPrefix = _strPrefix;
        }

        Dictionary<string, class目录列表> dic目录 = new Dictionary<string, class目录列表>();
        public void Set目录(string _strPrefix, class目录列表 class目录)
        {
            if (_strPrefix != strPrefix)
            {
                dic目录[_strPrefix] = class目录;
            }
        }

        public List<string> Getlist目录()
        {
            List<string> list目录 = new List<string>();
            foreach (var obj in dic目录)
            {
                list目录.Add(obj.Key);
            }

            return list目录;
        }

        public List<Amazon.S3.Model.S3Object> list文件列表 = new List<Amazon.S3.Model.S3Object>();
        public void add文件(Amazon.S3.Model.S3Object obj)
        {
            list文件列表.Add(obj);
        }

        public List<string> Getlist文件()
        {
            List<string> list文件 = new List<string>();
            foreach (var obj in list文件列表)
            {
                list文件.Add(obj.Key);
            }

            return list文件;
        }
    }
}
