using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

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
        /// 注册弱事件中继
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner">事件拥有者</param>
        /// <param name="e">事件名称</param>
        /// <param name="register">
        /// 向事件源注册一个调用 <see cref="WeakEventRelay.Raise"/> 的事件处理
        /// </param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static WeakEventRelay RegisterWeakEvent<T>(
            this T owner,    
            EventInfo e,
            RegisterWeakEventHandler<T> register = null)
        {
            if (null == owner)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (null == e)
            {
                throw new ArgumentNullException(nameof(e));
            }

            //  不允许静态事件
            if (e.AddMethod.IsStatic)
            {
                throw new ArgumentOutOfRangeException(nameof(e));
            }

            // 检查事件源是否包含当前事件
            if (WeakEventRelay.GetEvents<T>().All(p => p.Name != e.Name))
            {
                throw new ArgumentOutOfRangeException(nameof(e));
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
                    register?.Invoke(owner, relay);
                }

                return relay;
            }
        }
            
        /// <summary>
        /// 获取已注册的弱事件中继
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static WeakEventRelay GetRelay(object owner, EventInfo e)
        {
            lock (_locker)
            {
                if (null == owner)
                {
                    throw new ArgumentNullException(nameof(owner));
                }
                if (!_relayTable.TryGetValue(owner, out var relays))
                {
                    throw new ArgumentOutOfRangeException(nameof(e));
                }
                var relay = relays.SingleOrDefault(p => p._e == e);
                if (null == relay)
                {
                    throw new InvalidOperationException();
                }

                return relay;
            }
        }

    }
}