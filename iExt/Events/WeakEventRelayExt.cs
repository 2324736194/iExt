using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Events
{
    /// <summary>
    /// 
    /// </summary>
    public static class WeakEventRelayExt
    {
        private static readonly object _locker = new object();

        /// <summary>
        /// 外部弱事件中继表格
        /// </summary>
        private static readonly ConditionalWeakTable<object, List<WeakEventRelay>> _relayTable;
        
        static WeakEventRelayExt()
        {
            _relayTable = new ConditionalWeakTable<object, List<WeakEventRelay>>();
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="owner">事件拥有者</param>
        /// <param name="eventName">事件名称</param>
        /// <param name="raise">事件调用委托</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static IWeakEventRelay RegisterWeakEvent<T>(
            this T owner,   
            string eventName,
            Action<T, IWeakEventRelay> raise = null)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("注册 {0}：", nameof(WeakEventRelay));
            if (null == owner)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            var events = WeakEventRelay.GetEvents<T>();
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
            
            lock (_locker)
            {
                if (!_relayTable.TryGetValue(owner, out var relays))
                {
                    relays = new List<WeakEventRelay>();
                    _relayTable.Add(owner, relays);
                }

                var relay = relays.SingleOrDefault(p => p._e == e);
                if (null == relay)
                {
                    relay = new WeakEventRelay(owner, e);
                    relays.Add(relay);
                    raise?.Invoke(owner, relay);
                }

                return relay;
            }
        }
        
        //public static IWeakEventRelay GetWeakEventRelay<T>(T owner, string eventName)
        //{           
        //    var builder = new StringBuilder();
        //    builder.AppendFormat("获取 {0}：", nameof(WeakEventRelay));
        //    lock (_locker)
        //    {
        //        if (null == owner)
        //        {
        //            builder.Append("事件拥有者不能为空");
        //            throw new ArgumentNullException(nameof(owner), builder.ToString());
        //        }
        //        var events = WeakEventRelay.GetEvents<T>();
        //        var e = events.SingleOrDefault(p => p.Name == eventName);
        //        if (null == e)
        //        {
        //            builder.Append($"对象 {nameof(T)} 中不存在该事件 {eventName}");
        //            throw new ArgumentNullException(eventName, builder.ToString());
        //        }
        //        if (!_relayTable.TryGetValue(owner, out var relays))
        //        {
        //            builder.AppendFormat("事件 {0} 未注册 {1}", eventName, nameof(WeakEventRelay));
        //            throw new ArgumentOutOfRangeException(nameof(e), builder.ToString());
        //        }
        //        var relay = relays.Single(p => p._e == e);
        //        return relay;
        //    }
        //}


    }
}