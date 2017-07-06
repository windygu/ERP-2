using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Dictionary;
using ERP.BLL.ERP.ShipmentSample;
using ERP.BLL.ERP.ThreeTimesQC;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.ThreeTimesQC;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.WebNew.Attributes;
using ERP.WebNew.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ERP.WebNew.Controllers
{
    [UserTrackerLog]
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.PurchaseManagement_ShipmentSampleManagement)]
    public class ShipmentSampleController : Controller
    {
        private UserServices _userService = new UserServices();
        private DictionaryServices _dictionaryServices = new DictionaryServices();
        private ShipmentSampleService _shipmentSampleService = new ShipmentSampleService();

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
                case "PurchaseNumber":
                    dbColumnName = "Purchase_Contract.PurchaseNumber";
                    break;

                case "FactoryAbbreviation":
                    dbColumnName = "Purchase_Contract.Factory.Abbreviation";
                    break;

                case "PurchaseDate":
                    dbColumnName = "Purchase_Contract.PurchaseDate";
                    break;

                case "CustomerCode":
                    dbColumnName = "Purchase_Contract.Orders_Customers.CustomerCode";
                    break;

                case "DateStart":
                    dbColumnName = "Purchase_Contract.DateStart";
                    break;

                case "DateEnd":
                    dbColumnName = "Purchase_Contract.DateEnd";
                    break;

                case "RecoveryStatusName":
                    dbColumnName = "RecoveryStatusID";
                    break;

                default:
                    dbColumnName = name;
                    break;
            }
            return dbColumnName;
        }

        #region HelperMethod

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        public static SelectList GetSelectListUser()
        {
            List<SelectListItem> list = new List<SelectListItem>() { };
            list.Add(new SelectListItem() { Text = "", Value = "" });
            UserServices _userService = new UserServices();
            var userList = _userService.GetUserList();

            foreach (var item in userList)
            {
                list.Add(new SelectListItem() { Text = item.UserName + ";" + item.DisplayName + ";" + item.Email, Value = item.UserID.ToString() });
            }
            SelectList generateList = new SelectList(list, "Value", "Text");

            return generateList;
        }

        #endregion HelperMethod

        #region UserMethod

        //待收回的出货样
        public ActionResult Index(VMPurchaseSearch vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ShipmentSampleManagement_Index);
            vm_search.PageType = PageTypeEnum.PendingCheckList;

            return View(vm_search);
        }

        //已收回的出货样
        public ActionResult HadRecoveredList(VMPurchaseSearch vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ShipmentSampleManagement_HadRecovered);
            vm_search.PageType = PageTypeEnum.PassedCheckList;

            return View(vm_search);
        }

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAll(VMPurchaseSearch vm_search)
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
                (int)ERPPage.PurchaseManagement_ShipmentSampleManagement_Index,
                (int)ERPPage.PurchaseManagement_ShipmentSampleManagement_HadRecovered, });

            #endregion 筛选条件

            List<DTOPurchaseContract> listModel = _shipmentSampleService.GetAll(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows,
             vm_search);

            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        public ActionResult Edit(int id)
        {
            ViewBag.ListUser = GetSelectListUser();

            VMThreeTimesQC vm = _shipmentSampleService.GetDetailByID(CurrentUserServices.Me, id);

            string Type = DTRequest.GetQueryString("Type");
            if (Type == "Detail")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> {
                    (int)ERPPage.PurchaseManagement_ShipmentSampleManagement_Index,
                    (int)ERPPage.PurchaseManagement_ShipmentSampleManagement_HadRecovered });
                if ((pageElementPrivileges & (int)ThirdPartyVerificationElementPrivileges.View) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                vm.PageType = PageTypeEnum.Details;
                if (vm.RecoveryStatusID == (short)ShipmentSampleStatusEnum.HadRecovery)
                {
                    ViewBag.Title = "查看已收回的出货样";
                }
                else
                {
                    ViewBag.Title = "查看待收回的出货样";
                }
            }
            else
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ShipmentSampleManagement_Index);
                if ((pageElementPrivileges & (int)ThirdPartyVerificationElementPrivileges.UpLoad) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                vm.PageType = PageTypeEnum.Edit;
                ViewBag.Title = "收回登记出货样";
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult Edit([System.Web.Http.FromBody]VMThreeTimesQC vm)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ShipmentSampleManagement_Index);
            if ((pageElementPrivileges & (int)ThirdPartyVerificationElementPrivileges.UpLoad) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            var result = _shipmentSampleService.Save(CurrentUserServices.Me, vm);

            return Json(result);
        }

        #endregion UserMethod
    }
}