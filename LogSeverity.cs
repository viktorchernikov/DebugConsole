namespace VCUE
{
    /// <summary>
    /// Represents the severity levels of log messages in a debug console.
    /// </summary>
    public enum LogSeverity : byte
    {
        /// <summary>
        /// Used to provide detailed information for developers
        /// </summary>
        Debug,
        /// <summary>
        /// Used to provide general information about the state and behavior of a game. 
        /// </summary>
        Info,
        /// <summary>
        /// Used to indicate that a potential problem or issue has been detected, but it may not necessarily indicate an error. 
        /// </summary>
        Warning,
        /// <summary>
        /// Used to indicate that an error has occurred in the game, but the game is still able to continue running.
        /// </summary>
        Error,
        /// <summary>
        /// Used to indicate that a critical error has occurred in the game, preventing it from continuing its work.
        /// </summary>
        Critical
    }
}
