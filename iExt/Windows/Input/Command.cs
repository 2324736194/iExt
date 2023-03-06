using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

namespace System.Windows.Input
{
    /// <summary>
    /// 无参数命令
    /// </summary>
    public class Command : CommandBase
    {
        /// <summary>
        /// 创建无参的同步线程命令
        /// </summary>
        /// <param name="executeDelegate">[同步线程] 执行委托</param>
        /// <param name="once">是否仅执行一次，默认可重复执行</param>
        public Command(Action executeDelegate, bool once = false)
            : this(executeDelegate, () => true, once)
        {


        }

        /// <summary>
        /// 创建无参的同步线程命令
        /// </summary>
        /// <param name="executeDelegate">[同步线程] 同步线程执行委托</param>
        /// <param name="canExecuteDelegate">[同步线程] 能否执行判断</param>
        /// <param name="once">是否仅执行一次，默认可重复执行</param>
        public Command(Action executeDelegate, Func<bool> canExecuteDelegate, bool once = false)
            : base(executeDelegate, canExecuteDelegate, once)
        {

        }

        /// <summary>
        /// 创建无参的异步线程命令
        /// </summary>
        /// <param name="executeDelegate">[异步线程] 执行委托</param>
        /// <param name="once">是否仅执行一次，默认可重复执行</param>
        public Command(Func<Task> executeDelegate, bool once = false)
            : this(executeDelegate, () => true, once)
        {

        }

        /// <summary>
        /// 创建无参的异步线程命令
        /// </summary>
        /// <param name="executeDelegate">[异步线程] 执行委托</param>
        /// <param name="canExecuteDelegate">[同步线程] 能否执行判断</param>
        /// <param name="once">是否仅执行一次，默认可重复执行</param>
        public Command(Func<Task> executeDelegate, Func<bool> canExecuteDelegate, bool once = false)
            : base(executeDelegate, canExecuteDelegate, once)
        {

        }
    }
}