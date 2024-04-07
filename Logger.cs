using HkmpPouch.Networking;
using System;

namespace HkmpPouch
{
    internal interface ILogger {
        internal void Error(string str);
        internal void Info(string str);
        internal void Debug(string str);
    }

    /// <summary>
    /// The logger class
    /// </summary>
    public class Logger 
    {
        private readonly string Name;
        private readonly ILogger _logger;

        /// <summary>
        /// StupidError will try to log an error, it will not print what addon tried to print the error because it does not know.
        /// </summary>
        /// <param name="text">The error to log</param>
        [Obsolete("Ideally you shouldn't need to use this. Use your own instance of the logger from your pipe instead.")]
        public static void StupidError(string text)
        {
            if (Server.Instance != null)
            {
                Server.Instance.Error(text);
            } else if (Client.Instance != null)
            {
                Client.Instance.Error(text);
            } else
            {
                // we cannot log it, but ideally this will never happen because HKMP would initialise atleast one of these.
            }
        }


        internal Logger(string Name,ILogger L) {
            this.Name = Name;
            _logger = L;
        }

        /// <summary>
        /// Use for Debugging logs
        /// </summary>
        /// <param name="str">string to log</param>
        public void Debug(string str)
        {
            _logger.Debug($"[{Name}]:[{str}]");
        }

        /// <summary>
        /// Use for Logging Erros
        /// </summary>
        /// <param name="str">string to log</param>
        public void Error(string str)
        {
            _logger.Error($"[{Name}]:[{str}]");
        }

        /// <summary>
        /// Use for Logging non fatal stuff
        /// </summary>
        /// <param name="str">string to log</param>
        public void Info(string str)
        {
            _logger.Info($"[{Name}]:[{str}]");
        }
    }
}
