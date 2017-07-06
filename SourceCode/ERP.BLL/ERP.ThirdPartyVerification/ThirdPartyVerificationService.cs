using ERP.BLL.Consts;
using ERP.BLL.ERP.Order;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.Product;
using ERP.Models.ThirdPartyVerification;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP.BLL.ERP.ThirdPartyVerification
{
    public class ThirdPartyVerificationService
    {
        private OrderService _orderService = new OrderService();

        #region HelperMethod

        /// <summary>
        /// 获取状态描述的内容
        /// </summary>
        /// <param name="StatusID"></param>
        /// <returns></returns>
        private static string GetOrderStatusEnum_Description(int StatusID)
        {
            return EnumHelper.GetCustomEnumDesc(typeof(ThirdPartyVerificationStatusEnum), (ThirdPartyVerificationStatusEnum)StatusID);
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
                    var query = context.Purchase_ThirdPartyVerification.Include("Order").Where(d => d.ID >= 0);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForThirdParty);

                    #region 筛选条件

                    if (!string.IsNullOrEmpty(vm_search.OrderNumber))
                    {
                        query = query.Where(d => d.Order.OrderNumber.Contains(vm_search.OrderNumber));
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
                    if (!string.IsNullOrEmpty(vm_search.CustomerCode))
                    {
                        query = query.Where(d => d.Order.Orders_Customers.CustomerCode == vm_search.CustomerCode);
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
                    if (!string.IsNullOrEmpty(vm_search.POID))
                    {
                        query = query.Where(d => d.Order.POID.Contains(vm_search.POID));
                    }
                    if (!string.IsNullOrEmpty(vm_search.OrderOrigin))
                    {
                        query = query.Where(d => d.Order.OrderOrigin.Contains(vm_search.OrderOrigin));
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
                        query = query.OrderByDescending(d => d.DT_MODIFYDATE);
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
                                ID = item.ID,
                                OrderID = item.OrderID,
                                OrderNumber = item.Order.OrderNumber,
                                CustomerNo = item.Order.Orders_Customers.CustomerCode,
                                POID = item.Order.POID,
                                EHIPO = item.Order.EHIPO,
                                CustomerDate = Utils.DateTimeToStr(item.Order.CustomerDate),
                                OrderAmount = item.Order.OrderAmount,
                                OrderRate = item.Order.OrderRate,
                                OrderRate_En = item.Order.OrderRate_En,
                                OrderDateStart = Utils.DateTimeToStr(item.Order.OrderDateStart),
                                OrderDateEnd = Utils.DateTimeToStr(item.Order.OrderDateEnd),
                                OrderOrigin = item.Order.OrderOrigin,
                                StatusID = item.StatusID,
                                OrderStatusName = GetOrderStatusEnum_Description(item.StatusID),
                                DT_MODIFYDATE = Utils.DateTimeToStr2(item.DT_MODIFYDATE),
                                InspectionVerificationFeeFormatter = item.InspectionVerificationFee.HasValue ? Keys.RMB_Sign + item.InspectionVerificationFee : null,
                                InspectionVerificationFee_ForFactoryFormatter = item.InspectionVerificationFee_ForFactory.HasValue ? Keys.RMB_Sign + item.InspectionVerificationFee_ForFactory : null,
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

        /// <summary>
        /// 获取详情
        /// </summary>
        public VMThirdPartyVerification GetDetailByID(VMERPUser currentUser, int id)
        {
            VMThirdPartyVerification vm = null;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Purchase_ThirdPartyVerification.Where(p => p.ID == id);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForPurchase);
                    var dataFromDB = query.First();
                    vm = new VMThirdPartyVerification()
                    {
                        ID = dataFromDB.ID,
                        Comment = dataFromDB.Comment,
                        InspectionVerificationFee = dataFromDB.InspectionVerificationFee,
                        InspectionVerificationFee_ForFactory = dataFromDB.InspectionVerificationFee_ForFactory,
                        Order = _orderService.GetDetailByID(currentUser, query.First().OrderID),
                        UpLoadFileList = ConstsMethod.GetUploadFileList(id, UploadFileType.ThirdPartyVerification),
                        StatusID = dataFromDB.StatusID,
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return vm;
        }

        /// <summary>
        /// 保存
        /// </summary>
        public DBOperationStatus Save(VMERPUser currentUser, VMThirdPartyVerification vm)
        {
            DBOperationStatus result = new DBOperationStatus();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Purchase_ThirdPartyVerification.Find(vm.ID);
                    query.Comment = vm.Comment;
                    query.InspectionVerificationFee = vm.InspectionVerificationFee;
                    query.InspectionVerificationFee_ForFactory = vm.InspectionVerificationFee_ForFactory;
                    query.StatusID = vm.StatusID;

                    query.DT_MODIFYDATE = DateTime.Now;
                    query.ST_MODIFYUSER = currentUser.UserID;
                    query.IPAddress = CommonCode.GetIP();

                    ConstsMethod.SaveFileUpload(currentUser, vm.ID, vm.UpLoadFileList, context, UploadFileType.ThirdPartyVerification);

                    foreach (var item in vm.list_OrderProduct)
                    {
                        var query_OrderProduct = context.OrderProducts.Find(item.ID);
                        if (query_OrderProduct != null)
                        {
                            query_OrderProduct.InspectionVerificationFee = item.InspectionVerificationFee;
                            query_OrderProduct.InspectionVerificationFee_ForFactory = item.InspectionVerificationFee_ForFactory;
                        }

                    }


                    int affectRows = context.SaveChanges();
                    if (affectRows == 0)
                    {
                        result = DBOperationStatus.NoAffect;
                    }
                    else
                    {
                        result = DBOperationStatus.Success;
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