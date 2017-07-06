using ERP.BLL.Consts;
using ERP.BLL.ERP.Order;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.Order;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP.BLL.ERP.PortChargesInvoice
{
    public class PortChargesInvoiceService
    {
        private ERP.Dictionary.DictionaryServices _dictionaryServices = new Dictionary.DictionaryServices();
        private OrderService _orderService = new OrderService();

        #region HelperMethod

        /// <summary>
        /// 获取状态描述的内容
        /// </summary>
        /// <param name="StatusID"></param>
        /// <returns></returns>
        private static string GetPortChargesInvoiceStatusEnum_Description(int StatusID)
        {
            return EnumHelper.GetCustomEnumDesc(typeof(PortChargesInvoiceStatusEnum), (PortChargesInvoiceStatusEnum)StatusID);
        }

        #endregion HelperMethod

        #region UserMethod

        /// <summary>
        /// 获取列表数据
        /// </summary>
        public List<DTOOrder> GetAll(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows,
          VMOrderSearch vm_search)
        {
            List<DTOOrder> listModel = null;
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var OrderIDList = context.Delivery_ShipmentOrder.Where(d => !d.IsDelete && d.StatusID == (int)ShipmentOrderStatusEnum.PassedCheck).Select(d => d.OrderID);//TODO 待完善

                    var query = context.Orders.Where(d => !d.IsDelete && OrderIDList.Contains(d.OrderID));
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForInspectionReceipt);

                    #region 筛选条件

                    if (!string.IsNullOrEmpty(vm_search.OrderNumber))
                    {
                        query = query.Where(d => d.OrderNumber.Contains(vm_search.OrderNumber));
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.CustomerDateStart);
                        query = query.Where(d => d.CustomerDate >= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerDateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.CustomerDateEnd);
                        query = query.Where(d => d.CustomerDate <= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerCode))
                    {
                        query = query.Where(d => d.Orders_Customers.CustomerCode == vm_search.CustomerCode);
                    }
                    if (!string.IsNullOrEmpty(vm_search.OrderDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.OrderDateStart);
                        query = query.Where(d => d.OrderDateStart >= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.OrderDateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.OrderDateEnd);
                        query = query.Where(d => d.OrderDateEnd <= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.POID))
                    {
                        query = query.Where(d => d.POID.Contains(vm_search.POID));
                    }
                    if (!string.IsNullOrEmpty(vm_search.OrderOrigin))
                    {
                        query = query.Where(d => d.OrderOrigin.Contains(vm_search.OrderOrigin));
                    }
                    if (!string.IsNullOrEmpty(vm_search.OrderStatusID))
                    {
                        int i = Utils.StrToInt(vm_search.OrderStatusID, 0);
                        query = query.Where(d => d.OrderStatusID == i);
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
                        query = query.OrderByDescending(d => d.PortChargesInvoice_UpdateDate);
                    }

                    #endregion 排序

                    totalRows = query.Count();//获取总条数

                    var dataFromDB = query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();//分页

                    #region 给Model赋值

                    if (dataFromDB != null && dataFromDB.Count > 0)
                    {
                        listModel = new List<DTOOrder>();
                        foreach (var item in dataFromDB)
                        {
                            listModel.Add(new DTOOrder()
                            {
                                OrderID = item.OrderID,
                                OrderNumber = item.OrderNumber,
                                CustomerID = item.CustomerID,
                                CustomerNo = item.Orders_Customers.CustomerCode,
                                POID = item.POID,
                                EHIPO = item.EHIPO,
                                OrderDateStart = Utils.DateTimeToStr(item.OrderDateStart),
                                OrderDateEnd = Utils.DateTimeToStr(item.OrderDateEnd),
                                DesignatedAgencyAmount = item.DesignatedAgencyAmount,
                                OurAgencyAmount = item.OurAgencyAmount,
                                PortChargesInvoice_StatusID = item.PortChargesInvoice_StatusID ?? 0,
                                PortChargesInvoice_StatusName = GetPortChargesInvoiceStatusEnum_Description(item.PortChargesInvoice_StatusID ?? 0),
                                PortChargesInvoice_UpdateDate = Utils.DateTimeToStr2(item.PortChargesInvoice_UpdateDate),
                            });
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

        public VMOrderEdit GetDetailByID(VMERPUser currentUser, int id)
        {
            var vm = _orderService.GetDetailByID(currentUser, id);
            vm.list_UploadPortChargesInvoice = ConstsMethod.GetUploadFileList(id, UploadFileType.PortChargesInvoice);
            return vm;
        }

        /// <summary>
        /// 保存
        /// </summary>
        public VMAjaxProcessResult Save(VMERPUser currentUser, VMOrderEdit vm)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Orders.Find(vm.OrderID);
                    query.DesignatedAgencyAmount = vm.DesignatedAgencyAmount;
                    query.OurAgencyAmount = vm.OurAgencyAmount;
                    query.PortChargesInvoice_StatusID = vm.PortChargesInvoice_StatusID;
                    query.PortChargesInvoice_UpdateDate = DateTime.Now;
                    query.PortChargesInvoice_CreateUserID = currentUser.UserID;

                    ConstsMethod.SaveFileUpload(currentUser, vm.OrderID, vm.list_UploadPortChargesInvoice, context, UploadFileType.PortChargesInvoice);

                    int affectRows = context.SaveChanges();
                    if (affectRows == 0)
                    {
                        result.IsSuccess = false;
                        result.Msg = "出错了！";
                        return result;
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
            }
            return result;
        }

        #endregion UserMethod
    }
}