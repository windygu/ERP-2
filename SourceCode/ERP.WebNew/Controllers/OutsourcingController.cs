using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Customer;
using ERP.BLL.ERP.Factory;
using ERP.BLL.ERP.OutSourcing;
using ERP.BLL.Workflow;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.OutSourcing;
using ERP.Tools;
using ERP.WebNew.Attributes;
using ERP.WebNew.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ERP.WebNew.Controllers
{
    [UserTrackerLog]
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.PurchaseManagement_OutSourcing)]
    public class OutsourcingController : Controller
    {
        private OutSourcingService _OutContractService = new OutSourcingService();
        private OrderCustomerServices _orderCustomerServices = new OrderCustomerServices();
        private UserServices _userService = new UserServices();

        /// <summary>
        /// 添加测试页面()
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SendContract(int id)
        {
            DTOOutsourcing vmOuts = new DTOOutsourcing();
            vmOuts.ID = id;
            vmOuts.PageTypeID = 2;
            vmOuts = _OutContractService.GetDetailByID(CurrentUserServices.Me, vmOuts);
            ViewBag.FromAddress = CurrentUserServices.Me.Email;
            ViewBag.ToAddress = vmOuts.FactoryEmail;
            return View(vmOuts);
        }

        [HttpPost]
        public ActionResult SendContract(int id, VMSendEmail vm)
        {
            var result = _OutContractService.SendContract(CurrentUserServices.Me, id, vm);
            return Json(result);
        }

        [HttpPost]
        public ActionResult MakeExcel(int id, string extension)
        {
            DTOOutsourcing model = new DTOOutsourcing();
            model.ID = id;
            model.PageTypeID = 2;

            model = _OutContractService.GetDetailByID(CurrentUserServices.Me, model);

            string result = _OutContractService.MakeExcel(CurrentUserServices.Me, id, extension, model);
            return Content(result.ToString());
        }

        /// <summary>
        /// 验证请求
        /// </summary>
        /// <returns></returns>
        private string ValidRequest()
        {
            if (CurrentUserServices.Me == null)
            {
                return "未登陆！";
            }
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> { (int)ERPPage.PurchaseManagement_OutSourcing_Index,
                (int)ERPPage.PurchaseManagement_OutSourcing_PassedApproval });
            if ((pageElementPrivileges & (int)QuoteElementPrivileges.View) == 0)
            {
                return "没权限！";
            }
            return null;
        }

        [HttpGet]
        public JsonResult GetQuote(int id)
        {
            #region 添加权限

            string validMsg = ValidRequest();
            if (!string.IsNullOrEmpty(validMsg))
            {
                return Json(validMsg, JsonRequestBehavior.AllowGet);
            }

            #endregion 添加权限

            var model = _OutContractService.GetQuote(CurrentUserServices.Me, id);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetContacts(int id)
        {
            #region 添加权限

            string validMsg = ValidRequest();
            if (!string.IsNullOrEmpty(validMsg))
            {
                return Json(validMsg, JsonRequestBehavior.AllowGet);
            }

            #endregion 添加权限

            var model = _OutContractService.GetContacts(CurrentUserServices.Me, id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

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
        /// 获取代购公司的下拉框数据列表
        /// </summary>
        /// <param name="dataFlag"></param>
        /// <returns></returns>
        public static SelectList GetSupplierSelectList(int dataFlag)
        {
            List<SelectListItem> list = new List<SelectListItem>() { };
            list.Add(new SelectListItem() { Text = "", Value = "" });
            FactoryServices _FactorySer = new FactoryServices();
            var data = _FactorySer.GetSupplierSelectList(CurrentUserServices.Me.UserID, dataFlag);

            foreach (var item in data)
            {
                list.Add(new SelectListItem() { Value = item.ID.ToString(), Text = item.Abbreviation });
            }
            SelectList generateList = new SelectList(list, "Value", "Text");

            return generateList;
        }

        public ActionResult Index(VMFilterOC vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_OutSourcing_Index);
            vm_search.PageType = PageTypeEnum.PendingCheckList;

            //装载筛选条件中的待审核代购合同状态下拉框数据
            Dictionary<int, string> dataList = new Dictionary<int, string>();

            dataList.Add(1, "草稿");
            dataList.Add(2, "待审核");
            dataList.Add(3, "审核未通过");

            ViewData["OutContractStatus"] = GetSelectList(dataList);

            return View(vm_search);
        }

        public ActionResult PassedApproval(VMFilterOC vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_OutSourcing_PassedApproval);
            vm_search.PageType = PageTypeEnum.PassedCheckList;

            //装载筛选条件中的已审核代购合同状态下拉框数据
            Dictionary<int, string> dataList = new Dictionary<int, string>();

            dataList.Add(4, "审核已通过");
            dataList.Add(5, "合同已发送");
            dataList.Add(6, "合同已上传");

            ViewData["OutContractStatus"] = GetSelectList(dataList);

            return View(vm_search);
        }

        public ActionResult GetAll(VMFilterOC vmFilter)
        {
            int currentPage = 0, pageSize = 0, totalRows = 0;
            int.TryParse(HttpContext.Request[EasyuiPagenationConsts.CURRENT_PAGE], out currentPage);
            int.TryParse(HttpContext.Request[EasyuiPagenationConsts.PAGE_SIZE], out pageSize);

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

            List<DTOOutsourcing> products = _OutContractService.GetAll(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows, vmFilter);

            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = products });
        }

        /// <summary>
        /// 新建代购合同->返回选择采购合同视图页面
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectContract()
        {
            #region 新建权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_OutSourcing_Index);

            if ((pageElementPrivileges & (int)OutSourcingElementPrivileges.OutAdd) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 新建权限

            ViewBag.CustomerInfos = _orderCustomerServices.GetAllCustomersKeyInfo(CurrentUserServices.Me.UserID);

            return View();
        }

        /// <summary>
        /// 根据筛选条件查询审核通过的采购合同
        /// </summary>
        /// <returns></returns>
        public ActionResult GeContractDataList()
        {
            #region 排序

            int currentPage = 0, pageSize = 0, totalRows = 0;
            int.TryParse(HttpContext.Request[EasyuiPagenationConsts.CURRENT_PAGE], out currentPage);
            int.TryParse(HttpContext.Request[EasyuiPagenationConsts.PAGE_SIZE], out pageSize);

            List<string> sortColumnsNames = new List<string>();
            if (!string.IsNullOrEmpty(DTRequest.GetQueryString(EasyuiPagenationConsts.SORT_COLUMN_NAMES)))
            {
                sortColumnsNames = DTRequest.GetQueryString(EasyuiPagenationConsts.SORT_COLUMN_NAMES).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            List<string> sortColumnOrders = new List<string>();
            if (!string.IsNullOrEmpty(DTRequest.GetQueryString(EasyuiPagenationConsts.SORT_COLUMN_ORDERS)))
            {
                sortColumnOrders = DTRequest.GetQueryString(EasyuiPagenationConsts.SORT_COLUMN_ORDERS).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            #endregion 排序

            #region 筛选条件

            DTOOutsourcing vmOuts = new DTOOutsourcing();
            vmOuts.OCID = DTRequest.GetQueryInt("CustomerID", 0);
            vmOuts.FactoryAbbreviation = DTRequest.GetQueryString("FactoryAbbreviation");
            vmOuts.PurchaseNumber = DTRequest.GetQueryString("PurchaseNumber");
            vmOuts.PurchaseDateStart = DTRequest.GetQueryString("PurchaseDateStart");
            vmOuts.PurchaseDateEnd = DTRequest.GetQueryString("PurchaseDateEnd");

            #endregion 筛选条件

            List<VMPCDataList> vmpcdtList = new List<VMPCDataList>();

            vmpcdtList = _OutContractService.GetAll(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows, vmOuts);

            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = vmpcdtList });
        }

        /// <summary>
        /// 从DB中取得采购、代购合同相关数据
        /// </summary>
        /// <param name="ID">新建代购合同时，为选定采购合同自编号，其余是已存在的代购合同自编号</param>
        /// <param name="PageType">请求数据状态：1=新建；2=查看；3=编辑；4=审核；</param>
        /// <returns></returns>
        public ActionResult Edit(int ID, int PageType)
        {
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_OutSourcing_Index);
            int iPrivilegeswResult = 0;
            string sPageTitle = string.Empty;

            switch (PageType)
            {
                case 1:
                    sPageTitle = "新建代购合同";
                    iPrivilegeswResult = pageElementPrivileges & (int)OutSourcingElementPrivileges.OutAdd;
                    break;

                case 2:
                    sPageTitle = "查看代购合同";
                    iPrivilegeswResult = pageElementPrivileges & (int)OutSourcingElementPrivileges.OutWatch;
                    break;

                case 3:
                    sPageTitle = "编辑代购合同";
                    iPrivilegeswResult = pageElementPrivileges & (int)OutSourcingElementPrivileges.OutEdit;
                    break;

                case 4:
                    sPageTitle = "审核代购合同";
                    iPrivilegeswResult = pageElementPrivileges & (int)OutSourcingElementPrivileges.OutCheck;

                    if (iPrivilegeswResult > 0)
                    {
                        DTOOutsourcing vmOC = new DTOOutsourcing();
                        vmOC = _OutContractService.GetOutContractInfo(CurrentUserServices.Me, vmOC, ID);

                        bool isHasApprovalPermission = ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalOutsourcingContract, CurrentUserServices.Me, vmOC.CreateUserID, vmOC.ApproverIndex);

                        if (!isHasApprovalPermission)
                        {
                            return RedirectToAction("NoAuthPop", "Account");
                        }
                    }

                    break;

                default:
                    sPageTitle = "未定义状态";
                    break;
            }

            if (iPrivilegeswResult == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            DTOOutsourcing vmOuts = new DTOOutsourcing();

            vmOuts.PageTypeID = PageType;
            vmOuts.PageTitle = sPageTitle;
            vmOuts.ID = ID;
            vmOuts = _OutContractService.GetDetailByID(CurrentUserServices.Me, vmOuts);

            ViewData["OutCompanyList"] = GetSupplierSelectList(2);//装载代购公司下拉框数据

            //装载交货地下拉框数据
            //Dictionary<int, string> dataList = new Dictionary<int, string>();

            //dataList.Add(1, "交货地一");
            //dataList.Add(2, "交货地二");
            //dataList.Add(3, "交货地三");

            //ViewData["DeliveryList"] = GetSelectList(dataList);

            return View(vmOuts);
        }
        
        /// <summary>
        /// 保存代购合同至DB
        /// </summary>
        /// <param name="vmoc"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save(DTOOutsourcing vmoc)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            if (vmoc.OCPacksData == null)
            {
                result.IsSuccess = false;
            }
            else
            {
                string a = string.Empty;
                //新建
                if (vmoc.PageTypeID == 1)
                {
                    result = _OutContractService.Add(CurrentUserServices.Me, vmoc);
                }
                else//编辑、审核
                {
                    result = _OutContractService.Save(CurrentUserServices.Me, vmoc);
                }
            }

            return Json(result);
        }

        /// <summary>
        /// 删除未审核代购合同数据
        /// </summary>
        /// <param name="ID">代购合同自编号，多个</param>
        /// <returns></returns>
        public ActionResult DeleteData(string ID)
        {
            #region 删除权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_OutSourcing_Index);

            if ((pageElementPrivileges & (int)OutSourcingElementPrivileges.OutDelete) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 删除权限

            DBOperationStatus result = default(DBOperationStatus);

            string[] arrID = ID.Split(',');
            List<int> OCIDs = new List<int>();
            int id = 0;

            foreach (string item1 in arrID)
            {
                int.TryParse(item1, out id);
                OCIDs.Add(id);
            }

            result = _OutContractService.Delete(CurrentUserServices.Me, OCIDs);
            return new CustomJsonResult((short)result);
        }

        public ActionResult Upload(int id)
        {
            #region 上传权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_OutSourcing_PassedApproval);

            if ((id == 0 && (pageElementPrivileges & (int)PurchaseContractElementPrivileges.Create) == 0) ||
                (id != 0 && (pageElementPrivileges & (int)PurchaseContractElementPrivileges.UpLoad) == 0))
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 上传权限

            VMUpLoad vm = _OutContractService.GetUploadDetial(id);
            vm.UpLoadType = UploadFileType.OutContract;
            return View(vm);
        }

        [HttpPost]
        public ActionResult Upload(int id, VMUpLoad vm)
        {
            DBOperationStatus result = _OutContractService.SaveUpLoad(CurrentUserServices.Me, vm);
            return Content(result.ToString());
        }
    }
}