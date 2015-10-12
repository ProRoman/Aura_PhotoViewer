using log4net;
using Prism.Logging;

namespace AuraPhotoViewer
{
    class Log4NetPrismFacade : ILoggerFacade
    {
        #region Log4net

        private static readonly ILog Log4Net = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region ILoggerFacade implementation
        
        public void Log(string message, Category category, Priority priority)
        {
            switch (category)
            {
                case Category.Debug:
                    Log4Net.Debug(message);
                    break;
                case Category.Warn:
                    Log4Net.Warn(message);
                    break;
                case Category.Exception:
                    Log4Net.Error(message);
                    break;
                case Category.Info:
                    Log4Net.Info(message);
                    break;
            }
        }

        #endregion
        
    }
}
