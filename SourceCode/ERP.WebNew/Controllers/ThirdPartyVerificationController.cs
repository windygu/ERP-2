using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Customer;
using ERP.BLL.ERP.Dictionary;
using ERP.BLL.ERP.Order;
using ERP.BLL.ERP.Shipment;
using ERP.BLL.ERP.ThirdPartyVerification;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.Product;
using ERP.Models.ThirdPartyVerification;
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
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.ThirdParty_Inspection)]
    public class ThirdPartyVerificationController : Controller
    {
        private UserServices _userService = new UserServices();
        private DictionaryServices _dictionaryServices = new DictionaryServices();
        private ThirdPartyVerificationService _thirdPartyVerificationService = new ThirdPartyVerificationService();

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
                case "OrderNumber":
                    dbColumnName = "Order.OrderNumber";
                    break;

                case "POID":
                    dbColumnName = "Order.POID";
                    break;

                case "EHIPO":
                    dbColumnName = "Order.EHIPO";
                    break;

                case "CustomerNo":
                    dbColumnName = "Order.Orders_Customers.CustomerCode";
                    break;

                case "CustomerDate":
                    dbColumnName = "Order.CustomerDate";
                    break;

                case "OrderDateStart":
                    dbColumnName = "Order.OrderDateStart";
                    break;

                case "OrderDateEnd":
                    dbColumnName = "Order.OrderDateEnd";
                    break;

                case "OrderOrigin":
                    dbColumnName = "Order.OrderOrigin";
                    break;

                case "OrderStatusName":
                    dbColumnName = "StatusID";
                    break;

                default:
                    dbColumnName = name;
                    break;
            }
            return dbColumnName;
        }

        #region UserMethod

        //第三方验货列表
        public ActionResult Index(VMOrderSearch vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_Verification);
            vm_search.PageType = PageTypeEnum.PendingCheckList;

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

            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> { (int)ERPPage.ThirdParty_Inspection_Verification });

            #endregion 筛选条件

            List<DTOOrder> listModel = _thirdPartyVerificationService.GetAll(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows, vm_search);

            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        public ActionResult Edit(int id)
        {
            VMThirdPartyVerification vm = _thirdPartyVerificationService.GetDetailByID(CurrentUserServices.Me, id);

            string Type = DTRequest.GetQueryString("Type");
            if (Type == "Detail")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> { (int)ERPPage.ThirdParty_Inspection_Verification });
                if ((pageElementPrivileges & (int)ThirdPartyVerificationElementPrivileges.View) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                vm.PageType = PageTypeEnum.Details;
                ViewBag.Title = "查看第三方验货报告";
            }
            else
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_Verification);
                if ((pageElementPrivileges & (int)ThirdPartyVerificationElementPrivileges.UpLoad) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                vm.PageType = PageTypeEnum.Edit;
                ViewBag.Title = "上传第三方验货报告";
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult Edit([System.Web.Http.FromBody]VMThirdPartyVerification vm)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_Verification);
            if ((pageElementPrivileges & (int)ThirdPartyVerificationElementPrivileges.UpLoad) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            DBOperationStatus result = _thirdPartyVerificationService.Save(CurrentUserServices.Me, vm);

            return new CustomJsonResult((short)result);
        }

        #endregion UserMethod
    }
}