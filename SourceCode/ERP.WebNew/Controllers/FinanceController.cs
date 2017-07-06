using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Dictionary;
using ERP.BLL.ERP.Finance;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.Order;
using ERP.Models.Purchase;
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
    /// 财务
    /// </summary>
    [UserTrackerLog]
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.FinanceManagement)]
    public class FinanceController : Controller
    {
        private UserServices _userService = new UserServices();
        private DictionaryServices _dictionaryServices = new DictionaryServices();
        private FinanceService _financeService = new FinanceService();

        #region HelperMethod

        /// <summary>
        /// 绑定可见的列
        /// </summary>
        /// <param name="type"></param>
        private void BindColumnsVisible(DatagridCustomColumnVisibilityModules type)
        {
            var settings = _userService.GetUserCustomPageSettings(CurrentUserServices.Me.UserID, type);
            if (settings != null && !string.IsNullOrEmpty(settings.DatagridColumnVisibility))
            {
                ViewBag.ColumnsVisible = ((Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(settings.DatagridColumnVisibility)).ToObject(typeof(List<string>));
            }
        }

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
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.FinanceManagement_Maintain_Index);
            vm_search.PageType = PageTypeEnum.PendingCheckList;

            return View(vm_search);
        }

        public ActionResult Finance_Factory(VMOrderSearch vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.FinanceManagement_StatisticsList_Factory);
            vm_search.PageType = PageTypeEnum.Finance_Factory;

            return View(vm_search);
        }

        public ActionResult Finance_Analysis(VMOrderSearch vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.FinanceManagement_StatisticsList_Analysis);
            vm_search.PageType = PageTypeEnum.Finance_Analysis;

            BindColumnsVisible(DatagridCustomColumnVisibilityModules.FinanceManagement_Analysis);

            return View(vm_search);
        }

        public ActionResult Finance_SelfExportList(VMOrderSearch vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.FinanceManagement_StatisticsList_SelfExportList);
            vm_search.PageType = PageTypeEnum.Finance_SelfExportList;

            BindColumnsVisible(DatagridCustomColumnVisibilityModules.FinanceManagement_SelfExportList);

            return View(vm_search);
        }

        public ActionResult Finance_DetailList(VMOrderSearch vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.FinanceManagement_StatisticsList_DetailList);
            vm_search.PageType = PageTypeEnum.Finance_DetailList;

            BindColumnsVisible(DatagridCustomColumnVisibilityModules.FinanceManagement_DetailList);
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
                (int)ERPPage.FinanceManagement_Maintain_Index, });

            #endregion 筛选条件

            List<DTOOrder> listModel = _financeService.GetAll(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows, vm_search);
            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        /// <summary>
        /// 获取列表数据——工厂往来账
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAll_ForFactory(VMOrderSearch vm_search)
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
                (int)ERPPage.FinanceManagement_StatisticsList_Factory, });

            #endregion 筛选条件

            List<DTOPurchaseContract> listModel = _financeService.GetAll_ForFactory(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows, vm_search);
            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAll_ForOther(VMOrderSearch vm_search)
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
                (int)ERPPage.FinanceManagement_StatisticsList_Analysis, });

            #endregion 筛选条件

            var listModel = _financeService.GetAll_ForOther(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows, vm_search);
            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        public ActionResult Edit(int id)
        {
            VMOrderEdit vm = _financeService.GetDetailByID(CurrentUserServices.Me, id);

            BindColumnsVisible(DatagridCustomColumnVisibilityModules.FinanceManagement_ProductList);

            List<string> list_RoleNames = CurrentUserServices.Me.RoleNames;
            ViewBag.IsFinancialOfficer = list_RoleNames.Contains("财务主管");
            ViewBag.IsBusiness = false;
            foreach (var item in list_RoleNames)
            {
                if (item.Contains("业务"))
                {
                    ViewBag.IsBusiness = true;
                }
            }
            ViewBag.IsBusinessOfficer = list_RoleNames.Contains("业务总监");

            if (DTRequest.GetQueryString("Type") == "Detail")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> {
                        (int)ERPPage.FinanceManagement_Maintain_Index,});
                if ((pageElementPrivileges & (int)FinanceElementPrivileges.View) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                ViewBag.Title = "查看财务数据";
                vm.PageType = PageTypeEnum.Details;
            }
            else if (DTRequest.GetQueryString("Type") == "Add")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.FinanceManagement_Maintain_Index);
                if ((pageElementPrivileges & (int)FinanceElementPrivileges.Maintenance) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                ViewBag.Title = "维护财务数据";
                vm.PageType = PageTypeEnum.Edit;

            }
            else
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.FinanceManagement_Maintain_Index);
                if ((pageElementPrivileges & (int)FinanceElementPrivileges.Edit) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                ViewBag.Title = "编辑财务数据";
                vm.PageType = PageTypeEnum.Edit;

            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult Edit([System.Web.Http.FromBody]VMOrderEdit vm)
        {
            VMAjaxProcessResult result = _financeService.Save(CurrentUserServices.Me, vm);

            return Json(result);
        }

        public ActionResult Edit_ForFactory(int id)
        {
            VMOrderEdit vm = _financeService.GetDetailByID_ForFactory(CurrentUserServices.Me, id);

            BindColumnsVisible(DatagridCustomColumnVisibilityModules.FinanceManagement_Factory);

            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> {
                (int)ERPPage.FinanceManagement_StatisticsList_Factory,});
            if ((pageElementPrivileges & (int)FinanceElementPrivileges.View_ForFactory) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            ViewBag.Title = "查看工厂往来账";
            vm.PageType = PageTypeEnum.Details;

            return View(vm);
        }

        #region 生成Excel

        public JsonResult ExportExcel_Factory(string idList)
        {
            List<int> PurchaseIDList = CommonCode.IdListToList(idList);
            var temp = _financeService.ExportExcel_Factory(CurrentUserServices.Me, PurchaseIDList);
            return Json(temp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportExcel_Analysis(string idList)
        {
            List<int> OrderProductIDList = CommonCode.IdListToList(idList);
            var temp = _financeService.ExportExcel_Analysis(CurrentUserServices.Me, OrderProductIDList);
            return Json(temp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportExcel_SelfExportList(string idList)
        {
            List<int> OrderProductIDList = CommonCode.IdListToList(idList);
            var temp = _financeService.ExportExcel_SelfExportList(CurrentUserServices.Me, OrderProductIDList);
            return Json(temp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportExcel_DetailList(string idList)
        {
            List<int> OrderProductIDList = CommonCode.IdListToList(idList);
            var temp = _financeService.ExportExcel_DetailList(CurrentUserServices.Me, OrderProductIDList);
            return Json(temp, JsonRequestBehavior.AllowGet);
        }

        #endregion 生成Excel

        #endregion UserMethod
    }
}