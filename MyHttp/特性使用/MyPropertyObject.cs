using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MyHelper.特性使用
{
    /// <summary>
    /// 特性对象赋值
    /// </summary>
    public class MyPropertyObject
    {
        public MyPropertyObject(PropertyInfo pi, string colName)
        {
            Property = pi;
            StrColName = colName;
            TypeCode = MyGetTypeCode(pi.PropertyType);


            switch (TypeCode)
            {
                case TypeCode.Empty:
                case TypeCode.DBNull:
                case TypeCode.Object:
                    throw new Exception($"【{pi.Name}】属性【{pi.PropertyType.FullName}】获取基础类型TypeCode失败");
            }

        }

        /// <summary>
        /// 获取数据类型枚举
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private TypeCode MyGetTypeCode(Type type)
        {
            TypeCode code = Type.GetTypeCode(type);
            if (code == TypeCode.Object)
            {
                //对象类型 (int?) 获取类型的基础类型参数
                Type type1 = Nullable.GetUnderlyingType(type);
                code = Type.GetTypeCode(type1);
            }

            return code;
        }

        /// <summary>
        /// 对应属性 类型
        /// </summary>
        public TypeCode TypeCode
        {
            get;
        }

        /// <summary>
        /// 属性对象
        /// </summary>
        public PropertyInfo Property
        {
            get;
        }

        /// <summary>
        /// 属性对应列
        /// </summary>
        public string StrColName
        {
            get;
        }

        bool is需要转换 = true;
        public void Set是否需要转换(TypeCode ColTypeCode)
        {
            if (ColTypeCode == TypeCode)
            {
                is需要转换 = false;
            }
            else if (Is数字类型(ColTypeCode) && Is数字类型(TypeCode))
            {
                is需要转换 = false;
            }
            else
            {
                is需要转换 = true;
            }
        }

        /// <summary>
        /// 设置对应属性的值
        /// </summary>
        /// <param name="objClass"></param>
        /// <param name="row"></param>
        public void SetValue(object objClass, System.Data.DataRow row)
        {
            object objData = row[StrColName];
            if (objData != DBNull.Value)
            {
                if (is需要转换)
                {
                    Property.SetValue(objClass, GetTypeCodeData(objData));
                }
                else
                {
                    Property.SetValue(objClass, objData); //设置对应属性值
                }
            }
        }

        private Boolean Is数字类型(TypeCode code)
        {
            switch (code)
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Double:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 转换成对应的对象数据
        /// </summary>
        /// <param name="objData"></param>
        /// <returns></returns>
        private object GetTypeCodeData(object objData)
        {
            switch (TypeCode)
            {
                case TypeCode.Boolean: return Convert.ToBoolean(objData);
                case TypeCode.Char: return Convert.ToChar(objData);
                case TypeCode.SByte: return Convert.ToSByte(objData);
                case TypeCode.Byte: return Convert.ToByte(objData);
                case TypeCode.Int16: return Convert.ToInt16(objData);
                case TypeCode.UInt16: return Convert.ToUInt16(objData);
                case TypeCode.Int32: return Convert.ToInt32(objData);
                case TypeCode.UInt32: return Convert.ToUInt32(objData);
                case TypeCode.Int64: return Convert.ToInt64(objData);
                case TypeCode.UInt64: return Convert.ToUInt64(objData);
                case TypeCode.Single: return Convert.ToSingle(objData);
                case TypeCode.Double: return Convert.ToDouble(objData);
                case TypeCode.Decimal: return Convert.ToDecimal(objData);
                case TypeCode.DateTime: return Convert.ToDateTime(objData);
                case TypeCode.String: return Convert.ToString(objData);
                default:
                    throw new Exception($"【TrueLore.BidBookConvertServer.API.MyPropertyObject.GetTypeCodeData()】未实现对应类型【{TypeCode.ToString()}】转换！");
            }
        }
    }
}
