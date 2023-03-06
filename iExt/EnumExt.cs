using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System
{
    /// <summary>
    /// 仅针对 <see cref="Enum"/> 的扩展
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
            var enumType = typeof(T);
            if (Enum.IsDefined(enumType, value))
            {
                if (Enum.ToObject(enumType, value) is T result)
                {
                    return result;
                }
            }

            throw new Exception($"无法将 {value} 转换为 {enumType.FullName} 的枚举成员");
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

        /// <summary>
        /// 获取枚举特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this Enum e)
            where T : Attribute
        {
            var name = e.ToString();
            var field = e.GetType().GetField(name);

            if (null == field)
            {
                throw new NotImplementedException($"[{name}] 未找到对应的枚举字段信息，请检查代码逻辑。");
            }

            var attribute = field.GetCustomAttribute<T>();
            if (null == attribute)
            {
                throw new NotImplementedException($"[{typeof(T).Name}] 未找到对应的特性。");
            }

            return attribute;
        }
    }
}
