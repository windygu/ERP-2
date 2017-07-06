using ERP.BLL.ERP.AdminUser;
using ERP.Models.CustomEnums;
using ERP.Tools;
using ERP.WebNew.Attributes;
using ERP.WebNew.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.WebNew.Controllers
{
    /// <summary>
    /// 公用的Controller
    /// </summary>
    [UserTrackerLog]
    public class CommonController : Controller
    {
        /// <summary>
        /// 删除上传的文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteUpLoadFile()
        {
            string path = DTRequest.GetFormString("path");
            bool b = Utils.DeleteFile(path);
            return Content(b.ToString());
        }

        /// <summary>
        /// 设置用户列表页中datagrid的列是否显示
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveOrUpdateUserGridSettings(int userID, int pageID, List<string> settings)
        {
            UserServices userServices = new UserServices();
            DBOperationStatus result = userServices.SaveOrUpdateUserCustomPageSettings(userID, (ERP.Models.CustomEnums.DatagridCustomColumnVisibilityModules)pageID, new ERP.Models.AdminUser.DTOUserCustomPageSettings() { DatagridColumnVisibility = JsonConvert.SerializeObject(settings) });
            return new CustomJsonResult(result);
        }
    }
}