using log4net;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AuraPhotoViewer
{
    class Log4NetFacade : ILoggerFacade
    {
        #region Log4net

        private static readonly ILog Log4Net = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region ILoggerFacade implementation

        /// <summary>
        /// Write a log message.
        /// </summary>
        /// <param name="message">The message to write</param>
        /// <param name="category">The message category</param>
        /// <param name="priority">Not used by Log4Net; pass Priority.None.</param>
        public void Log(string message, Category category, Priority priority)
        {
            StackFrame previousStackFrame = new StackFrame(1, true);
            string clazz = previousStackFrame.GetMethod().ReflectedType.Name;
            string method = previousStackFrame.GetMethod().Name;
            string line = previousStackFrame.GetFileLineNumber().ToString();
            message = String.Format("{0}.{1} {2} - {3}", clazz, method, line, message); 
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
