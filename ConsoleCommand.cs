using System;

namespace VCUE
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ConsoleCommand : Attribute
    {
        public string Name;
        public string Description;

        public ConsoleCommand(string Name)
        {
            this.Name = Name;
        }
    }
}