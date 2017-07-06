using ERP.BLL.Consts;
using ERP.BLL.ERP.Consts;
using ERP.BLL.ERP.ProductFitting;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.Product;
using ERP.Tools;
using ERP.Tools.Logs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.BLL.ERP.Product
{
    public class ProductServices
    {
        private ERP.Dictionary.DictionaryServices _dictionaryServices = new Dictionary.DictionaryServices();

        private ProductFittingServices _productFittingServices = new ProductFittingServices();

        #region HelperMethod

        #region 计算列

        public decimal CalculateFOBUS(DAL.Product entity, decimal freightRate)
        {
            decimal FOBUS = 0;
            try
            {
                decimal priceFactory = entity.PriceFactory;
                decimal dutyPercent = entity.DutyPercent ?? 0m;
                decimal outerWidth = entity.OuterWidth ?? 0m;
                decimal outerHeight = entity.OuterHeight ?? 0m;
                decimal outerLength = entity.OuterLength ?? 0m;
                decimal outerBoxRate = entity.OuterBoxRate ?? 0m;
                decimal miscImportLoad = entity.MiscImportLoad ?? 0m;
                decimal agent = entity.Orders_Customers.Agent;

                FOBUS = Math.Round(Math.Round(((priceFactory / 5.4m + priceFactory / 5.4m * dutyPercent / 100) + freightRate * (outerWidth * outerHeight * outerLength / 1000000 * 35.315m) / outerBoxRate)
                            + priceFactory / 5.4m * miscImportLoad / 100 + (priceFactory / 5.4m * agent / 100), 2) / 0.8m / 0.95m / 0.92m / 0.9m, 2);
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return FOBUS;
        }

        public decimal CalculateDDP(DAL.Product entity)
        {
            decimal DDP = 0;
            try
            {
                decimal priceFactory = entity.PriceFactory;
                decimal dutyPercent = entity.DutyPercent ?? 0m;
                decimal outerWidth = entity.OuterWidth ?? 0m;
                decimal outerHeight = entity.OuterHeight ?? 0m;
                decimal outerLength = entity.OuterLength ?? 0m;
                decimal outerBoxRate = entity.OuterBoxRate ?? 0m;
                decimal freightRate = 0;
                decimal ctnsPallet = entity.CtnsPallet ?? 0m;

                DDP = Math.Round(((priceFactory / 6.1m + (100 / 6.1m) / (outerBoxRate * ctnsPallet)) + priceFactory / 5.4m * dutyPercent / 100)
                        + freightRate * (outerWidth * outerHeight * outerLength / 1000000m * 35.315m) / outerBoxRate / 0.95m / 0.9m / 0.8m, 2);
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return DDP;
        }

        public decimal CalculatePOE(DAL.Product entity)
        {
            decimal POE = 0;
            try
            {
                decimal priceFactory = entity.PriceFactory;
                decimal dutyPercent = entity.DutyPercent ?? 0m;
                decimal outerWidth = entity.OuterWidth ?? 0m;
                decimal outerHeight = entity.OuterHeight ?? 0m;
                decimal outerLength = entity.OuterLength ?? 0m;
                decimal outerBoxRate = entity.OuterBoxRate ?? 0m;
                decimal freightRate = 0;

                POE = Math.Round(priceFactory / 6.1m + priceFactory / 5.4m * dutyPercent / 100m + freightRate * outerWidth * outerHeight * outerLength / 1000000m * 35.315m / outerBoxRate / 0.8m / 0.95m, 2);
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return POE;
        }

        public decimal CalculateMU(DAL.Product entity)
        {
            decimal MU = 0;
            try
            {
                decimal priceFactory = entity.PriceFactory;
                decimal dutyPercent = entity.DutyPercent ?? 0m;
                decimal outerWidth = entity.OuterWidth ?? 0m;
                decimal outerHeight = entity.OuterHeight ?? 0m;
                decimal outerLength = entity.OuterLength ?? 0m;
                decimal outerBoxRate = entity.OuterBoxRate ?? 0m;
                decimal miscImportLoad = entity.MiscImportLoad ?? 0m;
                decimal freightRate = 0;
                decimal agent = entity.Orders_Customers.Agent;
                decimal srp = entity.SRP ?? 0m;

                if (outerBoxRate != 0m && srp != 0m)
                {
                    MU = Math.Round((srp - ((((priceFactory / 5.4m + priceFactory / 5.4m * dutyPercent / 100) + freightRate * (outerWidth * outerHeight * outerLength / 1000000m * 35.315m) / outerBoxRate)
                                       + priceFactory / 5.4m * miscImportLoad / 100) + priceFactory / 5.4m * agent / 100)) / srp, 2);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return MU;
        }

        public decimal CalculateAgentAmount(DAL.Product entity)
        {
            decimal agentAmount = 0;
            try
            {
                decimal priceFactory = entity.PriceFactory;
                decimal agent = entity.Orders_Customers.Agent;

                agentAmount = Math.Round(priceFactory / 5.4m * agent / 100m, 2);
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return agentAmount;
        }

        public decimal CalculateELC(DAL.Product entity)
        {
            decimal ELC = 0;
            try
            {
                decimal priceFactory = entity.PriceFactory;
                decimal dutyPercent = entity.DutyPercent ?? 0m;
                decimal outerWidth = entity.OuterWidth ?? 0m;
                decimal outerHeight = entity.OuterHeight ?? 0m;
                decimal outerLength = entity.OuterLength ?? 0m;
                decimal outerBoxRate = entity.OuterBoxRate ?? 0m;
                decimal miscImportLoad = entity.MiscImportLoad ?? 0m;
                decimal freightRate = 0;
                decimal agent = entity.Orders_Customers.Agent;

                ELC = Math.Round((((priceFactory / 5.4m + priceFactory / 5.4m * dutyPercent / 100) + freightRate * (outerWidth * outerHeight * outerLength / 1000000m * 35.315m) / outerBoxRate)
                    + priceFactory / 5.4m * miscImportLoad / 100m) + priceFactory / 5.4m * agent / 100m, 2);
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return ELC;
        }

        public decimal CalculateRetail(DAL.Product entity)
        {
            decimal retail = 0;
            try
            {
                decimal priceFactory = entity.PriceFactory;
                decimal dutyPercent = entity.DutyPercent ?? 0m;
                decimal outerWidth = entity.OuterWidth ?? 0m;
                decimal outerHeight = entity.OuterHeight ?? 0m;
                decimal outerLength = entity.OuterLength ?? 0m;
                decimal outerBoxRate = entity.OuterBoxRate ?? 0m;
                decimal miscImportLoad = entity.MiscImportLoad ?? 0m;
                decimal freightRate = 0;
                decimal agent = entity.Orders_Customers.Agent;

                retail = Math.Round(Math.Round(((((priceFactory / 5.4m + priceFactory / 5.4m * dutyPercent / 100) + freightRate * (outerWidth * outerHeight * outerLength) / 1000000m * 35.315m) / outerBoxRate)
                           + priceFactory / 5.4m * miscImportLoad / 100) + priceFactory / 5.4m * agent / 100, 2) / 0.5m, 2);
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return retail;
        }

        public decimal CalculateFreight(DAL.Product entity, decimal freightRate)
        {
            decimal freight = 0;
            try
            {
                decimal outerWidth = entity.OuterWidth ?? 0m;
                decimal outerHeight = entity.OuterHeight ?? 0m;
                decimal outerLength = entity.OuterLength ?? 0m;
                decimal outerBoxRate = entity.OuterBoxRate ?? 0m;

                freight = Math.Round(freightRate * (outerWidth * outerHeight * outerLength / 1000000m * 35.315m) / outerBoxRate, 2);
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return freight;
        }

        public int? CalculateMOQ(int? MOQ, decimal? Cost)
        {
            int MOQEn = 0;
            if (MOQ == 0 || MOQ == null)
            {
                //Moq默认值：5000/cost 取最小120的倍数。
                MOQEn = Utils.StrToInt((Math.Ceiling(5000 / Cost ?? 1)).ToString(), 1);

                MOQEn = Utils.StrToInt(Math.Ceiling(MOQEn / 120.0m).ToString(), 1) * 120;
                return MOQEn;
            }
            else
            {
                return MOQ;
            }
        }

        #endregion 计算列

        #endregion HelperMethod

        #region UserMethod

        public List<VMProductInfo> GetAll(VMERPUser currentUser, List<string> productNOs, string customerNo, string factoryName, string keyWord,
            List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows, bool includeQuoteProducts)
        {
            List<VMProductInfo> products = null;
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Products.Where(p => !p.Deleted && !p.IsProductMixed);

                    if (currentUser.HierachyType == HierachyType.Agency)
                    {
                        var userHierachy = new List<int?>()
                        {
                            currentUser.HierachyID,
                            context.SystemUsers.FirstOrDefault(p => p.HierarchyID == currentUser.HierachyID).Hierarchy.Hierarchy2.HierarchyID
                        };

                        query = query.Where(p => userHierachy.Contains(p.Factory.Hierarchy));
                    }

                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForProduct);
                    if (!includeQuoteProducts)
                    {
                        query = query.Where(p => !p.ParentProductID.HasValue);
                    }
                    else
                    {
                        query = query.Where(p => p.ParentProductID.HasValue);
                    }

                    if (productNOs != null && productNOs.Count > 0)
                    {
                        query = query.Where(p => productNOs.Contains(p.No));
                    }

                    if (!string.IsNullOrEmpty(customerNo))
                    {
                        query = query.Where(p => p.Orders_Customers.CustomerCode == customerNo);
                    }

                    if (!string.IsNullOrEmpty(factoryName))
                    {
                        query = query.Where(p => p.Factory.Abbreviation.Contains(factoryName));
                    }

                    if (!string.IsNullOrEmpty(keyWord))
                    {
                        query = query.Where(p => p.Name.Contains(keyWord) || p.Desc.Contains(keyWord));
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
                        products = new List<VMProductInfo>();

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

        public VMProductInfo GetProductByID(VMERPUser currentUser, int productID, bool createAsQuote)
        {
            VMProductInfo product = null;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Products.Include("Orders_Customers").Where(p => p.ID == productID && !p.Deleted);
                    //query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForProduct);

                    var dbEntity = query.FirstOrDefault();
                    if (dbEntity != null)
                    {
                        // 先查出字典表数据
                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());

                        product = GetViewModelFromDBEntity(dbEntity, list_Com_DataDictionary);
                        if (createAsQuote && !product.RootProductID.HasValue)//添加报价产品时，选择产品如果是大清单产品，货号后面加上后缀，后缀从客户里面的Customer Code取。
                        {
                            product.No = product.No + dbEntity.Orders_Customers.Code;
                        }

                        List<DAL.HarmonizedSystem> List_HarmonizedSystem = context.HarmonizedSystems.Where(p => !p.IsDelete).ToList();
                        product.IsNeedInspection = ConstsMethod.IsNeedInspection(List_HarmonizedSystem, product.HSCode ?? 0);

                        product.list_ProductIngredient = context.ProductIngredients.Where(d => d.ModuleType == (int)ModuleTypeEnum.Product && d.ProductID == product.ID).Select(p => new VMProductIngredients
                        {
                            IngredientName = p.IngredientName,
                            IngredientPercent = p.IngredientPercent,
                            ProductID = p.ProductID,
                        }).ToList();

                        var productFittingIdList = context.ProductFittings.Where(d => d.ProductID == product.ID && !d.Deleted && d.ModuleType == (int)ModuleTypeEnum.Product).Select(d => d.ID).ToList();
                        product.list_ProductFitting = _productFittingServices.GetProductFittingInfo(currentUser, productFittingIdList, (int)ModuleTypeEnum.Product);

                        List<VMProductInfo> list_ProductMixed = new List<VMProductInfo>();
                        var query_productMixed = context.Products.Where(d => d.ParentProductMixedID == product.ID && !d.Deleted && d.IsProductMixed);
                        foreach (var item in query_productMixed)
                        {
                            list_ProductMixed.Add(GetViewModelFromDBEntity(item, list_Com_DataDictionary));
                        }
                        product.list_ProductMixed = list_ProductMixed;

                        if (product.Length == 0)
                        {
                            product.Length = null;
                        }
                        if (product.Width == 0)
                        {
                            product.Width = null;
                        }
                        if (product.Height == 0)
                        {
                            product.Height = null;
                        }
                        if (product.Weight == 0)
                        {
                            product.Weight = null;
                        }

                        if (product.PDQLength == 0)
                        {
                            product.PDQLength = null;
                        }
                        if (product.PDQWidth == 0)
                        {
                            product.PDQWidth = null;
                        }
                        if (product.PDQHeight == 0)
                        {
                            product.PDQHeight = null;
                        }

                        if (product.InnerLength == 0)
                        {
                            product.InnerLength = null;
                        }
                        if (product.InnerWidth == 0)
                        {
                            product.InnerWidth = null;
                        }
                        if (product.InnerHeight == 0)
                        {
                            product.InnerHeight = null;
                        }
                        if (product.InnerWeight == 0)
                        {
                            product.InnerWeight = null;
                        }
                        if (product.InnerWeightGross == 0)
                        {
                            product.InnerWeightGross = null;
                        }

                        if (product.OuterLength == 0)
                        {
                            product.OuterLength = null;
                        }
                        if (product.OuterWidth == 0)
                        {
                            product.OuterWidth = null;
                        }
                        if (product.OuterHeight == 0)
                        {
                            product.OuterHeight = null;
                        }
                        if (product.OuterWeightGross == 0)
                        {
                            product.OuterWeightGross = null;
                        }
                        if (product.OuterWeightNet == 0)
                        {
                            product.OuterWeightNet = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return product;
        }

        public DBOperationStatus Delete(VMERPUser currentUser, List<int> IDs, string ipAddress)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Products.Where(p => IDs.Contains(p.ID));
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForProduct);

                    var dataFromDB = query.ToList();
                    if (dataFromDB != null)
                    {
                        foreach (var p in dataFromDB)
                        {
                            p.Deleted = true;
                            p.DT_MODIFYDATE = DateTime.Now;
                            p.ST_MODIFYUSER = currentUser.UserID;

                            foreach (var item in p.ProductFittings)
                            {
                                item.Deleted = true;
                                item.DT_MODIFYDATE = DateTime.Now;
                                item.ST_MODIFYUSER = currentUser.UserID;
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
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return result;
        }

        public DBOperationStatus Save(VMERPUser currentUser, string modifyUserIP, VMProductInfo productInfo)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Products.Where(p => p.ID == productInfo.ID);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForProduct);
                    var dataFromDB = query.FirstOrDefault();

                    if (dataFromDB.No != productInfo.No.StrTrim())
                    {
                        var tempProducts = context.Products.Where(d => d.No == productInfo.No && !d.Deleted && d.RootProductID.HasValue == productInfo.RootProductID.HasValue && d.CustomerID == productInfo.CustomerID && !d.IsProductMixed);

                        if (tempProducts.Count() > 0)//如果修改了货号，与其他的产品存在相同的货号
                        {
                            result = DBOperationStatus.ExistingRecord;
                            return result;
                        }
                    }

                    if (dataFromDB != null)
                    {
                        dataFromDB = SetDBEntityFromViewModel(dataFromDB, currentUser.UserID, modifyUserIP, productInfo);
                        dataFromDB.ST_MODIFYUSER = currentUser.UserID;

                        #region 添加产品成分信息

                        var temp = context.ProductIngredients.Where(p => p.ProductID == productInfo.ID && p.ModuleType == (int)ModuleTypeEnum.Product);
                        context.ProductIngredients.RemoveRange(temp);

                        if (productInfo.list_ProductIngredient != null && productInfo.list_ProductIngredient.Count > 0)
                        {
                            var ProductIngredient = new List<DAL.ProductIngredient>();
                            foreach (var item in productInfo.list_ProductIngredient)
                            {
                                DAL.ProductIngredient query_ProductIngredient = new DAL.ProductIngredient()
                                {
                                    ProductID = productInfo.ID,
                                    IngredientName = item.IngredientName,
                                    IngredientPercent = item.IngredientPercent,
                                    ModuleType = (int)ModuleTypeEnum.Product,
                                    IsDelete = false,
                                };
                                context.ProductIngredients.Add(query_ProductIngredient);
                            }
                        }

                        #endregion 添加产品成分信息

                        #region 添加产品配件

                        var temp2 = context.ProductFittings.Where(p => p.ProductID == productInfo.ID && !p.Deleted && p.ModuleType == (int)ModuleTypeEnum.Product);
                        context.ProductFittings.RemoveRange(temp2);

                        if (productInfo.list_ProductFitting != null && productInfo.list_ProductFitting.Count > 0)
                        {
                            var ProductFittings = new List<DAL.ProductFitting>();
                            foreach (var item in productInfo.list_ProductFitting)
                            {
                                var query_ProductFitting = context.ProductFittings.Where(p => p.RootID == item.RootID && !p.Deleted && p.ModuleType == (int)ModuleTypeEnum.ProductFitting).First();

                                query_ProductFitting.ProductID = productInfo.ID;
                                query_ProductFitting.ParentID = productInfo.ID;
                                query_ProductFitting.RootID = item.RootID;
                                query_ProductFitting.ModuleType = (int)ModuleTypeEnum.Product;

                                query_ProductFitting.Qty = item.Qty ?? 0;
                                query_ProductFitting.FeesRate = item.FeesRate ?? 0;

                                query_ProductFitting.DT_CREATEDATE = DateTime.Now;
                                query_ProductFitting.ST_CREATEUSER = currentUser.UserID;
                                query_ProductFitting.DT_MODIFYDATE = DateTime.Now;
                                query_ProductFitting.ST_MODIFYUSER = currentUser.UserID;
                                query_ProductFitting.Deleted = false;
                                context.ProductFittings.Add(query_ProductFitting);
                            }
                            dataFromDB.IsProductFitting = true;
                        }
                        else
                        {
                            dataFromDB.IsProductFitting = false;
                        }

                        #endregion 添加产品配件

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

        public DBOperationStatus Create(VMERPUser currentUser, string modifyUserIP, VMProductInfo productInfo, bool isQuoteProduct)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var tempProducts = context.Products.Where(d => d.No == productInfo.No && !d.Deleted && d.RootProductID.HasValue == isQuoteProduct && d.CustomerID == productInfo.CustomerID && !d.IsProductMixed);

                    if (tempProducts.Count() > 0)//如果修改了货号，与其他的产品存在相同的货号
                    {
                        result = DBOperationStatus.ExistingRecord;
                        return result;
                    }

                    DAL.Product product = new DAL.Product();
                    product = SetDBEntityFromViewModel(product, currentUser.UserID, modifyUserIP, productInfo);
                    product.ST_CREATEUSER = currentUser.UserID;
                    product.DT_CREATEDATE = DateTime.Now;
                    product.Deleted = false;

                    context.Products.Add(product);
                    int affectRows = context.SaveChanges();
                    if (affectRows == 0)
                    {
                        result = DBOperationStatus.NoAffect;
                    }
                    else
                    {
                        result = DBOperationStatus.Success;

                        productInfo.ID = product.ID;

                        #region 添加产品成分信息

                        var temp = context.ProductIngredients.Where(p => p.ProductID == productInfo.ID && p.ModuleType == (int)ModuleTypeEnum.Product);
                        context.ProductIngredients.RemoveRange(temp);

                        if (productInfo.list_ProductIngredient != null && productInfo.list_ProductIngredient.Count > 0)
                        {
                            var ProductIngredient = new List<DAL.ProductIngredient>();
                            foreach (var item in productInfo.list_ProductIngredient)
                            {
                                DAL.ProductIngredient query_ProductIngredient = new DAL.ProductIngredient()
                                {
                                    ProductID = productInfo.ID,
                                    IngredientName = item.IngredientName,
                                    IngredientPercent = item.IngredientPercent,
                                    ModuleType = (int)ModuleTypeEnum.Product,
                                    IsDelete = false,
                                };
                                context.ProductIngredients.Add(query_ProductIngredient);
                            }
                        }

                        #endregion 添加产品成分信息

                        #region 添加产品配件

                        var temp2 = context.ProductFittings.Where(p => p.ProductID == productInfo.ID && !p.Deleted && p.ModuleType == (int)ModuleTypeEnum.Product);
                        context.ProductFittings.RemoveRange(temp2);

                        if (productInfo.list_ProductFitting != null && productInfo.list_ProductFitting.Count > 0)
                        {
                            var ProductFittings = new List<DAL.ProductFitting>();
                            foreach (var item in productInfo.list_ProductFitting)
                            {
                                var query_ProductFitting = context.ProductFittings.Where(p => p.RootID == item.RootID && !p.Deleted && p.ModuleType == (int)ModuleTypeEnum.ProductFitting).First();

                                query_ProductFitting.ProductID = productInfo.ID;
                                query_ProductFitting.ParentID = productInfo.ID;
                                query_ProductFitting.RootID = item.RootID;
                                query_ProductFitting.ModuleType = (int)ModuleTypeEnum.Product;

                                query_ProductFitting.Qty = item.Qty ?? 0;
                                query_ProductFitting.FeesRate = item.FeesRate ?? 0;

                                query_ProductFitting.DT_CREATEDATE = DateTime.Now;
                                query_ProductFitting.ST_CREATEUSER = currentUser.UserID;
                                query_ProductFitting.DT_MODIFYDATE = DateTime.Now;
                                query_ProductFitting.ST_MODIFYUSER = currentUser.UserID;
                                query_ProductFitting.Deleted = false;
                                context.ProductFittings.Add(query_ProductFitting);
                            }

                            product.IsProductFitting = true;
                        }
                        else
                        {
                            product.IsProductFitting = false;
                        }

                        #endregion 添加产品配件

                        affectRows = context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return result;
        }

        private VMProductInfo GetViewModelFromDBEntity(DAL.Product entity, List<Com_DataDictionary> dictionaries)
        {
            VMProductInfo productInfo = new VMProductInfo();

            productInfo.ID = entity.ID;
            productInfo.No = entity.No;
            productInfo.NoFactory = entity.NoFactory;
            productInfo.Name = entity.Name;
            productInfo.Desc = entity.Desc;
            productInfo.Image = entity.Image;
            productInfo.Length = entity.Length;
            productInfo.Height = entity.Height;
            productInfo.Width = entity.Width;
            productInfo.Weight = entity.Weight;
            productInfo.IngredientZh = entity.IngredientZh;
            productInfo.IngredientEn = entity.IngredientEn;
            productInfo.MOQZh = entity.MOQZh;
            productInfo.MOQEn = entity.MOQEn;
            productInfo.PDQPackRate = entity.PDQPackRate;
            productInfo.PDQLength = entity.PDQLength;
            productInfo.PDQWidth = entity.PDQWidth;
            productInfo.PDQHeight = entity.PDQHeight;
            productInfo.InnerBoxRate = entity.InnerBoxRate;
            productInfo.InnerLength = entity.InnerLength;
            productInfo.InnerWidth = entity.InnerWidth;
            productInfo.InnerHeight = entity.InnerHeight;
            productInfo.InnerWeight = entity.InnerWeight;
            productInfo.InnerWeightGross = entity.InnerWeightGross;
            productInfo.OuterBoxRate = entity.OuterBoxRate;
            productInfo.OuterLength = entity.OuterLength;
            productInfo.OuterWidth = entity.OuterWidth;
            productInfo.OuterHeight = entity.OuterHeight;
            productInfo.PriceFactory = entity.PriceFactory;
            productInfo.MiscImportLoad = entity.MiscImportLoad;
            productInfo.SRP = entity.SRP;
            productInfo.CtnsPallet = entity.CtnsPallet;
            productInfo.DutyPercent = entity.DutyPercent;
            productInfo.Remarks = entity.Remarks;
            productInfo.Comment = entity.Comment;
            productInfo.UPC = entity.UPC;

            productInfo.LengthIN = entity.LengthIN;
            productInfo.HeightIN = entity.HeightIN;
            productInfo.WidthIN = entity.WidthIN;
            productInfo.WeightLBS = entity.WeightLBS;
            productInfo.PDQLengthIN = entity.PDQLengthIN;
            productInfo.PDQWidthIN = entity.PDQWidthIN;
            productInfo.PDQHeightIN = entity.PDQHeightIN;
            productInfo.InnerLengthIN = entity.InnerLengthIN;
            productInfo.InnerWidthIN = entity.InnerWidthIN;
            productInfo.InnerHeightIN = entity.InnerHeightIN;
            productInfo.OuterVolume = entity.OuterVolume;
            productInfo.InnerVolume = entity.InnerVolume;
            productInfo.InnerWeightLBS = entity.InnerWeightLBS;
            productInfo.InnerWeightGrossLBS = entity.InnerWeightGrossLBS;
            productInfo.OuterLengthIN = entity.OuterLengthIN;
            productInfo.OuterWidthIN = entity.OuterWidthIN;
            productInfo.OuterHeightIN = entity.OuterHeightIN;
            productInfo.FOBFTY = entity.FOBFTY;
            productInfo.FOBNET = entity.FOBNET;
            productInfo.Rate = entity.Rate;
            productInfo.FinalFOB = entity.FinalFOB;
            productInfo.MiscImportLoadAmount = entity.MiscImportLoadAmount;
            productInfo.PcsPallet = entity.PcsPallet;
            productInfo.PalletPc = entity.PalletPc;
            productInfo.Duty = entity.Duty;
            productInfo.FOBChinaLCL = entity.FOBChinaLCL;
            productInfo.Cost = entity.Cost;
            productInfo.Commission = entity.Commission;
            productInfo.CommissionAmount = entity.CommissionAmount;

            productInfo.Allowance = entity.Allowance;
            productInfo.CustomerID = entity.CustomerID;
            productInfo.CustomerNo = entity.Orders_Customers.CustomerCode;
            productInfo.CustomerName = entity.Orders_Customers.CustomerName;
            productInfo.FreightRate = entity.FreightRate;
            productInfo.Agent = entity.Agent;
            productInfo.FactoryID = entity.FactoryID;
            productInfo.FactoryName = entity.Factory.Abbreviation;

            productInfo.StyleID = entity.StyleID;
            productInfo.StyleName = _dictionaryServices.GetDictionary_StyleName(entity.StyleID, dictionaries);
            productInfo.StyleNumber = _dictionaryServices.GetDictionary_StyleNumber(entity.StyleID, dictionaries);
            productInfo.PortID = entity.PortID;
            productInfo.PortName = _dictionaryServices.GetDictionary_PortName(entity.PortID, dictionaries);
            productInfo.PortEnName = _dictionaryServices.GetDictionary_PortEnName(entity.PortID, dictionaries);
            productInfo.PackingMannerZhID = entity.PackingMannerZhID;
            productInfo.PackingMannerZhName = _dictionaryServices.GetDictionary_PackingMannerZhName(entity.PackingMannerZhID, dictionaries);
            productInfo.PackingMannerEnID = entity.PackingMannerEnID;
            productInfo.PackingMannerEnName = entity.PackingMannerEnID == null ? "" : _dictionaryServices.GetDictionary_PackingMannerEnName(entity.PackingMannerZhID, dictionaries);
            productInfo.UnitID = entity.UnitID;
            productInfo.UnitName = _dictionaryServices.GetDictionary_UnitName(entity.UnitID, dictionaries);
            productInfo.CurrencyType = entity.CurrencyType;
            productInfo.CurrencyName = _dictionaryServices.GetDictionary_CurrencyName(entity.CurrencyType, dictionaries);
            productInfo.CurrencySign = _dictionaryServices.GetDictionary_CurrencySign(entity.CurrencyType, dictionaries);

            productInfo.HTS = entity.HTS;
            productInfo.HSCode = entity.HSCode;
            productInfo.ParentProductID = entity.ParentProductID;
            productInfo.ParentNo = entity.Product2 == null ? entity.No : entity.Product2.No;

            productInfo.RootProductID = entity.RootProductID;
            productInfo.RootNo = entity.Product3 == null ? entity.No : entity.Product3.No;

            productInfo.DT_CREATEDATE = entity.DT_CREATEDATE;
            productInfo.DT_MODIFYDATE = entity.DT_MODIFYDATE == null ? "" : Utils.DateTimeToStr2((DateTime)entity.DT_MODIFYDATE);
            productInfo.Deleted = entity.Deleted;
            productInfo.FOBChinaPort = entity.FOBChinaPort ?? 0;
            productInfo.ClassifyID = entity.ClassifyID;
            productInfo.PriceInputDate = entity.PriceInputDate;
            productInfo.PriceInputDateFormat = entity.PriceInputDate == null ? "" : Utils.DateTimeToStr((DateTime)entity.PriceInputDate);
            productInfo.ValidDate = entity.ValidDate;
            productInfo.ValidDateFormat = entity.ValidDate == null ? "" : Utils.DateTimeToStr((DateTime)entity.ValidDate);
            productInfo.OuterWeightGrossLBS = entity.OuterWeightGrossLBS;
            productInfo.OuterWeightNetLBS = entity.OuterWeightNetLBS;
            productInfo.FOBC5 = entity.FOBC5;
            productInfo.OuterWeightGross = entity.OuterWeightGross;
            productInfo.OuterWeightNet = entity.OuterWeightNet;
            productInfo.ProductID = entity.ID;

            //productInfo.FOBUS = CalculateFOBUS(entity);
            //productInfo.DDP = CalculateDDP(entity);
            //productInfo.POE = CalculatePOE(entity);
            //productInfo.MU = CalculateMU(entity);
            //productInfo.AgentAmount = CalculateAgentAmount(entity);
            //productInfo.ELC = CalculateELC(entity);
            //productInfo.Retail = CalculateRetail(entity);
            //productInfo.Freight = CalculateFreight(entity);
            productInfo.Status = (ProductStatusEnum)entity.Status;
            productInfo.UserHierachyID = entity.SystemUser.HierarchyID;
            productInfo.ProductCopyRight = entity.ProductCopyRight;
            productInfo.Season = entity.Season;
            productInfo.SeasonName = _dictionaryServices.GetDictionary_AllSeasonName(entity.Season, dictionaries);
            productInfo.ColorID = entity.ColorID;
            productInfo.ColorName = _dictionaryServices.GetDictionaryByName(entity.ColorID, dictionaries);
            productInfo.IsProductFitting = entity.IsProductFitting;
            productInfo.IsProductFittingFormatter = entity.IsProductFitting ? "有" : "";
            productInfo.IsProductMixed = entity.IsProductMixed;
            productInfo.ParentProductMixedID = entity.ParentProductMixedID;
            productInfo.Qty = entity.Qty;

            return productInfo;
        }

        private static DAL.Product SetDBEntityFromViewModel(DAL.Product product, int modifyUser, string modifyUserIP, VMProductInfo entity)
        {
            product.ID = entity.ID;
            product.ClassifyID = entity.ClassifyID;
            product.FactoryID = entity.FactoryID;
            product.No = entity.No.StrTrim();
            product.NoFactory = entity.NoFactory;
            product.Name = entity.Name;
            product.Desc = entity.Desc;
            product.CustomerID = entity.CustomerID;
            product.Image = entity.Image;
            product.UnitID = entity.UnitID;
            product.Length = entity.Length ?? 0;
            product.Height = entity.Height ?? 0;
            product.Width = entity.Width ?? 0;
            product.StyleID = entity.StyleID;
            product.Weight = entity.Weight;
            product.PortID = entity.PortID;
            product.PackingMannerZhID = entity.PackingMannerZhID;
            product.IngredientZh = entity.IngredientZh;
            product.MOQZh = entity.MOQZh;
            product.PackingMannerEnID = entity.PackingMannerEnID;
            product.IngredientEn = entity.IngredientEn;
            product.PDQPackRate = entity.PDQPackRate;
            product.PDQLength = entity.PDQLength;
            product.PDQWidth = entity.PDQWidth;
            product.PDQHeight = entity.PDQHeight;
            product.InnerBoxRate = entity.InnerBoxRate;
            product.InnerLength = entity.InnerLength;
            product.InnerWidth = entity.InnerWidth;
            product.InnerHeight = entity.InnerHeight;
            product.InnerWeight = entity.InnerWeight;
            product.InnerWeightGross = entity.InnerWeightGross;
            product.OuterBoxRate = entity.OuterBoxRate;
            product.OuterLength = entity.OuterLength;
            product.OuterWidth = entity.OuterWidth;
            product.OuterHeight = entity.OuterHeight;
            product.OuterWeightGross = entity.OuterWeightGross;
            product.OuterWeightNet = entity.OuterWeightNet;
            product.PriceFactory = entity.PriceFactory ?? 0;
            product.MiscImportLoad = entity.MiscImportLoad;
            product.Agent = entity.Agent;
            product.SRP = entity.SRP;
            product.CtnsPallet = entity.CtnsPallet;
            product.DutyPercent = entity.DutyPercent;
            product.FreightRate = entity.FreightRate;
            product.Remarks = entity.Remarks;
            product.Comment = entity.Comment;
            product.PriceInputDate = entity.PriceInputDate;
            product.ValidDate = entity.ValidDate;
            product.MOQEn = entity.MOQEn;
            product.UPC = entity.UPC;
            product.CurrencyType = entity.CurrencyType;
            product.DT_MODIFYDATE = DateTime.Now;
            product.ST_MODIFYUSER = modifyUser;
            product.IPAddress = modifyUserIP;
            product.Allowance = entity.Allowance;
            product.HTS = entity.HTS;
            product.HSCode = entity.HSCode;
            product.ParentProductID = entity.ParentProductID;
            product.RootProductID = entity.RootProductID;
            product.Status = (short)entity.Status;
            product.ProductCopyRight = entity.ProductCopyRight;
            product.Season = entity.Season;
            product.ColorID = entity.ColorID;

            return product;
        }

        public List<VMProductInfo> GetProductByIDs(VMERPUser currentUser, List<int> productIDs)
        {
            List<VMProductInfo> products = null;
            if (productIDs == null)
            {
                return products;
            }
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Products.Include("Orders_Customers").Where(p => productIDs.Contains(p.ID) && !p.Deleted && !p.IsProductMixed);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForProduct);

                    var dbEntities = query.ToList();
                    if (dbEntities != null)
                    {
                        products = new List<VMProductInfo>();

                        // 先查出字典表数据
                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                        foreach (var entity in dbEntities)
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

        public List<VMViewProductList> GetViewProductList(VMERPUser currentUser, List<int> IDs)
        {
            List<VMViewProductList> list_vm = new List<VMViewProductList>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Products.Where(p => IDs.Contains(p.ID));
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForProduct);

                    foreach (var item in query)
                    {
                        list_vm.Add(new VMViewProductList()
                        {
                            No = item.No,
                            Image = item.Image,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return list_vm;
        }

        public List<VMViewLabelList> GetViewLabelList(VMERPUser currentUser, List<int> IDs)
        {
            List<VMViewLabelList> list_vm = new List<VMViewLabelList>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Products.Where(p => IDs.Contains(p.ID));
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForProduct);

                    List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());

                    foreach (var item in query)
                    {
                        list_vm.Add(new VMViewLabelList()
                        {
                            ProductID = item.ID,
                            No = item.No,
                            StyleNumber = Utils.StrToInt(_dictionaryServices.GetDictionary_StyleNumber(item.StyleID, list_Com_DataDictionary), 1),
                            StyleName = _dictionaryServices.GetDictionary_StyleName(item.StyleID, list_Com_DataDictionary),
                            InnerBoxRate = item.InnerBoxRate,
                            OuterBoxRate = item.OuterBoxRate,
                            OuterVolume = item.OuterVolume,
                            LengthIN = item.LengthIN,
                            WidthIN = item.WidthIN,
                            HeightIN = item.HeightIN,
                            PriceFactory = item.PriceFactory,
                            CurrencySign = _dictionaryServices.GetDictionary_CurrencySign(item.CurrencyType, list_Com_DataDictionary),
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return list_vm;
        }

        /// <summary>
        /// 批量导入产品
        /// </summary>
        /// <returns></returns>
        public VMAjaxProcessResult BatchImport(VMERPUser currentUser, int classifyID, string excelFileName, string excelName, List<DTOBatchUploadProduct> list, DTOBatchUploadProduct dtobatch)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Products.Where(d => !d.Deleted && !d.RootProductID.HasValue && !d.IsProductMixed);
                    List<string> list_No = query.Select(d => d.No).ToList();
                    List<string> list_UPC = query.Select(d => d.UPC).ToList();
                    //List<DAL.Classify> list_Classify = context.Classifies.Where(d => d.IsDelete == 0).ToList();
                    List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                    List<DAL.Factory> list_Factory = context.Factories.Where(d => d.IsDelete == 0).ToList();
                    List<DAL.Orders_Customers> list_Orders_Customers = context.Orders_Customers.Where(d => !d.IsDelete).ToList();

                    int AllCount = 0;
                    int SuccessCount = 0;
                    List<VMProductInfo> list_vm = ProductBatchImport.BatchImport(list_No, classifyID, excelFileName, list_UPC, list_Com_DataDictionary, list_Factory, list_Orders_Customers, excelName, list, dtobatch, out AllCount);
                    foreach (var item in list_vm)
                    {
                        DBOperationStatus dBOperationStatus = Create(currentUser, CommonCode.GetIP(), item, false);
                        if (dBOperationStatus == DBOperationStatus.Success)
                        {
                            SuccessCount++;
                        }
                    }

                    result.Msg = "批量导入产品的结果是，共：" + AllCount + "。成功：" + SuccessCount + "，失败：" + (AllCount - SuccessCount) + "。";
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);

                result.IsSuccess = false;
                result.Msg = ex.Message;
            }
            return result;
        }

        #endregion UserMethod

        #region PublicMethod

        /// <summary>
        /// 选择产品 报价单页面使用的
        /// </summary>
        public List<VMProductInfo> SelectProduct(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows,
            List<string> productNOs, int? customerID, string FactoryName, bool includeQuoteProducts, bool IsProductMixed = false)
        {
            List<VMProductInfo> listModel = null;
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Products.Include("Orders_Customers").Include("Factory").Where(p => !p.Deleted && p.Status == (short)ProductStatusEnum.Normal);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForProduct);

                    #region 筛选条件

                    if (productNOs != null && productNOs.Count > 0)
                    {
                        query = query.Where(p => productNOs.Contains(p.No));
                    }
                    if (customerID.HasValue)
                    {
                        query = query.Where(p => p.Orders_Customers.OCID == customerID.Value);
                    }
                    if (!string.IsNullOrEmpty(FactoryName))
                    {
                        query = query.Where(p => p.Factory.Abbreviation.Contains(FactoryName));
                    }

                    if (!includeQuoteProducts)
                    {
                        query = query.Where(p => !p.ParentProductID.HasValue);
                    }
                    if (IsProductMixed)
                    {
                        query = query.Where(p => p.IsProductMixed && !p.ParentProductMixedID.HasValue);
                    }
                    else
                    {
                        query = query.Where(p => !p.IsProductMixed);

                    }

                    #endregion 筛选条件

                    #region 排序

                    //if (includeQuoteProducts)
                    //{
                    //    query = query.OrderByDescending(p => p.No);
                    //}
                    //else
                    //{
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
                    //}

                    #endregion 排序

                    totalRows = query.Count();//获取总条数

                    var dataFromDB = query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();//分页

                    #region 给Model赋值

                    if (dataFromDB != null && dataFromDB.Count > 0)
                    {
                        listModel = new List<VMProductInfo>();

                        // 先查出字典表数据
                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());

                        foreach (var item in dataFromDB)
                        {
                            listModel.Add(GetViewModelFromDBEntity(item, list_Com_DataDictionary));
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

        public int? GetFactoryCurrencyType(int factoryID)
        {
            int? CurrencyType = null;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Factories.Find(factoryID);
                    if (query != null)
                    {
                        return query.CurrencyType;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return CurrencyType;
        }

        public string GetSelectCustomer_SeasonList(int CustomerID, int? season = null)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Orders_Customers.Find(CustomerID);
                    if (query != null)
                    {
                        string SelectCustomer = query.SelectCustomer.Replace("新客户", "所有客户");
                        var query_Com_DataDictionary = context.Com_DataDictionary.Where(d => d.TableKind == (int)DictionaryTableKind.Season && d.IsDelete == 0 && (d.SelectCustomer == SelectCustomer || d.SelectCustomer == "所有客户"));

                        sb.Append("<option value=''></option>");

                        foreach (var item in query_Com_DataDictionary.OrderBy(d => d.Alias))
                        {
                            string temp = item.Name + " - " + item.Alias;
                            if (string.IsNullOrEmpty(item.Alias))
                            {
                                temp = item.Name;
                            }
                            sb.Append("<option value='" + item.Code + "' " + (season == item.Code ? "selected='selected'" : "") + ">" + temp + "</option>");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 从客户里面取数据
        /// </summary>
        /// <param name="PortID"></param>
        /// <param name="customer"></param>
        /// <param name="vm_product"></param>
        /// <returns></returns>
        private static VMProductInfo SetEntityFromCustomer(int PortID, Orders_Customers customer, VMProductInfo vm_product)
        {
            vm_product.FreightRate = GetCustomerFreightRate(PortID, customer);

            vm_product.MiscImportLoad = customer.MiscImportLoadAmount;
            vm_product.Commission = customer.Commission;
            vm_product.Agent = customer.Agent;
            vm_product.Allowance = customer.Allowance;
            vm_product.MU = customer.MU ?? 0;
            vm_product.FOBNET = customer.FOBNET;
            vm_product.FinalFOB = null;
            vm_product.CtnsPallet = customer.CtnsPallet;
            vm_product.PcsPallet = customer.PcsPallet;
            if (vm_product.OuterVolume != 0 && vm_product.OuterBoxRate != 0)
            {
                vm_product.PalletPc = (customer.Palletpc / (2 * 35.315m) * vm_product.OuterVolume / vm_product.OuterBoxRate).Round();
            }
            vm_product.QuoteTemplateFileName = customer.QuoteTemplateFileName;
            vm_product.ELCFill = customer.ELCFill;
            return vm_product;
        }

        /// <summary>
        /// 获取客户的FreightRate
        /// </summary>
        /// <param name="PortID"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        private static decimal GetCustomerFreightRate(int PortID, Orders_Customers customer)
        {
            decimal FreightRate = 0;
            if (customer.Orders_FreightRate.Where(d => d.PortID == PortID).Count() > 0)
            {
                var query_FreightRate = customer.Orders_FreightRate;
                if (query_FreightRate.Count() > 0)
                {
                    var query_SinglePort = query_FreightRate.Where(d => d.Type == (int)CustomerFreightRateTypeEnum.SinglePort).Where(d => d.PortID == PortID);
                    var query_AllPort = query_FreightRate.Where(d => d.Type == (int)CustomerFreightRateTypeEnum.AllPort);
                    if (query_SinglePort.Count() > 0)
                    {
                        FreightRate = query_SinglePort.FirstOrDefault().FreightRate;
                    }
                    else if (query_AllPort.Count() > 0)//如果是全部港口
                    {
                        FreightRate = query_AllPort.FirstOrDefault().FreightRate;
                    }
                }
            }
            return FreightRate;
        }

        /// <summary>
        /// 获取产品信息 报价单页面使用的
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public List<VMProductInfo> GetProductInfoByQuote(VMERPUser currentUser, VMProductInfoByQuote vm)
        {
            List<VMProductInfo> listModel = new List<VMProductInfo>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    List<int> idList = CommonCode.IdListToList(vm.idList);
                    if (idList == null || idList.Count <= 0)
                    {
                        return listModel;
                    }
                    var query = context.Products.Where(p => idList.Contains(p.ID));
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForProduct);

                    var customer = context.Orders_Customers.Find(vm.CustomerID);
                    if (customer != null)
                    {
                        // 先查出字典表数据
                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                        List<DAL.HarmonizedSystem> list_HarmonizedSystem = context.HarmonizedSystems.Where(d => !d.IsDelete).ToList();

                        foreach (var item in query)
                        {
                            VMProductInfo vm_product = GetViewModelFromDBEntity(item, list_Com_DataDictionary);

                            vm_product = SetEntityFromCustomer(vm.PortID, customer, vm_product);

                            if (item.IsProductFitting)
                            {
                                var query_ProductFitting = context.ProductFittings.Where(d => d.ProductID == item.ID && !d.Deleted && d.ModuleType == (int)ModuleTypeEnum.Product);
                                decimal? PriceFactory = 0;
                                foreach (var item2 in query_ProductFitting)
                                {
                                    PriceFactory += item2.Qty * item2.PriceFactory * (1 + item2.FeesRate / 100);
                                }
                                //FTY PRICE=PriceFactory+配件单价*配件数量*（1+配件跟单费用%）
                                vm_product.PriceFactory += PriceFactory;
                            }

                            #region 计算Model

                            var HarmonizedSystem = list_HarmonizedSystem.Where(d => d.ID == vm_product.HTS);
                            if (HarmonizedSystem.Count() > 0)
                            {
                                vm_product.HTS_Name = HarmonizedSystem.FirstOrDefault().HSCode;
                                //vm_product.DutyPercent = HarmonizedSystem.FirstOrDefault().Cess;//DutyPercent。不可修改。取HSCode里面的税率。
                            }

                            vm_product.ProductID = item.ID;//报价单页面用到了
                            vm_product.TermsID = vm.TermsID;//报价单页面用到了
                            if (vm.PortID > 0)
                            {
                                vm_product.PortID = vm.PortID;
                            }
                            if (vm.TermsID == 3)
                            {
                                vm_product.Cost = null;
                            }

                            //Fobfty($) 。当工厂价格是美金报价，Fobfty=工厂价格。当工厂价格是人民币报价，Fobfty=工厂价格/汇率。
                            //Rate 。如果产品的工厂价格是人民币，取人民币的换汇。如果产品的工厂价格是美元，取美元的换汇。
                            if (vm_product.CurrencyName == Keys.RMB)
                            {
                                vm_product.FOBFTY = (vm_product.PriceFactory / vm.ExchangeRate).Round(4);
                                vm_product.Rate = vm.CurrencyExchangeRMB;
                            }
                            else if (vm_product.CurrencyName == Keys.USD)
                            {
                                vm_product.FOBFTY = vm_product.PriceFactory;
                                vm_product.Rate = vm.CurrencyExchangeUSD;
                            }

                            if (vm_product.QuoteTemplateFileName == "S164")//如果客人是S164时，Rate都取美元换汇
                            {
                                vm_product.Rate = vm.CurrencyExchangeUSD;
                            }

                            if (vm_product.OuterBoxRate != 0)
                            {
                                //Freight Amount = Freight rate*外箱材积/外箱率
                                var Freight = (vm_product.FreightRate * vm_product.OuterVolume / vm_product.OuterBoxRate);
                                vm_product.Freight = Freight.Round();
                            }

                            vm_product.MOQEn = CalculateMOQ(vm_product.MOQEn, vm_product.Cost);

                            #endregion 计算Model

                            vm_product.DutyPercentFormatter = vm_product.DutyPercent.HasValue ? vm_product.DutyPercent + "%" : "";
                            vm_product.CommissionFormatter = vm_product.Commission + "%";
                            vm_product.MiscImportLoadFormatter = vm_product.MiscImportLoad.HasValue ? vm_product.MiscImportLoad + "%" : "";
                            vm_product.FactoryID_ForQuote = vm_product.FactoryID;

                            vm_product.ExchangeRate = vm.ExchangeRate;
                            vm_product.CurrencyExchangeUSD = vm.CurrencyExchangeUSD;
                            vm_product.CurrencyExchangeRMB = vm.CurrencyExchangeRMB;

                            listModel.Add(vm_product);
                        }
                    }
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
        public List<VMProductInfo> GetProductInfoByOrder(VMERPUser currentUser, List<int> idList, int CustomerID, int PortID)
        {
            List<VMProductInfo> listModel = new List<VMProductInfo>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    if (idList == null || idList.Count <= 0)
                    {
                        return listModel;
                    }
                    var query = context.Products.Where(p => idList.Contains(p.ID));
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForProduct);
                    var query_customer = context.Orders_Customers.Find(CustomerID);
                    if (query_customer != null)
                    {
                        // 先查出字典表数据
                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());

                        foreach (var item in query)
                        {
                            VMProductInfo vm_product = GetViewModelFromDBEntity(item, list_Com_DataDictionary);
                            vm_product.ProductFrom = (short)OrderProductSource.FromProduct;

                            vm_product = SetEntityFromCustomer(PortID, query_customer, vm_product);

                            if (item.IsProductFitting)
                            {
                                var query_ProductFitting = context.ProductFittings.Where(d => d.ProductID == item.ID && !d.Deleted && d.ModuleType == (int)ModuleTypeEnum.Product);
                                decimal? PriceFactory = 0;
                                foreach (var item2 in query_ProductFitting)
                                {
                                    PriceFactory += item2.Qty * item2.PriceFactory * (1 + item2.FeesRate ?? 0 / 100);
                                }
                                //FTY PRICE=PriceFactory+配件单价*配件数量*（1+配件跟单费用%）
                                vm_product.PriceFactory += PriceFactory;
                            }
                            listModel.Add(vm_product);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return listModel;
        }

        /// <summary>
        /// 获取产品信息 混装产品页面使用的
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="idArray"></param>
        /// <returns></returns>
        public List<VMProductInfo> GetProductInfoByMixed(VMERPUser currentUser, List<int> idList)
        {
            List<VMProductInfo> listModel = new List<VMProductInfo>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    if (idList == null || idList.Count <= 0)
                    {
                        return listModel;
                    }
                    var query = context.Products.Where(p => idList.Contains(p.ID));
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForProduct);


                    // 先查出字典表数据
                    List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());

                    foreach (var item in query)
                    {
                        VMProductInfo vm_product = GetViewModelFromDBEntity(item, list_Com_DataDictionary);
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

        #endregion PublicMethod

        #region 定时缓存产品

        public bool CacheOneProduct(int userID, VMProductInfo productInfo)
        {
            bool result = false;
            try
            {
                var dto = new DTOMongo<VMProductInfo>() { LastUpdateDate = DateTime.Now, UserID = userID, Type = MongoCachedTypes.ProductEditing, Data = productInfo };
                var dtoJSON = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
                var document = BsonDocument.Parse(dtoJSON);

                var builder = Builders<BsonDocument>.Filter;
                var filter = builder.Eq("UserID", userID);
                filter &= builder.Eq("Type", MongoCachedTypes.ProductEditing);
                filter &= builder.Eq("Data.ID", productInfo.ID);

                new MongoDBHelper().UpsetOne(document, MongoDBHelper.CollectionName_UserCache, filter);
                result = true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return result;
        }

        public VMProductInfo GetOneCachedProduct(int userID, int productID)
        {
            VMProductInfo productInfo = null;

            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("UserID", userID);
            filter &= builder.Eq("Type", MongoCachedTypes.ProductEditing);
            filter &= builder.Eq("Data.ID", productID);

            BsonDocument document = new MongoDBHelper().GetOneRecord(filter, MongoDBHelper.CollectionName_UserCache);
            if (document != null)
            {
                DTOMongo<VMProductInfo> cachedData = BsonSerializer.Deserialize<DTOMongo<VMProductInfo>>(document);
                if (cachedData != null)
                {
                    productInfo = cachedData.Data;
                }
            }
            return productInfo;
        }

        public bool RemoveOneCachedProduct(int userID, int productID)
        {
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("UserID", userID);
            filter &= builder.Eq("Type", MongoCachedTypes.ProductEditing);
            filter &= builder.Eq("Data.ID", productID);

            return new MongoDBHelper().RemoveOneRecord(filter, MongoDBHelper.CollectionName_UserCache);
        }

        #endregion 定时缓存产品
    }
}