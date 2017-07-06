using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Index;
using ERP.Models.Index;
using ERP.WebNew.Attributes;
using ERP.WebNew.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.WebNew.Controllers
{
    public class HomeController : Controller
    {
        private IndexService _indexService = new IndexService();

        [UserTrackerLog]
        [Authorize]
        public ActionResult Index()
        {
            //string menuHomeNavPath = String.Format("<li><i class='fa fa-home'></i><a href='/Default.aspx'>首页</a><i class='fa fa-angle-right'></i></li><li> 欢迎 <b>{0}</b> 使用运营管理系统", CurrentUserServices.Me.UserName, CurrentUserServices.Me.UserID);
            //ViewBag.BreadcrumbNav = menuHomeNavPath;

            return View();
        }

        public ActionResult GetDashboardStat() 
        {
            List<VMDashboardStat> statList = _indexService.GetAllDashboardStat(CurrentUserServices.Me);

            return new CustomJsonResult(new { StatList=statList});
        }
    }
}