using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Core;

namespace RemotePlusLibrary.Core
{
    public class LogWrapper : ILog
    {
        public ILog Logger { get; private set; }
        public int ErrorCount { get; private set; }
        public int WarningCount { get; private set; }

        public bool IsDebugEnabled => Logger.IsDebugEnabled;

        public bool IsInfoEnabled => Logger.IsInfoEnabled;

        public bool IsWarnEnabled => Logger.IsWarnEnabled;

        public bool IsErrorEnabled => Logger.IsErrorEnabled;

        public bool IsFatalEnabled => Logger.IsFatalEnabled;

        ILogger ILoggerWrapper.Logger => Logger.Logger;

        public LogWrapper(string Name)
        {
            Logger = LogManager.GetLogger(Name);
        }

        public void Debug(object message)
        {
            Logger.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            Logger.Debug(message, exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            Logger.DebugFormat(format, args);
        }

        public void DebugFormat(string format, object arg0)
        {
            Logger.DebugFormat(format, arg0);
        }

        public void DebugFormat(string format, object arg0, object arg1)
        {
            Logger.DebugFormat(format, arg0, arg1);
        }

        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            Logger.DebugFormat(format, arg0, arg1, arg2);
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            Logger.DebugFormat(provider, format, args);
        }

        public void Info(object message)
        {
            Logger.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            Logger.Info(message, exception);
        }

        public void InfoFormat(string format, params object[] args)
        {
            Logger.InfoFormat(format, args);
        }

        public void InfoFormat(string format, object arg0)
        {
            Logger.InfoFormat(format, arg0);
        }

        public void InfoFormat(string format, object arg0, object arg1)
        {
            Logger.InfoFormat(format, arg0, arg1);
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            Logger.InfoFormat(format, arg0, arg1, arg2);
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            Logger.InfoFormat(provider, format, args);
        }

        public void Warn(object message)
        {
            WarningCount++;
            Logger.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            WarningCount++;
            Logger.Warn(message, exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            WarningCount++;
            Logger.WarnFormat(format, args);
        }

        public void WarnFormat(string format, object arg0)
        {
            WarningCount++;
            Logger.WarnFormat(format, arg0);
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
            WarningCount++;
            Logger.WarnFormat(format, arg0, arg1);
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            WarningCount++;
            Logger.WarnFormat(format, arg0, arg1, arg2);
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            WarningCount++;
            Logger.WarnFormat(provider, format, args);
        }

        public void Error(object message)
        {
            ErrorCount++;
            Logger.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            ErrorCount++;
            Logger.Error(message, exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            ErrorCount++;
            Logger.ErrorFormat(format, args);
        }

        public void ErrorFormat(string format, object arg0)
        {
            ErrorCount++;
            Logger.ErrorFormat(format, arg0);
        }

        public void ErrorFormat(string format, object arg0, object arg1)
        {
            ErrorCount++;
            Logger.ErrorFormat(format, arg0, arg1);
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            ErrorCount++;
            Logger.ErrorFormat(format, arg0, arg2, arg2); ;
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            ErrorCount++;
            Logger.ErrorFormat(provider, format, args);
        }

        public void Fatal(object message)
        {
            Logger.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            Logger.Fatal(message, exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            Logger.FatalFormat(format, args);
        }

        public void FatalFormat(string format, object arg0)
        {
            Logger.FatalFormat(format, arg0);
        }

        public void FatalFormat(string format, object arg0, object arg1)
        {
            Logger.FatalFormat(format, arg0, arg1);
        }

        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            Logger.FatalFormat(format, arg0, arg1, arg2);
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            Logger.FatalFormat(provider, format, args);
        }
    }
}