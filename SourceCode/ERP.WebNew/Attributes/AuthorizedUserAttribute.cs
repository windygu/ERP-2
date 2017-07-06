using ERP.BLL.ERP.AdminUser;
using ERP.WebNew.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ERP.WebNew.Attributes
{
    public class AuthorizedUserAttribute : AuthorizeAttribute
    {
        public int PageID { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (CurrentUserServices.Me == null)
            {
                return false;
            }

            if (CurrentUserServices.Me != null && CurrentUserServices.Me.IsSuperAdmin)
            {
                return true;
            }
            var userName = httpContext.User.Identity.Name;
            UserServices userServices = new UserServices();
            bool hasPermission = userServices.HasPagePermission(CurrentUserServices.Me.UserID, PageID);
            if (hasPermission)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new
                            {
                                controller = "Account",
                                action = "LogOn"
                            })
                        );
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }
    }
}