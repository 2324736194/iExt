using System.Reflection;

namespace System.Linq.Expressions
{
    /// <summary>
    /// Lambda 常用处理逻辑
    /// </summary>
    public static class ExpressionExt
    {
        /// <summary>
        /// 获取 Lambda 表达式中的属性
        /// </summary>
        /// <param name="selector"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="Exception"></exception>
        public static PropertyInfo GetProperty<TSource, TResult>(this Expression<Func<TSource, TResult>> selector)
        {
            try
            {
                if (null == selector)
                {
                    throw new ArgumentNullException();
                }
                if (selector.Body is MemberExpression memberExpression)
                {
                    if (memberExpression.Member is PropertyInfo propertyInfo)
                    {
                        return propertyInfo;
                    }
                }

                throw new NotImplementedException($"未实现的解析类型：{selector.Body}");
            }
            catch (Exception ex)
            {
                throw new Exception("Lambda 表达式解析失败。", ex);
            }
        }
    }
}
