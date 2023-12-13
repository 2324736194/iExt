namespace System.Events
{
    /// <summary>
    /// 调用弱事件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="owner"></param>
    /// <param name="relay"></param>
    public delegate void RaiseWeakEvent<in T>(T owner, IWeakEventRelay relay);
}