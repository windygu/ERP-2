using ERP.Tools.Logs;
using ERP.WebNew.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ERP.WebNew.Attributes
{
    public class UserTrackerLogAttribute : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (CurrentUserServices.Me != null)
            {
                var actionDescriptor = filterContext.ActionDescriptor;
                string controllerName = actionDescriptor.ControllerDescriptor.ControllerName;
                string actionName = actionDescriptor.ActionName;
                // 不需要记录日志的情况
                if (controllerName == "Account" && (actionName == "Logs" || actionName == "GetLogs"))
                {

                }
                else
                {
                    DateTime timeStamp = filterContext.HttpContext.Timestamp;
                    string routeId = string.Empty;
                    if (filterContext.RouteData.Values["id"] != null)
                    {
                        routeId = filterContext.RouteData.Values["id"].ToString();
                    }
                    StringBuilder message = new StringBuilder();
                    message.AppendFormat("UserName={0}|UserID={1}|", CurrentUserServices.Me.UserName, CurrentUserServices.Me.UserID);
                    message.AppendFormat("URL={0}|UrlReferer={1}|", HttpContext.Current.Request.Url.ToString(), HttpContext.Current.Request.UrlReferrer == null ? string.Empty : HttpContext.Current.Request.UrlReferrer.ToString());
                    message.AppendFormat("Agent={0}|", HttpContext.Current.Request.UserAgent);
                    message.AppendFormat("TimeStamp={0}|", timeStamp);
                    LogHelper.WriteLog(message.ToString());
                }
            }
            base.OnActionExecuted(filterContext);
        }
    }
}