using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Input
{
    /// <summary>
    /// 泛型参数命令
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Command<T> : CommandBase
    {
        /// <summary>
        /// 创建泛型参数的同步线程命令
        /// </summary>
        /// <param name="executeDelegate">[同步线程] 执行委托</param>
        /// <param name="once">是否仅执行一次，默认可重复执行</param>
        public Command(Action<T> executeDelegate, bool once = false)
            : this(executeDelegate, p => true,once)
        {

        }

        /// <summary>
        /// 创建泛型参数的同步线程命令
        /// </summary>
        /// <param name="executeDelegate">[同步线程] 同步线程执行委托</param>
        /// <param name="canExecuteDelegate">[同步线程] 能否执行判断</param>
        /// <param name="once">是否仅执行一次，默认可重复执行</param>
        public Command(Action<T> executeDelegate, Func<T, bool> canExecuteDelegate, bool once = false)
            : base(executeDelegate, canExecuteDelegate, once)
        {

        }

        /// <summary>
        /// 创建泛型参数的异步线程命令
        /// </summary>
        /// <param name="executeDelegate">[异步线程] 执行委托</param>
        /// <param name="once">是否仅执行一次，默认可重复执行</param>
        public Command(Func<T, Task> executeDelegate, bool once = false)
            : this(executeDelegate, p => true, once)
        {

        }

        /// <summary>
        /// 创建泛型参数的异步线程命令
        /// </summary>
        /// <param name="executeDelegate">[异步线程] 执行委托</param>
        /// <param name="canExecuteDelegate">[异步线程] 能否执行判断</param>
        /// <param name="once">是否仅执行一次，默认可重复执行</param>
        public Command(Func<T, Task> executeDelegate, Func<T, bool> canExecuteDelegate, bool once = false)
            : base(executeDelegate, canExecuteDelegate, once)
        {

        }
    }
}
