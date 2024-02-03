using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq.Tests
{
    [TestClass()]
    public class EnumerableExtTests
    {
        [TestMethod()]
        public void SliceTest()
        {
            var dataSource = GetDataSource();
            var maxIndex = dataSource.Count - 1;
            var random = new Random();
            var startIndex = random.Next(0, maxIndex);
            var size = random.Next(0, maxIndex);
            if (dataSource.Count - (startIndex + size) < 0)
            {
                size = dataSource.Count - startIndex;
            }

            var slice = dataSource.Slice(startIndex, size);
            var separator = ",";
            Console.WriteLine($"{"索引",-7} {"数据",-7}");
            Console.WriteLine("------------------------------------------");
            for (int i = 0; i < dataSource.Count; i++)
            {
                Console.WriteLine($"{i,-7} {dataSource[i],-7}");
            }
            Console.WriteLine();
            Console.WriteLine($"切片位置：{startIndex}");
            Console.WriteLine($"切片大小：{size}");
            Console.WriteLine($"切片数据：{string.Join(separator, slice)}");
        }

        private IReadOnlyList<int> GetDataSource(int length = 10)
        {
            var array = new int[length];
            var random = new Random();
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = random.Next(1, 30);
            }
            return array;
        }
    }
}