using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Tests
{
    [TestClass()]
    public class WeakEventRelayTests
    {
        class EventOwner : INotifyPropertyChanged
        {
            private readonly WeakEventRelay relayInsideDemo;
            private readonly WeakEventRelay relayPropertyChanged;

            public event EventHandler InsideDemo
            {
                add { relayInsideDemo.Add(value); }
                remove { relayInsideDemo.Remove(value); }
            }

            event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
            {
                add => relayPropertyChanged.Add(value);
                remove => relayPropertyChanged.Remove(value);
            }

            public event EventHandler OutsideDemo;

            public EventOwner()
            {
                var events = WeakEventRelay.GetEvents<EventOwner>();
                var eventName =
                    $"{typeof(INotifyPropertyChanged).FullName}.{nameof(INotifyPropertyChanged.PropertyChanged)}";
                var e = events.Single(p => p.Name == eventName);
                relayPropertyChanged = this.RegisterWeakEvent(e);
                eventName = nameof(InsideDemo);
                e = events.Single(p => p.Name == eventName);
                relayInsideDemo = this.RegisterWeakEvent(e);
            }

            public void Raise()
            {
                relayInsideDemo.Raise(this, EventArgs.Empty);
                relayPropertyChanged.Raise(this, new PropertyChangedEventArgs(string.Empty));
            }

            public void OnOutsideDemo()
            {
                OutsideDemo?.Invoke(this, EventArgs.Empty);
            }

        }

        class EventSubscriber
        {
            public static int HandlerCount { get; private set; }

            public void Handler(object sender, EventArgs e)
            {
                HandlerCount++;
            }

            public static void StaticHandler(object sender, EventArgs e)
            {
                HandlerCount++;
            }
        }

        /// <summary>
        /// 测试目标：注册 <see cref="EventOwner.InsideDemo"/> 事件
        /// </summary>
        [TestMethod()]
        public void Test()
        {
            var count = 3;
            var owner = new EventOwner();
            var subscribers = new List<EventSubscriber>();
            for (int i = 0; i < count; i++)
            {
                var sub = new EventSubscriber();
                subscribers.Add(sub);
                owner.InsideDemo += sub.Handler;
            }
            owner.Raise();
            Assert.AreEqual(EventSubscriber.HandlerCount, count);
        }

        /// <summary>
        /// 测试目标：注销事件
        /// </summary>
        [TestMethod()]
        public void Test1()
        {
            var count = 3;
            var owner = new EventOwner();
            var subscribers = new List<EventSubscriber>();
            for (int i = 0; i < count; i++)
            {
                var sub = new EventSubscriber();
                subscribers.Add(sub);
                owner.InsideDemo += sub.Handler;
            }

            foreach (var sub in subscribers)
            {
                owner.InsideDemo -= sub.Handler;
            }
            owner.Raise();
            Assert.AreEqual(EventSubscriber.HandlerCount, 0);
        }

        /// <summary>
        /// 测试目标：事件订阅者可以被 GC 回收
        /// </summary>
        [TestMethod()]
        public void Test2()
        {
            var owner = new EventOwner();
            SubEvent(owner);
            Task.Delay(1000).Wait();
            GC.Collect(0);
            Task.Delay(3000).Wait();
            owner.Raise();
            Assert.AreEqual(EventSubscriber.HandlerCount, 0);
        }

        void SubEvent(EventOwner owner)
        {
            var time = DateTime.Now.AddMinutes(1);
            while (true)
            {
                var sub = new EventSubscriber();
                owner.InsideDemo += sub.Handler;
                if (time < DateTime.Now)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 测试目标：在事件源外面使用事件中继管理
        /// </summary>
        [TestMethod()]
        public void Test3()
        {
            var owner = new EventOwner();
            var eventName = nameof(EventOwner.OutsideDemo);
            var events = WeakEventRelay.GetEvents<EventOwner>();
            var e = events.Single(p => p.Name == eventName);
            var relay = owner.RegisterWeakEvent(e,
                (eOwner, eRelay) =>
                {
                    // 注册 OutsideDemo 的事件处理
                    // 每个事件仅会注册一次，不会重复注册
                    eOwner.OutsideDemo += (sender, args) =>
                    {
                        // 触发弱事件处理
                        eRelay.Raise(sender, args);
                    };
                });
            var count = 3;
            var subscribers = new List<EventSubscriber>();
            for (int i = 0; i < count; i++)
            {
                var sub = new EventSubscriber();
                subscribers.Add(sub);
                relay.Add(new EventHandler(sub.Handler));
            }

            owner.OnOutsideDemo();
            Assert.AreEqual(EventSubscriber.HandlerCount, count);
        }

        /// <summary>
        /// 测试目标：事件源被回收后，对应的弱事件中继是否存在
        /// </summary>
        [TestMethod()]
        public void Test4()
        {
            //Register();
            //GC.Collect();
            //var enumerable = (IEnumerable<KeyValuePair<object, List<WeakEventRelay>>>)WeakEventRelay.relayTable;
            //var pairs = enumerable.ToList();
            //Assert.AreEqual(pairs.Count, 0);
        }

        void Register()
        {
            for (int i = 0; i < 100; i++)
            {

                var owner = new EventOwner();
                var events = WeakEventRelay.GetEvents<EventOwner>();
                var eventName = nameof(EventOwner.OutsideDemo);
                var e = events.Single(p => p.Name == eventName);
                owner.RegisterWeakEvent(e,
                    (eOwner, eRelay) =>
                    {
                        eOwner.OutsideDemo += (sender, args) =>
                        {
                            eRelay.Raise(sender, args);
                        };
                    });
                owner = null;
            }
        }

        /// <summary>
        /// 测试目标：注册 <see cref="INotifyPropertyChanged.PropertyChanged"/> 事件
        /// </summary>
        [TestMethod()]
        public void Test5()
        {
            var count = 3;
            var owner = new EventOwner();
            var source = (INotifyPropertyChanged)owner;
            var subscribers = new List<EventSubscriber>();
            for (int i = 0; i < count; i++)
            {
                var sub = new EventSubscriber();
                subscribers.Add(sub);
                source.PropertyChanged += sub.Handler;
            }
            owner.Raise();
            Assert.AreEqual(EventSubscriber.HandlerCount, count);
        }

        /// <summary>
        /// 测试目标：使用静态事件处理函数注册事件
        /// </summary>
        [TestMethod()]
        public void Test6()
        {
            var count = 3;
            var owner = new EventOwner();
            var source = (INotifyPropertyChanged)owner;
            for (int i = 0; i < count; i++)
            {
                source.PropertyChanged += EventSubscriber.StaticHandler;
            }
            owner.Raise();
            Assert.AreEqual(EventSubscriber.HandlerCount, count);
        }
        
    }
}