namespace System
{
    /// <summary>
    /// 仅针对 IComparable 接口的扩展
    /// </summary>
    public static class ComparableExt
    {
        /// <summary>
        /// 当前值是否处于某个范围之间。
        /// <para>最小值 &lt; 值 &lt; 最大值</para>
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <returns></returns>
        public static bool Between<T>(this T value, T minValue, T maxValue)
            where T : IComparable
        {
            if (minValue.CompareTo(maxValue) > 0)
            {
                throw new ArgumentException("最小值不能大于最大值", nameof(minValue));
            }
            var result1 = minValue.CompareTo(value) < 0;
            var result2 = value.CompareTo(maxValue) < 0;
            return result1 && result2;
        }

        /// <summary>
        /// 当前值是否处于某个范围。
        /// <para>最小值 &lt;= 值 &lt;= 最大值</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static bool Range<T>(this T value, T minValue, T maxValue)
            where T : IComparable
        {
            if (minValue.CompareTo(maxValue) > 0)
            {
                throw new ArgumentException("最小值不能大于最大值", nameof(minValue));
            }
            var result1 = minValue.CompareTo(value) <= 0;
            var result2 = value.CompareTo(maxValue) <= 0;
            return result1 && result2;
        }
    }
}
