using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Customer;
using ERP.BLL.ERP.Sample;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.Sample;
using ERP.WebNew.Attributes;
using ERP.WebNew.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ERP.WebNew.Controllers
{
    [UserTrackerLog]
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.SaleManagement_SampleManagement)]
    public class SampleController : Controller
    {
        //筛选条件元素id
        #region
        private const string SFilterCustomerCode = "CustomerCode";
        private const string SFilterFactoryAbbreviation = "FactoryAbbreviation";
        private const string SFilterProducts = "Products";
        private const string SFilterSampleStatus = "SampleStatus";

        private const string SFilterQuotDateStart = "QuotDateStart";
        private const string SFilterQuotDateEnd = "QuotDateEnd";
        private const string SFilterOrderDateStart = "OrderDateStart";
        private const string SFilterOrderDateEnd = "OrderDateEnd";
        private const string SFilterIssueDateStart = "IssueDateStart";
        private const string SFilterIssueDateEnd = "IssueDateEnd";
        private const string SFilterPFDateStart = "PFDateStart";
        private const string SFilterPFDateEnd = "PFDateEnd";
        private const string SFilterStatusIntervalStart = "StatusIntervalStart";
        private const string SFilterStatusIntervalEnd = "StatusIntervalEnd";

        #endregion

        private SampleService _sampleService = new SampleService();
        private OrderCustomerServices _orderCustomerServices = new OrderCustomerServices();
        private UserServices _userService = new UserServices();

        // GET: Sample
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 用指定数据装载成SelectList
        /// </summary>
        /// <returns></returns>
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
        /// 根据指定客户自编号查询收货地址列表信息
        /// </summary>
        /// <param name="ocid">客户自编号</param>
        /// <returns></returns>
        private SelectList GetCASelectList(int ocid)
        {
            List<SelectListItem> list = new List<SelectListItem>() { };
            list.Add(new SelectListItem() { Text = "", Value = "" });
            var data = _orderCustomerServices.GetCustomerAcceptedList(ocid);

            foreach (var item in data)
            {
                list.Add(new SelectListItem() { Value = item.AIID.ToString(), Text = item.AcceptedDetail });
            }
            SelectList generateList = new SelectList(list, "Value", "Text");

            return generateList;
        }

        /// <summary>
        /// 获取筛选条件的VM
        /// </summary>
        /// <param name="vmFilterSample">筛选条件VM对象</param>
        /// <param name="pageName">功能页面枚举</param>
        /// <param name="iPageValue">权限页面枚举成员值</param>
        /// <returns></returns>
        private VMFilterSample GetFilterVM(VMFilterSample vmFilterSample, PageTypeEnum pageName, int iPageValue)
        {
            VMFilterSample vm = new VMFilterSample();
            //vm = GetFilterDataVM(vm);

            vm.PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, iPageValue);//添加权限
            vm.PageType = pageName;

            return vm;
        }

        /// <summary>
        /// 待打样列表页面
        /// </summary>
        /// <param name="vmFilterSample">筛选条件VM对象</param>
        /// <returns></returns>
        public ActionResult Manufacturing(VMFilterSample vmFilterSample)
        {
            //装载筛选条件中的待审核代印合同状态下拉框数据
            Dictionary<int, string> dataList = new Dictionary<int, string>();

            dataList.Add(1, "待接单");
            dataList.Add(2, "已接单");

            ViewData["SampleStatus"] = GetSelectList(dataList);

            return View(GetFilterVM(vmFilterSample, PageTypeEnum.ManufacturingPage, (int)ERPPage.SaleManagement_SampleManagement_Manufacturing));
        }

        /// <summary>
        /// 已打样列表页面
        /// </summary>
        /// <param name="vmFilterSample">筛选条件VM对象</param>
        /// <returns></returns>
        public ActionResult Manufactured(VMFilterSample vmFilterSample)
        {
            //装载筛选条件中的待审核代印合同状态下拉框数据
            Dictionary<int, string> dataList = new Dictionary<int, string>();

            dataList.Add(3, "正在生产");
            dataList.Add(4, "生产完成");
            dataList.Add(5, "样品已确认");

            ViewData["SampleStatus"] = GetSelectList(dataList);

            return View(GetFilterVM(vmFilterSample, PageTypeEnum.ManufacturedPage, (int)ERPPage.SaleManagement_SampleManagement_Manufactured));
        }

        /// <summary>
        /// 待寄样列表页面
        /// </summary>
        /// <param name="vmFilterSample">筛选条件VM对象</param>
        /// <returns></returns>
        public ActionResult Sending(VMFilterSample vmFilterSample)
        {
            //装载筛选条件中的待审核代印合同状态下拉框数据
            Dictionary<int, string> dataList = new Dictionary<int, string>();
            dataList.Add(6, "待寄出");
            dataList.Add(7, "汇集中");

            ViewData["SampleStatus"] = GetSelectList(dataList);

            return View(GetFilterVM(vmFilterSample, PageTypeEnum.SendingPage, (int)ERPPage.SaleManagement_SampleManagement_Sending));
        }

        /// <summary>
        /// /已寄样列表页面
        /// </summary>
        /// <param name="vmFilterSample">筛选条件VM对象</param>
        /// <returns></returns>
        public ActionResult Sended(VMFilterSample vmFilterSample)
        {
            //装载筛选条件中的待审核代印合同状态下拉框数据
            //Dictionary<int, string> dataList = new Dictionary<int, string>();
            //dataList.Add(1, "");
            //dataList.Add(2, "");

            //ViewData["SampleStatus"] = GetSelectList(dataList);

            return View(GetFilterVM(vmFilterSample, PageTypeEnum.SendedPage, (int)ERPPage.SaleManagement_SampleManagement_Sent));
        }

        /// <summary>
        /// 查询并返回寄样信息列表数据
        /// </summary>
        /// <param name="vmFilter">筛选条件视图模型</param>
        /// <returns></returns>
        public ActionResult GetSampleDataList(VMFilterSample vmFilter)
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
            List<DTOSample> vm = new List<DTOSample>();

            int iSampleStatusS = 0, iSampleStatusE = 0;
            switch (vmFilter.PageType)
            {
                case PageTypeEnum.ManufacturingPage:
                    iSampleStatusS = (int)SampleStatus.SampleStatus1;
                    iSampleStatusE = (int)SampleStatus.SampleStatus2;
                    break;

                case PageTypeEnum.ManufacturedPage:
                    iSampleStatusS = (int)SampleStatus.SampleStatus3;
                    iSampleStatusE = (int)SampleStatus.SampleStatus5;
                    break;

                case PageTypeEnum.SendingPage:
                    iSampleStatusS = (int)SampleStatus.SampleStatus6;
                    iSampleStatusE = (int)SampleStatus.SampleStatus7;
                    break;

                case PageTypeEnum.SendedPage:
                    iSampleStatusS = (int)SampleStatus.SampleStatus8;
                    iSampleStatusE = (int)SampleStatus.SampleStatus10;
                    break;

                default:
                    iSampleStatusS = 0;
                    iSampleStatusE = 0;
                    break;
            }
            vmFilter.StatusIntervalStart = iSampleStatusS;
            vmFilter.StatusIntervalEnd = iSampleStatusE;

            vm = _sampleService.GetManufactureDataList(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows, vmFilter);
            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = vm });
        }

        /// <summary>
        /// 新建打样、查看打样、安排生产
        /// </summary>
        /// <param name="PageType"></param>
        /// <returns></returns>
        public ActionResult GetCreateWayData(int SSID, int CreateWay, int PageType)
        {
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_SampleManagement_Manufacturing);
            int iPrivilegeswResult = 0;
            string sPageTitle = string.Empty;

            switch (PageType)
            {
                case 1:
                    iPrivilegeswResult = pageElementPrivileges & (int)SampleElementPrivileges.AddManu;
                    sPageTitle = "新建样品单";
                    break;

                //查看页面分别在待、已打样列表中都有
                case 2:
                    List<int> listPage = new List<int>();
                    listPage.Add((int)ERPPage.SaleManagement_SampleManagement_Manufacturing);
                    listPage.Add((int)ERPPage.SaleManagement_SampleManagement_Manufactured);
                    pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, listPage);

                    iPrivilegeswResult = pageElementPrivileges & (int)SampleElementPrivileges.ReadOnly;
                    sPageTitle = "查看样品单";
                    break;

                case 4:
                    iPrivilegeswResult = pageElementPrivileges & (int)SampleElementPrivileges.Schedule;
                    sPageTitle = "安排生产";
                    break;

                case 5:
                    iPrivilegeswResult = pageElementPrivileges & (int)SampleElementPrivileges.Edit;
                    sPageTitle = "编辑样品单";
                    break;

                default:
                    break;
            }

            if (iPrivilegeswResult == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            DTOSample vm = new DTOSample();
            vm.PageTypeID = PageType;

            if (PageType == 1 || PageType == 5)
            {
                //装载筛选条件中的待审核代印合同状态下拉框数据
                Dictionary<int, string> dataList = new Dictionary<int, string>();

                dataList.Add(1, "手工创建");
                dataList.Add(2, "选择报价单");
                dataList.Add(3, "选择销售订单");

                ViewData["CreateWay"] = GetSelectList(dataList);

                ViewBag.CustomerInfos = _orderCustomerServices.GetAllCustomersKeyInfo(CurrentUserServices.Me.UserID);
            }
            else
            {
                vm.SSID = SSID;
            }
            if (PageType == 5)
            {
                vm.SSID = SSID;
            }
            vm.PageTitle = sPageTitle;

            return View(vm);
        }

        /// <summary>
        /// 新建打样：从不同的数据源获取数据
        /// </summary>
        /// <param name="ID">数据源所在表自编号</param>
        /// <param name="OCID">客户自编号</param>
        /// <param name="CreateWay">数据源：1=手工创建；2=选择报价单；3=选择销售订单</param>
        /// <param name="PageType"></param>
        /// <returns></returns>
        public ActionResult GetManufactureInfo(string ID, int OCID, int CreateWay, int PageType)
        {
            string[] arrID = ID.Split(',');
            List<int> list = new List<int>();
            int id = 0;
            foreach (string item in arrID)
            {
                if (int.TryParse(item, out id))
                {
                    list.Add(id);
                }
            }

            DTOSample vm = _sampleService.GetNewDemand(CreateWay, OCID, list);
            vm.PageTypeID = PageType;

            return PartialView("_PartialManufacture", vm);
        }

        /// <summary>
        /// 请求确认接受、安排生产状态下的查看数据
        /// </summary>
        /// <param name="SSID"></param>
        /// <param name="PageType"></param>
        /// <returns></returns>
        public ActionResult GetReadonlyManufacture(int SSID, int PageType)
        {
            DTOSample vm = new DTOSample();
            vm = _sampleService.ManufactureInfo(vm, SSID, 0, 0);
            vm.PageTypeID = PageType;

            return PartialView("_PartialManufacture", vm);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="SSID"></param>
        /// <param name="CreateWay"></param>
        /// <param name="DataStatus"></param>
        /// <param name="PageType"></param>
        /// <returns></returns>
        public ActionResult ManufactureInfo(int SSID, int CreateWay, int DataStatus, int PageType)
        {
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.SaleManagement_SampleManagement_Manufactured);
            int iPrivilegeswResult = 0;
            string sPageTitle = string.Empty;

            switch (PageType)
            {
                case 1:
                    iPrivilegeswResult = pageElementPrivileges & (int)SampleElementPrivileges.ReadOnly;

                    if (DataStatus == (int)SampleStatus.SampleStatus4)
                    {
                        sPageTitle = "查看生产跟踪";
                    }
                    else
                    {
                        sPageTitle = "查看样品确认";
                    }
                    break;

                case 2:
                    iPrivilegeswResult = pageElementPrivileges & (int)SampleElementPrivileges.Tracking;
                    sPageTitle = "生产跟踪";
                    break;

                case 3:
                    iPrivilegeswResult = pageElementPrivileges & (int)SampleElementPrivileges.AffirmSample;
                    sPageTitle = "样品确认";
                    break;
            }

            if (iPrivilegeswResult == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            DTOSample vm = new DTOSample();
            vm.PageTypeID = PageType;
            vm.PageTitle = sPageTitle;
            vm = _sampleService.ManufactureInfo(vm, SSID, CreateWay, DataStatus);

            return View(vm);
        }

        public ActionResult SendSample(int SSID, int CreateWay, int DataStatus, int PageType)
        {
            List<int> pageList = new List<int>();
            pageList.Add((int)ERPPage.SaleManagement_SampleManagement_Manufactured);
            pageList.Add((int)ERPPage.SaleManagement_SampleManagement_Sending);
            pageList.Add((int)ERPPage.SaleManagement_SampleManagement_Sent);

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, pageList);
            int iPrivilegeswResult = 0;
            string sPageTitle = string.Empty;

            switch (PageType)
            {
                case 1:
                    iPrivilegeswResult = pageElementPrivileges & (int)SampleElementPrivileges.ReadOnly;

                    if (DataStatus <= (int)SampleStatus.SampleStatus7)
                    {
                        sPageTitle = "查看寄出需求";
                    }
                    else if (DataStatus == (int)SampleStatus.SampleStatus8)
                    {
                        sPageTitle = "查看样品寄出";
                    }
                    else
                    {
                        sPageTitle = "查看确认收货";
                    }
                    break;

                case 2:
                    iPrivilegeswResult = pageElementPrivileges & (int)SampleElementPrivileges.SendDemand;
                    sPageTitle = "寄出需求";
                    break;

                case 3:
                    iPrivilegeswResult = pageElementPrivileges & (int)SampleElementPrivileges.SampleSend;
                    sPageTitle = "样品寄出";
                    break;

                case 4:
                    iPrivilegeswResult = pageElementPrivileges & (int)SampleElementPrivileges.AffirmReceving;
                    sPageTitle = "确认收货";
                    break;
            }

            if (iPrivilegeswResult == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            DTOSample vm = new DTOSample();
            vm.PageTypeID = PageType;
            vm.PageTitle = sPageTitle;
            vm = _sampleService.ManufactureInfo(vm, SSID, CreateWay, DataStatus);

            if (vm.Manufactures != null)
            {
                ViewData["AcceptedList"] = GetCASelectList(vm.Manufactures[0].CustomerID);
            }

            return View(vm);
        }

        /// <summary>
        /// 保存新建打样需求、安排生产至DB
        /// </summary>
        /// <param name="DataStatus">保存前的数据状态：1=新建；；</param>
        /// <param name="vmSample"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveManufactureInfo(int DataStatus, DTOSample vmSample)
        {
            DBOperationStatus result = default(DBOperationStatus);

            if (vmSample == null && vmSample.Manufactures != null)
            {
                result = DBOperationStatus.NoAffect;
            }
            else
            {
                if (vmSample.Manufactures.Count > 0)
                {
                    string sClientIP = CurrentUserServices.GetCurrentRequestIPAddress();
                    string a = string.Empty;

                    if (vmSample.SampleStatusID == (int)SampleStatus.SampleStatus1)
                    {
                        result = _sampleService.Save(CurrentUserServices.Me.UserID, sClientIP, vmSample);
                    }
                    else
                    {
                        result = _sampleService.UpdateSampleStatus(CurrentUserServices.Me.UserID, sClientIP, vmSample.SSID, vmSample);
                    }
                }
            }

            return new CustomJsonResult((short)result);
        }

        /// <summary>
        /// 更新寄样信息状态
        /// </summary>
        /// <param name="SSID"></param>
        /// <param name="SampleStatus"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateSampleStatus(int SSID, int SampleStatus)
        {
            DBOperationStatus result = default(DBOperationStatus);
            string sClientIP = CurrentUserServices.GetCurrentRequestIPAddress();
            DTOSample vmSample = new DTOSample();
            vmSample.SampleStatusID = SampleStatus;
            result = _sampleService.UpdateSampleStatus(CurrentUserServices.Me.UserID, sClientIP, SSID, vmSample);

            return new CustomJsonResult((short)result);
        }

        [HttpPost]
        public ActionResult SaveAlterStage(int DataStatus, DTOSample vmSample)
        {
            DBOperationStatus result = default(DBOperationStatus);
            if (vmSample != null && vmSample.Manufactures != null)
            {
                string sClientIP = CurrentUserServices.GetCurrentRequestIPAddress();

                result = _sampleService.SaveAlterStage(CurrentUserServices.Me, sClientIP, DataStatus, vmSample);
            }

            return new CustomJsonResult((short)result);
        }

        public ActionResult SaveSendSample(int DataStatus, DTOSample vmSample)
        {
            DBOperationStatus result = default(DBOperationStatus);
            string sClientIP = CurrentUserServices.GetCurrentRequestIPAddress();
            result = _sampleService.SaveSendSample(CurrentUserServices.Me.UserID, sClientIP, DataStatus, vmSample);

            return new CustomJsonResult((short)result);
        }

        /// <summary>
        /// 删除删除寄样信息
        /// </summary>
        /// <param name="ID">删除寄样信息自编号，多个</param>
        /// <returns></returns>
        public ActionResult DeleteData(string ID)
        {
            DBOperationStatus result = default(DBOperationStatus);

            string sClientIP = CurrentUserServices.GetCurrentRequestIPAddress();

            string[] arrID = ID.Split(',');
            List<int> listIDs = new List<int>();
            int id = 0;

            foreach (string item1 in arrID)
            {
                int.TryParse(item1, out id);
                listIDs.Add(id);
            }

            result = _sampleService.DeleteSample(CurrentUserServices.Me, sClientIP, listIDs);

            return new CustomJsonResult((short)result);
        }

        /// <summary>
        /// 获取寄样箱唛预览数据
        /// </summary>
        /// <param name="SSID">寄样信息自编号</param>
        /// <param name="HTMLCODE">前台编辑的富文本内容</param>
        /// <returns></returns>
        public ActionResult DetailBoxMark(int SSID, string HTMLCODE)
        {
            DTOSample vmSample = new DTOSample();
            vmSample.SSID = SSID;
            vmSample = _sampleService.GetDetailBoxMark(CurrentUserServices.Me, HTMLCODE, vmSample);
            return View(vmSample);
        }
    }
}