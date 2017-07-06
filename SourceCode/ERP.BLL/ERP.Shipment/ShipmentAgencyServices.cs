using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.CustomEnums;
using ERP.Models.Shipment;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.ERP.Shipment
{
    public class ShipmentAgencyServices
    {
        public List<VMShipmentAgency> GetAgencies(VMERPUser currentUser, string agencyName, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows)
        {
            List<VMShipmentAgency> agencies = null;
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Shipment_Agencies.Where(p => !p.IsDeleted);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForShipmentAgency);

                    if (!string.IsNullOrEmpty(agencyName))
                    {
                        query = query.Where(p => p.ShippingAgencyName.Contains(agencyName));
                    }

                    if (sortColumnsNames != null && sortColumnsNames.Count > 0
                        && sortColumnOrders != null && sortColumnOrders.Count > 0
                        && sortColumnsNames.Count == sortColumnOrders.Count)
                    {
                        query = query.OrderBy(sortColumnsNames, sortColumnOrders);
                    }
                    else
                    {
                        query = query.OrderByDescending(p => p.CreateDate);
                    }

                    totalRows = query.Count();

                    query = query.Skip((currentPage - 1) * pageSize).Take(pageSize);
                    agencies = (from agencyEntity in query
                                select new VMShipmentAgency()
                                {
                                    ShippingAgencyID = agencyEntity.ShippingAgencyID,
                                    ShippingAgencyName = agencyEntity.ShippingAgencyName,
                                    IsDeleted = agencyEntity.IsDeleted,
                                    CurrentShipmentAgentFees = (from fee in agencyEntity.Shipment_AgentFees
                                                                where fee.IsValid
                                                                select new DTOShipmentAgentFees()
                                                                {
                                                                    IsValid = fee.IsValid,
                                                                    CreateDate = fee.CreateDate,
                                                                    LastModifyBy = fee.LastModifyBy,
                                                                    LastModifyDate = fee.LastModifyDate,
                                                                    Currency = fee.Currency,
                                                                    FeeCustomDeclaration = fee.FeeCustomDeclaration,
                                                                    FeeDockOperation = fee.FeeDockOperation,
                                                                    FeeDocument = fee.FeeDocument,
                                                                    FeeFacilityManagement = fee.FeeFacilityManagement,
                                                                    FeeImporterSecurityClassify = fee.FeeImporterSecurityClassify,
                                                                    FeePicking = fee.FeePicking,
                                                                    FeePortSecurity = fee.FeePortSecurity,
                                                                    FeeWarehousing = fee.FeeWarehousing,
                                                                    FeeYangShanPicking = fee.FeeYangShanPicking
                                                                }).FirstOrDefault()
                                }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return agencies;
        }

        public VMShipmentAgency GetAgencyByID(VMERPUser currentUser, int agencyID)
        {
            VMShipmentAgency agency = null;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Shipment_Agencies.Where(p => p.ShippingAgencyID == agencyID && p.IsDeleted == false);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForShipmentAgency);

                    agency = (from agencyEntity in query
                              select new VMShipmentAgency()
                              {
                                  ShippingAgencyID = agencyEntity.ShippingAgencyID,
                                  ShippingAgencyName = agencyEntity.ShippingAgencyName,
                                  AgencyAddress = agencyEntity.AgencyAddress,
                                  WarehouseAddress = agencyEntity.WarehouseAddress,
                                  IsDeleted = agencyEntity.IsDeleted,
                                  CurrentShipmentAgentFees = (from fee in agencyEntity.Shipment_AgentFees
                                                              where fee.IsValid
                                                              select new DTOShipmentAgentFees()
                                                              {
                                                                  IsValid = fee.IsValid,
                                                                  CreateDate = fee.CreateDate,
                                                                  LastModifyBy = fee.LastModifyBy,
                                                                  LastModifyDate = fee.LastModifyDate,
                                                                  Currency = fee.Currency,
                                                                  FeeCustomDeclaration = fee.FeeCustomDeclaration,
                                                                  FeeDockOperation = fee.FeeDockOperation,
                                                                  FeeDocument = fee.FeeDocument,
                                                                  FeeFacilityManagement = fee.FeeFacilityManagement,
                                                                  FeeImporterSecurityClassify = fee.FeeImporterSecurityClassify,
                                                                  FeePicking = fee.FeePicking,
                                                                  FeePortSecurity = fee.FeePortSecurity,
                                                                  FeeWarehousing = fee.FeeWarehousing,
                                                                  FeeYangShanPicking = fee.FeeYangShanPicking
                                                              }).FirstOrDefault()
                              }).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return agency;
        }

        public DBOperationStatus Delete(VMERPUser currentUser, List<int> deleteList)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Shipment_Agencies.Where(p => deleteList.Contains(p.ShippingAgencyID));
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForShipmentAgency);

                    var dataFromDB = query.ToList();
                    if (dataFromDB != null)
                    {
                        foreach (var p in dataFromDB)
                        {
                            p.IsDeleted = true;
                            p.LastModifyBy = currentUser.UserName;
                            p.LastModifyDate = DateTime.Now;
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
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return result;
        }

        public DBOperationStatus Create(int currentUserID, VMShipmentAgency agency)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var dbAgency = new Shipment_Agencies()
                    {
                        CreateDate = DateTime.Now,
                        CreateUserID = currentUserID,
                        ShippingAgencyName = agency.ShippingAgencyName,
                        AgencyAddress=agency.AgencyAddress,
                        WarehouseAddress=agency.WarehouseAddress,
                        Shipment_AgentFees = new List<Shipment_AgentFees>()
                        {
                            new Shipment_AgentFees()
                                {
                                    CreateDate = DateTime.Now,
                                    CreateUserID = currentUserID,
                                    Currency = agency.CurrentShipmentAgentFees.Currency,
                                    FeeCustomDeclaration = agency.CurrentShipmentAgentFees.FeeCustomDeclaration,
                                    FeeDockOperation = agency.CurrentShipmentAgentFees.FeeDockOperation,
                                    FeeDocument = agency.CurrentShipmentAgentFees.FeeDocument,
                                    FeeFacilityManagement = agency.CurrentShipmentAgentFees.FeeFacilityManagement,
                                    FeeImporterSecurityClassify = agency.CurrentShipmentAgentFees.FeeImporterSecurityClassify,
                                    FeePicking = agency.CurrentShipmentAgentFees.FeePicking,
                                    FeePortSecurity = agency.CurrentShipmentAgentFees.FeePortSecurity,
                                    FeeWarehousing = agency.CurrentShipmentAgentFees.FeeWarehousing,
                                    FeeYangShanPicking = agency.CurrentShipmentAgentFees.FeeYangShanPicking,
                                    IsValid = true
                                }
                        }
                    };
                    context.Shipment_Agencies.Add(dbAgency);

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

        public DBOperationStatus Update(VMERPUser currentUser, VMShipmentAgency agency)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var dataFromDB = context.Shipment_Agencies.FirstOrDefault(p => p.ShippingAgencyID == agency.ShippingAgencyID);
                    if (dataFromDB != null)
                    {
                        dataFromDB.LastModifyBy = currentUser.UserName;
                        dataFromDB.LastModifyDate = DateTime.Now;
                        dataFromDB.ShippingAgencyName = agency.ShippingAgencyName;

                        dataFromDB.AgencyAddress = agency.AgencyAddress;
                       dataFromDB.WarehouseAddress = agency.WarehouseAddress;

                        //如果价格有变化，则将当前价格记录作为历史记录
                        var fee = dataFromDB.Shipment_AgentFees.FirstOrDefault(p => p.IsValid);
                        if (fee != null && (fee.Currency != agency.CurrentShipmentAgentFees.Currency ||
                            fee.FeeCustomDeclaration != agency.CurrentShipmentAgentFees.FeeCustomDeclaration ||
                            fee.FeeDockOperation != agency.CurrentShipmentAgentFees.FeeDockOperation ||
                            fee.FeeDocument != agency.CurrentShipmentAgentFees.FeeDocument ||
                            fee.FeeFacilityManagement != agency.CurrentShipmentAgentFees.FeeFacilityManagement ||
                            fee.FeeImporterSecurityClassify != agency.CurrentShipmentAgentFees.FeeImporterSecurityClassify ||
                            fee.FeePicking != agency.CurrentShipmentAgentFees.FeePicking ||
                            fee.FeePortSecurity != agency.CurrentShipmentAgentFees.FeePortSecurity ||
                            fee.FeeWarehousing != agency.CurrentShipmentAgentFees.FeeWarehousing ||
                            fee.FeeYangShanPicking != agency.CurrentShipmentAgentFees.FeeYangShanPicking))
                        {
                            var fees = context.Shipment_AgentFees.Where(p => p.ShippingAgencyID == agency.ShippingAgencyID && p.IsValid);
                            foreach (var f in fees)
                            {
                                f.IsValid = false;
                            }

                            //再添加一条IsValid=true的新数据
                            Shipment_AgentFees newFee = new Shipment_AgentFees();
                            newFee.CreateDate = DateTime.Now;
                            newFee.CreateUserID = currentUser.UserID;
                            newFee.Currency = agency.CurrentShipmentAgentFees.Currency;
                            newFee.FeeCustomDeclaration = agency.CurrentShipmentAgentFees.FeeCustomDeclaration;
                            newFee.FeeDockOperation = agency.CurrentShipmentAgentFees.FeeDockOperation;
                            newFee.FeeDocument = agency.CurrentShipmentAgentFees.FeeDocument;
                            newFee.FeeFacilityManagement = agency.CurrentShipmentAgentFees.FeeFacilityManagement;
                            newFee.FeeImporterSecurityClassify = agency.CurrentShipmentAgentFees.FeeImporterSecurityClassify;
                            newFee.FeePicking = agency.CurrentShipmentAgentFees.FeePicking;
                            newFee.FeePortSecurity = agency.CurrentShipmentAgentFees.FeePortSecurity;
                            newFee.FeeWarehousing = agency.CurrentShipmentAgentFees.FeeWarehousing;
                            newFee.FeeYangShanPicking = agency.CurrentShipmentAgentFees.FeeYangShanPicking;
                            newFee.IsValid = true;
                            dataFromDB.Shipment_AgentFees.Add(newFee);
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
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return result;
        }

        /// <summary>
        /// 订舱信息页面用到了 获取船运公司列表
        /// </summary>
        /// <returns></returns>
        public List<VMShipmentAgency> GetList()
        {
            List<VMShipmentAgency> list = new List<VMShipmentAgency>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Shipment_Agencies.Where(p => !p.IsDeleted);
                    foreach (var item in query)
                    {
                        list.Add(new VMShipmentAgency()
                        {
                            ShippingAgencyID = item.ShippingAgencyID,
                            ShippingAgencyName = item.ShippingAgencyName,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return list;
        }

        /// <summary>
        /// 获取船运公司列表
        /// </summary>
        /// <returns></returns>
        public List<VMShipmentAgency> SelectAll()
        {
            List<VMShipmentAgency> agencies = new List<VMShipmentAgency>();

            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                var query = context.Shipment_Agencies.Where(p => !p.IsDeleted);

                agencies = (from agencyEntity in query
                            select new VMShipmentAgency()
                            {
                                ShippingAgencyID = agencyEntity.ShippingAgencyID,
                                ShippingAgencyName = agencyEntity.ShippingAgencyName,
                                IsDeleted = agencyEntity.IsDeleted,
                                CurrentShipmentAgentFees = (from fee in agencyEntity.Shipment_AgentFees
                                                            where fee.IsValid
                                                            select new DTOShipmentAgentFees()
                                                            {
                                                                IsValid = fee.IsValid,
                                                                CreateDate = fee.CreateDate,
                                                                LastModifyBy = fee.LastModifyBy,
                                                                LastModifyDate = fee.LastModifyDate,
                                                                Currency = fee.Currency,
                                                                FeeCustomDeclaration = fee.FeeCustomDeclaration,
                                                                FeeDockOperation = fee.FeeDockOperation,
                                                                FeeDocument = fee.FeeDocument,
                                                                FeeFacilityManagement = fee.FeeFacilityManagement,
                                                                FeeImporterSecurityClassify = fee.FeeImporterSecurityClassify,
                                                                FeePicking = fee.FeePicking,
                                                                FeePortSecurity = fee.FeePortSecurity,
                                                                FeeWarehousing = fee.FeeWarehousing,
                                                                FeeYangShanPicking = fee.FeeYangShanPicking
                                                            }).FirstOrDefault()
                            }).ToList();
            }

            return agencies;
        }
    }
}