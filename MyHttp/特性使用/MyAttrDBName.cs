using System;
using System.Collections.Generic;
using System.Text;

namespace MyHelper.特性使用
{
    //自定义特性类
    class MyAttrDBName : System.Attribute
    {
        public String Name
        {
            get;
        }

        public MyAttrDBName(string _name)
        {
            Name = _name;
        }
    }
}
