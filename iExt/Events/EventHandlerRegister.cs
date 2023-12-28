using System.Reflection;

namespace System.Events
{
    /// <summary>
    /// 注册委托类型为 <see cref="EventHandler"/> 的事件
    /// </summary>
    public class EventHandlerRegister : IWeakEventRegister
    {
        private WeakReference<IWeakEventRelay> _relay;

        /// <inheritdoc />
        public MethodInfo GetRegisterMethod(IWeakEventRelay relay)
        {
            if (null == relay)
            {
                throw new ArgumentNullException(nameof(relay));
            }

            var bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
            var method = GetType().GetMethod(nameof(Rasie), bindingAttr);
            _relay = new WeakReference<IWeakEventRelay>(relay);
            return method;
        }

        private void Rasie(object sender, EventArgs e)
        {
            if (_relay.TryGetTarget(out var target))
            {
                target.Raise(sender, e);
            }
        }
    }
}