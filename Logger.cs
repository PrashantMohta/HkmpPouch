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
