using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Dictionary;
using ERP.BLL.ERP.Factory;
using ERP.BLL.ERP.ProductFitting;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.ProductFitting;
using ERP.Tools;
using ERP.WebNew.Attributes;
using ERP.WebNew.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace ERP.WebNew.Controllers
{
    [UserTrackerLog]
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.ProductManagement_ProductFitting)]
    public class ProductFittingController : Controller
    {
        private ProductFittingServices _productFittingServices = new ProductFittingServices();
        private FactoryServices _factoryServices = new FactoryServices();
        private DictionaryServices _dictionaryServices = new DictionaryServices();
        private UserServices _userService = new UserServices();

        #region UserMethod

        public ActionResult Index(VMProductFittingSearchModel searchModel)
        {
            searchModel.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ProductManagement_ProductFitting);
            return View(searchModel);
        }

        private string ReplaceNames(string name)
        {
            string dbColumnName = string.Empty;

            switch (name)
            {
                case "FactoryAbbreviation":
                    dbColumnName = "Factory.Abbreviation";
                    break;

                default:
                    dbColumnName = name;
                    break;
            }
            return dbColumnName;
        }

        public ActionResult GetAll(VMProductFittingSearchModel vm_search)
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

            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> { (int)ERPPage.ThirdParty_Inspection_AuditNotice });

            #endregion 筛选条件

            vm_search.isAll = false;

            List<VMProductFittingInfo> listModel = _productFittingServices.GetAll(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows, vm_search);

            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        public ActionResult Edit(int id)
        {
            VMProductFittingInfo productInfo = new VMProductFittingInfo();

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int>() { (int)ERPPage.ProductManagement_ProductFitting });

            if (id == 0)
            {
                #region 添加权限

                if ((id == 0 && (pageElementPrivileges & (int)ProductFittingElementPrivileges.CreateProduct) == 0))
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                productInfo.PageType = PageTypeEnum.Add;
                ViewBag.Title = "添加配件产品";
            }
            else if (DTRequest.GetQueryString("Type") == "Detail")
            {
                #region 添加权限

                if ((pageElementPrivileges & (int)ProductFittingElementPrivileges.ViewProduct) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                productInfo = _productFittingServices.GetProductByID(CurrentUserServices.Me, id);
                if (productInfo == null)
                {
                    return RedirectToAction("PageNotFind", "Account");
                }

                productInfo.PageType = PageTypeEnum.Details;
                ViewBag.Title = "查看配件产品";
            }
            else
            {
                #region 添加权限

                if ((id != 0 && (pageElementPrivileges & (int)ProductFittingElementPrivileges.EditProduct) == 0))
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                productInfo = _productFittingServices.GetProductByID(CurrentUserServices.Me, id);
                if (productInfo == null)
                {
                    return RedirectToAction("PageNotFind", "Account");
                }
                productInfo.PageType = PageTypeEnum.Edit;
                ViewBag.Title = "修改配件产品";
            }

            ViewBag.FactoryInfos = _factoryServices.GetSupplierSelectList(CurrentUserServices.Me.UserID, 1);

            var dictionaries = _dictionaryServices.GetDictionaryInfos(CurrentUserServices.Me.UserID, null);
            ViewBag.Currencies = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.Currency).ToList();

            return View(productInfo);
        }

        [HttpPost]
        public ActionResult Edit(int id, [System.Web.Http.FromBody]VMProductFittingInfo productInfo)
        {
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ProductManagement_ProductFitting);
            if ((id == 0 && (pageElementPrivileges & (int)ProductFittingElementPrivileges.CreateProduct) == 0) ||
                (id != 0 && (pageElementPrivileges & (int)ProductFittingElementPrivileges.EditProduct) == 0))
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            try
            {
                DBOperationStatus result = default(DBOperationStatus);

                if (id == 0)
                {
                    productInfo.ID = 0;
                    result = _productFittingServices.Create(CurrentUserServices.Me, productInfo);
                }
                else
                {
                    result = _productFittingServices.Save(CurrentUserServices.Me, productInfo);
                }

                return new CustomJsonResult((short)result);
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Delete([System.Web.Http.FromBody]string idList)
        {
            List<int> deleteList = null;
            if (!string.IsNullOrEmpty(idList))
            {
                deleteList = idList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
            }

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ProductManagement_ProductFitting);
            if (deleteList != null && deleteList.Count == 1 && (pageElementPrivileges & (int)ProductFittingElementPrivileges.DeleteProduct) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }
            if (deleteList != null && deleteList.Count > 1 && (pageElementPrivileges & (int)ProductFittingElementPrivileges.BatchDelete) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            DBOperationStatus result = _productFittingServices.Delete(CurrentUserServices.Me, deleteList);
            return new CustomJsonResult((short)result);
        }

        #endregion UserMethod

        #region PublicMethod

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
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ProductManagement_ProductFitting);
            if ((pageElementPrivileges & (int)QuoteElementPrivileges.View) == 0)
            {
                return "没权限！";
            }
            return null;
        }

        [HttpGet]
        public JsonResult GetProductFittingInfo(string idList, int ModuleType)
        {
            #region 添加权限

            string validMsg = ValidRequest();
            if (!string.IsNullOrEmpty(validMsg))
            {
                return Json(validMsg, JsonRequestBehavior.AllowGet);
            }

            #endregion 添加权限

            List<int> idArray = null;
            if (!string.IsNullOrEmpty(idList))
            {
                idArray = idList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
            }
            List<VMProductFittingInfo> list_vm = _productFittingServices.GetProductFittingInfo(CurrentUserServices.Me, idArray, ModuleType);
            return Json(list_vm, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetProductFittingInfoByID(int ID, int ModuleType)
        {
            List<VMProductFittingInfo> list_vm = _productFittingServices.GetProductFittingInfo(CurrentUserServices.Me, ID, ModuleType);
            return Json(list_vm, JsonRequestBehavior.AllowGet);
        }
        #endregion PublicMethod
    }
}