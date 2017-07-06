using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.CustomEnums;
using ERP.Models.ProductFitting;
using ERP.Tools;
using ERP.Tools.Logs;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP.BLL.ERP.ProductFitting
{
    public class ProductFittingServices
    {
        private ERP.Dictionary.DictionaryServices _dictionaryServices = new Dictionary.DictionaryServices();

        #region UserMethod

        public List<VMProductFittingInfo> GetAll(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows,
          VMProductFittingSearchModel vm_search)
        {
            List<VMProductFittingInfo> products = null;
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.ProductFittings.Where(p => !p.Deleted && p.ModuleType == (int)ModuleTypeEnum.ProductFitting);

                    if (!vm_search.isAll)
                    {
                        query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForProduct);
                    }

                    if (!string.IsNullOrEmpty(vm_search.No))
                    {
                        query = query.Where(p => p.No.Contains(vm_search.No));
                    }

                    if (!string.IsNullOrEmpty(vm_search.FactoryAbbreviation))
                    {
                        query = query.Where(p => p.Factory.Abbreviation.Contains(vm_search.FactoryAbbreviation));
                    }

                    if (vm_search.FactoryID.HasValue)
                    {
                        query = query.Where(p => p.FactoryID == vm_search.FactoryID);
                    }

                    if (sortColumnsNames != null && sortColumnsNames.Count > 0
                        && sortColumnOrders != null && sortColumnOrders.Count > 0
                        && sortColumnsNames.Count == sortColumnOrders.Count)
                    {
                        query = query.OrderBy(sortColumnsNames, sortColumnOrders);
                    }
                    else
                    {
                        query = query.OrderByDescending(p => p.No);
                    }

                    var dataFromDB = query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                    totalRows = query.Count();
                    if (dataFromDB != null && dataFromDB.Count > 0)
                    {
                        products = new List<VMProductFittingInfo>();

                        // 先查出字典表数据
                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                        foreach (var entity in dataFromDB)
                        {
                            products.Add(GetViewModelFromDBEntity(entity, list_Com_DataDictionary));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return products;
        }

        public VMProductFittingInfo GetProductByID(VMERPUser currentUser, int id)
        {
            VMProductFittingInfo product = null;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.ProductFittings.Where(p => p.ID == id && !p.Deleted && p.ModuleType == (int)ModuleTypeEnum.ProductFitting);

                    var dbEntity = query.FirstOrDefault();
                    if (dbEntity != null)
                    {
                        // 先查出字典表数据
                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());

                        product = GetViewModelFromDBEntity(dbEntity, list_Com_DataDictionary);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return product;
        }

        public DBOperationStatus Delete(VMERPUser currentUser, List<int> IDs)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.ProductFittings.Where(p => IDs.Contains(p.ID) && !p.Deleted && p.ModuleType == (int)ModuleTypeEnum.ProductFitting);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForProduct);

                    var dataFromDB = query.ToList();
                    if (dataFromDB != null)
                    {
                        foreach (var p in dataFromDB)
                        {
                            p.Deleted = true;
                            p.DT_MODIFYDATE = DateTime.Now;
                            p.ST_MODIFYUSER = currentUser.UserID;
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

        public DBOperationStatus Create(VMERPUser currentUser, VMProductFittingInfo productInfo)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var tempProducts = context.ProductFittings.Where(d => d.No == productInfo.No && !d.Deleted && d.ModuleType == (int)ModuleTypeEnum.ProductFitting);

                    if (tempProducts.Count() > 0)//如果修改了货号，与其他的产品存在相同的货号
                    {
                        result = DBOperationStatus.ExistingRecord;
                        return result;
                    }

                    DAL.ProductFitting product = new DAL.ProductFitting();
                    product = SetDBEntityFromViewModel(product, currentUser.UserID, productInfo);

                    product.ST_CREATEUSER = currentUser.UserID;
                    product.DT_CREATEDATE = DateTime.Now;
                    product.Deleted = false;

                    context.ProductFittings.Add(product);
                    int affectRows = context.SaveChanges();
                    if (affectRows == 0)
                    {
                        result = DBOperationStatus.NoAffect;
                    }
                    else
                    {
                        result = DBOperationStatus.Success;

                        product.RootID = product.ID;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return result;
        }

        public DBOperationStatus Save(VMERPUser currentUser, VMProductFittingInfo productInfo)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.ProductFittings.Where(p => p.ID == productInfo.ID && !p.Deleted && p.ModuleType == (int)ModuleTypeEnum.ProductFitting);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForProduct);
                    var dataFromDB = query.FirstOrDefault();

                    if (dataFromDB.No != productInfo.No.Trim())
                    {
                        var tempProducts = context.ProductFittings.Where(d => d.No == productInfo.No && !d.Deleted && d.ModuleType == (int)ModuleTypeEnum.ProductFitting);

                        if (tempProducts.Count() > 0)//如果修改了货号，与其他的产品存在相同的货号
                        {
                            result = DBOperationStatus.ExistingRecord;
                            return result;
                        }
                    }

                    if (dataFromDB != null)
                    {
                        dataFromDB = SetDBEntityFromViewModel(dataFromDB, currentUser.UserID, productInfo);
                        int affectRows = context.SaveChanges();
                        if (affectRows == 0)
                        {
                            result = DBOperationStatus.NoAffect;
                        }
                        else
                        {
                            result = DBOperationStatus.Success;

                            dataFromDB.RootID = dataFromDB.ID;
                            context.SaveChanges();
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

        private VMProductFittingInfo GetViewModelFromDBEntity(DAL.ProductFitting entity, List<Com_DataDictionary> dictionaries)
        {
            VMProductFittingInfo productInfo = new VMProductFittingInfo();

            productInfo.ID = entity.ID;
            productInfo.No = entity.No;
            productInfo.Name = entity.Name;
            productInfo.Image = entity.Image;

            productInfo.Length = entity.Length;
            productInfo.Height = entity.Height;
            productInfo.Width = entity.Width;

            productInfo.Comment = entity.Comment;
            productInfo.FactoryID = entity.FactoryID;
            productInfo.FactoryName = entity.Factory.Abbreviation;

            productInfo.PriceFactory = entity.PriceFactory;

            productInfo.CurrencyType = entity.CurrencyType;
            productInfo.CurrencyName = _dictionaryServices.GetDictionary_CurrencyName(entity.CurrencyType, dictionaries);
            productInfo.CurrencySign = _dictionaryServices.GetDictionary_CurrencySign(entity.CurrencyType, dictionaries);

            productInfo.DT_CREATEDATE = entity.DT_CREATEDATE;
            productInfo.DT_MODIFYDATE = entity.DT_MODIFYDATE == null ? "" : Utils.DateTimeToStr2((DateTime)entity.DT_MODIFYDATE);
            productInfo.Deleted = entity.Deleted;
            productInfo.ProductID = entity.ID;
            productInfo.Qty = entity.Qty;
            productInfo.FeesRate = entity.FeesRate;

            productInfo.UserHierachyID = entity.SystemUser.HierarchyID;
            productInfo.PackageName = entity.PackageName;

            productInfo.PriceFactoryFormatter = productInfo.CurrencySign + productInfo.PriceFactory;
            productInfo.RootID = entity.RootID;
            productInfo.ParentID = entity.ParentID;

            return productInfo;
        }

        public DAL.ProductFitting SetDBEntityFromViewModel(DAL.ProductFitting product, int modifyUser, VMProductFittingInfo entity)
        {
            product.FactoryID = entity.FactoryID;
            product.No = entity.No;
            product.Name = entity.Name;
            product.Image = entity.Image;

            product.Length = entity.Length ?? 0;
            product.Height = entity.Height ?? 0;
            product.Width = entity.Width ?? 0;

            product.PriceFactory = entity.PriceFactory ?? 0;
            product.Comment = entity.Comment;
            product.CurrencyType = entity.CurrencyType;

            product.PackageName = entity.PackageName;

            product.DT_MODIFYDATE = DateTime.Now;
            product.ST_MODIFYUSER = modifyUser;
            product.IPAddress = CommonCode.GetIP();

            return product;
        }

        public List<VMProductFittingInfo> GetProductFittingInfo(VMERPUser currentUser, int ID, int ModuleType)
        {
            List<VMProductFittingInfo> listModel = new List<VMProductFittingInfo>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var productFittingIdList = context.ProductFittings.Where(d => d.ParentID == ID && !d.Deleted && d.ModuleType == ModuleType).Select(d => d.ID).ToList();
                    listModel = GetProductFittingInfo(currentUser, productFittingIdList, ModuleType);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return listModel;
        }

        /// <summary>
        /// 获取产品信息 订单页面使用的
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="idArray"></param>
        /// <returns></returns>
        public List<VMProductFittingInfo> GetProductFittingInfo(VMERPUser currentUser, List<int> idList, int ModuleType)
        {
            List<VMProductFittingInfo> listModel = new List<VMProductFittingInfo>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    if (idList == null || idList.Count <= 0)
                    {
                        return listModel;
                    }
                    var query = context.ProductFittings.Where(p => idList.Contains(p.ID) && !p.Deleted && p.ModuleType == ModuleType);

                    // 先查出字典表数据
                    List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());

                    foreach (var item in query)
                    {
                        VMProductFittingInfo vm_product = GetViewModelFromDBEntity(item, list_Com_DataDictionary);
                        listModel.Add(vm_product);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return listModel;
        }

        #endregion UserMethod
    }
}