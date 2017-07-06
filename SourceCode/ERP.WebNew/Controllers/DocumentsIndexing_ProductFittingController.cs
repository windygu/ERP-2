using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Dictionary;
using ERP.BLL.ERP.DocumentsIndexing;
using ERP.BLL.ERP.DocumentsIndexing_ProductFitting;
using ERP.BLL.Workflow;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.DocumentIndexing;
using ERP.Models.Order;
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
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.DocumentsIndexingManagement)]
    public class DocumentsIndexing_ProductFittingController : Controller
    {
        private UserServices _userService = new UserServices();
        private DictionaryServices _dictionaryServices = new DictionaryServices();

        private DocumentsIndexing_ProductFittingService _documentsIndexingService = new DocumentsIndexing_ProductFittingService();

        #region HelperMethod

        /// <summary>
        /// 绑定ViewBag
        /// </summary>
        private void BindViewBag()
        {
            //List<DTOCabinet> listModel = _cabinetService.GetAll();
            //Dictionary<int, string> di = new Dictionary<int, string>();
            //foreach (var item in listModel)
            //{
            //    di.Add(item.ID, item.Name + " [" + item.Size + "(m³)]");
            //}
            //ViewBag.CabinetList = CommonCode.GetSelectDataList(di);

            //var dictionaries = _dictionaryServices.GetDictionaryInfos(CurrentUserServices.Me.UserID, null);
            //ViewBag.Ports = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.OutPort).ToList();
            //ViewBag.GoalPort = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.GoalPort).ToList();

            var enumValues = EnumHelper.GetEnumKeyValuesWithDescription<short>(typeof(CustomsUnitTypeEnum));
            Dictionary<int, string> di3 = new Dictionary<int, string>();
            foreach (var item in enumValues)
            {
                di3.Add(item.Key, item.Value.Value);
            }
            ViewData["list_CustomsUnit"] = CommonCode.GetSelectDataList(di3);
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
                case "OrderNumber":
                    dbColumnName = "Order.OrderID";
                    break;

                case "CustomerNo":
                    dbColumnName = "Order.Orders_Customers.CustomerCode";
                    break;

                case "POID":
                    dbColumnName = "Order.POID";
                    break;

                case "OrderDateStart":
                    dbColumnName = "Order.OrderDateStart";
                    break;

                case "OrderDateEnd":
                    dbColumnName = "Order.OrderDateEnd";
                    break;

                case "Managers_UserName":
                    dbColumnName = "Managers_UserID";
                    break;

                case "StatusName":
                    dbColumnName = "StatusID";
                    break;

                default:
                    dbColumnName = name;
                    break;
            }
            return dbColumnName;
        }

        #endregion HelperMethod

        #region UserMethod

        #region 列表页面

        public ActionResult Index(VMOrderSearch vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DocumentsIndexingManagement_Maintain);
            vm_search.PageType = PageTypeEnum.PendingMaintenanceList;

            return View(vm_search);
        }

        public ActionResult PendingApprovalList(VMOrderSearch vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DocumentsIndexingManagement_PendingApprovalList);
            vm_search.PageType = PageTypeEnum.PendingApproval;

            return View(vm_search);
        }

        public ActionResult PassedApprovalList(VMOrderSearch vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DocumentsIndexingManagement_PassedApprovalList);
            vm_search.PageType = PageTypeEnum.PassedApproval;

            return View(vm_search);
        }

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
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
                (int)ERPPage.DocumentsIndexingManagement_Maintain,
                (int)ERPPage.DocumentsIndexingManagement_PendingApprovalList,
                (int)ERPPage.DocumentsIndexingManagement_PassedApprovalList});

            #endregion 筛选条件

            List<VMDocumentIndexing> listModel = _documentsIndexingService.GetAll(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows, vm_search);

            if (vm_search.PageType == PageTypeEnum.PendingApproval && listModel != null)
            {
                listModel.Where(d => d.StatusID == (int)DocumentsIndexingStatusEnum.PendingCheck).ToList().ForEach(p => p.NextApproverInfos = ApprovalService.GetNextApproverInfos(WorkflowTypes.ApprovalDocumentsIndexing, p.ST_CREATEUSER, p.ApproverIndex, p.CustomerID));
            }

            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        #endregion 列表页面

        public ActionResult Edit(int id)
        {
            BindViewBag();
            ViewData["IsView"] = false;

            VMDocumentIndexing vm = _documentsIndexingService.GetDetailByID(CurrentUserServices.Me, id);

            if (DTRequest.GetQueryString("Type") == "Add")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DocumentsIndexingManagement_Maintain);
                if ((pageElementPrivileges & (int)DocumentsIndexingElementPrivileges.Maintenance) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                vm.PageType = PageTypeEnum.Add;
                ViewBag.Title = "新建单证索引";
            }
            else if (DTRequest.GetQueryString("Type") == "Detail")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> { (int)ERPPage.DocumentsIndexingManagement_Maintain,
                    (int)ERPPage.DocumentsIndexingManagement_PendingApprovalList,
                    (int)ERPPage.DocumentsIndexingManagement_PassedApprovalList,});
                if ((pageElementPrivileges & (int)DocumentsIndexingElementPrivileges.View) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                vm.PageType = PageTypeEnum.Details;
                ViewBag.Title = "查看单证索引";
                ViewData["IsView"] = true;
            }
            else if (DTRequest.GetQueryString("Type") == "Approval")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DocumentsIndexingManagement_PendingApprovalList);
                if ((pageElementPrivileges & (int)DocumentsIndexingElementPrivileges.Approval) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                vm.PageType = PageTypeEnum.Approval;
                ViewBag.Title = "审核单证索引";
                ViewData["IsView"] = true;
            }
            else
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.DocumentsIndexingManagement_Maintain);
                if ((pageElementPrivileges & (int)DocumentsIndexingElementPrivileges.Edit) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                vm.PageType = PageTypeEnum.Edit;
                ViewBag.Title = "编辑单证索引";
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult Edit(VMDocumentIndexing vm)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();

            result = _documentsIndexingService.Save(CurrentUserServices.Me, vm);

            return Json(result);
        }

        [HttpPost]
        public ActionResult MakerExcel(int ID, int MakerTypeEnum, int PurchaseID)
        {
            List<string> makeFileList = _documentsIndexingService.MakerExcel(CurrentUserServices.Me, ID, (MakerTypeEnum)MakerTypeEnum, PurchaseID);

            string files = string.Empty;

            foreach (var item in makeFileList)
            {
                if (string.IsNullOrEmpty(files))
                {
                    files = item;
                }
                else
                {
                    files += "," + item;
                }
            }

            return Content(files);
        }

        #endregion UserMethod
    }
}