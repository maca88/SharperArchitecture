using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.Common.Specifications
{
    public interface ILogger
    {
        /// <summary>Logs the specified message with Debug severity.</summary>
        /// <param name="message">The message.</param>
        void Debug(string message);

        /// <summary>Logs the specified message with Debug severity.</summary>
        /// <param name="format">The message or format template.</param>
        /// <param name="args">Any arguments required for the format template.</param>
        void Debug(string format, params object[] args);

        /// <summary>Logs the specified exception with Debug severity.</summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="format">The message or format template.</param>
        /// <param name="args">Any arguments required for the format template.</param>
        void Debug(Exception exception, string format, params object[] args);

        /// <summary>Logs the specified exception with Debug severity.</summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception to log.</param>
        void DebugException(string message, Exception exception);

        /// <summary>Logs the specified message with Info severity.</summary>
        /// <param name="message">The message.</param>
        void Info(string message);

        /// <summary>Logs the specified message with Info severity.</summary>
        /// <param name="format">The message or format template.</param>
        /// <param name="args">Any arguments required for the format template.</param>
        void Info(string format, params object[] args);

        /// <summary>Logs the specified exception with Info severity.</summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="format">The message or format template.</param>
        /// <param name="args">Any arguments required for the format template.</param>
        void Info(Exception exception, string format, params object[] args);

        /// <summary>Logs the specified exception with Info severity.</summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception to log.</param>
        void InfoException(string message, Exception exception);

        /// <summary>Logs the specified message with Trace severity.</summary>
        /// <param name="message">The message.</param>
        void Trace(string message);

        /// <summary>Logs the specified message with Trace severity.</summary>
        /// <param name="format">The message or format template.</param>
        /// <param name="args">Any arguments required for the format template.</param>
        void Trace(string format, params object[] args);

        /// <summary>Logs the specified exception with Trace severity.</summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="format">The message or format template.</param>
        /// <param name="args">Any arguments required for the format template.</param>
        void Trace(Exception exception, string format, params object[] args);

        /// <summary>Logs the specified exception with Trace severity.</summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception to log.</param>
        void TraceException(string message, Exception exception);

        /// <summary>Logs the specified message with Warn severity.</summary>
        /// <param name="message">The message.</param>
        void Warn(string message);

        /// <summary>Logs the specified message with Warn severity.</summary>
        /// <param name="format">The message or format template.</param>
        /// <param name="args">Any arguments required for the format template.</param>
        void Warn(string format, params object[] args);

        /// <summary>Logs the specified exception with Warn severity.</summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="format">The message or format template.</param>
        /// <param name="args">Any arguments required for the format template.</param>
        void Warn(Exception exception, string format, params object[] args);

        /// <summary>Logs the specified message with Warn severity.</summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception to log.</param>
        void WarnException(string message, Exception exception);

        /// <summary>Logs the specified message with Error severity.</summary>
        /// <param name="message">The message.</param>
        void Error(string message);

        /// <summary>Logs the specified message with Error severity.</summary>
        /// <param name="format">The message or format template.</param>
        /// <param name="args">Any arguments required for the format template.</param>
        void Error(string format, params object[] args);

        /// <summary>Logs the specified exception with Error severity.</summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="format">The message or format template.</param>
        /// <param name="args">Any arguments required for the format template.</param>
        void Error(Exception exception, string format, params object[] args);

        /// <summary>Logs the specified exception with Error severity.</summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception to log.</param>
        void ErrorException(string message, Exception exception);

        /// <summary>Logs the specified message with Fatal severity.</summary>
        /// <param name="message">The message.</param>
        void Fatal(string message);

        /// <summary>Logs the specified message with Fatal severity.</summary>
        /// <param name="format">The message or format template.</param>
        /// <param name="args">Any arguments required for the format template.</param>
        void Fatal(string format, params object[] args);

        /// <summary>Logs the specified exception with Fatal severity.</summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="format">The message or format template.</param>
        /// <param name="args">Any arguments required for the format template.</param>
        void Fatal(Exception exception, string format, params object[] args);

        /// <summary>Logs the specified exception with Fatal severity.</summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception to log.</param>
        void FatalException(string message, Exception exception);
    }
}
