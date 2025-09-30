using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Datamigratie.Common.Extensions
{
    public static class LoggerExtensions
    {
        /// <summary>
        /// Logs an informational message with sanitized arguments to prevent log injection.
        /// </summary>
        /// <param name="logger">The logger to write to.</param>
        /// <param name="message">Format string of the log message using message templates.</param>
        /// <param name="args">Arguments to format into the message, which will be sanitized if they are strings.</param>
        public static void LogInformationSanitized(this ILogger logger, string? message, params object?[] args)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (message == null) throw new ArgumentNullException(nameof(message));

            var sanitizedArgs = args?.Select(arg => Sanitize(arg)).ToArray() ?? Array.Empty<object>();

            logger.Log(LogLevel.Information, message, sanitizedArgs);
        }

        /// <summary>
        /// Sanitizes an object by removing line breaks if it's a string.
        /// </summary>
        /// <param name="arg">The object to sanitize.</param>
        /// <returns>The sanitized object.</returns>
        private static object? Sanitize(object? arg)
        {
            if (arg is string s)
            {
                // Remove all ASCII control characters (0x00-0x1F, 0x7F) from string
                return Regex.Replace(s, @"[\p{Cc}]", "");
            }

            return arg;
        }
    }
}
