using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Dictionary;
using ERP.BLL.ERP.PortChargesInvoice;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.Order;
using ERP.Tools;
using ERP.WebNew.Attributes;
using ERP.WebNew.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ERP.WebNew.Controllers
{
    /// <summary>
    /// 港杂费发票
    /// </summary>
    [UserTrackerLog]
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.DocumentsManagement)]
    public class PortChargesInvoiceController : Controller
    {
        private UserServices _userService = new UserServices();
        private DictionaryServices _dictionaryServices = new DictionaryServices();

        private PortChargesInvoiceService _portChargesInvoiceService = new PortChargesInvoiceService();

        #region HelperMethod

        /// <summary>
        /// 替换排序的字段
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string ReplaceSortNames(string name)
        {
            string dbColumnName = string.Empty;

            switch (name)
            {
                case "CustomerNo":
                    dbColumnName = "Orders_Customers.CustomerCode";
                    break;

                case "OrderStatusName":
                    dbColumnName = "OrderStatusID";
                    break;

                default:
                    dbColumnName = name;
                    break;
            }
            return dbColumnName;
        }

        #endregion HelperMethod

        #region UserMethod

        public ActionResult Index(VMOrderSearch vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DocumentsManagement_PortChargesInvoice);
            vm_search.PageType = PageTypeEnum.PendingApproval;

            return View(vm_search);
        }

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAll(VMOrderSearch vm_search)
        {
            int currentPage = DTRequest.GetQueryInt(EasyuiPagenationConsts.CURRENT_PAGE, 1);
            int pageSize = DTRequest.GetQueryInt(EasyuiPagenationConsts.PAGE_SIZE, Keys.DEFAULT_PAGE_SIZE);
            int totalRows = 0;

            #region 排序

            List<string> sortColumnsNames = new List<string>();
            if (!string.IsNullOrEmpty(DTRequest.GetQueryString(EasyuiPagenationConsts.SORT_COLUMN_NAMES)))
            {
                var sortColumnsNamesTemp = HttpContext.Request[EasyuiPagenationConsts.SORT_COLUMN_NAMES].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                // 需要替换转换过的属性
                foreach (var n in sortColumnsNamesTemp)
                {
                    sortColumnsNames.Add(ReplaceSortNames(n));
                }
            }

            List<string> sortColumnOrders = new List<string>();
            if (!string.IsNullOrEmpty(DTRequest.GetQueryString(EasyuiPagenationConsts.SORT_COLUMN_ORDERS)))
            {
                sortColumnOrders = DTRequest.GetQueryString(EasyuiPagenationConsts.SORT_COLUMN_ORDERS).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            #endregion 排序

            #region 筛选条件

            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> {
                (int)ERPPage.DocumentsManagement_PortChargesInvoice});

            #endregion 筛选条件

            List<DTOOrder> listModel = _portChargesInvoiceService.GetAll(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows, vm_search);
            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        public ActionResult Edit(int id)
        {
            VMOrderEdit vm = _portChargesInvoiceService.GetDetailByID(CurrentUserServices.Me, id);

            if (DTRequest.GetQueryString("Type") == "Add")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> { (int)ERPPage.DocumentsManagement_PortChargesInvoice });
                if ((pageElementPrivileges & (int)PortChargesInvoiceElementPrivileges.Maintenance) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                ViewBag.Title = "维护港杂费发票";
                vm.PageType = PageTypeEnum.Edit;
            }
            else if (DTRequest.GetQueryString("Type") == "Detail")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> { (int)ERPPage.DocumentsManagement_PortChargesInvoice });
                if ((pageElementPrivileges & (int)PortChargesInvoiceElementPrivileges.View) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                ViewBag.Title = "查看港杂费发票";
                vm.PageType = PageTypeEnum.Details;
            }
            else
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> { (int)ERPPage.DocumentsManagement_PortChargesInvoice });
                if ((pageElementPrivileges & (int)PortChargesInvoiceElementPrivileges.Edit) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                ViewBag.Title = "编辑港杂费发票";
                vm.PageType = PageTypeEnum.Edit;
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult Edit([System.Web.Http.FromBody]VMOrderEdit vm)
        {
            var result = _portChargesInvoiceService.Save(CurrentUserServices.Me, vm);

            return Json(result);
        }

        #endregion UserMethod
    }
}