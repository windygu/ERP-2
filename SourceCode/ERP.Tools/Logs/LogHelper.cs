using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Tools.Logs
{
    public static class LogHelper
    {
        private static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger("loginfo");
        private static readonly log4net.ILog logerror = log4net.LogManager.GetLogger("logerror");
        private static readonly log4net.ILog logemail = log4net.LogManager.GetLogger("logemail");
        private static readonly log4net.ILog logapproval = log4net.LogManager.GetLogger("logapproval");

        public static void WriteLog(string info)
        {
            if (loginfo.IsInfoEnabled)
            {
                loginfo.Info(info);
            }
        }

        public static void WriteError(string info, Exception ex)
        {
            if (logerror.IsErrorEnabled)
            {
                logerror.Error(info, ex);
            }
        }

        public static void WriteError(Exception ex)
        {
            if (logerror.IsErrorEnabled)
            {
                logerror.Error(ex.Message, ex);
            }
        }

        public static void WriteEmail(string info)
        {
            if (logemail.IsInfoEnabled)
            {
                logemail.Info(info);
            }
        }

        public static void WriteApproval(string info)
        {
            if (logapproval.IsInfoEnabled)
            {
                logapproval.Info(info);
            }
        }
    }
}