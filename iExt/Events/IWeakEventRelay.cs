using System.Reflection;

namespace System.Events
{
    /// <summary>
    /// <para>弱事件中继</para>
    /// <para>弱事件中继都拥有唯一的事件和事件拥有者</para>
    /// <para>同一个事件拥有者，可以对应多个事件</para>
    /// <para>同一个事件，也可对应多个事件拥有者</para>
    /// </summary>
    public interface IWeakEventRelay
    {
        /// <summary>
        /// 引发事件
        /// </summary>
        /// <param name="parameters">事件参数</param>
        void Raise(params object[] parameters);

        /// <summary>
        /// 新增事件处理
        /// </summary>
        /// <param name="handler"></param>
        void Add(Delegate handler);

        /// <summary>
        /// 移除事件处理
        /// </summary>
        /// <param name="handler"></param>
        void Remove(Delegate handler);

        /// <summary>
        /// 清理事件处理
        /// </summary>
        void Clear();

        /// <summary>
        /// 注册弱事件的事件调用
        /// </summary>
        /// <param name="register"></param>
        void RegisterRaise(IWeakEventRegister register);
            
        /// <summary>
        /// 比较弱事件的监听事件
        /// </summary>
        /// <param name="eventInfo"></param>
        /// <returns></returns>
        bool Equals(EventInfo eventInfo);
    }
}