using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    /// <summary>
    /// <see cref="IEnumerable{T}"/> 的扩展函数
    /// </summary>
    public static class EnumerableExt
    {
        /// <summary>
        /// <para>对 <see cref="IEnumerable{T}"/> 进行切片操作</para>
        /// <para>等同于调用 <see cref="Array.Copy(Array,int, Array,int,int)"/></para> 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">数据源</param>
        /// <param name="startIndex">从数据源中切片的索引位置</param>
        /// <param name="size">切片大小</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static IEnumerable<T> Slice<T>(this IEnumerable<T> source, uint startIndex, uint size)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var arraySource = source as T[] ?? source.ToArray();
            var length = arraySource.Length;

            if (startIndex < 0 || length < startIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            if (size < 0 || startIndex + size > length)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            var array = new T[size];
            Array.Copy(arraySource, startIndex, array, 0, size);
            return array;
        }
    }
}
