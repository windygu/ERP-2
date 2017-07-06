using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.ProducePlan;
using ERP.BLL.Workflow;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.ProducePlan;
using ERP.Models.Quote;
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
    [AuthorizedUser(PageID = (int)ERPPage.ProduceManagement_Plan)]
    public class ProducePlanController : Controller
    {
        private ProducePlanServices _ProducePlanServices = new ProducePlanServices();
        private UserServices _userService = new UserServices();

        #region HelperMethod

        public static SelectList GetSelectList(Dictionary<int, string> dataList)
        {
            List<SelectListItem> list = new List<SelectListItem>() { };
            list.Add(new SelectListItem() { Text = "", Value = "" });

            foreach (var item in dataList)
            {
                list.Add(new SelectListItem() { Text = item.Value, Value = item.Key.ToString() });
            }

            Dictionary<int, string> dictionary = new Dictionary<int, string>();

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

                case "FactoryAbbreviation":
                    dbColumnName = "Purchase_Contract.Factory.Abbreviation";
                    break;

                case "PurchaseDate":
                    dbColumnName = "Purchase_Contract.PurchaseDate";
                    break;

                case "CustomerCode":
                    dbColumnName = "Purchase_Contract.Orders_Customers.CustomerCode";
                    break;

                case "StatusName":
                    dbColumnName = "Status";
                    break;

                case "DT_MODIFYDATEFormatter":
                    dbColumnName = "DT_MODIFYDATE";
                    break;

                default:
                    dbColumnName = name;
                    break;
            }
            return dbColumnName;
        }

        #endregion HelperMethod

        /// <summary>
        /// 上传生产计划列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(VMDTOProduceSearch vm_search)
        {
            vm_search.PageType = PageTypeEnum.PendingMaintenanceList;
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ProduceManagement_Index);

            return View(vm_search);
        }

        /// <summary>
        /// 待审核生产计划列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ApprovalList(VMDTOProduceSearch vm_search)
        {
            vm_search.PageType = PageTypeEnum.PendingCheckList;
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ProduceManagement_ApprovalList);

            return View(vm_search);
        }

        /// <summary>
        /// 已审核生产计划列表
        /// </summary>
        /// <returns></returns>
        public ActionResult PassedApprovalList(VMDTOProduceSearch vm_search)
        {
            vm_search.PageType = PageTypeEnum.PassedCheckList;
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ProduceManagement_PassedApprovalList);

            return View(vm_search);
        }

        public ActionResult GetAll(VMDTOProduceSearch vm_search)
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

            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> {(int)ERPPage.ProduceManagement_Index,
               (int)ERPPage.ProduceManagement_ApprovalList,
               (int)ERPPage.ProduceManagement_PassedApprovalList });

            #endregion 筛选条件

            List<DTOProducePlan> listModel = _ProducePlanServices.GetAll(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows, vm_search);

            if (vm_search.PageType == PageTypeEnum.PendingCheckList && listModel != null)
            {
                listModel.ForEach(p => p.NextApproverInfos = ApprovalService.GetNextApproverInfos(WorkflowTypes.ApprovalProducePlan, p.ST_CREATEUSER2 ?? 0, p.ApproverIndex, p.CustomerID));
            }

            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        /// <summary>
        /// 上传文件的页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Upload(int id, int DataFlag)
        {
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ProduceManagement_Index);
            if ((pageElementPrivileges & (int)ProducePlanElementPricileges.Upload) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            if (DataFlag == 0)
            {
                ViewBag.title = "查看上传生产计划信息";
            }
            else
            {
                ViewBag.title = "上传生产计划信息";
            }
            DTOProducePlan vm = _ProducePlanServices.GetDetailByID(CurrentUserServices.Me, id);
            return View(vm);
        }

        [HttpPost]
        public ActionResult Upload([System.Web.Http.FromBody]DTOProducePlan vm)
        {
            VMAjaxProcessResult result = _ProducePlanServices.SaveUpLoad(CurrentUserServices.Me, vm);
            return Json(result);
        }

        /// <summary>
        /// 审核主页面
        /// </summary>
        /// <returns></returns>
        public ActionResult WaitCheck(int id, int DataFlag)
        {
            DTOProducePlan vm = _ProducePlanServices.GetDetailByID(CurrentUserServices.Me, id);

            if (DataFlag == 0)
            {
                ViewBag.title = "查看生产计划";
                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ProduceManagement_Index);
                if ((pageElementPrivileges & (int)ProducePlanElementPricileges.Watch) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }
                vm.PageTypeEnum = PageTypeEnum.Details;
            }
            else if (DataFlag == 1)
            {
                ViewBag.title = "查看待审核生产计划";
                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ProduceManagement_ApprovalList);
                if ((pageElementPrivileges & (int)ProducePlanElementPricileges.Watch) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }
                vm.PageTypeEnum = PageTypeEnum.Details;
            }
            else if (DataFlag == 2)
            {
                ViewBag.title = "审核生产计划";
                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ProduceManagement_ApprovalList);
                if ((pageElementPrivileges & (int)ProducePlanElementPricileges.Check) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #region 判断是否有审批流的权限

                bool isHasApprovalPermission = ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalProducePlan, CurrentUserServices.Me, vm.ST_CREATEUSER, vm.ApproverIndex,vm.CustomerID);
                if (!isHasApprovalPermission)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 判断是否有审批流的权限

                vm.PageTypeEnum = PageTypeEnum.Check;
            }
            else if (DataFlag == 3)
            {
                ViewBag.title = "查看已审核生产计划";
                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ProduceManagement_PassedApprovalList);
                if ((pageElementPrivileges & (int)ProducePlanElementPricileges.Watch) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }
                vm.PageTypeEnum = PageTypeEnum.Details;
            }

            return View(vm);
        }

        /// <summary>
        /// 验证请求
        /// </summary>
        /// <returns></returns>
        private ApiResult ValidRequest(ProducePlanElementPricileges ElementPrivileges)
        {
            ApiResult result = new ApiResult();

            if (CurrentUserServices.Me == null)
            {
                result.Success = false;
                result.Info = "未登录！";
                return result;
            }
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> { (int)ERPPage.ProduceManagement_ApprovalList });
            if ((pageElementPrivileges & (int)ElementPrivileges) == 0)
            {
                result.Success = false;
                result.Info = "没权限！";
                return result;
            }

            result.Success = true;
            return result;
        }

        /// <summary>
        /// 审核方法
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult WaitCheck(DTOProducePlan vm)
        {
            #region 添加权限

            ApiResult validResult = ValidRequest(ProducePlanElementPricileges.Check);
            if (!validResult.Success)
            {
                return Json(validResult, JsonRequestBehavior.AllowGet);
            }

            #endregion 添加权限

            DBOperationStatus result = _ProducePlanServices.WaitCheck(CurrentUserServices.Me, vm);
            return Json(result);
        }

        /// <summary>
        /// 提交审核
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Submit(int id)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ProduceManagement_Index);
            if ((pageElementPrivileges & (int)ProducePlanElementPricileges.Submit) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            DBOperationStatus result = _ProducePlanServices.Save(id, CurrentUserServices.Me);
            return new CustomJsonResult((short)result);
        }
    }
}