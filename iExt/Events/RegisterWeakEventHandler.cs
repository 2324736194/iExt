namespace System.Events
{
    /// <summary>
    /// 注册弱事件委托
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public delegate void RegisterWeakEventHandler<in T>(T owner, WeakEventRelay relay);
}