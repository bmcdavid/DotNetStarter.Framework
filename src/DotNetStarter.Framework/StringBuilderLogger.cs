using DotNetStarter.Abstractions;
using DotNetStarter.Framework.Abstractions;
using System;
using System.Text;

namespace DotNetStarter.Framework
{
    /// <summary>
    /// ILogger using a string builder, this implementation should be swapped in production environments
    /// </summary>
    [Register(typeof(ILogger), LifeTime.Singleton)]
    public class StringBuilderLogger : ILogger
    {
        private StringBuilder logs = new StringBuilder(200);

        /// <summary>
        /// Logs string message and exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        /// <param name="source"></param>
        /// <param name="level"></param>
        public virtual void Log(string message, Exception e, Type source, ErrorLevel level = ErrorLevel.Error)
        {
            CommonLogger(message, e, source, level);
        }

        /// <summary>
        /// Logs exception
        /// </summary>
        /// <param name="e"></param>
        /// <param name="source"></param>
        /// <param name="level"></param>
        public virtual void LogException(Exception e, Type source, ErrorLevel level = ErrorLevel.Error)
        {
            CommonLogger(string.Empty, e, source, level);
        }

        /// <summary>
        /// Logs string message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="source"></param>
        /// <param name="level"></param>
        public virtual void LogMessage(string message, Type source, ErrorLevel level = ErrorLevel.Error)
        {
            CommonLogger(message, null, source, level);
        }

        /// <summary>
        /// Gets stringbuilder contents
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return logs.ToString();
        }

        /// <summary>
        /// Shows log information for web
        /// </summary>
        /// <returns></returns>
        public virtual string ToWebString() => ToString().Replace(Environment.NewLine, "<br />");

        /// <summary>
        /// Common logger
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="source"></param>
        /// <param name="level"></param>
        protected virtual void CommonLogger(string message, Exception exception, Type source, ErrorLevel level)
        {
            if (IsLoggable(level))
            {
                logs.AppendLine(string.Format("{0}: {1} {2} at {3}", level.ToString(), message ?? "", source?.FullName ?? "", DateTime.Now.ToString()));

                if (exception != null)
                {
                    logs.AppendLine(string.Format("Exception Details: {0}", exception.ToString()));
                }

                logs.AppendLine("###########" + Environment.NewLine);
            }
        }

        /// <summary>
        /// Determines if item is loggable
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        protected virtual bool IsLoggable(ErrorLevel level) => true;
    }
}