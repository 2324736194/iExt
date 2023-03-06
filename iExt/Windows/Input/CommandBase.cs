using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace System.Windows.Input
{
    /// <summary>
    /// <see cref="ICommand"/> 接口实现基类
    /// </summary>
    public  abstract partial class CommandBase : ICommand
    {
        private int count;
        private readonly Delegate executeDelegate;
        private readonly Delegate canExecuteDelegate;
        private bool executing;
        
        /// <summary>
        /// 是否仅执行一次
        /// </summary>
        public bool Once { get; }

        /// <summary>
        /// 创建一个委托命令
        /// </summary>
        /// <param name="executeDelegate">执行委托</param>
        /// <param name="canExecuteDelegate">能否执行判断</param>
        /// <param name="once">是否仅执行一次，默认可重复执行</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected CommandBase(Delegate executeDelegate, Delegate canExecuteDelegate, bool once)
        {   
            Once = once;
            this.executeDelegate = executeDelegate ?? throw new ArgumentNullException(nameof(executeDelegate));
            this.canExecuteDelegate = canExecuteDelegate ?? throw new ArgumentNullException(nameof(canExecuteDelegate));
        }

        /// <inheritdoc />
        public virtual bool CanExecute(object parameter)
        {
            if (executing)
            {
                return false;
            }

            if (Once && count > 0)
            {
                return false;
            }

            if (null != canExecuteDelegate)
            {
                var has = canExecuteDelegate.Method.GetParameters().Any();
                var result = has? canExecuteDelegate.DynamicInvoke(parameter):
                    canExecuteDelegate.DynamicInvoke();
                if (result is bool can)
                {
                    return can;
                }
                throw new NotImplementedException($"{nameof(canExecuteDelegate)} 的返回值必须是 {nameof(Boolean)} 类型");
            }
            return true;
        }

        /// <inheritdoc />
        public virtual async void Execute(object parameter)
        {
            try
            {
                count++;
                executing = true;
                this.RaiseCanExecuteChanged();
                var has = canExecuteDelegate.Method.GetParameters().Any();
                var result = has ? executeDelegate.DynamicInvoke(parameter) :
                    executeDelegate.DynamicInvoke();
                if (result is Task task)
                {
                    await task;
                }
            }
            finally
            {
                executing = false;
                this.RaiseCanExecuteChanged();
            }
        }
    }

    public partial class CommandBase : IRaiseCanExecuteChanged
    {
        /// <inheritdoc cref="ICommand.CanExecuteChanged" />
        public event EventHandler CanExecuteChanged;

        void IRaiseCanExecuteChanged.RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}