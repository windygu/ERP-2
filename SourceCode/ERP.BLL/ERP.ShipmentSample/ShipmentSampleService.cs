using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.ThreeTimesQC;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP.BLL.ERP.ShipmentSample
{
    public class ShipmentSampleService
    {
        private ERP.Dictionary.DictionaryServices _dictionaryServices = new ERP.Dictionary.DictionaryServices();

        #region HelperMethod

        /// <summary>
        /// 获取状态描述的内容
        /// </summary>
        /// <param name="StatusID"></param>
        /// <returns></returns>
        private static string GetStatusEnum_Description(int StatusID)
        {
            return EnumHelper.GetCustomEnumDesc(typeof(ShipmentSampleStatusEnum), (ShipmentSampleStatusEnum)StatusID);
        }

        #endregion HelperMethod

        #region UserMethod

        /// <summary>
        /// 获取列表数据
        /// </summary>
        public List<DTOPurchaseContract> GetAll(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows,
          VMPurchaseSearch vm_search)
        {
            List<DTOPurchaseContract> listModel = null;
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Purchase_ThreeTimesQC.Include("Purchase_Contract").Where(d => d.StatusID >= (short)ThreeTimesQCStatusEnum.PassedCheck);//已审核、已上传的三期QC
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForPurchase);

                    if (currentUser.HierachyType == HierachyType.Agency)
                    {
                        var userHierachy = new List<int?>()
                        {
                            currentUser.HierachyID,
                            context.SystemUsers.FirstOrDefault(p => p.HierarchyID == currentUser.HierachyID).Hierarchy.Hierarchy2.HierarchyID
                        };

                        query = query.Where(p => userHierachy.Contains(p.Purchase_Contract.Factory.Hierarchy));
                    }

                    if (vm_search.PageType == PageTypeEnum.PassedCheckList)//已审核
                    {
                        query = query.Where(d => d.RecoveryStatusID == (short)ShipmentSampleStatusEnum.HadRecovery);
                    }
                    else
                    {
                        query = query.Where(d => d.RecoveryStatusID != (short)ShipmentSampleStatusEnum.HadRecovery);
                    }

                    #region 筛选条件

                    if (!string.IsNullOrEmpty(vm_search.PurchaseNumber))
                    {
                        query = query.Where(d => d.Purchase_Contract.PurchaseNumber.Contains(vm_search.PurchaseNumber));
                    }
                    if (!string.IsNullOrEmpty(vm_search.FactoryAbbreviation))
                    {
                        query = query.Where(d => d.Purchase_Contract.Factory.Abbreviation.Contains(vm_search.FactoryAbbreviation));
                    }
                    if (!string.IsNullOrEmpty(vm_search.PurchaseDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.PurchaseDateStart);
                        query = query.Where(d => d.Purchase_Contract.PurchaseDate >= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.PurchaseDateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.PurchaseDateEnd);
                        query = query.Where(d => d.Purchase_Contract.PurchaseDate <= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.DateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.DateStart);
                        query = query.Where(d => d.Purchase_Contract.DateStart <= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.DateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.DateEnd);
                        query = query.Where(d => d.Purchase_Contract.DateEnd >= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerCode))
                    {
                        query = query.Where(p => p.Purchase_Contract.Orders_Customers.CustomerCode == vm_search.CustomerCode);
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
                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                        listModel = new List<DTOPurchaseContract>();
                        foreach (var entity in dataFromDB)
                        {
                            listModel.Add(new DTOPurchaseContract()
                            {
                                ID = entity.ID,
                                PurchaseNumber = entity.Purchase_Contract.PurchaseNumber,
                                FactoryAbbreviation = entity.Purchase_Contract.Factory.Abbreviation,
                                CustomerCode = entity.Purchase_Contract.Orders_Customers.CustomerCode,
                                PurchaseDate = Utils.DateTimeToStr(entity.Purchase_Contract.PurchaseDate),
                                AfterDate = entity.Purchase_Contract.AfterDate,
                                PortName = entity.Purchase_Contract.Port,
                                ApproverIndex = entity.Purchase_Contract.ApproverIndexPurchaseContract,
                                DateStart = Utils.DateTimeToStr(entity.Purchase_Contract.DateStart),
                                DateEnd = Utils.DateTimeToStr(entity.Purchase_Contract.DateEnd),
                                RecoveryStatusID = entity.RecoveryStatusID,
                                RecoveryStatusName = GetStatusEnum_Description(entity.RecoveryStatusID ?? 0),
                                RecoveryModifyDate = Utils.DateTimeToStr2(entity.RecoveryModifyDate),
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
        public VMThreeTimesQC GetDetailByID(VMERPUser currentUser, int id)
        {
            VMThreeTimesQC vm = null;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Purchase_ThreeTimesQC.Where(p => p.ID == id);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForPurchase);
                    var dataFromDB = query.First();

                    var list_history = new List<ThreeTimesQCHistory>();
                    var query_history = context.Purchase_ThreeTimesQCHistory.Where(d => d.ThreeTimesQCID == dataFromDB.ID);
                    List<DAL.SystemUser> list_SystemUser = context.SystemUsers.ToList();
                    foreach (var item_history in query_history)
                    {
                        list_history.Add(new ThreeTimesQCHistory()
                        {
                            ST_CREATEUSER = list_SystemUser.Find(d => d.UserID == item_history.ST_CREATEUSER).UserName,
                            DT_CREATEDATE = Utils.DateTimeToStr2(item_history.DT_CREATEDATE),
                            Comment = item_history.Comment,
                            CheckSuggest = item_history.CheckSuggest,
                        });
                    }

                    vm = new VMThreeTimesQC()
                    {
                        ID = dataFromDB.ID,
                        Comment = dataFromDB.Comment,
                        PurchaseContract = new Purchase.PurchaseContractService().GetDetailByID(currentUser, query.First().PurchaseContractID),
                        list_history = list_history,
                        StatusID = dataFromDB.StatusID,
                        RecoveryDateFormatter = Utils.DateTimeToStr(dataFromDB.RecoveryDate),
                        RecoveryDate = dataFromDB.RecoveryDate,
                        RecoveryUserID = dataFromDB.RecoveryUserID,
                        RecoveryStatusID = dataFromDB.RecoveryStatusID,
                        RecoveryAddress = dataFromDB.RecoveryAddress,
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
        public VMAjaxProcessResult Save(VMERPUser currentUser, VMThreeTimesQC vm)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Purchase_ThreeTimesQC.Find(vm.ID);
                    query.RecoveryStatusID = vm.RecoveryStatusID;
                    query.RecoveryDate = Utils.StrToDateTime(vm.RecoveryDateFormatter);
                    query.RecoveryUserID = vm.RecoveryUserID;
                    query.RecoveryAddress = vm.RecoveryAddress;

                    query.RecoveryModifyDate = DateTime.Now;
                    query.RecoveryModifyUser = currentUser.UserID;
                    query.IPAddress = CommonCode.GetIP();

                    int affectRows = context.SaveChanges();
                    if (affectRows == 0)
                    {
                        result.IsSuccess = false;
                        result.Msg = "出错了";
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