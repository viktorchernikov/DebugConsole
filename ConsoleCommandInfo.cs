using System.Reflection;

namespace VCUE
{
    public class ConsoleCommandInfo
    {
        internal ConsoleCommandInfo(string name, MethodInfo method)
        {
            Name = name;
            MethodRef = method;
        }

        public readonly string Name;
        public readonly MethodInfo MethodRef;

        public bool Invoke(params object[] args)
        {
            return DebugConsole.InvokeCommand(this, args);
        }
    }
}
