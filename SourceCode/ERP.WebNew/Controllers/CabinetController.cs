using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Shipment;
using ERP.Models.Cabinet;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.Shipment;
using ERP.Tools;
using ERP.WebNew.Attributes;
using ERP.WebNew.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.WebNew.Controllers
{
    [UserTrackerLog]
    [AuthorizedUser(PageID = (int)ERPPage.ShipmentManagement_Cabinet)]
    public class CabinetController : Controller
    {

        CabinetService cabinetservices = new CabinetService();
        private const string sSize = "Size";
        private const string sName = "Name";
        private UserServices _userService = new UserServices();
        //
        // GET: /Cabinet/
        public ActionResult Index()
        {
            VMDTOCabinet vm = new VMDTOCabinet()
            {
                PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ShipmentManagement_Cabinet),
                Name = DTRequest.GetQueryString(sName),
                Size = DTRequest.GetQueryString(sSize)
            };
            return View(vm);
        }

        public ActionResult GetAll()
        {
            int currentPage = ERP.Tools.Utils.StrToInt(HttpContext.Request[EasyuiPagenationConsts.CURRENT_PAGE], 1);
            int pageSize = ERP.Tools.Utils.StrToInt(HttpContext.Request[EasyuiPagenationConsts.PAGE_SIZE], 1);
            int totalRows = 0;

            #region 排序

            List<string> sortColumnsNames = new List<string>();
            if (!string.IsNullOrEmpty(HttpContext.Request[EasyuiPagenationConsts.SORT_COLUMN_NAMES]))
            {
                sortColumnsNames = HttpContext.Request[EasyuiPagenationConsts.SORT_COLUMN_NAMES].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            List<string> sortColumnOrders = new List<string>();
            if (!string.IsNullOrEmpty(HttpContext.Request[EasyuiPagenationConsts.SORT_COLUMN_ORDERS]))
            {
                sortColumnOrders = HttpContext.Request[EasyuiPagenationConsts.SORT_COLUMN_ORDERS].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            #endregion 排序

            #region 筛选

            string Name = DTRequest.GetQueryString(sName);
            string Size = DTRequest.GetQueryString(sSize);
            //string DataFlag = DTRequest.GetQueryString(sDataFlag);

            #endregion 筛选

            List<DTOCabinet> listModel = cabinetservices.selectALL(CurrentUserServices.Me.UserID, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows, Name, Size);

            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }



        public ActionResult Add(int id)
        {
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ShipmentManagement_Cabinet);
            if ((pageElementPrivileges & (int)CabinetElementPrivileges.Create) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }
            return View();
        }


        [HttpPost]
        public ActionResult Add(string Name, string Size, string Sizetwo, string Length, string Width, string Height)
        {
            DTOCabinet cabinet = new DTOCabinet();
            cabinet.Name = Name;
            if (Sizetwo == "")
            {
                cabinet.Size = Size;
            }
            else
            {
                cabinet.Size = Size + "~" + Sizetwo;
            }
            cabinet.Length = Length;
            cabinet.Width = Width; ;
            cabinet.Height = Height;
            DBOperationStatus result = cabinetservices.AddCacinet(cabinet, CurrentUserServices.Me.UserID);
            return new CustomJsonResult((short)result);
        }


        public ActionResult Edit(int id, int status)
        {
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ShipmentManagement_Cabinet);
            if ((id == 0 && (pageElementPrivileges & (int)CabinetElementPrivileges.Create) == 0) ||
                (id != 0 && (pageElementPrivileges & (int)CabinetElementPrivileges.Edit) == 0))
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            List<DTOCabinet> list = cabinetservices.SelectById(id);
            if (status == 1)
            {
                //编辑
                ViewBag.title = "编辑柜型信息";
                ViewData["Value"] = list;
                ViewBag.id = id;
                ViewBag.status = status;
            }
            else
            {
                //查看
                ViewBag.title = "查看柜型信息";
                ViewData["Value"] = list;
                ViewBag.id = id;
                ViewBag.status = status;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Edit(string Name, string Size, string Sizetwo, int id,string Length,string Width,string Height)
        {
            DTOCabinet cabinet = new DTOCabinet();
            cabinet.Name = Name;
            cabinet.ID = id;
            if (Sizetwo == "")
            {
                cabinet.Size = Size;
            }
            else
            {
                cabinet.Size = Size + "~" + Sizetwo;
            }
            cabinet.Length = Length;
            cabinet.Width = Width;
            cabinet.Height = Height;
            DBOperationStatus result = cabinetservices.UpdateCacinet(cabinet, CurrentUserServices.Me.UserID);
            return new CustomJsonResult((short)result);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ShipmentManagement_Cabinet);
            if ((pageElementPrivileges & (int)CabinetElementPrivileges.delete) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }
            DBOperationStatus result = cabinetservices.DeleteCacinet(id);
            return new CustomJsonResult((short)result);
        }
        [HttpPost]
        public ActionResult Deleted(string idList)
        {
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ShipmentManagement_Cabinet);
            if ((pageElementPrivileges & (int)CabinetElementPrivileges.deleteAll) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }
            List<int> deleteList = null;
            if (!string.IsNullOrEmpty(idList))
            {
                deleteList = idList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
            }
            DBOperationStatus result = cabinetservices.DeleteCacinet(deleteList);
            return new CustomJsonResult((short)result);
        }
    }
}