namespace System.Windows.Input
{
    /// <summary>
    /// 接口：可主动引发 <see cref="CanExecuteChanged"/> 事件
    /// </summary>
    public interface IRaiseCanExecuteChanged 
    {
        /// <summary>
        /// 当出现影响是否应执行的更改时发生
        /// </summary>
        event EventHandler CanExecuteChanged;

        /// <summary>
        /// 引发 <see cref="CanExecuteChanged"/> 事件
        /// </summary>
        void RaiseCanExecuteChanged();
    }
}