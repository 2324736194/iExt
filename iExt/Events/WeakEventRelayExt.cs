using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Events
{
    /// <summary>
    /// 弱事件扩展
    /// </summary>
    public static class WeakEventRelayExt
    {
        private static readonly object _locker = new object();

        /// <summary>
        /// 外部弱事件中继表格
        /// </summary>
        private static readonly ConditionalWeakTable<object, IList<IWeakEventRelay>> _relayTable;

        static WeakEventRelayExt()
        {
            _relayTable = new ConditionalWeakTable<object, IList<IWeakEventRelay>>();
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="owner">事件拥有者</param>
        /// <param name="eventName">事件名称</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static IWeakEventRelay RegisterWeakEvent<T>(this T owner, string eventName)
        {
            lock (_locker)
            {
                var builder = new StringBuilder();
                builder.AppendFormat("注册 {0}：", nameof(WeakEventRelay));
                if (null == owner)
                {
                    throw new ArgumentNullException(nameof(owner));
                }

                var events = WeakEventRelay.GetEvents(owner.GetType());
                var e = events.SingleOrDefault(p => p.Name == eventName);

                if (null == e)
                {
                    builder.Append($"对象 {nameof(T)} 中不存在该事件 {eventName}");
                    throw new ArgumentNullException(eventName, builder.ToString());
                }

                if (e.AddMethod.IsStatic)
                {
                    builder.Append("不支持静态事件");
                    throw new ArgumentOutOfRangeException(nameof(e), builder.ToString());
                }

                if (!_relayTable.TryGetValue(owner, out var relays))
                {
                    relays = new List<IWeakEventRelay>();
                    _relayTable.Add(owner, relays);
                }

                var relay = relays.SingleOrDefault(p => p.Equals(e));
                if (null == relay)
                {
                    relay = new WeakEventRelay(owner, e);
                    relays.Add(relay);
                }
                return relay;
            }
        }
    }
}