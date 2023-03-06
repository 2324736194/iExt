using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Input
{
    /// <summary>
    /// 针对 <see cref="ICommand"/> 的扩展
    /// </summary>
    public static class CommandExt
    {   
        /// <summary>
        /// 触发 <see cref="ICommand.CanExecuteChanged"/> 事件
        /// </summary>
        /// <param name="interface"></param>
        public static void RaiseCanExecuteChanged(this IRaiseCanExecuteChanged @interface)
        {
            @interface.RaiseCanExecuteChanged();
        }
    }
}
