using System.Reflection;

namespace System.Events
{
    /// <summary>
    /// <para>关联事件和弱事件</para>
    /// <para>当事件被触发时，调用弱事件中继已添加的委托</para>
    /// </summary>
    public interface IWeakEventRegister
    {
        /// <summary>
        /// 获取弱事件注册事件的函数
        /// </summary>
        /// <param name="relay"></param>
        /// <returns></returns>
        MethodInfo GetRegisterMethod(IWeakEventRelay relay);
    }
}