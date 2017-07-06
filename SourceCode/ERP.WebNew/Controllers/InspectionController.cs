using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Inspection;
using ERP.BLL.ERP.Purchase;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.Inspection;
using ERP.Models.Purchase;
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
    public class InspectionController : Controller
    {
        private UserServices _userService = new UserServices();
        private InspectionServices _inspectionServices = new InspectionServices();
        private PurchaseContractService purchaseservices = new PurchaseContractService();

        /// <summary>
        /// 获取状态列表
        /// </summary>
        /// <returns></returns>
        public static SelectList GetSelectListStatus()
        {
            List<SelectListItem> list = new List<SelectListItem>() { };
            list.Add(new SelectListItem() { Text = "", Value = "" });

            Dictionary<int, string> dictionary = EnumHelper.GetCustomEnums<int>(typeof(InspectionStatusEnum));

            foreach (var item in dictionary)
            {
                list.Add(new SelectListItem() { Text = item.Value, Value = item.Key.ToString() });
            }
            SelectList generateList = new SelectList(list, "Value", "Text");

            return generateList;
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
                case "PurchaseNumber":
                    dbColumnName = "Purchase_Contract.PurchaseNumber";
                    break;

                case "FactoryName":
                    dbColumnName = "Purchase_Contract.Factory.Name";
                    break;

                case "StatusName":
                    dbColumnName = "StatusID";
                    break;

                case "InspectionDetectFeeFormatter":
                    dbColumnName = "InspectionDetectFee";
                    break;

                case "InspectionDetectFee_ForFactoryFormatter":
                    dbColumnName = "InspectionDetectFee_ForFactory";
                    break;

                case "InspectionAuditFeeFormatter":
                    dbColumnName = "InspectionAuditFee";
                    break;

                case "InspectionAuditFee_ForFactoryFormatter":
                    dbColumnName = "InspectionAuditFee_ForFactory";
                    break;

                default:
                    dbColumnName = name;
                    break;
            }
            return dbColumnName;
        }

        public ActionResult Index(VMInspectionSearch vm_search)
        {
            //第三方验厂列表
            ViewData["Status"] = GetSelectListStatus();

            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_AuditNotice);
            vm_search.PageType = PageTypeEnum.InspectionAuditNoticeList;

            return View(vm_search);
        }

        public ActionResult DetectNoticeList(VMInspectionSearch vm_search)
        {
            //第三方检测列表
            ViewData["Status"] = GetSelectListStatus();

            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_DetectNotice);
            vm_search.PageType = PageTypeEnum.InspectionDetectNoticeList;

            return View(vm_search);
        }

        public ActionResult SamplingNoticeList(VMInspectionSearch vm_search)
        {
            //第三方抽检列表
            ViewData["Status"] = GetSelectListStatus();

            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_SamplingNotice);
            vm_search.PageType = PageTypeEnum.InspectionSamplingNoticeList;

            return View(vm_search);
        }

        public ActionResult GetAll(VMInspectionSearch vm_search)
        {
            int currentPage = ERP.Tools.Utils.StrToInt(HttpContext.Request[EasyuiPagenationConsts.CURRENT_PAGE], 1);
            int pageSize = ERP.Tools.Utils.StrToInt(HttpContext.Request[EasyuiPagenationConsts.PAGE_SIZE], 1);
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

            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> { (int)ERPPage.ThirdParty_Inspection_AuditNotice });
            if (vm_search.PageType == PageTypeEnum.InspectionAuditNoticeList)
            {
                vm_search.TypeID = InspectionTypeEnum.AuditNotice;
            }
            if (vm_search.PageType == PageTypeEnum.InspectionDetectNoticeList)
            {
                vm_search.TypeID = InspectionTypeEnum.DetectNotice;
            }
            if (vm_search.PageType == PageTypeEnum.InspectionSamplingNoticeList)
            {
                vm_search.TypeID = InspectionTypeEnum.SamplingNotice;
            }

            #endregion 筛选条件

            List<DTOInspection> listModel = _inspectionServices.GetAll(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows, vm_search);

            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        #region 第三方验厂

        //编辑,录入验厂通知
        public ActionResult InputAuditNotice(int id)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_AuditNotice);
            if ((pageElementPrivileges & (int)InspectionAuditElementPrivileges.InputNotice) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            VMInspection vm = _inspectionServices.GetDetailByID(CurrentUserServices.Me, id);
            ViewBag.FromAddress = CurrentUserServices.Me.Email;
            ViewBag.ToAddress = vm.FactoryEmail;
            vm.PageType = PageTypeEnum.Edit;

            return View(vm);
        }

        [HttpPost]
        //保存并发送
        public ActionResult InputAuditNotice([System.Web.Http.FromBody]VMInspection vm)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_AuditNotice);
            if ((pageElementPrivileges & (int)InspectionAuditElementPrivileges.InputNotice) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            vm.TypeID = (int)InspectionTypeEnum.AuditNotice;
            DBOperationStatus result = _inspectionServices.Save(CurrentUserServices.Me, vm);

            return new CustomJsonResult((short)result);
        }

        //查看,录入结果
        public ActionResult InputAuditResult(int id)
        {
            VMInspection vm = _inspectionServices.GetDetailByID(CurrentUserServices.Me, id);
            if (DTRequest.GetQueryString("Type") == "Detail")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_AuditNotice);
                if ((pageElementPrivileges & (int)InspectionAuditElementPrivileges.View) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                vm.PageType = PageTypeEnum.Details;
            }
            else
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_AuditNotice);
                if ((pageElementPrivileges & (int)InspectionAuditElementPrivileges.InputResult) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                vm.PageType = PageTypeEnum.Edit;
            }
            string StatusName = DTRequest.GetQueryString("Status");
            ViewBag.statusName = StatusName;
            return View(vm);
        }

        [HttpPost]
        public ActionResult InputAuditResult([System.Web.Http.FromBody]VMInspection vm)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_AuditNotice);
            if ((pageElementPrivileges & (int)InspectionAuditElementPrivileges.InputResult) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            vm.TypeID = (int)InspectionTypeEnum.AuditNotice;
            DBOperationStatus result = _inspectionServices.Save_InputResult(CurrentUserServices.Me, vm);

            return new CustomJsonResult((short)result);
        }

        //第三方验厂》检测费用
        public ActionResult InputAuditFees(int id)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_AuditNotice);
            if ((pageElementPrivileges & (int)InspectionAuditElementPrivileges.InputFees) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            VMInspection vm = _inspectionServices.GetDetailByID(CurrentUserServices.Me, id);
            vm.PageType = PageTypeEnum.Edit;
            return View(vm);
        }

        [HttpPost]
        public ActionResult InputAuditFees([System.Web.Http.FromBody]VMInspection vm)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_AuditNotice);
            if ((pageElementPrivileges & (int)InspectionAuditElementPrivileges.InputFees) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            vm.TypeID = (int)InspectionTypeEnum.AuditNotice;
            DBOperationStatus result = _inspectionServices.Save_InputFees(CurrentUserServices.Me, vm);

            return new CustomJsonResult((short)result);
        }

        #endregion 第三方验厂

        #region 第三方检测

        /// <summary>
        /// 第三方检测
        /// </summary>
        /// <returns></returns>
        public ActionResult InputDetectNotice(int id)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_DetectNotice);
            if ((pageElementPrivileges & (int)InspectionDetectElementPrivileges.InputNotice) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            VMInspection vm = _inspectionServices.GetDetailByID(CurrentUserServices.Me, id);

            ViewBag.FromAddress = CurrentUserServices.Me.Email;
            ViewBag.ToAddress = vm.FactoryEmail;
            vm.PageType = PageTypeEnum.Edit;
            ViewData["ProductList"] = GetSelectionProductList(id);

            return View(vm);
        }

        [HttpPost]
        public ActionResult InputDetectNotice([System.Web.Http.FromBody]VMInspection vm)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_DetectNotice);
            if ((pageElementPrivileges & (int)InspectionDetectElementPrivileges.InputNotice) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            vm.TypeID = (int)InspectionTypeEnum.DetectNotice;
            DBOperationStatus result = _inspectionServices.Save(CurrentUserServices.Me, vm);

            return new CustomJsonResult((short)result);
        }

        //第三方检测的查看
        public ActionResult InputDetectResult(int id)
        {
            VMInspection vm = _inspectionServices.GetDetailByID(CurrentUserServices.Me, id);
            if (DTRequest.GetQueryString("Type") == "Detail")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_DetectNotice);
                if ((pageElementPrivileges & (int)InspectionDetectElementPrivileges.View) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                vm.PageType = PageTypeEnum.Details;
            }
            else
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_DetectNotice);
                if ((pageElementPrivileges & (int)InspectionDetectElementPrivileges.InputResult) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                vm.PageType = PageTypeEnum.Edit;
            }
            string StatusName = DTRequest.GetQueryString("status");
            ViewBag.status = StatusName;
            return View(vm);
        }

        [HttpPost]
        public ActionResult InputDetectResult([System.Web.Http.FromBody]VMInspection vm)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_DetectNotice);
            if ((pageElementPrivileges & (int)InspectionDetectElementPrivileges.InputResult) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            vm.TypeID = (int)InspectionTypeEnum.DetectNotice;
            DBOperationStatus result = _inspectionServices.Save_InputResult(CurrentUserServices.Me, vm);

            return new CustomJsonResult((short)result);
        }

        //第三方检测》检测费用
        public ActionResult InputDetectFees(int id)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_DetectNotice);
            if ((pageElementPrivileges & (int)InspectionDetectElementPrivileges.InputFees) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            VMInspection vm = _inspectionServices.GetDetailByID(CurrentUserServices.Me, id);
            vm.PageType = PageTypeEnum.Edit;
            return View(vm);
        }

        [HttpPost]
        public ActionResult InputDetectFees([System.Web.Http.FromBody]VMInspection vm)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_DetectNotice);
            if ((pageElementPrivileges & (int)InspectionDetectElementPrivileges.InputFees) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            vm.TypeID = (int)InspectionTypeEnum.DetectNotice;
            DBOperationStatus result = _inspectionServices.Save_InputFees(CurrentUserServices.Me, vm);

            return new CustomJsonResult((short)result);
        }

        #endregion 第三方检测

        #region 第三方抽检

        //编辑,录入抽检通知
        public ActionResult InputSamplingNotice(int id)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_SamplingNotice);
            if ((pageElementPrivileges & (int)InspectionSamplingElementPrivileges.InputNotice) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            VMInspection vm = _inspectionServices.GetDetailByID(CurrentUserServices.Me, id);
            ViewBag.FromAddress = CurrentUserServices.Me.Email;
            ViewBag.ToAddress = vm.FactoryEmail;
            vm.PageType = PageTypeEnum.Edit;

            return View(vm);
        }

        [HttpPost]
        //保存并发送
        public ActionResult InputSamplingNotice([System.Web.Http.FromBody]VMInspection vm)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_SamplingNotice);
            if ((pageElementPrivileges & (int)InspectionSamplingElementPrivileges.InputNotice) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            vm.TypeID = (int)InspectionTypeEnum.SamplingNotice;
            DBOperationStatus result = _inspectionServices.Save(CurrentUserServices.Me, vm);

            return new CustomJsonResult((short)result);
        }

        //查看,录入结果
        public ActionResult InputSamplingResult(int id)
        {
            VMInspection vm = _inspectionServices.GetDetailByID(CurrentUserServices.Me, id);
            if (DTRequest.GetQueryString("Type") == "Detail")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_SamplingNotice);
                if ((pageElementPrivileges & (int)InspectionSamplingElementPrivileges.View) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                vm.PageType = PageTypeEnum.Details;
            }
            else
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_SamplingNotice);
                if ((pageElementPrivileges & (int)InspectionSamplingElementPrivileges.InputResult) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                vm.PageType = PageTypeEnum.Edit;
            }
            string StatusName = DTRequest.GetQueryString("Status");
            ViewBag.statusName = StatusName;
            return View(vm);
        }

        [HttpPost]
        public ActionResult InputSamplingResult([System.Web.Http.FromBody]VMInspection vm)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_SamplingNotice);
            if ((pageElementPrivileges & (int)InspectionSamplingElementPrivileges.InputResult) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            vm.TypeID = (int)InspectionTypeEnum.SamplingNotice;
            DBOperationStatus result = _inspectionServices.Save_InputResult(CurrentUserServices.Me, vm);

            return new CustomJsonResult((short)result);
        }

        //第三方抽检》检测费用
        public ActionResult InputSamplingFees(int id)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_SamplingNotice);
            if ((pageElementPrivileges & (int)InspectionSamplingElementPrivileges.InputFees) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            VMInspection vm = _inspectionServices.GetDetailByID(CurrentUserServices.Me, id);
            vm.PageType = PageTypeEnum.Edit;
            return View(vm);
        }

        [HttpPost]
        public ActionResult InputSamplingFees([System.Web.Http.FromBody]VMInspection vm)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ThirdParty_Inspection_SamplingNotice);
            if ((pageElementPrivileges & (int)InspectionSamplingElementPrivileges.InputFees) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            vm.TypeID = (int)InspectionTypeEnum.SamplingNotice;
            DBOperationStatus result = _inspectionServices.Save_InputFees(CurrentUserServices.Me, vm);

            return new CustomJsonResult((short)result);
        }

        #endregion 第三方抽检

        /// <summary>
        /// 采购合同的产品列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SelectList GetSelectionProductList(int id)
        {
            List<SelectListItem> list = new List<SelectListItem>() { };

            List<VMPurchaseProduct> vm_list = _inspectionServices.GetDetailByIDD(CurrentUserServices.Me, id);
            foreach (var item in vm_list)
            {
                list.Add(new SelectListItem() { Text = item.No, Value = item.ProductID.ToString() });
            }
            SelectList generateList = new SelectList(list, "Value", "Text");
            return generateList;
        }
    }
}