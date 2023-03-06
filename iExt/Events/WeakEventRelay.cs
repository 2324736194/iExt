using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Reflection.Emit;
using System.Xml.Linq;
using System.Diagnostics;
using System.Linq;
using System.Collections;

namespace System
{
    /// <summary>
    /// <para>弱事件中继</para>
    /// <para>弱事件中继都拥有唯一的事件和事件拥有者</para>
    /// <para>同一个事件拥有者，可以对应多个事件</para>
    /// <para>同一个事件，也可对应多个事件拥有者</para>
    /// </summary>
    public class WeakEventRelay
    {
        private static readonly object locker = new object();
        private static readonly Dictionary<Type, IReadOnlyList<EventInfo>> eDictionary;

        /// <summary>
        /// <para>事件拥有者</para>
        /// </summary>
        private readonly WeakReference ownerReference;
        /// <summary>
        /// <para>中继的事件</para>
        /// </summary>
        internal readonly EventInfo e;
        /// <summary>
        /// <para>事件处理函数拥有者引用</para>
        /// <para>此对象存在的意义是为了方便遍历，因为 <see cref="ConditionalWeakTable{TKey,TValue}"/> 不允许遍历</para>
        /// </summary>
        private readonly HashSet<WeakReference> handlerOwnerReferences;
        /// <summary>
        /// <para>----------------------------------------------------------------------------------------</para>
        /// <para><see cref="ConditionalWeakTable{TKey,TValue}"/> 对象说明</para> 
        /// <para>TKey：事件处理函数拥有者</para>
        /// <para>TValue：事件处理函数字典</para>
        /// <para>----------------------------------------------------------------------------------------</para>
        /// <para><see cref="Dictionary{TKey,TValue}"/> 对象说明</para>
        /// <para>TKey：事件处理函数</para>
        /// <para>TValue：事件处理函数调用次数</para>
        /// <para>----------------------------------------------------------------------------------------</para>
        /// <para>事件处理函数有两种情况，分别为实例函数和静态函数。</para>
        /// <para>- 实例函数：事件处理函数为实例函数时，事件处理函数拥有者为实例对象。</para>
        /// <para>- 静态函数：事件处理函数为静态函数时，事件处理函数拥有者为事件源实例对象。</para>
        /// </summary>
        private readonly ConditionalWeakTable<object, Dictionary<MethodInfo, int>> handlerTable;

        /// <summary>
        /// 表示当前事件中继是否能继续使用
        /// </summary>
        public bool IsEnabled => ownerReference.IsAlive;
        
        static WeakEventRelay()
        {
            eDictionary = new Dictionary<Type, IReadOnlyList<EventInfo>>();
        }

        /// <summary>
        /// 实例化弱事件中继
        /// </summary>
        /// <param name="owner">
        /// <para>事件拥有者有两种情况，分别为静态和实例</para>
        /// <para>- 实例事件拥有者为事件的实例对象</para>
        /// <para>- 静态事件拥有者为事件的声明对象类型</para>
        /// </param>
        /// <param name="eventInfo">事件名</param>
        /// <exception cref="ArgumentNullException"></exception>
        internal WeakEventRelay(object owner, EventInfo eventInfo)
        {
            e = eventInfo;
            ownerReference = new WeakReference(owner);
            handlerOwnerReferences = new HashSet<WeakReference>();
            handlerTable = new ConditionalWeakTable<object, Dictionary<MethodInfo, int>>();
        }

        /// <summary>
        /// 新增事件处理
        /// </summary>
        /// <param name="handler"></param>
        public void Add(Delegate handler)
        {
            lock (locker)
            {
                if (!IsEnabled)
                {
                    return;
                }
                HandlerOwnerReferenceClear();
                var methods = GetHandlerMethods(handler);
                var handerMethod = handler.Method;
                if (!methods.ContainsKey(handerMethod))
                {
                    methods.Add(handerMethod, 0);
                }

                methods[handerMethod]++;
            }
        }

        /// <summary>
        /// 事件处理函数拥有者弱引用清理
        /// </summary>
        private void HandlerOwnerReferenceClear()
        {
            handlerOwnerReferences.RemoveWhere(p => p.IsAlive == false);
        }

        /// <summary>
        /// 根据注册的事件处理，找到与之对应的事件处理函数字典
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private Dictionary<MethodInfo, int> GetHandlerMethods(Delegate handler)
        {
            if (null == handler)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            if (e.EventHandlerType != handler.GetType())
            {
                throw new ArgumentOutOfRangeException(nameof(handler));
            }
            // [handler.Target == null] 表示当前事件处理函数为静态函数
            // 静态函数的生命周期和事件源实例对象一致
            // ReSharper disable AssignNullToNotNullAttribute
            var handlerOwner = handler.Target ?? ownerReference.Target;
            if (!handlerTable.TryGetValue(handlerOwner, out var methods))
            {
                // 首次注册事件时（相对于 handlerOwner）
                var reference = new WeakReference(handlerOwner);
                methods = new Dictionary<MethodInfo, int>();
                handlerOwnerReferences.Add(reference);
                handlerTable.Add(handlerOwner, methods);
            }

            return methods;
        }

        /// <summary>
        /// 移除事件处理
        /// </summary>
        /// <param name="handler"></param>
        public void Remove(Delegate handler)
        {
            lock (locker)
            {
                if (!IsEnabled)
                {
                    return;
                }
                HandlerOwnerReferenceClear();
                var methods = GetHandlerMethods(handler);
                var handerMethod = handler.Method;
                if (!methods.ContainsKey(handerMethod))
                {
                    return;
                }
                methods[handerMethod]--;
            }
        }

        /// <summary>
        /// 引发中继事件
        /// </summary>
        /// <param name="parameters">事件参数</param>
        public void Raise(params object[] parameters)
        {
            lock (locker)
            {
                foreach (var reference in handlerOwnerReferences)
                {
                    if (reference.IsAlive)
                    {
                        Invoke(reference.Target, parameters);
                    }
                }
            }
        }

        /// <summary>
        /// 调用事件处理函数
        /// </summary>
        /// <param name="handlerOwner">事件处理函数拥有者</param>
        /// <param name="parameters">调用参数</param>
        private void Invoke(object handlerOwner, object[] parameters)
        {
            // ReSharper disable PossibleNullReferenceException
            if (!handlerTable.TryGetValue(handlerOwner, out var methods))
            {
                return;
            }
            foreach (var method in methods)
            {
                var obj = method.Key.IsStatic ? null : handlerOwner;
                for (int i = 0; i < method.Value; i++)
                {
                    method.Key.Invoke(obj, parameters);
                }
            }
        }


        /// <summary>
        /// 获取当前类型的所有实例事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IReadOnlyList<EventInfo> GetEvents<T>()
        {
            lock (locker)
            {
                var owner = typeof(T);
                if (!eDictionary.ContainsKey(owner))
                {
                    var bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                    var events = owner.GetEvents(bindingAttr);
                    eDictionary.Add(owner, events);
                }

                return eDictionary[owner];
            }
        }
    }
}