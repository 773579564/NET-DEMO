using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace MyHelper
{
    public class ClassHelper
    {
        public static TypeCode MyGetTypeCode(Type type)
        {
            TypeCode code = Type.GetTypeCode(type);
            if (code == TypeCode.Object)
            {
                //对象类型 (int?) 获取类型的基础类型参数
                Type type1 = Nullable.GetUnderlyingType(type);
                code = Type.GetTypeCode(type1);
            }
            switch (code)
            {
                case TypeCode.Empty:
                case TypeCode.DBNull:
                case TypeCode.Object:
                    throw new Exception($"【{type.FullName}】获取基础类型TypeCode失败");
            }

            return code;
        }
    }
}
