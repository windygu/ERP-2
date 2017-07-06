using ERP.BLL.Consts;
using ERP.BLL.ERP.Address;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.DataDictionary;
using ERP.BLL.ERP.Dictionary;
using ERP.Models.Address;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.DataDictionary;
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
    [AuthorizedUser(PageID = (int)ERPPage.SystemManagement_Dictionaries)]
    public class DictionaryController : Controller
    {
        private DictionaryServices _dictionaryServices = new DictionaryServices();
        private AddressServices _addressServices = new AddressServices();
        private DataDictionaryServices _dataDictionaryServices = new DataDictionaryServices();
        private UserServices _userService = new UserServices();

        /// <summary>
        /// 绑定ViewBag
        /// </summary>
        private void BindViewBag()
        {


            ViewData["AttrName"] = DataDitionaryTBandAN();

            Dictionary<int, string> di = EnumHelper.GetCustomEnums<int>(typeof(DataFlagEnum));
            ViewData["GetSelectionDataFlag"] = CommonCode.GetSelectDataList(di);

            ViewData["GetSelectionCustomer"] = GetSelectionCustomer();

            var countries = _addressServices.GetAllCountry();
            ViewData["Countries"] = GetSelectionCountries(countries);

        }

        public static SelectList GetSelectionCustomer()
        {
            List<SelectListItem> list = new List<SelectListItem>() { };

            Dictionary<string, string> di = EnumHelper.GetCustomKeyEnums(typeof(SelectCustomerEnum));
            foreach (var item in di)
            {
                string temp = item.Key.Replace("新客户", "所有客户");
                list.Add(new SelectListItem() { Text = temp, Value = temp });
            }
            SelectList generateList = new SelectList(list, "Value", "Text");

            return generateList;
        }

        public static SelectList DataDitionaryTBandAN()
        {
            List<SelectListItem> list = new List<SelectListItem>() { };
            list.Add(new SelectListItem() { Text = "", Value = "" });

            Dictionary<int, string> di = DataDictionaryServices.GetTabkindContractStatus();
            foreach (var item in di)
            {
                list.Add(new SelectListItem() { Text = item.Value, Value = item.Key.ToString() });
            }
            SelectList generateList = new SelectList(list, "Value", "Text");
            return generateList;
        }


        /// <summary>
        /// 获取国家对应的Province列表
        /// </summary>
        /// <returns></returns>
        public static SelectList GetSelectionProvinces(List<VMArea> provinces)
        {
            List<SelectListItem> list = new List<SelectListItem>() { };
            foreach (var p in provinces)
            {
                list.Add(new SelectListItem() { Text = p.AreaName, Value = p.ARID.ToString() });
            }
            SelectList generateList = new SelectList(list, "Value", "Text");
            return generateList;
        }

        public static SelectList GetSelectionCountries(List<VMCountry> countries)
        {
            List<SelectListItem> list = new List<SelectListItem>() { };
            foreach (var p in countries)
            {
                list.Add(new SelectListItem() { Text = p.CountryName, Value = p.COID.ToString() });
            }
            SelectList generateList = new SelectList(list, "Value", "Text");
            return generateList;
        }

        public ActionResult Index(VMDataDictionary vm_search)
        {
            vm_search.PageType = PageTypeEnum.PendingCheckList;
            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SystemManagement_Dictionaries);
            ViewData["AttrName"] = DataDitionaryTBandAN();
            return View(vm_search);
        }


        public ActionResult GetAll(VMDataDictionary vm_search)
        {
            int currentPage = Utils.StrToInt(HttpContext.Request[EasyuiPagenationConsts.CURRENT_PAGE], 1);
            int pageSize = Utils.StrToInt(HttpContext.Request[EasyuiPagenationConsts.PAGE_SIZE], 1);
            int totalRows = 0;

            #region 排序

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

            #endregion 排序

            #region 筛选条件

            vm_search.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int> { (int)ERPPage.SystemManagement_Dictionaries });

            #endregion 筛选条件

            List<DTODataDictionary> listModel = _dataDictionaryServices.GetAll(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows, vm_search);

            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        public ActionResult Edit(int id)
        {
            BindViewBag();

            var vm = _dataDictionaryServices.GetDetailByID(id);
            
            if (id < 0)
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SystemManagement_Dictionaries);
                if ((pageElementPrivileges & (int)DictionaryElementPrivilrges.Add) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                ViewBag.Title = "添加数据信息";
                vm.PageType = PageTypeEnum.Add;

                vm.Country = (int)CountryEnum.USA;
            }
            else
            {
                #region 添加权限

                int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SystemManagement_Dictionaries);
                if ((pageElementPrivileges & (int)DictionaryElementPrivilrges.Edit) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }

                #endregion 添加权限

                ViewBag.Title = "修改数据信息";
                vm.PageType = PageTypeEnum.Edit;
            }

            List<VMArea> provinces = _addressServices.GetAllAreaByCountryID(vm.Country);
            if (provinces != null && provinces.Count > 0)
            {
                ViewData["Provinces"] = GetSelectionProvinces(provinces);
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult Edit(DTODataDictionary vm)
        {
            DBOperationStatus result = _dataDictionaryServices.Save(vm, CurrentUserServices.Me);
            return new CustomJsonResult((short)result);
        }

        public ActionResult Delete(string idList)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SystemManagement_Dictionaries);
            if ((pageElementPrivileges & (int)DictionaryElementPrivilrges.Delete) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            int a = int.Parse(idList);
            DBOperationStatus result = _dataDictionaryServices.Delete(a);
            return new CustomJsonResult((short)result);
        }

        public ActionResult BatchDelete(string idList)
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SystemManagement_Dictionaries);
            if ((pageElementPrivileges & (int)DictionaryElementPrivilrges.BatchDelete) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            List<int> deleteList = null;
            if (!string.IsNullOrEmpty(idList))
            {
                deleteList = idList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
            }
            DBOperationStatus result = _dataDictionaryServices.BatchDelete(deleteList);
            return new CustomJsonResult((short)result);
        }


    }
}