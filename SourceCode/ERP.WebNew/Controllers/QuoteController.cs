using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Customer;
using ERP.BLL.ERP.DataDictionary;
using ERP.BLL.ERP.Dictionary;
using ERP.BLL.ERP.Factory;
using ERP.BLL.ERP.Product;
using ERP.BLL.ERP.Quote;
using ERP.BLL.Workflow;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.DataDictionary;
using ERP.Models.Product;
using ERP.Models.Quote;
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
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.SaleManagement_QuoteManagement)]
    public class QuoteController : Controller
    {
        private QuoteService _quoteService = new QuoteService();
        private ProductServices _productServices = new ProductServices();
        private OrderCustomerServices _orderCustomerServices = new OrderCustomerServices();
        private FactoryServices _factoryServices = new FactoryServices();

        private DictionaryServices _dictionaryServices = new DictionaryServices();

        private UserServices _userService = new UserServices();

        #region HelperMethod

        /// <summary>
        /// 获取状态列表
        /// </summary>
        /// <returns></returns>
        public static SelectList GetSelectListStatus()
        {
            List<SelectListItem> list = new List<SelectListItem>() { };
            list.Add(new SelectListItem() { Text = "", Value = "" });

            Dictionary<int, string> di = EnumHelper.GetCustomEnums<int>(typeof(QuoteStatusEnum));

            foreach (var item in di)
            {
                list.Add(new SelectListItem() { Text = item.Value, Value = item.Key.ToString() });
            }
            SelectList generateList = new SelectList(list, "Value", "Text");

            return generateList;
        }

        /// <summary>
        /// 绑定ViewBag
        /// </summary>
        private void BindViewBag()
        {
            ViewBag.CustomerInfos = _orderCustomerServices.GetAllCustomersKeyInfo(CurrentUserServices.Me.UserID);

            var dictionaries = _dictionaryServices.GetDictionaryInfos(CurrentUserServices.Me.UserID, null);
            ViewBag.Units = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.ProductUnit).ToList();
            ViewBag.Styles = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.ProductStyle).ToList();
            ViewBag.Ports = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.OutPort).ToList();
            ViewBag.Packings = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.Packing).ToList();
            ViewBag.Currencies = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.Currency).ToList();

            ViewData["Porty"] = GetSelectionPortList();

            Dictionary<int, string> di = _factoryServices.GetAllFactoryKeyInfo2();
            ViewBag.FactoryList = CommonCode.GetSelectDataList(di);
        }

        /// <summary>
        /// 港口
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SelectList GetSelectionPortList()
        {
            List<SelectListItem> list = new List<SelectListItem>() { };

            List<DTODataDictionary> di = DataDictionaryServices.selectPotyName();
            foreach (var item in di)
            {
                list.Add(new SelectListItem() { Text = item.Alias, Value = item.ID.ToString() });
            }
            SelectList generateList = new SelectList(list, "Value", "Text");

            return generateList;
        }

        /// <summary>
        /// 验证请求
        /// </summary>
        /// <returns></returns>
        private ApiResult ValidRequest(QuoteElementPrivileges quoteElementPrivileges)
        {
            ApiResult result = new ApiResult();

            if (CurrentUserServices.Me == null)
            {
                result.Success = false;
                result.Info = "未登录！";
                return result;
            }
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int>{
                (int)ERPPage.SaleManagement_QuoteManagement_QuoteList,
                (int)ERPPage.SaleManagement_QuoteManagement_QuoteApproval
            });
            if ((pageElementPrivileges & (int)quoteElementPrivileges) == 0)
            {
                result.Success = false;
                result.Info = "没权限！";
                return result;
            }

            result.Success = true;
            return result;
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
                case "CustomerCode":
                    dbColumnName = "Orders_Customers.CustomerCode";
                    break;

                case "AuthorName":
                    dbColumnName = "AuthorID";
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

        public ActionResult Index(VMQuoteSearch vm_search)
        {
            ViewData["Status"] = GetSelectListStatus();

            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_QuoteManagement_QuoteList);
            vm_search.PageType = PageTypeEnum.List;
            return View(vm_search);
        }

        public ActionResult PassedCheck(VMQuoteSearch vm_search)
        {
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_QuoteManagement_QuoteApproval);
            vm_search.PageType = PageTypeEnum.PendingCheckList;
            return View(vm_search);
        }

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAll(VMQuoteSearch vm_search)
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
                (int)ERPPage.SaleManagement_QuoteManagement_QuoteList,
                (int)ERPPage.SaleManagement_QuoteManagement_QuoteApproval});

            #endregion 筛选条件

            List<DTOQuote> listModel = _quoteService.GetAll(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows,
             vm_search);

            if (vm_search.PageType == PageTypeEnum.PendingCheckList && listModel != null)
            {
                listModel.ForEach(p => p.NextApproverInfos = ApprovalService.GetNextApproverInfos(WorkflowTypes.ApprovalQuot, p.ST_CREATEUSER, p.ApproverIndex,p.CustomerID));
            }

            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public ActionResult Delete([System.Web.Http.FromBody]string idList)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_QuoteManagement_QuoteList);
            if ((pageElementPrivileges & (int)QuoteElementPrivileges.Delete) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            List<int> deleteList = null;
            if (!string.IsNullOrEmpty(idList))
            {
                deleteList = idList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
            }
            DBOperationStatus result = _quoteService.Delete(CurrentUserServices.Me, deleteList);
            return new CustomJsonResult((short)result);
        }

        public ActionResult Edit(int id, bool openCached = false)
        {
            BindViewBag();
            VMQuoteEdit vm = null;

            ViewBag.ShowSuggest = false;

            if (id == -1)
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_QuoteManagement_QuoteList);
                if ((pageElementPrivileges & (int)QuoteElementPrivileges.Create) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                ViewBag.Title = "新建报价单";
                if (openCached)
                {
                    vm = _quoteService.GetOneCachedQuote(CurrentUserServices.Me.UserID, id);
                }
                else
                {
                    vm = new VMQuoteEdit();
                }
                vm.PageType = PageTypeEnum.Add;
                vm.ID = -1;
                vm.StatusID = (int)QuoteStatusEnum.OutLine;
            }
            else
            {
                if (openCached)
                {
                    vm = _quoteService.GetOneCachedQuote(CurrentUserServices.Me.UserID, id);
                }
                else
                {
                    vm = _quoteService.GetDetailByID(CurrentUserServices.Me, id);
                }

                if (DTRequest.GetQueryString("Type") == "Detail")
                {
                    #region 添加权限

                    int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_QuoteManagement_QuoteList);
                    if ((pageElementPrivileges & (int)QuoteElementPrivileges.View) == 0)
                    {
                        return RedirectToAction("NoAuthPop", "Account");
                    }

                    #endregion 添加权限

                    ViewBag.Title = "查看报价单";
                    vm.PageType = PageTypeEnum.Details;
                }
                else if (DTRequest.GetQueryString("Type") == "Copy")
                {
                    #region 添加权限

                    int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_QuoteManagement_QuoteList);
                    if ((pageElementPrivileges & (int)QuoteElementPrivileges.CopyQuote) == 0)
                    {
                        return RedirectToAction("NoAuthPop", "Account");
                    }

                    #endregion 添加权限

                    ViewBag.Title = "复制报价单";
                    vm.PageType = PageTypeEnum.Copy;

                    vm.StatusID = (int)QuoteStatusEnum.OutLine;
                }
                else
                {
                    vm.PageType = PageTypeEnum.Edit;
                    switch (vm.StatusID)
                    {
                        case (int)QuoteStatusEnum.OutLine:
                        case (int)QuoteStatusEnum.NotPassCheck:

                            #region 添加权限

                            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_QuoteManagement_QuoteList);
                            if ((pageElementPrivileges & (int)QuoteElementPrivileges.Edit) == 0)
                            {
                                return RedirectToAction("NoAuthPop", "Account");
                            }

                            #endregion 添加权限

                            ViewBag.Title = "编辑报价单";
                            break;

                        case (int)QuoteStatusEnum.PendingCheck:

                            #region 添加权限

                            pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_QuoteManagement_QuoteApproval);
                            if ((pageElementPrivileges & (int)QuoteElementPrivileges.Check) == 0)
                            {
                                return RedirectToAction("NoAuthPop", "Account");
                            }

                            #endregion 添加权限

                            #region 判断是否有审批流的权限

                            bool isHasApprovalPermission = ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalQuot, CurrentUserServices.Me, vm.ST_CREATEUSER, vm.ApproverIndex);
                            if (!isHasApprovalPermission)
                            {
                                return RedirectToAction("NoAuthPop", "Account");
                            }

                            #endregion 判断是否有审批流的权限

                            ViewBag.Title = "审核报价单";
                            ViewBag.ShowSuggest = true;
                            break;

                        case (int)QuoteStatusEnum.ReQutes:
                        case (int)QuoteStatusEnum.HadSend:

                            #region 添加权限

                            pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_QuoteManagement_QuoteList);
                            if ((pageElementPrivileges & (int)QuoteElementPrivileges.ReQuote) == 0)
                            {
                                return RedirectToAction("NoAuthPop", "Account");
                            }

                            #endregion 添加权限

                            ViewBag.Title = "重新报价";
                            break;
                    }
                }
            }

            return View(vm);
        }

        [HttpPost]
        public JsonResult Edit(VMQuoteEdit vm)
         {
            ApiResult result = new ApiResult();
            if (vm.ID == -1)//添加
            {
                #region 添加权限

                ApiResult validResult = ValidRequest(QuoteElementPrivileges.Create);
                if (!validResult.Success)
                {
                    return Json(validResult, JsonRequestBehavior.AllowGet);
                }

                #endregion 添加权限
                 
                result = _quoteService.Add(CurrentUserServices.Me, vm);
            }
            else if (vm.IsCopy)
            {
                #region 添加权限

                ApiResult validResult = ValidRequest(QuoteElementPrivileges.CopyQuote);
                if (!validResult.Success)
                {
                    return Json(validResult, JsonRequestBehavior.AllowGet);
                }

                #endregion 添加权限

                result = _quoteService.Add(CurrentUserServices.Me, vm);
            }
            else
            {
                if (vm.StatusID == (int)QuoteStatusEnum.PendingCheck)
                {
                    #region 添加权限

                    ApiResult validResult = ValidRequest(QuoteElementPrivileges.Check);
                    if (!validResult.Success)
                    {
                        return Json(validResult, JsonRequestBehavior.AllowGet);
                    }

                    #endregion 添加权限
                }
                else
                {
                    #region 添加权限

                    ApiResult validResult = ValidRequest(QuoteElementPrivileges.Edit);
                    if (!validResult.Success)
                    {
                        return Json(validResult, JsonRequestBehavior.AllowGet);
                    }

                    #endregion 添加权限
                }
                result = _quoteService.Save(CurrentUserServices.Me, vm);
            }

            BindViewBag();

            return Json(result);
        }

        //报价单模板
        public ActionResult Template(int id)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_QuoteManagement_QuoteList);
            if ((pageElementPrivileges & (int)QuoteElementPrivileges.View) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            ViewBag.id = id;
            var match = _quoteService.GetQuoteTemplatePath(CurrentUserServices.Me, id);
            return View(match);
        }

        //报价单模板 下载
        public ActionResult TemplateDownLoad(int id)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_QuoteManagement_QuoteList);
            if ((pageElementPrivileges & (int)QuoteElementPrivileges.View) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            bool isExist = _quoteService.TemplateDownLoad(id);
            if (isExist)
            {
                return View();
            }
            else
            {
                return RedirectToAction("FileNotFind", "Account");
            }
        }

        public ActionResult ViewProductList()
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_QuoteManagement_QuoteList);
            if ((pageElementPrivileges & (int)QuoteElementPrivileges.View) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            string idList = DTRequest.GetQueryString("id");
            List<VMViewProductList> list_vm = _quoteService.GetViewProductList(CurrentUserServices.Me, CommonCode.IdListToList(idList));
            ViewBag.id = idList;
            return View(list_vm);
        }

        public ActionResult SendEmail(int id)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_QuoteManagement_QuoteList);
            if ((pageElementPrivileges & (int)QuoteElementPrivileges.SendEmail) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            ViewBag.id = id;
            ViewBag.FromAddress = CurrentUserServices.Me.Email;

            return View();
        }

        public ActionResult Abandon(int id)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_QuoteManagement_QuoteList);
            if ((pageElementPrivileges & (int)QuoteElementPrivileges.Abandon) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            DBOperationStatus result = _quoteService.Save_ChangeStatus(CurrentUserServices.Me, id, QuoteStatusEnum.HadInvalid);

            return new CustomJsonResult(result);
        }

        public ActionResult Confirm(int id)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_QuoteManagement_QuoteList);
            if ((pageElementPrivileges & (int)QuoteElementPrivileges.ConfirmQuote) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            DBOperationStatus result = _quoteService.Save_ChangeStatus(CurrentUserServices.Me, id, QuoteStatusEnum.HadConfirm);

            return new CustomJsonResult(result);
        }

        [HttpPost]
        public JsonResult SendEmail(VMQuoteSendEmail model)
        {
            #region 添加权限

            ApiResult validResult = ValidRequest(QuoteElementPrivileges.SendEmail);
            if (!validResult.Success)
            {
                return Json(validResult, JsonRequestBehavior.AllowGet);
            }

            #endregion 添加权限

            ApiResult result = _quoteService.SendEmail(CurrentUserServices.Me, model);

            BindViewBag();

            return Json(result);
        }

        [HttpGet]
        public JsonResult GetQuote(int id, bool openCached = false)
        {
            #region 添加权限

            ApiResult validResult = ValidRequest(QuoteElementPrivileges.View);
            if (!validResult.Success)
            {
                return Json(validResult, JsonRequestBehavior.AllowGet);
            }

            #endregion 添加权限

            var model = new VMQuoteEdit();
            if (openCached)
            {
                model = _quoteService.GetOneCachedQuote(CurrentUserServices.Me.UserID, id);
            }
            else
            {
                model = _quoteService.GetDetailByID(CurrentUserServices.Me, id);
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetQuoteProducts(int id)
        {
            #region 添加权限

            ApiResult validResult = ValidRequest(QuoteElementPrivileges.View);
            if (!validResult.Success)
            {
                return Json(validResult, JsonRequestBehavior.AllowGet);
            }

            #endregion 添加权限

            var match = _quoteService.GetQuoteProducts(CurrentUserServices.Me, id, false);
            return Json(match, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetQuoteDeletedProducts(int id)
        {
            #region 添加权限

            ApiResult validResult = ValidRequest(QuoteElementPrivileges.View);
            if (!validResult.Success)
            {
                return Json(validResult, JsonRequestBehavior.AllowGet);
            }

            #endregion 添加权限

            var match = _quoteService.GetQuoteProducts(CurrentUserServices.Me, id, true);
            return Json(match, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetQuoteProductHistories(int id)
        {
            #region 添加权限

            ApiResult validResult = ValidRequest(QuoteElementPrivileges.View);
            if (!validResult.Success)
            {
                return Json(validResult, JsonRequestBehavior.AllowGet);
            }

            #endregion 添加权限

            var model = _quoteService.GetQuoteProductHistories(CurrentUserServices.Me, id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetQuoteHistories(int id)
        {
            #region 添加权限

            ApiResult validResult = ValidRequest(QuoteElementPrivileges.View);
            if (!validResult.Success)
            {
                return Json(validResult, JsonRequestBehavior.AllowGet);
            }

            #endregion 添加权限

            var model = _quoteService.GetQuoteHistories(CurrentUserServices.Me, id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCustomer(int CustomerID)
        {
            #region 添加权限

            ApiResult validResult = ValidRequest(QuoteElementPrivileges.View);
            if (!validResult.Success)
            {
                return Json(validResult, JsonRequestBehavior.AllowGet);
            }

            #endregion 添加权限

            var model = _quoteService.GetCustomer(CurrentUserServices.Me, CustomerID);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetContacts(int id)
        {
            #region 添加权限

            ApiResult validResult = ValidRequest(QuoteElementPrivileges.View);
            if (!validResult.Success)
            {
                return Json(validResult, JsonRequestBehavior.AllowGet);
            }

            #endregion 添加权限

            var model = _quoteService.GetContacts(CurrentUserServices.Me, id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MakeExcel(int id)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_QuoteManagement_QuoteList);
            if ((pageElementPrivileges & (int)QuoteElementPrivileges.SendEmail) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            var result = _quoteService.MakeExcel(CurrentUserServices.Me, id);
            return Json(result);
        }

        [HttpPost]
        public ActionResult SaveDraft([System.Web.Http.FromBody]VMQuoteEdit vm)
        {
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_QuoteManagement);
            if ((vm.ID == -1 && (pageElementPrivileges & (int)QuoteElementPrivileges.Create) == 0) ||
                (vm.ID != -1 && (pageElementPrivileges & (int)QuoteElementPrivileges.Edit) == 0) ||
                (vm.ID != -1 && (pageElementPrivileges & (int)QuoteElementPrivileges.Check) == 0) ||
                (vm.ID != -1 && (pageElementPrivileges & (int)QuoteElementPrivileges.ReQuote) == 0) ||
                (vm.ID != -1 && (pageElementPrivileges & (int)QuoteElementPrivileges.CopyQuote) == 0))
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            try
            {
                bool result = _quoteService.CacheOneQuote(CurrentUserServices.Me.UserID, vm);

                return new CustomJsonResult(result);
            }
            catch
            {
                return new CustomJsonResult(false);
            }
        }

        [HttpGet]
        public JsonResult GetProducts_Mixed(int ID)
        {
            var list = _quoteService.GetProducts_Mixed(CurrentUserServices.Me, ID);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #endregion UserMethod

        #region PublicMethod

        private string ReplaceNames(string name)
        {
            string dbColumnName = string.Empty;

            switch (name)
            {
                case "AuthorName":
                    dbColumnName = "AuthorID";
                    break;

                case "CustomerCode":
                    dbColumnName = "Orders_Customers.CustomerCode";
                    break;

                case "ValidDateFormat":
                    dbColumnName = "ValidDate";
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

        //销售订单页面用到了
        [HttpGet]
        public ActionResult SelectQuote(VMSelectQuote vm)
        {
            #region 添加权限

            ApiResult validResult = ValidRequest(QuoteElementPrivileges.View);
            if (!validResult.Success)
            {
                return Json(validResult, JsonRequestBehavior.AllowGet);
            }

            #endregion 添加权限

            int currentPage = DTRequest.GetQueryInt(EasyuiPagenationConsts.CURRENT_PAGE, 1);
            int pageSize = DTRequest.GetQueryInt(EasyuiPagenationConsts.PAGE_SIZE, Keys.DEFAULT_PAGE_SIZE);
            int totalRows = 0;

            #region 排序

            List<string> sortColumnsNames = new List<string>();
            if (!string.IsNullOrEmpty(DTRequest.GetQueryString(EasyuiPagenationConsts.SORT_COLUMN_NAMES)))
            {
                var sortColumnsNamesTmp = DTRequest.GetQueryString(EasyuiPagenationConsts.SORT_COLUMN_NAMES).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                // 需要替换转换过的属性
                foreach (var n in sortColumnsNamesTmp)
                {
                    sortColumnsNames.Add(ReplaceNames(n));
                }
            }
            List<string> sortColumnOrders = new List<string>();
            if (!string.IsNullOrEmpty(DTRequest.GetQueryString(EasyuiPagenationConsts.SORT_COLUMN_ORDERS)))
            {
                sortColumnOrders = DTRequest.GetQueryString(EasyuiPagenationConsts.SORT_COLUMN_ORDERS).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            #endregion 排序

            #region 筛选条件

            string No = DTRequest.GetQueryString("No");
            string strCustomerID = DTRequest.GetQueryString("CustomerID");
            string FactoryName = DTRequest.GetQueryString("FactoryName");
            string strIncludeQuoteProducts = DTRequest.GetQueryString("IncludeQuoteProducts");

            bool includeQuoteProducts = false;
            bool.TryParse(strIncludeQuoteProducts, out includeQuoteProducts);

            int? customerID = null;
            int tmp = 0;
            if (int.TryParse(strCustomerID, out tmp))
            {
                customerID = tmp;
            }

            List<string> productNOs = null;
            if (!string.IsNullOrEmpty(No))
            {
                productNOs = No.Split(new string[] { ";", "；", ",", "，", " ", Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            #endregion 筛选条件

            List<VMQuoteEdit> listModel = _quoteService.SelectQuote(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows,
             vm);

            //var model = _quoteService.SelectQuote(CurrentUserServices.Me, vm);
            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        #endregion PublicMethod
    }
}