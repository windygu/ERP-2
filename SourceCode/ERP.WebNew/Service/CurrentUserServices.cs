using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.Models.AdminUser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace ERP.WebNew.Service
{
    public class CurrentUserServices
    {
        public static List<DTOAdminUserMenus> Menus
        {
            get
            {
                UserMenuServices userMenuServices = new UserMenuServices();
                List<DTOAdminUserMenus> menus = UserMenuServices.GetUserAllowedMenu(Me);
                return menus;
            }
        }

        public static VMERPUser Me
        {
            get
            {
                FormsIdentity userIdentity = HttpContext.Current.User.Identity as FormsIdentity;
                if (userIdentity != null)
                {
                    FormsAuthenticationTicket ticket = userIdentity.Ticket;
                    return JsonConvert.DeserializeObject<VMERPUser>(ticket.UserData);
                }
                return null;
            }
        }

        public static string GetCurrentRequestIPAddress()
        {
            string result = String.Empty;
            result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(result))
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (string.IsNullOrEmpty(result))
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }
            return result;
        }
    }
}