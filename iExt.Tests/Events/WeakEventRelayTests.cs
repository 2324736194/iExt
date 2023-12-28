using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Events;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Events
{
    [TestClass()]
    public partial class WeakEventRelayTests
    {
        [TestMethod()]
        public void Test()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var owner = new EventOwner();
            var count = 30;
            for (int i = 0; i < count; i++)
            {
                var sub = new EventSubscriber();
                owner.Outside += sub.Handler;
            }

            owner.RaiseOutside();
            stopwatch.Stop();
            Console.WriteLine("测试目标：常规方式创建的事件，常规注册事件");
            Console.WriteLine("测试内容：常规方式注册并调用耗时");
            Console.WriteLine("测试耗时：{0}", stopwatch.Elapsed.TotalMilliseconds);
            Assert.AreEqual(count, EventSubscriber.HandlerCount);
        }

        [TestMethod()]
        public void Test1()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var owner = new EventOwner();
            var count = 30;
            for (int i = 0; i < count; i++)
            {
                var sub = new EventSubscriber();
                owner.Inside += sub.Handler;
            }

            owner.RaiseInside();
            stopwatch.Stop();
            Console.WriteLine("测试目标：用 {0} 创建的事件，常规注册事件", nameof(IWeakEventRelay));
            Console.WriteLine("测试内容：{0} 调用耗时", nameof(IWeakEventRelay));
            Console.WriteLine("测试耗时：{0}", stopwatch.Elapsed.TotalMilliseconds);
            Assert.AreEqual(count, EventSubscriber.HandlerCount);
        }

        [TestMethod()]
        public void Test2()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var owner = new EventOwner();
            var count = 30;
            var eventName = nameof(EventOwner.Outside);
            var relay = owner.RegisterWeakEvent(eventName);
            relay.RegisterRaise(new WeakEventRegister());
            var subscribers = new List<EventSubscriber>();
            for (int i = 0; i < count; i++)
            {
                var subscriber = new EventSubscriber();
                subscribers.Add(subscriber);
            }

            foreach (var subscriber in subscribers)
            {
                relay.Add(new EventHandler(subscriber.Handler));
            }
            owner.RaiseOutside();
            stopwatch.Stop();
            Console.WriteLine("测试目标：常规方式创建的事件，弱事件注册");
            Console.WriteLine("测试内容：{0} 调用耗时", nameof(IWeakEventRelay));
            Console.WriteLine("测试耗时：{0}", stopwatch.Elapsed.TotalMilliseconds);
            Assert.AreEqual(count, EventSubscriber.HandlerCount);
        }

        [TestMethod()]
        public void Test3()
        {   
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var owner = new EventOwner();
            GcSubscriber(owner).Wait();
            Console.WriteLine("测试目标：常规方式创建的事件，弱事件注册");
            Console.WriteLine("测试内容：事件订阅者允许被资源回收");
            Assert.AreEqual(0, EventSubscriber.HandlerCount);
        }

        [TestMethod()]  
        public void Test4()
        {
            var owner = new EventOwner();
            var count = 30;
            var eventName = nameof(EventOwner.Outside);
            var relay = owner.RegisterWeakEvent(eventName);
            relay.RegisterRaise(new WeakEventRegister());
            var subscribers = new List<EventSubscriber>();
            for (int i = 0; i < count; i++)
            {
                var subscriber = new EventSubscriber();
                subscribers.Add(subscriber);
            }

            foreach (var subscriber in subscribers)
            {
                relay.Add(new EventHandler(subscriber.Handler));
                relay.Add(new EventHandler(EventSubscriber.StaticHandler));
            }

            foreach (var subscriber in subscribers)
            {
                relay.Remove(new EventHandler(subscriber.Handler));
                relay.Remove(new EventHandler(EventSubscriber.StaticHandler));
            }
            owner.RaiseOutside();
            Console.WriteLine("测试目标：常规方式创建的事件，弱事件注册");
            Console.WriteLine("测试内容：事件注销后，不触发调用");
            Assert.AreEqual(0, EventSubscriber.HandlerCount);
        }

        [TestMethod()]
        public void Test5()
        {   
            var owner = new EventOwner();
            var count = 30;
            var eventName = nameof(EventOwner.Outside);
            var relay = owner.RegisterWeakEvent(eventName);
            relay.RegisterRaise(new WeakEventRegister());
            var subscribers = new List<EventSubscriber>();
            for (int i = 0; i < count; i++)
            {
                var subscriber = new EventSubscriber();
                subscribers.Add(subscriber);
            }

            foreach (var subscriber in subscribers)
            {
                relay.Add(new EventHandler(subscriber.Handler));
                relay.Add(new EventHandler(EventSubscriber.StaticHandler));
            }

            relay.Clear();
            owner.RaiseOutside();
            Console.WriteLine("测试目标：常规方式创建的事件，弱事件注册");
            Console.WriteLine("测试内容：事件被清理后，不触发调用");
            Assert.AreEqual(0, EventSubscriber.HandlerCount);
        }

        [TestMethod()]
        public void Test6()
        {
            var owner = new EventOwner();
            var subscriber = new EventSubscriber();
            var count = 3;
            Register(owner, subscriber);
            Console.WriteLine("测试目标：隐式实现的事件");
            Console.WriteLine("测试内容：触发隐式事件");
            for (int i = 0; i < count; i++)
            {
                owner.Name = DateTime.Now.ToLongTimeString();
                Task.Delay(1000).Wait();
            }
            Assert.AreEqual(count, EventSubscriber.HandlerCount);
        }

        [TestMethod()]  
        public void Test7()
        {
            var owner = new EventOwner();
            var outsideRelay= owner.RegisterWeakEvent(nameof(EventOwner.Outside));
            outsideRelay.RegisterRaise(new WeakEventRegister());
            var handler = default(EventHandler);
            handler = new EventHandler((sender, args) =>
            {
                outsideRelay.Remove(handler);
            });
            outsideRelay.Add(handler);
            owner.RaiseOutside();
            owner.RaiseOutside();
            Console.WriteLine("测试目标：{0} 移除事件处理", nameof(IWeakEventRelay));
            Console.WriteLine("测试内容：在触发的事件处理函数中注销事件处理");
        }
    }

    partial class WeakEventRelayTests
    {
            
        private void Register(INotifyPropertyChanged eventSource,EventSubscriber subscriber)
        {
            eventSource.PropertyChanged += subscriber.PropertyChangedHandler;
        }
        
        private async Task GcSubscriber(EventOwner owner)
        {
            await Task.Run(() =>
            {
                var count = 30;
                var eventName = nameof(EventOwner.Outside);
                var relay = owner.RegisterWeakEvent(eventName);
                relay.RegisterRaise(new WeakEventRegister());
                var subscribers = new List<EventSubscriber>();
                for (int i = 0; i < count; i++)
                {
                    var subscriber = new EventSubscriber();
                    subscribers.Add(subscriber);
                }

                foreach (var subscriber in subscribers)
                {
                    relay.Add(new EventHandler(subscriber.Handler));
                }
            });
            GC.Collect(0);
            owner.RaiseOutside();
        }
        
        class EventOwner : INotifyPropertyChanged
        {
            private readonly IWeakEventRelay _relayInside;
            private readonly IWeakEventRelay _relayPropertyChanged;

            private string _name;

            /// <summary>
            /// 使用 <see cref="WeakEventRelay"/> 创建的事件
            /// </summary>
            public event EventHandler Inside
            {
                add => _relayInside.Add(value);
                remove => _relayInside.Remove(value);
            }

            /// <summary>
            /// 常规方式创建的事件
            /// </summary>
            public event EventHandler Outside;

            public string Name
            {
                get => _name;
                set
                {
                    _name = value;
                    _relayPropertyChanged.Raise(this, new PropertyChangedEventArgs(nameof(Name)));
                }
            }

            public EventOwner()
            {
                EventSubscriber.HandlerCount = 0;
                _relayInside = this.RegisterWeakEvent(nameof(Inside));
                _relayPropertyChanged = GetRelayPropertyChanged();
            }

            private IWeakEventRelay GetRelayPropertyChanged()
            {
                // 隐式接口实现的事件名称 = 接口类型全称 + 事件名称
                var interfaceName = typeof(INotifyPropertyChanged).FullName;
                var eventName = nameof(INotifyPropertyChanged.PropertyChanged);
                var name = $"{interfaceName}.{eventName}";
                return this.RegisterWeakEvent(name);
            }

            public void RaiseInside()
            {
                _relayInside.Raise(this, EventArgs.Empty);
            }

            public void RaiseOutside()
            {
                Outside?.Invoke(this, EventArgs.Empty);
            }

            event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
            {
                add => _relayPropertyChanged.Add(value);
                remove => _relayPropertyChanged.Remove(value);
            }
        }

        class EventSubscriber
        {
            public static int HandlerCount { get; internal set; }
            
            public void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
            {
                Console.WriteLine($"{sender}.{e.PropertyName} is Changed" );
                HandlerCount++;
            }

            public void Handler(object sender, EventArgs e)
            {
                HandlerCount++;
            }

            public static void StaticHandler(object sender, EventArgs e)
            {
                HandlerCount++;
            }
        }
    }
}