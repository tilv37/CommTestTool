using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommTestTool.Common
{
    public class LogHelper
    {
        static log4net.ILog  loginfo = log4net.LogManager.GetLogger("App.Log");
        static log4net.ILog logerror = log4net.LogManager.GetLogger("App.Log");
        static log4net.ILog logwarn = log4net.LogManager.GetLogger("App.Log");

        public static void ShowInfo(string msg)
        {
            if (loginfo.IsInfoEnabled)
            {
                loginfo.Info(msg);
            }
        }

        public static void ShowError(Type t, string msg)
        {
            if (logerror.IsErrorEnabled)
            {
                logerror.Error(msg);
            }
        }

        public static void ShowWarn(Type t, string msg)
        {
            if (logwarn.IsWarnEnabled)
            {
                logwarn.Warn(msg);
            }
        }

        public static void WriteLog(Type t, string msg)
        {
            log4net.ILog loginfo = log4net.LogManager.GetLogger(t);
            if (loginfo.IsInfoEnabled)
            {
                loginfo.Info(msg);
            }
        }

    }
}
