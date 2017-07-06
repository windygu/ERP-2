using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Customer;
using ERP.BLL.ERP.Dictionary;
using ERP.BLL.ERP.Order;
using ERP.BLL.ERP.Purchase;
using ERP.BLL.Workflow;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.Dictionary;
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
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.PurchaseManagement_ContractManagement)]
    public class PurchaseContractController : Controller
    {
        private PurchaseContractService _purchaseContractService = new PurchaseContractService();
        private OrderCustomerServices _orderCustomerServices = new OrderCustomerServices();
        private OrderService _orderService = new OrderService();
        private DictionaryServices _dictionaryServices = new DictionaryServices();
        private UserServices _userService = new UserServices();

        #region HelperMethod

        /// <summary>
        /// 绑定ViewBag
        /// </summary>
        private void BindViewBag()
        {
            ViewBag.CustomerInfos = _orderCustomerServices.GetAllCustomersKeyInfo(CurrentUserServices.Me.UserID);//选择销售订单用到了

            var dictionaries = _dictionaryServices.GetDictionaryInfos(CurrentUserServices.Me.UserID, null);
            ViewBag.Ports = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.OutPort).ToList();
        }

        /// <summary>
        /// 采购合同状态
        /// </summary>
        /// <returns></returns>
        private static SelectList GetPurchaseStatus()
        {
            List<SelectListItem> list = new List<SelectListItem>() { };
            list.Add(new SelectListItem() { Text = "", Value = "" });

            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            dictionary.Add((int)PurchaseStatusEnum.OutLine, EnumHelper.GetCustomEnumDesc(typeof(PurchaseStatusEnum), PurchaseStatusEnum.OutLine));
            dictionary.Add((int)PurchaseStatusEnum.PendingCheck, EnumHelper.GetCustomEnumDesc(typeof(PurchaseStatusEnum), PurchaseStatusEnum.PendingCheck));
            dictionary.Add((int)PurchaseStatusEnum.NotPassCheck, EnumHelper.GetCustomEnumDesc(typeof(PurchaseStatusEnum), PurchaseStatusEnum.NotPassCheck));

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
                case "FactoryAbbreviation":
                    dbColumnName = "Factory.Abbreviation";
                    break;

                case "CustomerCode":
                    dbColumnName = "Orders_Customers.CustomerCode";
                    break;

                //TODO 暂时注释
                //case "PortName":
                //    dbColumnName = "Com_DataDictionary.Name";
                //    break;

                default:
                    dbColumnName = name;
                    break;
            }
            return dbColumnName;
        }

        #endregion HelperMethod

        #region UserMethod

        public ActionResult Index(VMPurchaseSearch vm_search)
        {
            ViewData["PurchaseStatus"] = GetPurchaseStatus();

            vm_search.PageType = PageTypeEnum.PendingCheckList;
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ContractManagement_Index);

            return View(vm_search);
        }

        public ActionResult PassedCheck(VMPurchaseSearch vm_search)
        {

            vm_search.IsShowUpload2 = false;
            if (CurrentUserServices.Me.HierachyName == "业务二部")
            {
                vm_search.IsShowUpload2 = true;
            }

            vm_search.IsShowUpload3 = true;

            vm_search.PageType = PageTypeEnum.PassedCheckList;
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ContractManagement_PassedCheck);

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

            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> { (int)ERPPage.PurchaseManagement_ContractManagement_Index,
                (int)ERPPage.PurchaseManagement_ContractManagement_PassedCheck, });

            #endregion 筛选条件

            List<DTOPurchaseContract> listModel = _purchaseContractService.GetAll(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows,
             vm_search);
            if (vm_search.PageType == PageTypeEnum.PendingCheckList && listModel != null)
            {
                listModel.Where(d => d.PurchaseStatusID == (int)PurchaseStatusEnum.PendingCheck).ToList().ForEach(p => p.NextApproverInfos = ApprovalService.GetNextApproverInfos(WorkflowTypes.ApprovalPurchaseContract, p.ST_CREATEUSER, p.ApproverIndexPurchaseContract, p.CustomerID));
            }

            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        /// <summary>
        /// 批量删除采购合同
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public ActionResult BatchDelete([System.Web.Http.FromBody]string idList)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ContractManagement_Index);
            if ((pageElementPrivileges & (int)PurchaseContractElementPrivileges.BatchDelete) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            DBOperationStatus result = _purchaseContractService.Delete(CurrentUserServices.Me, CommonCode.IdListToList(idList));
            return new CustomJsonResult((short)result);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete([System.Web.Http.FromBody]int id)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ContractManagement_Index);
            if ((pageElementPrivileges & (int)QuoteElementPrivileges.Delete) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            List<int> list = new List<int>();
            list.Add(id);
            DBOperationStatus result = _purchaseContractService.Delete(CurrentUserServices.Me, list);
            return new CustomJsonResult((short)result);
        }

        /// <summary>
        /// 新建采购合同
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ContractManagement_Index);
            if ((pageElementPrivileges & (int)PurchaseContractElementPrivileges.Create) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            BindViewBag();

            return View(new VMPurchase());
        }

        [HttpPost]
        public ActionResult Add([System.Web.Http.FromBody]List<VMPurchase> vm_list)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ContractManagement_Index);
            if ((pageElementPrivileges & (int)PurchaseContractElementPrivileges.Create) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            VMAjaxProcessResult result = _purchaseContractService.Add(CurrentUserServices.Me, vm_list);

            return Json(result);
        }

        /// <summary>
        /// 新建采购合同——获取选中订单的数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetALL_Add(int OrderID)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ContractManagement_Index);
            if ((pageElementPrivileges & (int)PurchaseContractElementPrivileges.View) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            List<VMPurchase> listModel = _purchaseContractService.GetALL_Add(CurrentUserServices.Me, OrderID);
            return Json(listModel);
        }

        public ActionResult Edit(int id)
        {
            BindViewBag();

            VMPurchase vm = _purchaseContractService.GetDetailByID(CurrentUserServices.Me, id);

            if (DTRequest.GetQueryString("Type") == "Detail")
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> {
                        (int)ERPPage.PurchaseManagement_ContractManagement_Index,
                        (int)ERPPage.PurchaseManagement_ContractManagement_PassedCheck});
                if ((pageElementPrivileges & (int)PurchaseContractElementPrivileges.View) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                ViewBag.Title = "查看采购合同";
                vm.PageType = PageTypeEnum.Details;
            }
            else
            {
                switch (vm.PurchaseStatus)
                {
                    case (int)PurchaseStatusEnum.OutLine:
                    case (int)PurchaseStatusEnum.NotPassCheck:

                        #region 添加权限

                        int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ContractManagement_Index);
                        if ((pageElementPrivileges & (int)PurchaseContractElementPrivileges.Edit) == 0)
                        {
                            return RedirectToAction("NoAuthPop", "Account");
                        }

                        #endregion 添加权限

                        ViewBag.Title = "编辑采购合同";
                        vm.PageType = PageTypeEnum.Edit;
                        break;

                    case (int)PurchaseStatusEnum.PendingCheck:

                        #region 添加权限

                        pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ContractManagement_Index);
                        if ((pageElementPrivileges & (int)PurchaseContractElementPrivileges.Check) == 0)
                        {
                            return RedirectToAction("NoAuthPop", "Account");
                        }

                        #endregion 添加权限

                        #region 判断是否有审批流的权限

                        bool isHasApprovalPermission = ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalPurchaseContract, CurrentUserServices.Me, vm.ST_CREATEUSER, vm.ApproverIndexPurchaseContract, vm.CustomerID);
                        if (!isHasApprovalPermission)
                        {
                            return RedirectToAction("NoAuthPop", "Account");
                        }

                        #endregion 判断是否有审批流的权限

                        ViewBag.Title = "审核采购合同";
                        vm.PageType = PageTypeEnum.Approval;
                        break;
                }
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult Edit([System.Web.Http.FromBody]VMPurchase vm)
        {
            if (vm.PurchaseStatus == (int)PurchaseStatusEnum.NotPassCheck || vm.PurchaseStatus == (int)PurchaseStatusEnum.PassedCheck)
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ContractManagement_Index);
                if ((pageElementPrivileges & (int)PurchaseContractElementPrivileges.Check) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限
            }
            else
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ContractManagement_Index);
                if ((pageElementPrivileges & (int)PurchaseContractElementPrivileges.Edit) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限
            }
            VMAjaxProcessResult result = _purchaseContractService.Save(CurrentUserServices.Me, vm);
            ViewBag.status = DBOperationStatus.Success;

            BindViewBag();

            return Json(result);
        }

        public ActionResult UpLoad(int id)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ContractManagement_PassedCheck);
            if ((pageElementPrivileges & (int)PurchaseContractElementPrivileges.UpLoad) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            VMUpLoad vm = _purchaseContractService.GetUploadDetial(id);
            return View(vm);
        }

        [HttpPost]
        public ActionResult UpLoad(int id, VMUpLoad vm)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ContractManagement_PassedCheck);
            if ((pageElementPrivileges & (int)PurchaseContractElementPrivileges.UpLoad) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            DBOperationStatus result = _purchaseContractService.SaveUpLoad(CurrentUserServices.Me, vm);
            return Json(result.ToString());
        }

        public ActionResult UpLoad_MakeMoney(int id)
        {
            VMUpLoad vm = _purchaseContractService.GetUploadDetial_MakeMoney(id);

            ViewBag.Title = "上传请款合同";
            if (vm.SelectCustomer == SelectCustomerEnum.S288.ToString())
            {
                ViewBag.Title = "上传付款明细";
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult UpLoad_MakeMoney(int id, VMUpLoad vm)
        {
            DBOperationStatus result = _purchaseContractService.SaveUpLoad_MakeMoney(CurrentUserServices.Me, vm);
            return Json(result.ToString());
        }
        public ActionResult SendContract(int id)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ContractManagement_PassedCheck);
            if ((pageElementPrivileges & (int)PurchaseContractElementPrivileges.SendContract) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            VMPurchase vm = _purchaseContractService.GetDetailByID(CurrentUserServices.Me, id);
            ViewBag.FromAddress = CurrentUserServices.Me.Email;
            ViewBag.ToAddress = vm.FactoryEmail;
            return View(vm);
        }

        [HttpPost]
        public ActionResult SendContract(int id, VMSendEmail vm)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ContractManagement_PassedCheck);
            if ((pageElementPrivileges & (int)PurchaseContractElementPrivileges.SendContract) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            var result = _purchaseContractService.SendContract(CurrentUserServices.Me, id, vm);
            return Json(result);
        }

        [HttpPost]
        public ActionResult MakeExcel(int id,string extension)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_ContractManagement_PassedCheck);
            if ((pageElementPrivileges & (int)PurchaseContractElementPrivileges.UpLoad) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            string result = _purchaseContractService.MakeExcel(CurrentUserServices.Me, id, extension);
            return Json(result);
        }

        #endregion UserMethod

        #region TemplateMethod

        [HttpPost]
        public ActionResult FactoryTemplate(VMPurchase vm)
        {
            vm.PurchaseDate = Utils.DateTimeToStr(DateTime.Now);
            BindViewBag();

            return PartialView("_PartialFactory", vm);
        }

        #endregion TemplateMethod
    }
}