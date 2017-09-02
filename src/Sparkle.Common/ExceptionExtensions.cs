
namespace Sparkle.Common
{
    using System;
    using System.Collections.Generic;

    public static class ExceptionExtensions
    {
        /// <summary>
        /// Get a string smaller than exception.ToString() but bigger than exception.Message.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string ToSummarizedString(this Exception exception)
        {
            if (exception == null)
                return null;

            return exception.GetType().Name + ": " + exception.Message + Environment.NewLine + exception.GetCleanStackTrace();
        }

        /// <summary>
        /// Returns a stck trace that omit system lines.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string GetCleanStackTrace(this Exception exception)
        {
            if (exception == null || exception.StackTrace == null)
                return null;

            string stack = string.Empty, s = string.Empty;
            bool wasOk = true;
            foreach (var line in stack.Split(new char[] { '\r', '\n' }))
            {
                if (line.Contains("System.") || line.Contains("Microsoft."))
                {
                    if (wasOk)
                        stack += s + line;
                    else
                        stack += s + "  ...";
                    wasOk = false;
                }
                else
                {
                    stack += s + line;
                }
                s = Environment.NewLine;
            }

            return stack;
        }
    }
}
