using System.Reflection;

namespace System.Events
{
    /// <summary>
    /// 仅针对于委托类型参数与 <see cref="EventHandler"/> 相同的事件
    /// </summary>
    public class WeakEventRegister : IWeakEventRegister
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