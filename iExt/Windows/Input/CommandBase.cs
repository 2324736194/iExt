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
        private int _count;
        private readonly Delegate _executeDelegate;
        private readonly Delegate _canExecuteDelegate;
        private bool _executing;
        
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
            this._executeDelegate = executeDelegate ?? throw new ArgumentNullException(nameof(executeDelegate));
            this._canExecuteDelegate = canExecuteDelegate ?? throw new ArgumentNullException(nameof(canExecuteDelegate));
        }

        /// <inheritdoc />
        public virtual bool CanExecute(object parameter)
        {
            if (_executing)
            {
                return false;
            }

            if (Once && _count > 0)
            {
                return false;
            }

            if (null != _canExecuteDelegate)
            {
                var has = _canExecuteDelegate.Method.GetParameters().Any();
                var result = has? _canExecuteDelegate.DynamicInvoke(parameter):
                    _canExecuteDelegate.DynamicInvoke();
                if (result is bool can)
                {
                    return can;
                }
                throw new NotImplementedException($"{nameof(_canExecuteDelegate)} 的返回值必须是 {nameof(Boolean)} 类型");
            }
            return true;
        }

        /// <inheritdoc />
        public virtual async void Execute(object parameter)
        {
            try
            {
                _count++;
                _executing = true;
                this.RaiseCanExecuteChanged();
                var has = _canExecuteDelegate.Method.GetParameters().Any();
                var result = has ? _executeDelegate.DynamicInvoke(parameter) :
                    _executeDelegate.DynamicInvoke();
                if (result is Task task)
                {
                    await task;
                }
            }
            finally
            {
                _executing = false;
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