using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Customer;
using ERP.BLL.ERP.DataDictionary;
using ERP.BLL.ERP.Dictionary;
using ERP.BLL.ERP.Order;
using ERP.BLL.Workflow;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.DataDictionary;
using ERP.Models.Dictionary;
using ERP.Models.Order;
using ERP.Models.Product;
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
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.SaleManagement_OrderManagement)]
    public class OrderController : Controller
    {
        private UserServices _userService = new UserServices();
        private OrderCustomerServices _orderCustomerServices = new OrderCustomerServices();
        private DictionaryServices _dictionaryServices = new DictionaryServices();
        private OrderService _orderService = new OrderService();

        #region HelperMethod

        /// <summary>
        /// 获取状态列表
        /// </summary>
        /// <returns></returns>
        public static SelectList GetSelectListStatus()
        {
            List<SelectListItem> list = new List<SelectListItem>() { };
            list.Add(new SelectListItem() { Text = "", Value = "" });

            Dictionary<int, string> dictionary = EnumHelper.GetCustomEnums<int>(typeof(OrderStatusEnum));

            foreach (var item in dictionary)
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
            ViewBag.Ports = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.OutPort).ToList();
            ViewBag.GoalPort = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.GoalPort).ToList();
            List<DTODictionary> list_Department = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.Department).ToList();
            List<DTODictionary> list_color = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.Color).ToList();

            ViewData["Department"] = GetSelectDataList(list_Department);
            //ViewData["Color"] = GetSelectDataList2(list_color);

            Dictionary<short, KeyValuePair<string, string>> enumValues = EnumHelper.GetEnumKeyValuesWithDescription<short>(typeof(TransportTypeEnum));
            Dictionary<int, string> di3 = new Dictionary<int, string>();
            foreach (var item in enumValues)
            {
                di3.Add(item.Key, item.Value.Value);
            }
            ViewData["list_TransportType"] = CommonCode.GetSelectDataList(di3);
        }

        public SelectList GetSelectDataList(List<Models.Dictionary.DTODictionary> di)
        {
            List<SelectListItem> list = new List<SelectListItem>() { };

            foreach (var item in di)
            {
                list.Add(new SelectListItem() { Text = item.Alias, Value = item.Code.ToString() });
            }
            SelectList generateList = new SelectList(list, "Value", "Text");

            return generateList;
        }

        public SelectList GetSelectDataList2(List<Models.Dictionary.DTODictionary> di)
        {
            List<SelectListItem> list = new List<SelectListItem>() { };

            foreach (var item in di)
            {
                list.Add(new SelectListItem() { Text = item.Name, Value = item.Code.ToString() });
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
            ViewData["OrderStatus"] = GetSelectListStatus();

            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_OrderManagement_Index);
            vm_search.PageType = PageTypeEnum.PendingCheckList;

            return View(vm_search);
        }

        public ActionResult PassedApproval(VMOrderSearch vm_search)
        {
            ViewData["OrderStatus"] = GetSelectListStatus();

            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_OrderManagement_PassedApproval);
            vm_search.PageType = PageTypeEnum.PassedApproval;

            vm_search.IsShow_BuyingConfirmation = false;
            if (CurrentUserServices.Me.HierachyName == "业务一部")
            {
                vm_search.IsShow_BuyingConfirmation = true;
            }

            //vm_search.IsShowSC = false;
            //if (CurrentUserServices.Me.HierachyName == "业务五部"|| CurrentUserServices.Me.HierachyName == "业务二部")
            //{
            vm_search.IsShowSC = true;
            //}
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
                (int)ERPPage.SaleManagement_OrderManagement_Index,
                (int)ERPPage.SaleManagement_OrderManagement_PassedApproval });

            #endregion 筛选条件

            List<DTOOrder> listModel = _orderService.GetAll(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows, vm_search);
            if (vm_search.PageType == PageTypeEnum.PendingCheckList && listModel != null)
            {
                listModel.Where(d => d.OrderStatusID == (int)OrderStatusEnum.PendingApproval).ToList().ForEach(p => p.NextApproverInfos = ApprovalService.GetNextApproverInfos(WorkflowTypes.ApprovalOrder, p.ST_CREATEUSER, p.ApproverIndex, p.CustomerID));
            }
            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BatchDelete([System.Web.Http.FromBody]string idList)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_OrderManagement_Index);
            if ((pageElementPrivileges & (int)OrderElementPrivileges.BatchDelete) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            DBOperationStatus result = _orderService.Delete(CurrentUserServices.Me, CommonCode.IdListToList(idList));
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

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_OrderManagement_Index);
            if ((pageElementPrivileges & (int)OrderElementPrivileges.Delete) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            List<int> list = new List<int>();
            list.Add(id);
            DBOperationStatus result = _orderService.Delete(CurrentUserServices.Me, list);
            return new CustomJsonResult((short)result);
        }

        public ActionResult ViewProductList()
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_OrderManagement_Index);
            if ((pageElementPrivileges & (int)OrderElementPrivileges.ViewProductList) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            string idList = DTRequest.GetQueryString("id");
            List<VMViewProductList> list_vm = _orderService.GetViewProductList(CurrentUserServices.Me, CommonCode.IdListToList(idList));
            ViewBag.id = idList;
            return View(list_vm);
        }

        public ActionResult Edit(int id, bool openCached = false)
        {
            BindViewBag();

            VMOrderEdit vm = null;

            if (id == -1)
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_OrderManagement_Index);
                if ((pageElementPrivileges & (int)OrderElementPrivileges.Create) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                ViewBag.Title = "新建订单核算表";
                if (openCached)
                {
                    vm = _orderService.GetOneCachedOrder(CurrentUserServices.Me.UserID, id);
                }
                else
                {
                    vm = new VMOrderEdit();
                    vm.CustomerID = -1;
                }
                vm.PageType = PageTypeEnum.Add;
                vm.OrderID = -1;
            }
            else
            {
                if (openCached)
                {
                    vm = _orderService.GetOneCachedOrder(CurrentUserServices.Me.UserID, id);
                }
                else
                {
                    vm = _orderService.GetDetailByID(CurrentUserServices.Me, id);
                }

                if (DTRequest.GetQueryString("Type") == "Detail")
                {
                    #region 添加权限

                    int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> {
                        (int)ERPPage.SaleManagement_OrderManagement_Index,
                        (int)ERPPage.SaleManagement_OrderManagement_PassedApproval});
                    if ((pageElementPrivileges & (int)OrderElementPrivileges.View) == 0)
                    {
                        return RedirectToAction("NoAuthPop", "Account");
                    }

                    #endregion 添加权限

                    ViewBag.Title = "查看销售订单";
                    vm.PageType = PageTypeEnum.Details;
                }
                else
                {
                    switch (vm.OrderStatusID)
                    {
                        case (int)OrderStatusEnum.OutLine:
                        case (int)OrderStatusEnum.NotPassApproval:

                            #region 添加权限

                            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_OrderManagement_Index);
                            if ((pageElementPrivileges & (int)OrderElementPrivileges.Edit) == 0)
                            {
                                return RedirectToAction("NoAuthPop", "Account");
                            }

                            #endregion 添加权限

                            ViewBag.Title = "编辑销售订单";
                            vm.PageType = PageTypeEnum.Edit;
                            break;

                        case (int)OrderStatusEnum.PendingApproval:

                            #region 添加权限

                            pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_OrderManagement_Index);
                            if ((pageElementPrivileges & (int)OrderElementPrivileges.Check) == 0)
                            {
                                return RedirectToAction("NoAuthPop", "Account");
                            }

                            #endregion 添加权限

                            #region 判断是否有审批流的权限

                            bool isHasApprovalPermission = ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalOrder, CurrentUserServices.Me, vm.ST_CREATEUSER, vm.ApproverIndex);
                            if (!isHasApprovalPermission)
                            {
                                return RedirectToAction("NoAuthPop", "Account");
                            }

                            #endregion 判断是否有审批流的权限

                            ViewBag.Title = "审批销售订单";
                            vm.PageType = PageTypeEnum.Approval;
                            break;
                    }
                }
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult Edit([System.Web.Http.FromBody]VMOrderEdit vm)
        {
            BindViewBag();

            VMAjaxProcessResult result = default(VMAjaxProcessResult);
            if (vm.OrderID == -1)
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_OrderManagement_Index);
                if ((pageElementPrivileges & (int)OrderElementPrivileges.Create) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                result = _orderService.Add(CurrentUserServices.Me, vm);
            }
            else
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int>{
                    (int)ERPPage.SaleManagement_OrderManagement_Index
                });
                if ((pageElementPrivileges & (int)OrderElementPrivileges.Edit) == 0 && (pageElementPrivileges & (int)OrderElementPrivileges.Check) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                result = _orderService.Save(CurrentUserServices.Me, vm);
            }

            return Json(result);
        }

        //TODO 临时添加的
        public ActionResult TempCustoms()
        {
            return View();
        }

        public ActionResult TempS05()
        {
            return View();
        }

        public ActionResult TempF20()
        {
            return View();
        }

        public ActionResult TempS13()
        {
            return View();
        }

        public ActionResult TempS235()
        {
            return View();
        }

        public ActionResult TempS220()
        {
            return View();
        }

        public ActionResult TempS60()
        {
            return View();
        }

        public ActionResult TempDG()
        {
            return View();
        }

        public ActionResult TempS135()
        {
            return View();
        }

        public ActionResult TempS164()
        {
            return View();
        }

        public ActionResult TempS10()
        {
            return View();
        }

        public ActionResult TempS56()
        {
            return View();
        }

        public ActionResult KOTest()
        {
            return View();
        }

        public ActionResult AngularJSTemp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SaveDraft([System.Web.Http.FromBody]VMOrderEdit vm)
        {
            if (vm.OrderID == -1)
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_OrderManagement_Index);
                if ((pageElementPrivileges & (int)OrderElementPrivileges.Create) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限
            }
            else
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int>{
                    (int)ERPPage.SaleManagement_OrderManagement_Index
                });
                if ((pageElementPrivileges & (int)OrderElementPrivileges.Edit) == 0 && (pageElementPrivileges & (int)OrderElementPrivileges.Check) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限
            }

            try
            {
                bool result = _orderService.CacheOneOrder(CurrentUserServices.Me.UserID, vm);

                return new CustomJsonResult(result);
            }
            catch
            {
                return new CustomJsonResult(false);
            }
        }

        [HttpPost]
        public ActionResult Create_BuyingConfirmation([System.Web.Http.FromBody]string idList)
        {
            var result = _orderService.Create_BuyingConfirmation(CurrentUserServices.Me, idList);
            return Json(result);
        }

        [HttpPost]
        public JsonResult UpLoad_BC(int id)
        {
            VMUpLoad vm = _orderService.GetUploadDetial(id);
            return Json(vm);
        }

        public ActionResult UpLoad(int id)
        {
            VMUpLoad vm = _orderService.GetUploadDetial(id);
            return View(vm);
        }

        [HttpPost]
        public ActionResult UpLoad(int id, VMUpLoad vm)
        {
            DBOperationStatus result = _orderService.UpLoad(CurrentUserServices.Me, id, vm);
            return Json(result.ToString());
        }

        public ActionResult UpLoad_Order(int id)
        {
            VMUpLoad vm = _orderService.GetUploadDetial_Order(id);
            return View(vm);
        }

        [HttpPost]
        public ActionResult UpLoad_Order(int id, VMUpLoad vm)
        {
            DBOperationStatus result = _orderService.UpLoad_Order(CurrentUserServices.Me, id, vm);
            return Json(result.ToString());
        }
        [HttpPost]
        public ActionResult DownLoadBC(int id)
        {
            var result = _orderService.DownLoadBC(CurrentUserServices.Me, id);
            return Json(result);
        }

        [HttpPost]
        public ActionResult DownLoadSC(int id)
        {
            var result = _orderService.DownLoadSC(CurrentUserServices.Me, id);
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetProducts_Mixed(int ID)
        {
            var list = _orderService.GetProducts_Mixed(CurrentUserServices.Me, ID);
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        #endregion UserMethod

        #region PublicMethod

        /// <summary>
        /// 多场合下选择销售订单
        /// </summary>
        /// <param name="usingOccasion"></param>
        /// <returns></returns>
        public ActionResult SelectOrder(VMSelectOrder vm)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int>{
                (int)ERPPage.SaleManagement_OrderManagement_Index
            });
            if ((pageElementPrivileges & (int)OrderElementPrivileges.View) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

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

            List<DTOOrder> listModel = _orderService.SelectOrder(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows, vm);

            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        #endregion PublicMethod
    }
}