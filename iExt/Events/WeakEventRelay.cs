using Microsoft.Win32;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Events
{
    internal class WeakEventRelay: IWeakEventRelay
    {
        private static readonly Dictionary<Type, IReadOnlyList<EventInfo>> _eventDictionary;

        private readonly object _locker;
        private readonly EventInfo _event;
        private readonly WeakReference _ownerReference;
        private IWeakEventRegister _register;
        private Delegate _raiseHandler;
        private bool _registering;
            
        /// <summary>
        /// <para>事件处理函数拥有者引用</para>
        /// <para>此对象存在的意义是为了方便遍历，因为 <see cref="ConditionalWeakTable{TKey,TValue}"/> 不允许遍历</para>
        /// </summary>
        private readonly HashSet<WeakReference> _handlerOwnerReferences;

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
        private readonly ConditionalWeakTable<object, IList<WeakEventMethodInvoke>> _handlerTable;
        
        /// <summary>
        /// 表示当前事件中继是否能继续使用
        /// </summary>
        public bool IsEnabled => _ownerReference.IsAlive;

        static WeakEventRelay()
        {
            _eventDictionary = new Dictionary<Type, IReadOnlyList<EventInfo>>();
        }

        /// <summary>
        /// 实例化弱事件中继
        /// </summary>
        /// <param name="owner">
        /// <para>事件拥有者有两种情况，分别为静态和实例</para>
        /// <para>- 实例事件拥有者为事件的实例对象</para>
        /// <para>- 静态事件拥有者为事件的声明对象类型</para>
        /// </param>
        /// <param name="event">事件名</param>
        /// <exception cref="ArgumentNullException"></exception>
        internal WeakEventRelay(object owner, EventInfo @event)
        {
            _locker = new object();
            _event = @event;
            _ownerReference = new WeakReference(owner);
            _handlerOwnerReferences = new HashSet<WeakReference>();
            _handlerTable = new ConditionalWeakTable<object, IList<WeakEventMethodInvoke>>();
        }
        
        public void Add(Delegate handler)
        {
            lock (_locker)
            {
                if (!IsEnabled)
                {
                    return;
                }
                AddRaiseHandler();
                var method = GetMethodInvoke(handler);
                method.InvokeCount++;
            }
        }
        
        public void Remove(Delegate handler)
        {
            lock (_locker)
            {
                if (!IsEnabled)
                {
                    return;
                }
                var method = GetMethodInvoke(handler);
                method.InvokeCount--;
#if DEBUG
                Debug.WriteLine("移除弱事件处理函数 {0}", (object)handler.Method.Name);
#endif
            }
        }

        /// <summary>
        /// 获取已注册的事件处理函数
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private WeakEventMethodInvoke GetMethodInvoke(Delegate handler)
        {   
            if (null == handler)
            {   
                throw new ArgumentNullException(nameof(handler));
            }
            if (_event.EventHandlerType != handler.GetType())
            {
                throw new ArgumentOutOfRangeException(nameof(handler));
            }
            // [handler.Target == null] 表示当前事件处理函数为静态函数
            // 静态函数的生命周期和事件源实例对象一致
            // ReSharper disable AssignNullToNotNullAttribute
            var handlerOwner = handler.Target ?? _ownerReference.Target;
            if (!_handlerTable.TryGetValue(handlerOwner, out var methods))
            {
                // 首次注册事件时（相对于 handlerOwner）
                var reference = new WeakReference(handlerOwner);
                methods = new List<WeakEventMethodInvoke>();
                _handlerOwnerReferences.Add(reference);
                _handlerTable.Add(handlerOwner, methods);
            }
            var handlerMethod = handler.Method;
            var method = methods.SingleOrDefault(p => p.Method == handlerMethod);
            if (null == method)
            {
                // 首次注册处理函数
                method = new WeakEventMethodInvoke(handlerMethod);
                methods.Add(method);
            }

            return method;
        }

        public void Clear()
        {
            lock (_locker)
            {
                if (!IsEnabled)
                {
                    return;
                }

                foreach (var reference in _handlerOwnerReferences)
                {
                    _handlerTable.Remove(reference);
                }
                _handlerOwnerReferences.Clear();
            }
        }

        public void RegisterRaise(IWeakEventRegister register)
        {   
            if (null == register)
            {
                throw new ArgumentNullException(nameof(register));
            }
            lock (_locker)
            {
                if (!IsEnabled)
                {
                    return;
                }

                if (null == _register)
                {
                    _register = register;
                }
            }
        }

        public bool Equals(EventInfo eventInfo)
        {
            return _event == eventInfo;
        }

        private void AddRaiseHandler()
        {
            if (null == _register)
            {
                return;
            }

            if (null == _raiseHandler)
            {
                var raiseMethod = _register.GetRegisterMethod(this);
                var handler = Delegate.CreateDelegate(_event.EventHandlerType, _register, raiseMethod);
                _raiseHandler = handler;
            }

            if (_registering)
            {
                return;
            }
            var target = _ownerReference.Target;
            _event.AddEventHandler(target, _raiseHandler);
            _registering = true;
        }

        private void RemoveRaiseHandler()
        {
            if (null == _raiseHandler)
            {
                return;
            }
            var target = _ownerReference.Target;
            _event.RemoveEventHandler(target, _raiseHandler);
            _registering = false;
        }

        public void Raise(params object[] parameters)
        {
            lock (_locker)
            {
                var references = _handlerOwnerReferences.ToList();
                foreach (var reference in references)
                {
                    if (reference.IsAlive)
                    {
                        var handlerOwner = reference.Target;
                        // ReSharper disable PossibleNullReferenceException
                        if (!_handlerTable.TryGetValue(handlerOwner, out var invokes))
                        {
                            continue;
                        }

                        foreach (var invoke in invokes.ToArray())
                        {
                            if (invoke.InvokeCount == 0)
                            {
                                invokes.Remove(invoke);
                                continue;
                            }
                            var obj = invoke.Method.IsStatic ? null : handlerOwner;
                            for (int i = 0; i < invoke.InvokeCount; i++)
                            {
                                invoke.Method.Invoke(obj, parameters);
                            }
                        }

                        if (invokes.Count == 0)
                        {
                            _handlerTable.Remove(handlerOwner);
                            _handlerOwnerReferences.Remove(reference);
#if DEBUG
                            Debug.WriteLine("移除弱事件订阅者 {0}", handlerOwner);
#endif
                        }
                    }
                    else
                    {
                        _handlerOwnerReferences.Remove(reference);
                   
                    }
                }
                // 注销事件调用
                if (_handlerOwnerReferences.Count == 0)
                {
                    RemoveRaiseHandler();
#if DEBUG
                    Debug.WriteLine("注销弱事件的事件调用");
#endif
                }
            }
        }
        
        /// <summary>
        /// 获取当前类型的所有实例事件
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static IReadOnlyList<EventInfo> GetEvents(Type owner)
        {
            if (!_eventDictionary.ContainsKey(owner))
            {
                var bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                var events = owner.GetEvents(bindingAttr);
                _eventDictionary.Add(owner, events);
            }

            return _eventDictionary[owner];
        }
    }
    
}   