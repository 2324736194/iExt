using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Events;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Tests
{
    [TestClass()]
    public partial class EnumExtTests
    {
        [TestMethod()]
        public void ToEnumTest()
        {
            var intValue = 3;
            var value = intValue.ToEnum<MyEnum>();
            Assert.AreEqual(value, MyEnum.C);
            Console.WriteLine("测试目标：枚举");
            Console.WriteLine("测试内容：将 int 数据转换枚举");
            Console.WriteLine("测试值：{0}", intValue);
        }

        [TestMethod()]
        public void ToEnumTest1()
        {
            var intValue = 0;
            Assert.ThrowsException<ArgumentException>(() =>
            {
                intValue.ToEnum<MyEnum>();
            });
            Console.WriteLine("测试目标：枚举");
            Console.WriteLine("测试内容：将无效 int 数据转换枚举抛出异常");
            Console.WriteLine("测试值：{0}", intValue);
        }

        [TestMethod()]
        public void ToEnumTest2()
        {
            var stringValue = "c";
            var value = stringValue.ToEnum<MyEnum>();
            Assert.AreEqual(value, MyEnum.C);
            Console.WriteLine("测试目标：枚举");
            Console.WriteLine("测试内容：将 string 数据转换枚举");
            Console.WriteLine("测试值：{0}", stringValue);
        }
    }

    partial class EnumExtTests
    {
        enum MyEnum
        {
            A = 1,
            B,
            C,
            D,
            E,
            F,
            G
        }
    }
        
}