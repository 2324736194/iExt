using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Tests
{
    [TestClass()]
    public class ComparableExtTests
    {

        [TestMethod()]
        public void BetweenTest1()
        {
            var minValue = 1;
            var maxValue = 100;
            var value = 10;
            Console.WriteLine("测试目标：minValue < value < maxValue");
            Console.WriteLine("测试内容：测试值在指定范围之内");
            Console.WriteLine("- 最小值 {0}", minValue);
            Console.WriteLine("- 最大值 {0}", maxValue);
            Console.WriteLine("- 测试值 {0}", value);
            Assert.IsTrue(value.Between(minValue, maxValue));
        }
        
        [TestMethod()]
        public void BetweenTest2()
        {
            var minValue = 1;
            var maxValue = 100;
            Console.WriteLine("测试目标：minValue < value < maxValue");
            Console.WriteLine("测试内容：排除最小值");
            Console.WriteLine("- 最小值 {0}", minValue);
            Console.WriteLine("- 最大值 {0}", maxValue);
            Console.WriteLine("- 测试值 {0}", minValue);
            Assert.IsFalse(minValue.Between(minValue, maxValue));
        }

        [TestMethod()]
        public void BetweenTest3()
        {
            var minValue = 1;
            var maxValue = 100;
            Console.WriteLine("测试目标：minValue < value < maxValue");
            Console.WriteLine("测试内容：排除最大值");
            Console.WriteLine("- 最小值： {0}", minValue);
            Console.WriteLine("- 最大值： {0}", maxValue);
            Console.WriteLine("- 测试值： {0}", maxValue);
            Assert.IsFalse(maxValue.Between(minValue, maxValue));
        }

        [TestMethod()]
        public void BetweenTest4()
        {
            var minValue = 1;
            var maxValue = 100;
            Console.WriteLine("测试目标：minValue <= value < maxValue");
            Console.WriteLine("测试内容：包含最小值");
            Console.WriteLine("- 最小值： {0}", minValue);
            Console.WriteLine("- 最大值： {0}", maxValue);
            Console.WriteLine("- 测试值： {0}", minValue);
            Assert.IsTrue(minValue.Between(minValue, maxValue,true));
        }

        [TestMethod()]
        public void BetweenTest5()
        {
            var minValue = 1;
            var maxValue = 100;
            Console.WriteLine("测试目标：minValue < value <= maxValue");
            Console.WriteLine("测试内容：包含最大值");
            Console.WriteLine("- 最小值： {0}", minValue);
            Console.WriteLine("- 最大值： {0}", maxValue);
            Console.WriteLine("- 测试值： {0}", maxValue);
            Assert.IsTrue(maxValue.Between(minValue, maxValue, true, true));
        }
    }
}