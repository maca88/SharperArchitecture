using System;
using System.Configuration;
using log4net;
using log4net.Core;
using log4net.Util;
using ILogger = SharperArchitecture.Common.Specifications.ILogger;

namespace SharperArchitecture.Common.Adapters
{
    public class Log4NetAdapter<T> : ILogger
    {
        /// <summary>
        /// The logger used by this instance.
        /// </summary>
        private readonly log4net.Core.ILogger _log4NetLogger;

        public Log4NetAdapter()
        {
            _log4NetLogger = LogManager.GetLogger(typeof(T)).Logger;
        }

        /// <summary>
        /// Logs the specified message with Debug severity.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(string message)
        {
            Log(Level.Debug, message, null);
        }

        /// <summary>
        /// Logs the specified message with Debug severity.
        /// </summary>
        /// <param name="format">The message or format template.</param>
        /// <param name="args">Any arguments required for the format template.</param>
        public void Debug(string format, params object[] args)
        {
            Log(Level.Debug, format, null, args);
        }

        /// <summary>
        /// Logs the specified exception with Debug severity.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="format">The message or format template.</param>
        /// <param name="args">Any arguments required for the format template.</param>
        public void Debug(Exception exception, string format, params object[] args)
        {
            Log(Level.Debug, format, exception, args);
        }

        /// <summary>
        /// Logs the specified exception with Debug severity.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception to log.</param>
        public void DebugException(string message, Exception exception)
        {
            Log(Level.Debug, message, exception);
        }

        /// <summary>
        /// Logs the specified message with Info severity.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(string message)
        {
            Log(Level.Info, message, null);
        }

        /// <summary>
        /// Logs the specified message with Info severity.
        /// </summary>
        /// <param name="format">The message or format template.</param>
        /// <param name="args">Any arguments required for the format template.</param>
        public void Info(string format, params object[] args)
        {
            Log(Level.Info, format, null, args);
        }

        /// <summary>
        /// Logs the specified exception with Info severity.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="format">The message or format template.</param>
        /// <param name="args">Any arguments required for the format template.</param>
        public void Info(Exception exception, string format, params object[] args)
        {
            Log(Level.Info, format, exception, args);
        }

        /// <summary>
        /// Logs the specified exception with Info severity.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception to log.</param>
        public void InfoException(string message, Exception exception)
        {
            Log(Level.Info, message, exception);
        }

        /// <summary>
        /// Logs the specified message with Trace severity.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Trace(string message)
        {
            Log(Level.Trace, message, null);
        }

        /// <summary>
        /// Logs the specified message with Trace severity.
        /// </summary>
        /// <param name="format">The message or format template.</param>
        /// <param name="args">Any arguments required for the format template.</param>
        public void Trace(string format, params object[] args)
        {
            Log(Level.Trace, format, null, args);
        }

        /// <summary>
        /// Logs the specified exception with Trace severity.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="format">The message or format template.</param>
        /// <param name="args">Any arguments required for the format template.</param>
        public void Trace(Exception exception, string format, params object[] args)
        {
            Log(Level.Trace, format, exception, args);
        }

        /// <summary>
        /// Logs the specified exception with Trace severity.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception to log.</param>
        public void TraceException(string message, Exception exception)
        {
            Log(Level.Trace, message, exception);
        }

        /// <summary>
        /// Logs the specified message with Warn severity.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warn(string message)
        {
            Log(Level.Warn, message, null);
        }

        /// <summary>
        /// Logs the specified message with Warn severity.
        /// </summary>
        /// <param name="format">The message or format template.</param>
        /// <param name="args">Any arguments required for the format template.</param>
        public void Warn(string format, params object[] args)
        {
            Log(Level.Warn, format, null, args);
        }

        /// <summary>
        /// Logs the specified exception with Warn severity.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="format">The message or format template.</param>
        /// <param name="args">Any arguments required for the format template.</param>
        public void Warn(Exception exception, string format, params object[] args)
        {
            Log(Level.Warn, format, exception, args);
        }

        /// <summary>
        /// Logs the specified message with Warn severity.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception to log.</param>
        public void WarnException(string message, Exception exception)
        {
            Log(Level.Warn, message, exception);
        }

        /// <summary>
        /// Logs the specified message with Error severity.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(string message)
        {
            Log(Level.Error, message, null);
        }

        /// <summary>
        /// Logs the specified message with Error severity.
        /// </summary>
        /// <param name="format">The message or format template.</param>
        /// <param name="args">Any arguments required for the format template.</param>
        public void Error(string format, params object[] args)
        {
            Log(Level.Error, format, null, args);
        }

        /// <summary>
        /// Logs the specified exception with Error severity.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="format">The message or format template.</param>
        /// <param name="args">Any arguments required for the format template.</param>
        public void Error(Exception exception, string format, params object[] args)
        {
            Log(Level.Error, format, exception, args);
        }

        /// <summary>
        /// Logs the specified exception with Error severity.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception to log.</param>
        public void ErrorException(string message, Exception exception)
        {
            Log(Level.Error, message, exception);
        }

        /// <summary>
        /// Logs the specified message with Fatal severity.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Fatal(string message)
        {
            Log(Level.Fatal, message, null);
        }

        /// <summary>
        /// Logs the specified message with Fatal severity.
        /// </summary>
        /// <param name="format">The message or format template.</param>
        /// <param name="args">Any arguments required for the format template.</param>
        public void Fatal(string format, params object[] args)
        {
            Log(Level.Fatal, format, null, args);
        }

        /// <summary>
        /// Logs the specified exception with Fatal severity.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="format">The message or format template.</param>
        /// <param name="args">Any arguments required for the format template.</param>
        public void Fatal(Exception exception, string format, params object[] args)
        {
            Log(Level.Fatal, format, exception, args);
        }

        /// <summary>
        /// Logs the specified exception with Fatal severity.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception to log.</param>
        public void FatalException(string message, Exception exception)
        {
            Log(Level.Fatal, message, exception);
        }

        /// <summary>
        /// Calls the actual log4netlogger using the preferred wrapped method.
        /// </summary>
        /// <param name="level">The level to log at.</param>
        /// <param name="format">The message or format template.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="args">Any arguments required for the format template.</param>
        private void Log(Level level, string format, Exception exception, params object[] args)
        {
            if (_log4NetLogger.IsEnabledFor(level))
            {
                if (args != null && args.Length > 0)
                {
                    _log4NetLogger.Log(typeof(T), level, string.Format(format, args), exception);
                }
                else
                {
                    _log4NetLogger.Log(typeof(T), level, format, exception);
                }
            }
        }
    }
}
