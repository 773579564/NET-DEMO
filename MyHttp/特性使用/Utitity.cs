using MyHelper.文件数据库;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MyHelper.特性使用
{
    /// <summary>
    /// 遍历获取特性对象
    /// </summary>
    class Utitity
    {
        static bool bool测试调用抛出异常 = true;
        static 文件数据库.IDatabase ProjectDatabase;
        Dictionary<string, Dictionary<string, MyPropertyObject>> dic类属性对应表数据 = new Dictionary<string, Dictionary<string, MyPropertyObject>>();
        /// <summary>
        /// 获取 类属性 对应数据查询列
        /// </summary>
        /// <param name="type"></param>
        /// <returns>如果数据库表不存在返回null</returns>
        public Dictionary<string, MyPropertyObject> Get类属性对应表数据(Type type)
        {
            string strClassName = type.FullName;
            Dictionary<string, MyPropertyObject> dic属性对象;
            if (!dic类属性对应表数据.TryGetValue(strClassName, out dic属性对象))
            {
                //特性获取
                CustomAttributeData MyAttrDBName = type.CustomAttributes.FirstOrDefault(t => t.AttributeType == typeof(MyAttrDBName));
                if (MyAttrDBName == null)
                {
                    throw new Exception($"【{strClassName}】类未实现【MyAttrDBName】特性！");
                }
                //表名称
                string strTabelName = MyAttrDBName.ConstructorArguments[0].Value.ToString();
                //判断表是否存在
                if (ProjectDatabase.TableExists(strTabelName))
                {
                    //表所有列
                    Dictionary<string, 列对象> dic表所有列 = ProjectDatabase.GetTableColumns(strTabelName);
                    DataTable dt = ProjectDatabase.GetTableBySQL($"SELECT * FROM [{strTabelName}] WHERE 1=0");
                    //类所有属性
                    PropertyInfo[] propertys = type.GetProperties();

                    //初始化属性对应列
                    dic属性对象 = new Dictionary<string, MyPropertyObject>();
                    foreach (PropertyInfo pi in propertys)
                    {
                        //获取MyAttrDBName特性
                        MyAttrDBName = pi.CustomAttributes.FirstOrDefault(t => t.AttributeType == typeof(MyAttrDBName));
                        if (MyAttrDBName != null)
                        {
                            //获取特性记录的表列名
                            string colName = MyAttrDBName.ConstructorArguments[0].Value.ToString();

                            //无法查询的列跳过
                            if (dic表所有列.ContainsKey(colName))
                            {
                                MyPropertyObject ppobj = new MyPropertyObject(pi, colName);
                                ppobj.Set是否需要转换(Type.GetTypeCode(dt.Columns[colName].DataType));

                                dic属性对象.Add(pi.Name, ppobj);
                            }
                            else
                            {
                                if (bool测试调用抛出异常 && colName.Length > 0)
                                {
                                    //无法查询的抛出异常
                                    throw new Exception($"【{strClassName}】类【{pi.Name}】属性【{colName}】列不存在！");
                                }
                            }

                            if (!pi.CanWrite)
                            {
                                //没有写入属性的抛出异常
                                throw new Exception($"【{strClassName}】类【{pi.Name}】属性无法写入数据！");
                            }
                        }
                        else
                        {
                            //可设置值的必须需要 MyAttrDBName 特性
                            if (pi.CanWrite)
                            {
                                throw new Exception($"【{strClassName}】类【{pi.Name}】属性未实现【MyAttrDBName】特性！");
                            }
                        }

                    }
                }
                else
                {
                    //查询不到表的先抛出异常
                    throw new Exception($"【{strTabelName}】表在数据库不存在！");
                }
                dic类属性对应表数据.Add(strClassName, dic属性对象);
            }

            return dic属性对象;


        }
    }
}
