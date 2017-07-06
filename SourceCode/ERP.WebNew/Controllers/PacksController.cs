using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Dictionary;
using ERP.BLL.ERP.Packs;
using ERP.BLL.Workflow;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.Packs;
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
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.PurchaseManagement_PackManagement)]
    public class PacksController : Controller
    {
        private PacksServices _PacksService = new PacksServices();
        private DictionaryServices _dictionaryServices = new DictionaryServices();
        private UserServices _userService = new UserServices();

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
                case "FactoryAbbreviation":
                    dbColumnName = "Factory.Abbreviation";
                    break;

                case "CustomerCode":
                    dbColumnName = "Orders_Customers.CustomerCode";
                    break;

                case "PacksUpdateDateFormatter":
                    dbColumnName = "PacksUpdateDate";
                    break;

                default:
                    dbColumnName = name;
                    break;
            }
            return dbColumnName;
        }

        /// <summary>
        /// 生成包装资料数据状态下拉框数据
        /// </summary>
        /// <returns></returns>
        private static SelectList GetPacksStatus()
        {
            List<SelectListItem> list = new List<SelectListItem>() { };
            list.Add(new SelectListItem() { Text = "", Value = "" });

            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            dictionary.Add(1, "待维护");
            dictionary.Add(2, "草稿");
            dictionary.Add(3, "待审核");
            dictionary.Add(4, "审核未通过");

            foreach (var item in dictionary)
            {
                list.Add(new SelectListItem() { Text = item.Value, Value = item.Key.ToString() });
            }
            SelectList generateList = new SelectList(list, "Value", "Text");

            return generateList;
        }

        #endregion HelperMethod

        public ActionResult Index(VMFilterPacks vm)
        {
            vm.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_Packs_PendingApproval);
            vm.PageType = PageTypeEnum.PendingCheckList;

            ViewData["PacksStatus"] = GetPacksStatus();

            return View(vm);
        }

        public ActionResult PassedApproval(VMFilterPacks vm)
        {
            vm.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_Packs_PassedApproval);
            vm.PageType = PageTypeEnum.PassedCheckList;

            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            dictionary.Add(5, "审核已通过");
            dictionary.Add(6, "已上传");

            ViewData["PacksStatus"] = CommonCode.GetSelectDataList(dictionary);

            return View(vm);
        }

        /// <summary>
        /// 查询并返回待审核包装资料列表数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAll(VMFilterPacks vm_search)
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
                (int)ERPPage.PurchaseManagement_Packs_PendingApproval,
                (int)ERPPage.PurchaseManagement_Packs_PassedApproval, });

            #endregion 筛选条件

            List<VMPacks> products = _PacksService.GetAll(vm_search, CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows);

            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = products });
        }

        /// <summary>
        /// 根据采购合同自编号，查出采购合同产品信息供标签附加产品选择使用
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPurchaseProducts(int ID)
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

            List<DTOPackProducts> pContractProducts = _PacksService.GetPurchaseProducts(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows, ID);

            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = pContractProducts });
        }
        
        /// <summary>
        /// 获取维护包装资料组成部分所需要的数据
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(int ContractID)
        {
            VMPacks vm = new VMPacks();
            PageTypeEnum pageType = PageTypeEnum.Edit;

            if (DTRequest.GetQueryString("Type") == "Detail")
            {
                #region 操作权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> { (int)ERPPage.PurchaseManagement_Packs_PendingApproval,
                (int)ERPPage.PurchaseManagement_Packs_PassedApproval, });
                if ((pageElementPrivileges & (int)PacksElementsPrivileges.View) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 操作权限

                pageType = PageTypeEnum.Details;
                ViewBag.Title = "查看标签资料";
            }
            else if (DTRequest.GetQueryString("Type") == "Approval")
            {
                #region 操作权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_Packs_PendingApproval);
                if ((pageElementPrivileges & (int)PacksElementsPrivileges.Check) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 操作权限

                pageType = PageTypeEnum.Approval;
                ViewBag.Title = "审核标签资料";
            }
            else
            {
                #region 操作权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_Packs_PendingApproval);
                if ((pageElementPrivileges & (int)PacksElementsPrivileges.Edit) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 操作权限

                pageType = PageTypeEnum.Edit;
                ViewBag.Title = "编辑标签资料";
            }

            vm = _PacksService.GetDetailByID(CurrentUserServices.Me, ContractID, pageType);
            vm.PurchaseContractID = ContractID;
            vm.PageType = pageType;

            ViewBag.Packing = _dictionaryServices.GetDictionaryInfos(CurrentUserServices.Me.UserID, DictionaryTableKind.Packing);
            return View(vm);
        }

        /// <summary>
        ///保存采购合同编辑包装资料信息，包括标签、产品、采购合同对应产品UPC
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="PacksStatus"></param>
        /// <param name="packs"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save(VMPacks packs)
        {
            DBOperationStatus result = default(DBOperationStatus);
            if (packs == null)
            {
                result = DBOperationStatus.NoAffect;
            }
            else
            {
                if (packs.PageType == PageTypeEnum.Add)
                {
                    if (packs.PacksList == null || packs.PHProductsUPC == null)
                    {
                        result = DBOperationStatus.NoAffect;
                    }
                }
                else
                {
                    if (packs.PacksProducts == null)
                    {
                        result = DBOperationStatus.NoAffect;
                    }
                }
            }

            if (result != DBOperationStatus.NoAffect)
            {
                result = _PacksService.Save(CurrentUserServices.Me, packs);
            }

            return new CustomJsonResult((short)result);
        }

        public ActionResult Upload(int ContractID)
        {
            #region 操作权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_Packs_PassedApproval);

            if ((pageElementPrivileges & (int)PacksElementsPrivileges.Upload) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 操作权限

            VMPacks vmPacks = new VMPacks();
            vmPacks.PurchaseContractID = ContractID;
            vmPacks = _PacksService.GetDetailByID(CurrentUserServices.Me, ContractID, PageTypeEnum.Upload);

            return View(vmPacks);
        }

        /// <summary>
        /// 保存包装资料标签上传附件数据至DB
        /// </summary>
        /// <param name="packs"></param>
        /// <returns></returns>
        public ActionResult Save_Upload(VMPacks packs)
        {
            DBOperationStatus result = default(DBOperationStatus);
            if (packs == null)
            {
                result = DBOperationStatus.NoAffect;
            }
            else
            {
                result = _PacksService.Save_Upload(CurrentUserServices.Me, packs);
            }

            return new CustomJsonResult((short)result);
        }

        public ActionResult PacksDownload(int ContractID)
        {
            #region 操作权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.PurchaseManagement_Packs_PassedApproval);

            if ((pageElementPrivileges & (int)PacksElementsPrivileges.Download) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            string sFilesPath = string.Empty;
            sFilesPath = _PacksService.PacksDownload(ContractID, sFilesPath);

            #endregion 操作权限

            return new CustomJsonResult(sFilesPath);
        }

        

        public ActionResult ChangeStatus(int id)
        {
            DBOperationStatus result = _PacksService.Save_ChangeStatus(CurrentUserServices.Me, id);

            return new CustomJsonResult(result);
        }

        [HttpPost]
        public JsonResult GetPacksProductList(int ContractID)
        {
            VMPacks vmPacks = _PacksService.GetPacksProductList(CurrentUserServices.Me, ContractID, 3);
            vmPacks.PurchaseContractID = ContractID;

            return Json(vmPacks);
        }
    }
}