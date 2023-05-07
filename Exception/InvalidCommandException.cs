using System;

namespace VCUE
{
    /// <summary>
    /// Thrown when a debug console tries to load non-static commands, or when there are multiple command definitions with the same name.
    /// </summary>
    public class InvalidCommandException : Exception
    {
        public InvalidCommandException(string message) : base(message)
        {
        }
    }
}