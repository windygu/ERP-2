using ERP.BLL.Consts;
using ERP.BLL.Workflow;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.Order;
using ERP.Models.ShipmentOrder;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP.BLL.ERP.ShipmentNotification
{
    /// <summary>
    /// 出运通知信息
    /// </summary>
    public class ShipmentNotificationService
    {
        private ERP.Dictionary.DictionaryServices _dictionaryServices = new Dictionary.DictionaryServices();

        #region HelperMethod

        /// <summary>
        /// 获取状态描述的内容
        /// </summary>
        /// <param name="NotificationStatusID"></param>
        /// <returns></returns>
        private static string GetShipmentNotificationStatusEnum_Description(int StatusID)
        {
            return EnumHelper.GetCustomEnumDesc(typeof(ShipmentNotificationStatusEnum), (ShipmentNotificationStatusEnum)StatusID);
        }

        /// <summary>
        /// 获取拉柜费用状态描述的内容
        /// </summary>
        /// <param name="NotificationStatusID"></param>
        /// <returns></returns>
        private static string GetShipmentRegisterFeesStatusEnum_Description(int StatusID)
        {
            return EnumHelper.GetCustomEnumDesc(typeof(ShipmentRegisterFeesStatusEnum), (ShipmentRegisterFeesStatusEnum)StatusID);
        }

        /// <summary>
        /// 获取拉柜费用登记 状态描述的内容
        /// </summary>
        /// <param name="NotificationStatusID"></param>
        /// <returns></returns>
        private static string GetRegisterFeesStatusEnum_Description(int StatusID)
        {
            return EnumHelper.GetCustomEnumDesc(typeof(ShipmentRegisterFeesStatusEnum), (ShipmentRegisterFeesStatusEnum)StatusID);
        }

        #endregion HelperMethod

        #region UserMethod

        /// <summary>
        /// 获取列表数据
        /// </summary>
        public List<VMShipmentOrder> GetAll(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows,
          VMShipmentOrderSearch vm_search)
        {
            List<VMShipmentOrder> listModel = null;
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Delivery_ShipmentOrder.Where(p => !p.IsDelete && p.StatusID == (short)ShipmentOrderStatusEnum.PassedCheck);//查询审核已通过的订舱
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForDelivery);

                    switch (vm_search.PageType)
                    {
                        case PageTypeEnum.PendingApproval:
                            query = query.Where(d => d.NotificationStatusID != (short)ShipmentNotificationStatusEnum.PassedCheck);
                            break;

                        case PageTypeEnum.PassedApproval:
                        case PageTypeEnum.RegisterFeesList:
                            query = query.Where(d => d.NotificationStatusID == (short)ShipmentNotificationStatusEnum.PassedCheck);
                            break;

                        default:
                            break;
                    }

                    #region 筛选条件

                    if (!string.IsNullOrEmpty(vm_search.OrderNumber))
                    {
                        query = query.Where(d => d.Order.OrderNumber.Contains(vm_search.OrderNumber));
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerCode))
                    {
                        query = query.Where(d => d.Order.Orders_Customers.CustomerCode == vm_search.CustomerCode);
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.CustomerDateStart);
                        query = query.Where(d => d.Order.CustomerDate >= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerDateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.CustomerDateEnd);
                        query = query.Where(d => d.Order.CustomerDate <= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.OrderDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.OrderDateStart);
                        query = query.Where(d => d.Order.OrderDateStart >= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.OrderDateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.OrderDateEnd);
                        query = query.Where(d => d.Order.OrderDateEnd <= dt);
                    }

                    #endregion 筛选条件

                    #region 排序

                    if (sortColumnsNames != null && sortColumnsNames.Count > 0
                        && sortColumnOrders != null && sortColumnOrders.Count > 0
                        && sortColumnsNames.Count == sortColumnOrders.Count)
                    {
                        query = query.OrderBy(sortColumnsNames, sortColumnOrders);
                    }
                    else
                    {
                        query = query.OrderByDescending(d => d.Notification_DT_MODIFYDATE).ThenByDescending(d => d.DT_MODIFYDATE);
                    }

                    #endregion 排序

                    totalRows = query.Count();//获取总条数

                    var dataFromDB = query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();//分页

                    #region 给Model赋值

                    if (dataFromDB != null && dataFromDB.Count > 0)
                    {
                        listModel = new List<VMShipmentOrder>();
                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());

                        foreach (var item in dataFromDB)
                        {
                            bool IsHasApprovalPermission = true;

                            #region 判断是否有审批流的权限

                            if (vm_search.PageType == PageTypeEnum.PendingApproval)//待审核
                            {
                                if (item.ST_CREATEUSERNotification.HasValue)
                                {
                                    IsHasApprovalPermission = ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalShipmentNotification, currentUser, (int)item.ST_CREATEUSERNotification, item.ApproverIndexNotification, item.Order.CustomerID);
                                }
                            }

                            #endregion 判断是否有审批流的权限

                            string OrderNumberList = "";
                            List<string> listOrderNumberList = new List<string>();
                            if (item.IsMerge)
                            {
                                foreach (var OrderID in CommonCode.IdListToList(item.OrderIDList))
                                {
                                    if (OrderID != item.OrderID)
                                    {
                                        listOrderNumberList.Add(context.Orders.Find(OrderID).OrderNumber);
                                    }
                                }
                            }
                            if (listOrderNumberList.Count > 0)
                            {
                                OrderNumberList = string.Join(",", listOrderNumberList.ToArray());
                            }

                            bool valid = true;
                            int PullCabinetCount = item.Delivery_ShipmentOrderCabinet.Where(d => d.GatherType == (short)GatherTypeEnum.PullCabinet).Count();
                            if (vm_search.PageType == PageTypeEnum.RegisterFeesList && PullCabinetCount <= 0)//拉柜
                            {
                                valid = false;
                            }
                            if (valid)
                            {
                                listModel.Add(new VMShipmentOrder()
                                {
                                    ID = item.ID,
                                    OrderID = item.OrderID,
                                    OrderNumber = item.Order.OrderNumber,
                                    POID = item.Order.POID,
                                    EHIPO = item.Order.EHIPO,
                                    CustomerCode = item.Order.Orders_Customers.CustomerCode,
                                    PortName = _dictionaryServices.GetDictionary_PortName(item.Order.PortID, list_Com_DataDictionary),
                                    DestinationPortName = _dictionaryServices.GetDictionary_DestinationPortName(item.Order.DestinationPortID, list_Com_DataDictionary),
                                    CustomerDate = Utils.DateTimeToStr(item.Order.CustomerDate),
                                    OrderAmount = item.Order.OrderAmount,
                                    OrderDateStartFormatter = Utils.DateTimeToStr(item.Order.OrderDateStart),
                                    OrderDateEndFormatter = Utils.DateTimeToStr(item.Order.OrderDateEnd),
                                    StatusID = item.StatusID,
                                    Notification_DT_MODIFYDATEFormatter = Utils.DateTimeToStr2(item.Notification_DT_MODIFYDATE.HasValue ? item.Notification_DT_MODIFYDATE : item.DT_MODIFYDATE),
                                    RegisterFees_DT_MODIFYDATEFormatter = Utils.DateTimeToStr2(item.RegisterFees_DT_MODIFYDATE),
                                    IsHasApprovalPermission = IsHasApprovalPermission,
                                    ST_CREATEUSERNotification = item.ST_CREATEUSERNotification,
                                    ApproverIndex = item.ApproverIndex,
                                    ApproverIndexNotification = item.ApproverIndexNotification,
                                    CustomerID = item.Order.CustomerID,
                                    IsMerge = item.IsMerge,
                                    IsBatchShipped = item.IsBatchShipped,
                                    Merge = item.IsMerge ? "合并订舱" : (item.IsBatchShipped ? "分批订舱" : ""),
                                    OrderNumberList = OrderNumberList,
                                    OrderIDList = item.OrderIDList,

                                    NotificationStatusID = item.NotificationStatusID,
                                    NotificationStatusName = GetShipmentNotificationStatusEnum_Description(item.NotificationStatusID),
                                    RegisterFeesStatusID = item.RegisterFeesStatusID,
                                    RegisterFeesStatusName = GetRegisterFeesStatusEnum_Description(item.RegisterFeesStatusID),
                                });
                            }
                        }
                    }

                    #endregion 给Model赋值
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return listModel;
        }

        /// <summary>
        /// 保存出运通知
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public VMAjaxProcessResult Save(VMERPUser currentUser, VMShipmentOrder vm)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            try
            {
                if (vm.NotificationStatusID == (short)ShipmentNotificationStatusEnum.NotPassCheck)
                {
                    bool isEmpty = false;
                    if (string.IsNullOrEmpty(vm.NotificationCheckSuggest))
                    {
                        isEmpty = true;
                    }
                    else if (vm.NotificationCheckSuggest.Trim() == "")
                    {
                        isEmpty = true;
                    }

                    if (isEmpty)
                    {
                        result.IsSuccess = false;
                        result.Msg = "请输入审核意见！";
                        return result;
                    }
                }

                var OrderIDList = CommonCode.IdListToList(vm.OrderIDList);
                if (string.IsNullOrEmpty(vm.OrderIDList))
                {
                    OrderIDList = new List<int>();
                    OrderIDList.Add(vm.OrderID);
                }

                foreach (var OrderID in OrderIDList)
                {
                    using (ERPEntitiesNew context = new ERPEntitiesNew())
                    {
                        #region 修改订舱信息

                        var query = context.Delivery_ShipmentOrder.Where(d => d.OrderID == OrderID).FirstOrDefault();
                        if (OrderIDList.Count == 1)
                        {
                            query = context.Delivery_ShipmentOrder.Find(vm.ID);
                        }
                        int ST_CREATEUSERNotification = query.ST_CREATEUSERNotification ?? 0;
                        if (query.NotificationStatusID == (int)ShipmentNotificationStatusEnum.PendingMaintenance)//待维护时，给创建人赋值
                        {
                            query.ST_CREATEUSERNotification = currentUser.UserID;
                            ST_CREATEUSERNotification = currentUser.UserID;
                        }

                        if (vm.NotificationStatusID != (int)ShipmentNotificationStatusEnum.NotPassCheck && vm.NotificationStatusID != (int)ShipmentNotificationStatusEnum.PassedCheck)
                        {
                            query.NotificationStatusID = vm.NotificationStatusID;
                        }

                        query.NotificationCheckSuggest = vm.NotificationCheckSuggest;
                        query.Notification_DT_MODIFYDATE = DateTime.Now;
                        query.Notification_ST_MODIFYUSER = currentUser.UserID;

                        DAL.Delivery_ShipmentNotificationHistory query_history = new Delivery_ShipmentNotificationHistory()
                        {
                            DT_CREATEDATE = DateTime.Now,
                            ST_CREATEUSER = currentUser.UserID,
                            IPAddress = CommonCode.GetIP(),
                            StatusID = vm.NotificationStatusID,
                            CheckSuggest = vm.NotificationCheckSuggest,
                        };
                        query.Delivery_ShipmentNotificationHistory.Add(query_history);

                        foreach (var item in vm.list_cabinet.Where(d => !d.IsDelete))
                        {
                            foreach (var item2 in query.Delivery_ShipmentOrderCabinet)//遍历箱柜
                            {
                                if (item.ID == item2.ID)
                                {
                                    var query_cabinet = query.Delivery_ShipmentOrderCabinet.Where(d => d.ID == item.ID).First();

                                    query_cabinet.GatherType = item.GatherType;
                                    if (!string.IsNullOrEmpty(item.GatherDateFormatter))
                                    {
                                        query_cabinet.GatherDate = Utils.StrToDateTime(item.GatherDateFormatter);
                                    }
                                    if (!string.IsNullOrEmpty(item.GatherEndDateFormatter))
                                    {
                                        query_cabinet.GatherEndDate = Utils.StrToDateTime(item.GatherEndDateFormatter);
                                    }
                                    if (!string.IsNullOrEmpty(item.ShippingDateStartFormatter))
                                    {
                                        query_cabinet.ShippingDateStart = Utils.StrToDateTime(item.ShippingDateStartFormatter);
                                    }
                                    if (!string.IsNullOrEmpty(item.ShippingDateEndFormatter))
                                    {
                                        query_cabinet.ShippingDateEnd = Utils.StrToDateTime(item.ShippingDateEndFormatter);
                                    }
                                    query_cabinet.GatherAddress = item.GatherAddress;
                                    query_cabinet.GatherDescription = item.GatherDescription;
                                    query_cabinet.RegisterFees = item.RegisterFees;
                                    query_cabinet.OceanVessel = item.OceanVessel;

                                    query_cabinet.IsDelete = false;
                                    query_cabinet.ST_MODIFYUSER = currentUser.UserID;
                                    query_cabinet.DT_MODIFYDATE = DateTime.Now;
                                    query_cabinet.IPAddress = CommonCode.GetIP();

                                    ConstsMethod.SaveFileUpload(currentUser, item.ID, item.UpLoadFileList, context, UploadFileType.ShipmentNotification_DeliveryNotification);
                                }
                            }
                        }

                        #endregion 修改订舱信息

                        int affectRows = context.SaveChanges();
                        if (affectRows == 0)
                        {
                            result.IsSuccess = false;
                            result.Msg = Keys.ErrorMsg;
                        }
                        else
                        {
                            result.IsSuccess = true;

                            ExecuteApproval(ST_CREATEUSERNotification, query.ID, "", vm.NotificationStatusID, currentUser.UserID, false);//执行审批流
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
                result.IsSuccess = false;
            }

            return result;
        }

        /// <summary>
        /// 保存拉柜登记费用
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public VMAjaxProcessResult Save_RegisterFees(VMERPUser currentUser, VMShipmentOrder vm)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    #region 修改订舱信息

                    var query_list = context.Delivery_ShipmentOrder.Where(d => d.ID == vm.ID && !d.IsDelete);
                    if (query_list.First().IsMerge)
                    {
                        var OrderIDList = query_list.First().OrderIDList;
                        query_list = context.Delivery_ShipmentOrder.Where(d => d.OrderIDList == OrderIDList && !d.IsDelete);
                    }
                    foreach (var query in query_list)
                    {

                        query.RegisterFeesStatusID = vm.RegisterFeesStatusID;
                        query.RegisterFees_DT_MODIFYDATE = DateTime.Now;
                        query.RegisterFees_ST_MODIFYUSER = currentUser.UserID;

                        DAL.Delivery_ShipmentRegisterFeesHistory item_history = new Delivery_ShipmentRegisterFeesHistory()
                        {
                            DT_CREATEDATE = DateTime.Now,
                            ST_CREATEUSER = currentUser.UserID,
                            IPAddress = CommonCode.GetIP(),
                            StatusID = vm.RegisterFeesStatusID,
                            CheckSuggest = vm.CheckSuggest,
                        };
                        query.Delivery_ShipmentRegisterFeesHistory.Add(item_history);

                        foreach (var item in vm.list_cabinet.Where(d => !d.IsDelete))
                        {
                            foreach (var item2 in query.Delivery_ShipmentOrderCabinet)//遍历箱柜
                            {
                                if (item.ID == item2.ID && item2.GatherType == (short)GatherTypeEnum.PullCabinet)
                                {
                                    var item_cabinet = query.Delivery_ShipmentOrderCabinet.Where(d => d.ID == item.ID).First();
                                    item_cabinet.RegisterFees = item.RegisterFees;

                                    item_cabinet.IsDelete = false;
                                    item_cabinet.ST_MODIFYUSER = currentUser.UserID;
                                    item_cabinet.DT_MODIFYDATE = DateTime.Now;
                                    item_cabinet.IPAddress = CommonCode.GetIP();
                                }
                            }
                        }
                    }

                    #endregion 修改订舱信息

                    int affectRows = context.SaveChanges();
                    if (affectRows == 0)
                    {
                        result.IsSuccess = false;
                        result.Msg = Keys.ErrorMsg;
                    }
                    else
                    {
                        result.IsSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
                result.IsSuccess = false;
                result.Msg = Keys.ErrorMsg;
            }

            return result;
        }

        #endregion UserMethod

        /// <summary>
        /// 执行审批流
        /// </summary>
        /// <param name="createUserID">创建人</param>
        /// <param name="identityID">主键ID</param>
        /// <param name="CheckSuggest">审批意见</param>
        /// <param name="StatusID">报价单的状态</param>
        /// <param name="UserID">当前用户ID</param>
        private static void ExecuteApproval(int createUserID, int identityID, string CheckSuggest, int StatusID, int UserID, bool historyAdded)
        {
            bool isPass = true;
            if (StatusID == (int)ShipmentNotificationStatusEnum.NotPassCheck)
            {
                isPass = false;
            }

            List<int> validWaitingApproveStatus = new List<int>(){
                            (int)ShipmentNotificationStatusEnum.PendingCheck,
                            (int)ShipmentNotificationStatusEnum.NotPassCheck
                        };
            ApprovalService.ExcuteApproval(new ApprovalInfo
            {
                WorkflowType = WorkflowTypes.ApprovalShipmentNotification,
                IsPass = isPass,
                IdentityID = identityID,
                ValidWaitingApproveStatus = validWaitingApproveStatus,
                StatusApproving = (int)ShipmentNotificationStatusEnum.PendingCheck,
                StatusNextTo = (int)ShipmentNotificationStatusEnum.PassedCheck,
                StatusRejected = (int)ShipmentNotificationStatusEnum.NotPassCheck,
                ApproveOpinion = CheckSuggest,
                ApproveUserID = UserID,
                LogMethod = () =>
                {
                    return null;
                }
            });
        }
    }
}