using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Dictionary;
using ERP.BLL.ERP.Shipment;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.Shipment;
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
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.ShipmentManagement_Agencies)]
    public class ShipmentAgencyController : Controller
    {
        private const string QUERY_AGENCY_NAME = "ShippingAgencyName";
        ShipmentAgencyServices _shipmentAgencyServices = new ShipmentAgencyServices();
        UserServices _userService = new UserServices();
        DictionaryServices _dictionaryServices = new DictionaryServices();

        public ActionResult Index(VMShipmentAgencySearchModel searchModel)
        {
            searchModel.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ShipmentManagement_Agencies);
            return View(searchModel);
        }

        public ActionResult GetAll(VMShipmentAgencySearchModel searchModel)
        {
            int currentPage = 0, pageSize = 0, totalRows = 0;
            int.TryParse(HttpContext.Request[EasyuiPagenationConsts.CURRENT_PAGE], out currentPage);
            int.TryParse(HttpContext.Request[EasyuiPagenationConsts.PAGE_SIZE], out pageSize);

            string agencyName = HttpContext.Request[QUERY_AGENCY_NAME];

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

            List<VMShipmentAgency> agencies = _shipmentAgencyServices.GetAgencies(CurrentUserServices.Me, agencyName, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows);
            if (agencies != null)
            {
                var dataForEasyUI = agencies.Select(p => new
                {
                    ShippingAgencyID = p.ShippingAgencyID,
                    ShippingAgencyName = p.ShippingAgencyName,
                    Currency = p.CurrentShipmentAgentFees.Currency,
                    FeeDocument = p.CurrentShipmentAgentFees.FeeDocument,
                    FeeDockOperation = p.CurrentShipmentAgentFees.FeeDockOperation,
                    FeeYangShanPicking = p.CurrentShipmentAgentFees.FeeYangShanPicking,
                    FeeFacilityManagement = p.CurrentShipmentAgentFees.FeeFacilityManagement,
                    FeePortSecurity = p.CurrentShipmentAgentFees.FeePortSecurity,
                    FeeImporterSecurityClassify = p.CurrentShipmentAgentFees.FeeImporterSecurityClassify,
                    FeeWarehousing = p.CurrentShipmentAgentFees.FeeWarehousing,
                    FeePicking = p.CurrentShipmentAgentFees.FeePicking,
                    FeeCustomDeclaration = p.CurrentShipmentAgentFees.FeeCustomDeclaration,
                });
                return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = dataForEasyUI });
            }
            return new CustomJsonResult();
        }

        public ActionResult Edit(int ID, bool isView = false)
        {
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ShipmentManagement_Agencies);
            if ((pageElementPrivileges & (int)ShipmentAgencyElementPrivileges.Edit) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            if (isView)
            {
                ViewBag.Title = "查看船代公司信息";
            }
            else
            {
                ViewBag.Title = ID == 0 ? "新建船代公司信息" : "修改船代公司信息";
            }
            ViewData["IsView"] = isView;
            var currencies = _dictionaryServices.GetDictionaryInfos(CurrentUserServices.Me.UserID, DictionaryTableKind.Currency).Select(p => p.Name).ToList();
            ViewData["CurrencyType"] = currencies;

            VMShipmentAgency agency = null;
            if (ID == 0)
            {
                agency = new VMShipmentAgency();
                agency.CurrentShipmentAgentFees = new DTOShipmentAgentFees();
            }
            else
            {
                agency = _shipmentAgencyServices.GetAgencyByID(CurrentUserServices.Me, ID);
            }

            return View(agency);
        }

        [HttpPost]
        public ActionResult Edit(int ID, VMShipmentAgency agency)
        {
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ShipmentManagement_Agencies);
            if ((pageElementPrivileges & (int)ShipmentAgencyElementPrivileges.Edit) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            try
            {
                DBOperationStatus result = default(DBOperationStatus);
                if (ID == 0)
                {
                    result = _shipmentAgencyServices.Create(CurrentUserServices.Me.UserID, agency);
                }
                else
                {
                    result = _shipmentAgencyServices.Update(CurrentUserServices.Me, agency);
                }
                return new CustomJsonResult((short)result);
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Delete(string idList)
        {
            List<int> deleteList = null;
            if (!string.IsNullOrEmpty(idList))
            {
                deleteList = idList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
            }

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ShipmentManagement_Agencies);
            if (deleteList != null && deleteList.Count == 1 && (pageElementPrivileges & (int)ShipmentAgencyElementPrivileges.Delete) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }
            if (deleteList != null && deleteList.Count > 1 && (pageElementPrivileges & (int)ShipmentAgencyElementPrivileges.BatchDelete) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            DBOperationStatus result = _shipmentAgencyServices.Delete(CurrentUserServices.Me, deleteList);
            return new CustomJsonResult((short)result);
        }
    }
}