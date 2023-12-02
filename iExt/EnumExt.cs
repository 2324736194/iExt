using System.Collections.Generic;
using System.Linq;

namespace System
{
    /// <summary>
    /// 枚举扩展
    /// </summary>
    public static class EnumExt
    {
        /// <summary>
        /// 将数据转换成指定枚举类型的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this object value)
            where T : Enum
        {
            if (value is string stringValue)
            {
                return ToEnum<T>(stringValue);
            }
            var enumType = typeof(T);
            if (Enum.IsDefined(enumType, value))
            {
                if (Enum.ToObject(enumType, value) is T result)
                {
                    return result;
                }
            }

            throw new ArgumentException($"无法将 {value} 转换为 {enumType.FullName} 的枚举成员", nameof(value));
        }

        /// <summary>
        /// 将字符串转换成枚举，默认忽略大小写
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static T ToEnum<T>(this string value, bool ignoreCase = true)
            where T : struct
        {
            var enumType = typeof(T);
            if (enumType.IsEnum)
            {
                if (Enum.TryParse(value, ignoreCase, out T result))
                {
                    return result;
                }
            }

            throw new ArgumentException($"无法将 {value} 转换为 {enumType.FullName} 的枚举成员", nameof(value));
        }

        /// <summary>
        /// 获取枚举类型包含的所有枚举值
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyList<T> GetValues<T>()
            where T : Enum
        {
            return Enum.GetValues(typeof(T)).OfType<T>().ToList();
        }
    }
}