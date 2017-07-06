using ERP.BLL.Consts;
using ERP.BLL.ERP.Address;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Customer;
using ERP.BLL.ERP.DataDictionary;
using ERP.BLL.ERP.Dictionary;
using ERP.BLL.ERP.Rep;
using ERP.Models.Address;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.Customer;
using ERP.Models.DataDictionary;
using ERP.Models.Dictionary;
using ERP.Models.Rep;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using ERP.WebNew.Attributes;
using ERP.WebNew.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.WebNew.Controllers
{
    [UserTrackerLog]
    [AuthorizedUser(PageID = (int)ERPPage.CustomerManagement_List)]
    public class CustomerController : Controller
    {
        private const string QUERY_CUSTOMER_COUNTRY = "Country";
        private const string QUERY_CUSTOMER_PROVINCE = "Province";
        private const string QUERY_CUSTOMER_CODE = "CustomerCode";
        private const string QUERY_CUSTOMER_NAME = "CustomerName";

        private UserServices _userService = new UserServices();
        private AddressServices _addressServices = new AddressServices();
        private DictionaryServices _dictionaryServices = new DictionaryServices();
        private OrderCustomerServices _customerService = new OrderCustomerServices();

        public ActionResult Index()
        {
            VMCustomerSearchModel searchModel = new VMCustomerSearchModel()
            {
                PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.CustomerManagement_List),

                Country = Utils.StrToInt(HttpContext.Request[QUERY_CUSTOMER_COUNTRY], 0),
                Province = Utils.StrToInt(HttpContext.Request[QUERY_CUSTOMER_PROVINCE], 0),
                CustomerCode = HttpContext.Request[QUERY_CUSTOMER_CODE],
                CustomerName = HttpContext.Request[QUERY_CUSTOMER_NAME]
            };

            var countries = _addressServices.GetAllCountry();
            ViewData["Countries"] = GetSelectionCountries(countries);
            if (searchModel.Country != 0)
            {
                List<VMArea> provinces = _addressServices.GetAllAreaByCountryID(searchModel.Country);
                if (provinces != null && provinces.Count > 0)
                {
                    ViewData["Provinces"] = GetSelectionProvinces(provinces);
                }
            }
            else
            {
                ViewData["Provinces"] = null;
            }

            return View(searchModel);
        }

        public ActionResult GetAll()
        {
            int currentPage = 0, pageSize = 0, totalRows = 0;
            int.TryParse(HttpContext.Request[EasyuiPagenationConsts.CURRENT_PAGE], out currentPage);
            int.TryParse(HttpContext.Request[EasyuiPagenationConsts.PAGE_SIZE], out pageSize);

            int tmp = 0;
            int? country = null;
            if (!string.IsNullOrEmpty(HttpContext.Request[QUERY_CUSTOMER_COUNTRY]) && int.TryParse(HttpContext.Request[QUERY_CUSTOMER_COUNTRY], out tmp))
            {
                country = tmp;
            }
            int? province = null;
            if (!string.IsNullOrEmpty(HttpContext.Request[QUERY_CUSTOMER_PROVINCE]) && int.TryParse(HttpContext.Request[QUERY_CUSTOMER_PROVINCE], out tmp))
            {
                province = tmp;
            }
            string customerCode = HttpContext.Request[QUERY_CUSTOMER_CODE];
            string customerName = HttpContext.Request[QUERY_CUSTOMER_NAME];

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

            List<DTOOrderCustomers> products = _customerService.GetCustomers(CurrentUserServices.Me, country, province,
                customerCode, customerName, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows);
            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = products });
        }

        public ActionResult Provinces(int? ID)
        {
            List<VMArea> provinces = _addressServices.GetAllAreaByCountryID(ID);
            return new CustomJsonResult(provinces);
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

        public SelectList GetSelectDataList_ForSeason(List<Models.Dictionary.DTODictionary> di)
        {
            List<SelectListItem> list = new List<SelectListItem>() { };

            foreach (var item in di)
            {
                string SeasonName = item.Name;
                if (!string.IsNullOrEmpty(item.Alias))
                {
                    SeasonName += " - " + item.Alias;
                }

                list.Add(new SelectListItem() { Text = SeasonName, Value = item.Code.ToString() });
            }
            SelectList generateList = new SelectList(list, "Value", "Text");

            return generateList;
        }

        public ActionResult Edit(int id, bool isView = false)
        {
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.CustomerManagement_List);

            if (isView)
            {
                if ((pageElementPrivileges & (int)CustomerListElementPrivileges.ViewCustomer) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }
                ViewBag.Title = "查看客户信息";
            }
            else
            {
                if ((id == 0 && (pageElementPrivileges & (int)CustomerListElementPrivileges.CreateCustomer) == 0) ||
                    (id != 0 && (pageElementPrivileges & (int)CustomerListElementPrivileges.EditCustomer) == 0))
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }
                ViewBag.Title = id == 0 ? "新建客户信息" : "修改客户信息";
            }

            var countries = _addressServices.GetAllCountry();
            ViewData["Countries"] = GetSelectionCountries(countries);

            ViewData["Provinces"] = null;
            ViewData["AllProvinces"] = Provinces(null);
            ViewData["IsView"] = isView;
            ViewData["GetSelectionQuote"] = GetSelectionQuoteTemplate();
            ViewData["GetSelectionCustomer"] = GetSelectionCustomer();
            ViewData["Porty"] = GetSelectionPortList();
            ViewData["Rep"] = GetSelectionRepList();

            var dictionaries = _dictionaryServices.GetDictionaryInfos(CurrentUserServices.Me.UserID, null);
            List<DTODictionary> list_Season = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.Season).ToList();
            ViewData["Season"] = GetSelectDataList_ForSeason(list_Season);

            var list_ExchangeType = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.ExchangeType).ToList();

            Dictionary<int, string> di3 = new Dictionary<int, string>();
            foreach (var item in list_ExchangeType)
            {
                di3.Add(item.ID, item.Name);
            }
            ViewData["list_PaymentType"] = CommonCode.GetSelectDataList(di3);

            DTOOrderCustomers customerInfo = null;
            if (id == 0)
            {
                customerInfo = new DTOOrderCustomers();
            }
            else
            {
                customerInfo = _customerService.GetCustomerByID(CurrentUserServices.Me, id);
                // 查看和编辑的时候，均需要提前绑定Province信息
                List<VMArea> provinces = _addressServices.GetAllAreaByCountryID(customerInfo.Country);
                if (provinces != null && provinces.Count > 0)
                {
                    ViewData["Provinces"] = GetSelectionProvinces(provinces);
                }
            }
            return View(customerInfo);
        }

        [HttpPost]
        public ActionResult Edit(int id, DTOOrderCustomers customerInfo)
        {
            LogHelper.WriteLog("ID:" + id + "，Entity：" + JsonConvert.SerializeObject(customerInfo));

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.CustomerManagement_List);
            if ((id == 0 && (pageElementPrivileges & (int)CustomerListElementPrivileges.CreateCustomer) == 0) ||
                (id != 0 && (pageElementPrivileges & (int)CustomerListElementPrivileges.CreateCustomer) == 0))
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            try
            {
                customerInfo.OCID = id;

                VMAjaxProcessResult result = new VMAjaxProcessResult();
                if (id == 0)
                {
                    result = _customerService.Create(CurrentUserServices.Me.UserID, CurrentUserServices.GetCurrentRequestIPAddress(), customerInfo);
                }
                else
                {
                    result = _customerService.Update(CurrentUserServices.Me, CurrentUserServices.GetCurrentRequestIPAddress(), customerInfo);
                }
                return Json(result);
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

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.CustomerManagement_List);
            if (deleteList != null && deleteList.Count == 1 && (pageElementPrivileges & (int)CustomerListElementPrivileges.DeleteCustomer) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }
            if (deleteList != null && deleteList.Count > 1 && (pageElementPrivileges & (int)CustomerListElementPrivileges.BatchDelete) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            DBOperationStatus result = _customerService.Delete(CurrentUserServices.Me, deleteList, CurrentUserServices.GetCurrentRequestIPAddress());
            return new CustomJsonResult((short)result);
        }

        public ActionResult ContactTemplate(VMContact vmContact)
        {
            return PartialView("_PartialContactInfo", vmContact);
        }

        public ActionResult AccepTemplate(VMAcceptInformation vmAcceptInfo)
        {
            return PartialView("_PartialAcceptInfo", vmAcceptInfo);
        }

        /// <summary>
        /// 获取报价单模板列表
        /// </summary>
        /// <returns></returns>
        public static SelectList GetSelectionQuoteTemplate()
        {
            List<SelectListItem> list = new List<SelectListItem>() { };

            Dictionary<string, string> di = EnumHelper.GetCustomKeyEnums(typeof(QuotProductTypeEnum));
            foreach (var item in di)
            {
                list.Add(new SelectListItem() { Text = item.Key, Value = item.Key });
            }
            SelectList generateList = new SelectList(list, "Value", "Text");

            return generateList;
        }

        /// <summary>
        /// 获取报价单模板列表
        /// </summary>
        /// <returns></returns>
        public static SelectList GetSelectionCustomer()
        {
            List<SelectListItem> list = new List<SelectListItem>() { };

            Dictionary<string, string> di = EnumHelper.GetCustomKeyEnums(typeof(SelectCustomerEnum));
            foreach (var item in di)
            {
                list.Add(new SelectListItem() { Text = item.Key, Value = item.Key });
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

        /// <summary>
        /// 港口
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SelectList GetSelectionPortList()
        {
            List<SelectListItem> list = new List<SelectListItem>() { };
            list.Add(new SelectListItem() { Text = "全部港口", Value = "" + (int)CustomerFreightRateTypeEnum.AllPort });

            List<DTODataDictionary> di = DataDictionaryServices.selectPotyName();
            foreach (var item in di)
            {
                list.Add(new SelectListItem() { Text = item.Alias, Value = item.ID.ToString() });
            }
            SelectList generateList = new SelectList(list, "Value", "Text");

            return generateList;
        }

        /// <summary>
        /// Rep
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SelectList GetSelectionRepList()
        {
            List<SelectListItem> list = new List<SelectListItem>() { };
            list.Add(new SelectListItem() { Text = "", Value = "" });

            List<VMRep> di = new RepServices().GetRepList();
            foreach (var item in di)
            {
                list.Add(new SelectListItem() { Text = item.EmailAddress, Value = item.RepID.ToString() });
            }
            SelectList generateList = new SelectList(list, "Value", "Text");

            return generateList;
        }

        /// <summary>
        /// 获取客户的收货地址
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SelectList GetAcceptInformations(int id)
        {
            List<SelectListItem> list = new List<SelectListItem>() { };
            SelectList generateList = new SelectList(list, "Value", "Text");

            var list_AcceptInformations = _customerService.GetCustomerAcceptedList(id);
            if (list_AcceptInformations != null && list_AcceptInformations.Count() > 0)
            {
                Dictionary<int, string> di = new Dictionary<int, string>();
                foreach (var item in list_AcceptInformations)
                {
                    di.Add(item.AIID, item.FirstName + " " + item.LastName + " - " + item.CompanyName + " - " + item.StreetAddress + " - " + item.City + " - " + item.AreaName + " - " + item.PostalCode + " - " + item.CountryName);
                }
                generateList = CommonCode.GetSelectDataList(di);


            }
            return generateList;
        }
    }
}