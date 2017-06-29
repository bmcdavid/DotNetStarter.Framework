namespace DotNetStarter.Framework.Abstractions
{
    using System;

    /// <summary>
    /// Logger
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs both a message and exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        /// <param name="source"></param>
        /// <param name="level"></param>
        void Log(string message, Exception e, Type source, ErrorLevel level = ErrorLevel.Error);

        /// <summary>
        /// Logs exception
        /// </summary>
        /// <param name="e"></param>
        /// <param name="source"></param>
        /// <param name="level"></param>
        void LogException(Exception e, Type source, ErrorLevel level = ErrorLevel.Error);

        /// <summary>
        /// Logs string message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="source"></param>
        /// <param name="level"></param>
        void LogMessage(string message, Type source, ErrorLevel level = ErrorLevel.Error);

        /// <summary>
        /// Displays string contents for web
        /// </summary>
        /// <returns></returns>
        string ToWebString();
    }
}