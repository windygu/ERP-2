using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Dictionary;
using ERP.BLL.ERP.Factory;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.Country;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.Dictionary;
using ERP.Models.Factory;
using ERP.Tools;
using ERP.WebNew.Attributes;
using ERP.WebNew.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace ERP.WebNew.Controllers
{
    [UserTrackerLog]
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.FactoryManagement_List)]
    public class FactoryController : Controller
    {
        private FactoryServices _FactoryServices = new FactoryServices();
        private const string sCallPeople = "CallPeople";
        private const string sName = "Name";
        private const string sDataFlag = "DataFlag";
        private UserServices _userService = new UserServices();
        private DictionaryServices _dictionaryServices = new DictionaryServices();

        public void BindViewBag()
        {

            ViewData["Data"] = SelectAllProvince();
            ViewData["Da"] = GetOutStatus();
            ViewData["Work"] = GetBusinessID();

            var dictionaries = _dictionaryServices.GetDictionaryInfos(CurrentUserServices.Me.UserID, null);
            ViewBag.Currencies = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.Currency).ToList();
        }
        // GET: /Factory/
        /// <summary>
        /// 工厂主页面，列表页面显示
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            VMDTOFactory vm = new VMDTOFactory()
            {
                PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.FactoryManagement_List),
                Name = DTRequest.GetQueryString(sName),
                CallPeople = DTRequest.GetQueryString(sCallPeople),
                DataFlag = DTRequest.GetQueryInt(sDataFlag),
            };
            ViewData["Da"] = GetOutStatus();
            return View(vm);
        }

        public ActionResult GetAll()
        {
            int currentPage = ERP.Tools.Utils.StrToInt(HttpContext.Request[EasyuiPagenationConsts.CURRENT_PAGE], 1);
            int pageSize = ERP.Tools.Utils.StrToInt(HttpContext.Request[EasyuiPagenationConsts.PAGE_SIZE], 1);
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

            #region 筛选

            string Name = DTRequest.GetQueryString(sName);
            string CallPeople = DTRequest.GetQueryString(sCallPeople);
            string DataFlag = DTRequest.GetQueryString(sDataFlag);

            #endregion 筛选

            List<DTOFactory> listModel = _FactoryServices.SelectAll(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows, CallPeople, Name, DataFlag);

            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        /// <summary>
        ///编辑页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.FactoryManagement_List);
            if ((id == 0 && (pageElementPrivileges & (int)FactoryElementPrivileges.FactoryAdd) == 0) ||
                (id != 0 && (pageElementPrivileges & (int)FactoryElementPrivileges.FactoryEdit) == 0))
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            List<DTOFactory> list = _FactoryServices.selectByID(id);

            ViewData["List"] = list;
            BindViewBag();

            return View();
        }

        [HttpPost]
        public ActionResult EditFactory(FormCollection form)
        {
            DTOFactory dto = new DTOFactory();
            dto.ID = int.Parse(form["ID"]);
            dto.Name = form["Name"].Trim();
            dto.Abbreviation = form["Abbreviation"].Trim();
            dto.CallPeople = form["CallPeople"].Trim();
            dto.Duty = form["Duty"].Trim();
            dto.Telephone = form["Telephone"].Trim();
            dto.Cellphone = form["Cellphone"].Trim();
            dto.Fax = form["Fax"].Trim();
            dto.EmailAdress = form["EmailAdress"].Trim();

            string tempArr = form["Select2"];
            if (form["Select2"] == null)
            {
                dto.AreaID = 0;
                dto.CityAreaID = 0;
            }
            else
            {
                string[] sss = tempArr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                int a = sss.Length;
                if (a == 1)
                {
                    dto.AreaID = int.Parse(sss[0]);
                    dto.CityAreaID = 0;
                }
                else
                {
                    dto.AreaID = int.Parse(sss[0]);
                    dto.CityAreaID = int.Parse(sss[1]);
                }
            }
            dto.ProvinceID = int.Parse(form["Province"]);
            dto.F_Address = form["adress"];
            dto.DataFlag = int.Parse(form["DataFlag"]);
            if (form["Hierachy"].ToString() == "")
            {
            }
            else
            {
                dto.Hierachy = int.Parse(form["Hierachy"]);
            }

            if (!string.IsNullOrEmpty(form["RegisterFees"].ToString()))
            {
                dto.RegisterFees = Utils.StrToDecimal(form["RegisterFees"].ToString(), 0);
            }
            dto.EnglishName = form["EnglishName"];
            dto.EnglishAddress = form["EnglishAddress"];
            dto.CurrencyType = Utils.StrToInt(form["CurrencyType"], 0);

            DBOperationStatus result = _FactoryServices.UpdateFactory(dto, CurrentUserServices.Me.UserID);
            return new CustomJsonResult((short)result);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.FactoryManagement_List);
            if ((pageElementPrivileges & (int)FactoryElementPrivileges.FactoryAdd) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            BindViewBag();

            return View();
        }

        public static SelectList SelectAllProvince()
        {
            List<SelectListItem> listt = new List<SelectListItem>() { };
            listt.Add(new SelectListItem() { Text = "", Value = "" });

            FactoryServices fac = new FactoryServices();
            List<ChinseProvince> list = fac.selectProvince();

            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            foreach (var item in list)
            {
                dictionary.Add(item.ARID, item.ProvinceName);
            }

            foreach (var item in dictionary)
            {
                listt.Add(new SelectListItem() { Text = item.Value, Value = item.Key.ToString() });
            }

            SelectList generateList = new SelectList(listt, "Value", "Text");
            return generateList;
        }

        [HttpPost]
        public ActionResult SelectByCity(int id)
        {
            ViewData["Data1"] = SelectCity(id);

            var jsondata = Newtonsoft.Json.JsonConvert.SerializeObject(SelectCity(id));
            return Json(jsondata);
        }

        public static SelectList SelectCity(int id)
        {
            List<SelectListItem> listt = new List<SelectListItem>() { };
            FactoryServices fac = new FactoryServices();
            List<ChinseCity> list = fac.selectCity(id);

            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            foreach (var item in list)
            {
                dictionary.Add(item.CIID, item.cityName);
            }

            foreach (var item in dictionary)
            {
                listt.Add(new SelectListItem() { Text = item.Value, Value = item.Key.ToString() });
            }
            SelectList generateListt = new SelectList(listt, "Value", "Text");
            return generateListt;
        }

        [HttpPost]
        public ActionResult SelectByArea(int id)
        {
            var jsondata = Newtonsoft.Json.JsonConvert.SerializeObject(SelectArea(id));
            return Json(jsondata);
        }

        public static SelectList SelectArea(int id)
        {
            List<SelectListItem> listt = new List<SelectListItem>() { };
            //listt.Add(new SelectListItem() { Text = "", Value = "" });
            FactoryServices fac = new FactoryServices();
            List<ChinseArea> list = fac.selectArea(id);

            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            foreach (var item in list)
            {
                dictionary.Add(item.DIID, item.AreaName);
            }

            foreach (var item in dictionary)
            {
                listt.Add(new SelectListItem() { Text = item.Value, Value = item.Key.ToString() });
            }
            SelectList generateListtt = new SelectList(listt, "Value", "Text");
            return generateListtt;
        }

        public static SelectList GetOutStatus()
        {
            List<SelectListItem> list = new List<SelectListItem>() { };
            list.Add(new SelectListItem() { Text = "", Value = "" });
            Dictionary<int, string> di = FactoryServices.GetContractStatus();
            foreach (var item in di)
            {
                list.Add(new SelectListItem() { Text = item.Value, Value = item.Key.ToString() });
            }

            SelectList generateList = new SelectList(list, "Value", "Text");

            return generateList;
        }

        public static SelectList GetBusinessID()
        {
            List<SelectListItem> list = new List<SelectListItem>() { };
            list.Add(new SelectListItem() { Text = "", Value = "" });

            UserServices us = new UserServices();
            List<VMHierarchy> listt = us.GetAllHierarchies(HierachyType.Agency);

            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            foreach (var item in listt)
            {
                dictionary.Add(item.HierarchyID, item.Name);
            }

            foreach (var item in dictionary)
            {
                list.Add(new SelectListItem() { Text = item.Value, Value = item.Key.ToString() });
            }
            SelectList generateList = new SelectList(list, "Value", "Text");

            return generateList;
        }

        [HttpPost]
        public ActionResult AddFactory(FormCollection form)
        {
            DTOFactory dto = new DTOFactory();
            dto.Province = form["Province"];
            dto.City = form["Province"];

            string tempArr = form["Select2"];
            if (form["Select2"] == null)
            {
                dto.Area = "";
                dto.CityArea = "";
            }
            else
            {
                string[] sss = tempArr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                int A = sss.Length;
                if (A == 1)
                {
                    dto.Area = sss[0];
                }
                else
                {
                    dto.Area = sss[0];
                    dto.CityArea = (sss[1]);
                }
            }
            if (form["Select3"] == null)
            {
                dto.CityArea = "";
            }
            else
            {
                dto.CityArea = form["Select3"];
            }
            dto.Abbreviation = form["Abbreviation"];
            dto.CallPeople = form["CallPeople"];
            dto.Cellphone = form["Cellphone"];
            dto.Fax = form["Fax"];
            dto.EmailAdress = form["EmailAdress"];
            dto.F_Address = form["adress"];
            dto.Duty = form["Duty"];
            dto.Telephone = form["Telephone"];
            dto.Name = form["Name"];
            dto.DataFlag = int.Parse(form["DataFlag"]);
            dto.Duty = form["Duty"];
            if (form["Hierachy"].ToString() == "")
            {
            }
            else
            {
                dto.Hierachy = int.Parse(form["Hierachy"]);
            }

            if (!string.IsNullOrEmpty(form["RegisterFees"].ToString()))
            {
                dto.RegisterFees = Utils.StrToDecimal(form["RegisterFees"].ToString(), 0);
            }
            dto.EnglishName = form["EnglishName"];
            dto.EnglishAddress = form["EnglishAddress"];
            dto.CurrencyType = Utils.StrToInt(form["CurrencyType"], 0);

            DBOperationStatus result = _FactoryServices.AddFactory(dto, CurrentUserServices.Me.UserID);
            return new CustomJsonResult((short)result);
        }

        [HttpPost]
        public ActionResult Delete(string idList)
        {
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.CustomerManagement_List);
            if ((pageElementPrivileges & (int)FactoryElementPrivileges.FactoryDelete) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }
            List<int> deleteList = null;
            if (!string.IsNullOrEmpty(idList))
            {
                deleteList = idList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
            }
            DBOperationStatus result = _FactoryServices.DeleteFactory(deleteList);
            return new CustomJsonResult((short)result);
        }
    }
}