namespace System
{
    /// <summary>
    /// 仅针对 IComparable 接口的扩展
    /// </summary>
    public static class ComparableExt
    {
        /// <summary>
        /// 当前值是否处于某个范围之间，包含下列情况
        /// <para>- 不包含最小值和最大值（minValue &lt; value &lt; maxValue）</para>
        /// <para>- 仅包含最小值（minValue &lt;= value &lt; maxValue）</para>
        /// <para>- 仅包含最大值（minValue &lt; value &lt;= maxValue）</para>
        /// <para>- 包含最小值和最大值（minValue &lt;= value &lt;= maxValue）</para>
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="includeMin"> 是否包含最小值，默认包含最小值 </param>
        /// <param name="includeMax">是否包含最大值，默认包含最大值</param>
        /// <returns></returns>
        public static bool Between<T>(this T value, T minValue, T maxValue,
            bool includeMin = true,
            bool includeMax = true)
            where T : IComparable
        {
            if (minValue.CompareTo(maxValue) > 0)
            {
                throw new ArgumentException("最小值不能大于最大值", nameof(minValue));
            }

            var result1 = includeMin ? minValue.CompareTo(value) <= 0 : minValue.CompareTo(value) < 0;
            var result2 = includeMax ? value.CompareTo(maxValue) <= 0 : value.CompareTo(maxValue) < 0;
            return result1 && result2;
        }
    }
}
