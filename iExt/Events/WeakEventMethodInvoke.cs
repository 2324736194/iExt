using System.Reflection;

namespace System.Events
{
    internal class WeakEventMethodInvoke
    {
        public MethodInfo Method { get;  }
            
        public int InvokeCount { get; set; }
        
        public WeakEventMethodInvoke(MethodInfo method)
        {
            Method = method;
        }

    }
}