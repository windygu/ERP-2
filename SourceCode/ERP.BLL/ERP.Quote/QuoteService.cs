using AutoMapper;
using ERP.BLL.Consts;
using ERP.BLL.ERP.Consts;
using ERP.BLL.Workflow;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.Contacts;
using ERP.Models.CustomEnums;
using ERP.Models.Customer;
using ERP.Models.Product;
using ERP.Models.Quote;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ERP.BLL.ERP.Quote
{
    public class QuoteService
    {
        private ERP.Dictionary.DictionaryServices _dictionaryServices = new Dictionary.DictionaryServices();
        private Product.ProductServices _productServices = new Product.ProductServices();

        #region HelperMethod

        /// <summary>
        /// 获取报价单状态描述的内容
        /// </summary>
        /// <param name="i">编号</param>
        /// <returns></returns>

        private static string GetQuoteStatusEnum_Description(int StatusID)
        {
            return EnumHelper.GetCustomEnumDesc(typeof(QuoteStatusEnum), (QuoteStatusEnum)StatusID);
        }

        #endregion HelperMethod

        #region UserMethod

        /// <summary>
        /// 获取列表数据
        /// </summary>
        public List<DTOQuote> GetAll(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows,
          VMQuoteSearch vm_search)
        {
            List<DTOQuote> listModel = null;
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Quot_Quot.Where(p => !p.IsDelete);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForQuote);

                    if (vm_search.PageType == PageTypeEnum.PendingCheckList)//待审核
                    {
                        query = query.Where(d => d.StatusID == (short)QuoteStatusEnum.PendingCheck);
                    }
                    else
                    {
                        query = query.Where(d => d.StatusID != (short)QuoteStatusEnum.PendingCheck);
                    }

                    #region 筛选条件

                    if (!string.IsNullOrEmpty(vm_search.QuotNumber))
                    {
                        query = query.Where(d => d.QuotNumber.Contains(vm_search.QuotNumber));
                    }
                    if (!string.IsNullOrEmpty(vm_search.QuotDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(vm_search.QuotDateStart);
                        query = query.Where(d => d.QuotDate >= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.QuotDateEnd))
                    {
                        DateTime dt = CommonCode.GetDateEnd(vm_search.QuotDateEnd);
                        query = query.Where(d => d.QuotDate <= dt);
                    }
                    if (!string.IsNullOrEmpty(vm_search.Name))
                    {
                        query = query.Where(d => d.Quot_QuotProduct.Any(q => q.No.Contains(vm_search.Name.Trim()) && !q.IsDelete));
                    }
                    if (!string.IsNullOrEmpty(vm_search.CustomerCode))
                    {
                        query = query.Where(d => d.Orders_Customers.CustomerCode == vm_search.CustomerCode);
                    }
                    if (!string.IsNullOrEmpty(vm_search.FactoryName))
                    {
                        query = query.Where(d => d.Quot_QuotProduct.Any(q => q.Factory.Abbreviation.Contains(vm_search.FactoryName.Trim()) && !q.IsDelete));
                    }
                    if (!string.IsNullOrEmpty(vm_search.StatusID))
                    {
                        int statusID = Utils.StrToInt(vm_search.StatusID, (short)QuoteStatusEnum.OutLine);
                        query = query.Where(d => d.StatusID == statusID);
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
                        listModel = new List<DTOQuote>();
                        List<DAL.SystemUser> SystemUsers = context.SystemUsers.ToList();
                        foreach (var item in dataFromDB)
                        {
                            bool IsHasApprovalPermission = true;

                            #region 判断是否有审批流的权限

                            if (vm_search.PageType == PageTypeEnum.PendingCheckList)//待审核
                            {
                                IsHasApprovalPermission = ApprovalService.HasApprovalPermission(WorkflowTypes.ApprovalQuot, currentUser, item.ST_CREATEUSER, item.ApproverIndex, item.CustomerID);
                            }

                            #endregion 判断是否有审批流的权限

                            var Author = SystemUsers.Where(d => d.UserID == item.AuthorID);
                            var AuthorName = "";
                            if (Author.Count() > 0)
                            {
                                AuthorName = Author.FirstOrDefault().UserName;
                            }
                            listModel.Add(new DTOQuote()
                            {
                                ID = item.ID,
                                QuotNumber = item.QuotNumber,
                                CustomerID = item.CustomerID,
                                CustomerCode = item.Orders_Customers.CustomerCode,
                                QuotDate = Utils.DateTimeToStr(item.QuotDate),
                                ValidDate = Utils.DateTimeToStr(item.ValidDate),
                                AuthorName = AuthorName,
                                StatusName = GetQuoteStatusEnum_Description(item.StatusID),
                                StatusID = item.StatusID,
                                OrderID = item.OrderID == null ? "" : item.OrderID,
                                DT_MODIFYDATE = Utils.DateTimeToStr2(item.DT_MODIFYDATE),
                                IPAddress = item.IPAddress,
                                IsHasApprovalPermission = IsHasApprovalPermission,
                                ApproverIndex = item.ApproverIndex,
                                ST_CREATEUSER = item.ST_CREATEUSER,
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
        /// 批量删除、删除
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="IDs"></param>
        /// <returns></returns>
        public DBOperationStatus Delete(VMERPUser currentUser, List<int> IDs)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Quot_Quot.Where(p => IDs.Contains(p.ID));
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForQuote);
                    if (query != null)
                    {
                        foreach (var item in query)
                        {
                            item.IsDelete = true;
                            item.DT_MODIFYDATE = DateTime.Now;
                            item.ST_MODIFYUSER = currentUser.UserID;
                            item.IPAddress = CommonCode.GetIP();
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
        /// 新建报价单
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public ApiResult Add(VMERPUser currentUser, VMQuoteEdit vm)
        {
            var result = new ApiResult();
            if (vm.listProducts == null)
            {
                result.Success = false;
                result.Info = "请先选择产品！";
                return result;
            }

            string quotNumber = CommonCode.GetRandomNumber();

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    #region 添加报价单

                    //insert quot
                    var quot = new DAL.Quot_Quot
                    {
                        QuotNumber = quotNumber,
                        CustomerID = vm.CustomerID,
                        QuotDate = DateTime.Now,
                        ValidDate = Utils.StrToDateTime(vm.ValidDateFormat),
                        AuthorID = currentUser.UserID,
                        IsImmediatelySend = vm.IsImmediatelySend,
                        StatusID = vm.NewStatusID,
                        QuotTimes = 0,

                        ExchangeRate = vm.ExchangeRate,
                        CurrencyExchangeUSD = vm.CurrencyExchangeUSD,
                        CurrencyExchangeRMB = vm.CurrencyExchangeRMB,
                        TermsID = vm.TermsID,
                        PortID = vm.PortID,

                        IsDelete = false,
                        DT_CREATEDATE = DateTime.Now,
                        ST_CREATEUSER = currentUser.UserID,
                        DT_MODIFYDATE = DateTime.Now,
                        ST_MODIFYUSER = currentUser.UserID,
                        IPAddress = CommonCode.GetIP(),
                    };
                    context.Quot_Quot.Add(quot);

                    if (vm.NewStatusID != (short)QuoteStatusEnum.PendingCheck)
                    {
                        //insert quot history
                        var history = new DAL.Quot_QuotHistory
                        {
                            Comment = GetQuoteStatusEnum_Description(vm.NewStatusID),
                            DT_CREATEDATE = DateTime.Now,
                            ST_CREATEUSER = currentUser.UserID,
                            IPAddress = CommonCode.GetIP(),
                        };
                        quot.Quot_QuotHistory.Add(history);
                    }

                    //添加
                    foreach (var item in vm.listProducts)
                    {
                        var queryProudct = context.Products.Find(item.ProductID);
                        if (queryProudct != null)
                        {
                            #region 给Model赋值

                            item.PriceInputDate = queryProudct.PriceInputDate;
                            item.ValidDate = queryProudct.ValidDate;
                            item.CustomerID = queryProudct.CustomerID;
                            item.FactoryID = queryProudct.FactoryID;
                            item.NoFactory = queryProudct.NoFactory;
                            item.Name = queryProudct.Name;
                            item.UnitID = queryProudct.UnitID;
                            item.Length = queryProudct.Length;
                            item.Height = queryProudct.Height;
                            item.Width = queryProudct.Width;
                            item.Weight = queryProudct.Weight;
                            item.StyleID = queryProudct.StyleID;
                            item.PackingMannerZhID = queryProudct.PackingMannerZhID;
                            item.IngredientZh = queryProudct.IngredientZh;
                            item.PackingMannerEnID = queryProudct.PackingMannerEnID;

                            item.PDQLength = queryProudct.PDQLength;
                            item.PDQWidth = queryProudct.PDQWidth;
                            item.PDQHeight = queryProudct.PDQHeight;

                            item.InnerLength = queryProudct.InnerLength;
                            item.InnerWidth = queryProudct.InnerWidth;
                            item.InnerHeight = queryProudct.InnerHeight;

                            item.OuterLength = queryProudct.OuterLength;
                            item.OuterWidth = queryProudct.OuterWidth;
                            item.OuterHeight = queryProudct.OuterHeight;

                            item.CtnsPallet = queryProudct.CtnsPallet;
                            item.MOQEn = queryProudct.MOQEn;
                            item.CurrencyType = queryProudct.CurrencyType;
                            item.WeightLBS = queryProudct.WeightLBS;
                            item.InnerWeight = queryProudct.InnerWeight;
                            item.InnerWeightLBS = queryProudct.InnerWeightLBS;
                            item.InnerWeightGross = queryProudct.InnerWeightGross;
                            item.InnerWeightGrossLBS = queryProudct.InnerWeightGrossLBS;
                            item.FOBChinaLCL = queryProudct.FOBChinaLCL;
                            item.HSCode = queryProudct.HSCode;
                            item.HTS = queryProudct.HTS;
                            item.OuterWeightGross = queryProudct.OuterWeightGross;
                            item.OuterWeightNet = queryProudct.OuterWeightNet;

                            item.Season = queryProudct.Season;
                            item.ProductCopyRight = queryProudct.ProductCopyRight;
                            item.ColorID = queryProudct.ColorID;
                            item.IsProductFitting = queryProudct.IsProductFitting;

                            #endregion 给Model赋值

                            //insert quot products

                            DAL.Quot_QuotProduct quotProduct = SetDBEntityFromViewModel(item, currentUser.UserID);

                            quotProduct.PriceFactory = queryProudct.PriceFactory;
                            context.Quot_QuotProduct.Add(quotProduct);

                            //insert quot product history
                            Mapper.CreateMap<Quot_QuotProduct, Quot_QuotProductHistory>();
                            var productHistory = Mapper.Map<Quot_QuotProductHistory>(quotProduct);
                            productHistory.QuotProductID = item.ID;
                            productHistory.Status = "新增";
                            productHistory.ST_CREATEUSER = currentUser.UserID;
                            productHistory.DT_CREATEDATE = DateTime.Now;
                            quotProduct.Quot_QuotProductHistory.Add(productHistory);
                        }
                    }

                    int affectRows = context.SaveChanges();
                    if (affectRows > 0)
                    {
                        result.Identity = quotNumber;

                        ExecuteApproval(quot.ST_CREATEUSER, quot.ID, "", vm.NewStatusID, currentUser.UserID, false);//执行审批流

                        RemoveOneCachedQuote(currentUser.UserID, -1);
                        Save_ProductMixed(currentUser.UserID, quot.ID, vm.listProducts_Mixed);
                    }
                    else
                    {
                        result.Success = false;
                        result.Info = "出错了！";
                    }

                    #endregion 添加报价单
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Info = "出错了！";
                LogHelper.WriteError(ex);
            }
            return result;
        }

        /// <summary>
        /// 保存报价单
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public ApiResult Save(VMERPUser currentUser, VMQuoteEdit vm)
        {
            var result = new ApiResult();
            if (vm.listProducts == null)
            {
                result.Success = false;
                result.Info = "请先选择产品！";
                return result;
            }

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    bool historyAdded = false;
                    int quotID = vm.ID;
                    int NewStatusID = vm.NewStatusID;

                    var query = context.Quot_Quot.Find(quotID);
                    query.IsImmediatelySend = vm.IsImmediatelySend;

                    query.ExchangeRate = vm.ExchangeRate;
                    query.CurrencyExchangeUSD = vm.CurrencyExchangeUSD;
                    query.CurrencyExchangeRMB = vm.CurrencyExchangeRMB;
                    query.TermsID = vm.TermsID;
                    query.PortID = vm.PortID;

                    query.DT_MODIFYDATE = DateTime.Now;
                    query.ST_MODIFYUSER = currentUser.UserID;
                    query.IPAddress = CommonCode.GetIP();

                    if (query.StatusID == (int)QuoteStatusEnum.OutLine || NewStatusID == (int)QuoteStatusEnum.OutLine)
                    {
                        query.StatusID = NewStatusID;
                    }

                    if (NewStatusID == (int)QuoteStatusEnum.NotPassCheck || NewStatusID == (int)QuoteStatusEnum.PassedCheck)
                    {
                        query.CheckSuggest = vm.CheckSuggest;
                    }

                    if (NewStatusID == (int)QuoteStatusEnum.ReQutes)//重新报价
                    {
                        query.QuotTimes += 1;
                        query.StatusID = (int)QuoteStatusEnum.PendingCheck;
                        query.ApproverIndex = null;
                    }

                    //不是草稿和审核修改状态时，添加报价单的历史记录
                    if (NewStatusID != (int)QuoteStatusEnum.OutLine && NewStatusID != (int)QuoteStatusEnum.NotPassCheck && NewStatusID != (int)QuoteStatusEnum.PassedCheck)
                    {
                        historyAdded = true;
                        //insert quot history
                        var quotHistory = new DAL.Quot_QuotHistory
                        {
                            Comment = GetQuoteStatusEnum_Description(NewStatusID),
                            CheckSuggest = vm.CheckSuggest,
                            DT_CREATEDATE = DateTime.Now,
                            ST_CREATEUSER = currentUser.UserID,
                            IPAddress = CommonCode.GetIP(),
                        };
                        query.Quot_QuotHistory.Add(quotHistory);
                    }

                    var queryProducts_IDList = query.Quot_QuotProduct.Where(d => d.QuotID == quotID && !d.IsDelete && !d.ParentProductMixedID.HasValue).Select(d => d.ID).ToList();
                    var vmProducts_IDList = vm.listProducts.Select(d => d.ID).ToList();

                    var idlist_update = queryProducts_IDList.Intersect(vmProducts_IDList);//更新的ID列表
                    var idlist_delete = queryProducts_IDList.Except(vmProducts_IDList);//删除的ID列表

                    //删除
                    var listProducts_Delete = query.Quot_QuotProduct.Where(d => idlist_delete.Contains(d.ID) && d.QuotID == quotID && !d.ParentProductMixedID.HasValue);
                    foreach (var item in listProducts_Delete)
                    {
                        item.IsDelete = true;
                        item.DT_MODIFYDATE = DateTime.Now;
                        item.IPAddress = CommonCode.GetIP();
                        item.ST_MODIFYUSER = currentUser.UserID;
                    }

                    //删除混装产品
                    var listProductsMixed_Delete = query.Quot_QuotProduct.Where(d => d.IsProductMixed && d.ParentProductMixedID.HasValue && d.QuotID == quotID);
                    if (listProductsMixed_Delete.Count() > 0)
                    {
                        context.Quot_QuotProduct.RemoveRange(listProductsMixed_Delete);
                    }

                    //更新
                    foreach (var item_ID in idlist_update)
                    {
                        var queryProudct = query.Quot_QuotProduct.Where(d => d.ID == item_ID).FirstOrDefault();
                        if (queryProudct != null)
                        {
                            var Products_Update = vm.listProducts.Find(d => d.ID == item_ID);

                            SetDBEntityFromViewModel(Products_Update, currentUser.UserID, queryProudct);
                        }
                    }

                    //添加
                    foreach (var item in vm.listProducts.Where(d => d.ID <= 0))
                    {
                        var queryProudct = context.Products.Find(item.ProductID);
                        if (queryProudct != null)
                        {
                            #region 给Model赋值

                            item.PriceInputDate = queryProudct.PriceInputDate;
                            item.ValidDate = queryProudct.ValidDate;
                            item.QuotID = quotID;
                            item.Season = queryProudct.Season;
                            item.ProductCopyRight = queryProudct.ProductCopyRight;
                            item.ColorID = queryProudct.ColorID;
                            item.IsProductFitting = queryProudct.IsProductFitting;

                            #endregion 给Model赋值

                            //insert quot products

                            DAL.Quot_QuotProduct quotProduct = SetDBEntityFromViewModel(item, currentUser.UserID);

                            quotProduct.PriceFactory = queryProudct.PriceFactory;

                            context.Quot_QuotProduct.Add(quotProduct);

                            //insert quot product history
                            Mapper.CreateMap<Quot_QuotProduct, Quot_QuotProductHistory>();
                            var productHistory = Mapper.Map<Quot_QuotProductHistory>(quotProduct);
                            productHistory.QuotProductID = item.ID;
                            productHistory.Status = "新增";
                            productHistory.ST_CREATEUSER = currentUser.UserID;
                            productHistory.DT_CREATEDATE = DateTime.Now;
                            quotProduct.Quot_QuotProductHistory.Add(productHistory);
                        }
                    }

                    int affectRows = context.SaveChanges();

                    if (affectRows > 0)
                    {
                        ExecuteApproval(query.ST_CREATEUSER, query.ID, vm.CheckSuggest, NewStatusID, currentUser.UserID, historyAdded);//执行审批流

                        #region 审核通过后，立即发送邮件给客户

                        var match2 = new ERPEntitiesNew().Quot_Quot.Find(quotID);
                        if (query.IsImmediatelySend && match2.StatusID == (int)QuoteStatusEnum.PassedCheck)
                        {
                            if (IsCreateQuoteTemplate(query.TemplateLastCreateTime, query.DT_MODIFYDATE))
                            {
                                var customers = context.Orders_Customers.Find(vm.CustomerID);
                                if (string.IsNullOrEmpty(customers.QuoteTemplateFileName))//如果该客户的报价单模板不存在
                                {
                                    result.Success = false;
                                    result.Info = "请先修改该客户的报价单模板。您可以点击[<a href='/Customer/Edit/" + customers.OCID + "' target='_blank'>修改客户信息</a>]》客户参数》报价单模板！";
                                    return result;
                                }

                                //创建报价单模板
                                CreateQuoteTemplate(currentUser, quotID, query.Orders_Customers.QuoteTemplateFileName);

                                query.TemplateLastCreateTime = DateTime.Now;
                            }

                            query.StatusID = (int)QuoteStatusEnum.HadSend;
                            context.SaveChanges();

                            //get contacts email
                            var contact = context.Orders_Contacts.Where(d => d.OCID == query.CustomerID && !d.IsDelete);
                            if (contact.Count() > 0)
                            {
                                string default_email = contact.First().Email;
                                var default_contact = contact.Where(d => d.IsDefault);//如果有默认联系人
                                if (default_contact.Count() > 0)
                                {
                                    default_email = default_contact.First().Email;

                                    List<string> listFile = new List<string>();

                                    var dictionary = GetQuoteTemplatePath(currentUser, quotID);

                                    foreach (var item in dictionary)
                                    {
                                        string filePath = "/data/quot/out/" + quotID + "/" + item.Value + "/PDFAndExcel/" + item.Key.Replace(".jpg", ".pdf");
                                        listFile.Add(System.Web.HttpContext.Current.Server.MapPath("~" + filePath));
                                    }

                                    Email.SendEmail(default_email, "报价单信息", "报价单模板，链接地址：" + Keys.ERPUrl + "/Quote/Template/" + quotID, listFile, MailType.Quot, currentUser);
                                }
                            }
                        }

                        #endregion 审核通过后，立即发送邮件给客户

                        RemoveOneCachedQuote(currentUser.UserID, quotID);

                        Save_ProductMixed(currentUser.UserID, quotID, vm.listProducts_Mixed);
                    }
                    else
                    {
                        result.Success = false;
                        result.Info = "出错了！";
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
                result.Success = false;
                result.Info = "出错了！";
            }
            return result;
        }

        /// <summary>
        /// 保存混装产品
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="context"></param>
        /// <param name="quotID"></param>
        private void Save_ProductMixed(int UserID, int quotID, List<VMQuoteProduct> list)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    #region 添加混装产品

                    var query2 = context.Quot_QuotProduct.Where(d => d.QuotID == quotID && !d.IsDelete && d.IsProductMixed && !d.ParentProductMixedID.HasValue);
                    if (query2 != null && query2.Count() > 0)
                    {
                        foreach (var item in query2)
                        {
                            var thisProduct2 = list.Where(d => (d.ParentProductMixedID < 0 && d.ParentProductMixedID == -item.ProductID) || (d.ParentProductMixedID > 0 && d.ParentProductMixedID == item.ID));
                            if (thisProduct2.Count() > 0)
                            {
                                foreach (var item2 in thisProduct2)
                                {
                                    var queryProductMixed = context.Products.Where(d => d.IsProductMixed && !d.Deleted && d.ParentProductMixedID == item.ProductID && d.ID == item2.ProductID);
                                    if (queryProductMixed.Count() > 0)
                                    {
                                        var thisProduct = queryProductMixed.First();
                                        DAL.Quot_QuotProduct quotProductMixed = SetDBEntityFromViewModel(thisProduct, UserID);

                                        quotProductMixed.ParentProductMixedID = item.ID;
                                        quotProductMixed.QuotID = item.QuotID;
                                        quotProductMixed.QuoteTemplateFileName = item.QuoteTemplateFileName;

                                        quotProductMixed.Qty = item2.Qty;
                                        quotProductMixed.PortEnID = item2.PortEnID;
                                        quotProductMixed.Commission = item2.Commission;
                                        quotProductMixed.MiscImportLoad = item2.MiscImportLoad;
                                        quotProductMixed.Agent = item2.Agent;

                                        quotProductMixed.OuterVolume = item2.OuterVolume;
                                        quotProductMixed.FreightRate = item2.FreightRate;
                                        quotProductMixed.Freight = item2.Freight;
                                        quotProductMixed.Rate = item2.Rate;
                                        quotProductMixed.FinalFOB = item2.FinalFOB;

                                        quotProductMixed.Duty = item2.Duty;
                                        quotProductMixed.CommissionAmount = item2.CommissionAmount;
                                        quotProductMixed.MiscImportLoadAmount = item2.MiscImportLoadAmount;
                                        quotProductMixed.AgentAmount = item2.AgentAmount;
                                        quotProductMixed.ELC = item2.ELC;
                                        quotProductMixed.Retail = item2.Retail;
                                        quotProductMixed.MU = item2.MU;
                                        quotProductMixed.Cost = item2.Cost;
                                        quotProductMixed.TermsID = item2.TermsID;
                                        quotProductMixed.PortID = item2.PortID;
                                        quotProductMixed.FactoryID_ForQuote = item2.FactoryID_ForQuote;

                                        quotProductMixed.INR = item2.INR;
                                        quotProductMixed.TypeOfWood = item2.TypeOfWood;

                                        context.Quot_QuotProduct.Add(quotProductMixed);
                                    }
                                }

                            }

                        }
                        int i = context.SaveChanges();

                    }
                    #endregion 添加混装产品
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
        }

        /// <summary>
        /// 发送邮件给客户
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public ApiResult SendEmail(VMERPUser currentUser, VMQuoteSendEmail vm)
        {
            var result = new ApiResult();
            try
            {
                if (vm.StatusID == 2 && vm.Contacts == null)//通过系统发送邮件
                {
                    result.Success = false;
                    result.Info = "请先选择联系人！";
                    return result;
                }

                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var match = context.Quot_Quot.Find(vm.ID);
                    match.StatusID = (int)QuoteStatusEnum.HadSend;

                    //insert quot history
                    var quotHistory = new DAL.Quot_QuotHistory
                    {
                        Comment = GetQuoteStatusEnum_Description(match.StatusID),
                        DT_CREATEDATE = DateTime.Now,
                        ST_CREATEUSER = currentUser.UserID,
                        IPAddress = CommonCode.GetIP(),
                    };
                    match.Quot_QuotHistory.Add(quotHistory);

                    result.Info = "保存成功！";
                    if (vm.StatusID == 2)
                    {
                        result.Info = "邮件发送成功！";
                        //if (fileSize > 5 * 1024 * 1024)
                        //{
                        //    result.Info = "邮件发送成功！但是邮件附件大小超过5M可能导致客户收不到。";
                        //}
                        if (vm.SendEmail.IsContainMakerExcel)
                        {
                            var customers = context.Orders_Customers.Find(vm.Contacts.First().OCID);
                            if (string.IsNullOrEmpty(customers.QuoteTemplateFileName))//如果该客户的报价单模板不存在
                            {
                                result.Success = false;
                                result.Info = "请先修改该客户的报价单模板。您可以点击[<a href='/Customer/Edit/" + customers.OCID + "' target='_blank'>修改客户信息</a>]》客户参数》报价单模板！";
                                return result;
                            }
                            if (IsCreateQuoteTemplate(match.TemplateLastCreateTime, match.DT_MODIFYDATE))
                            {
                                CreateQuoteTemplate(currentUser, vm.ID, customers.QuoteTemplateFileName);//创建报价单模板
                                match.TemplateLastCreateTime = DateTime.Now;
                            }

                            vm.SendEmail.Attachs = ConstsMethod.ReplaceURLToLocalPath(vm.SendEmail.Attachs);
                        }

                        var dictionary = GetQuoteTemplatePath(currentUser, vm.ID);
                        foreach (var item in dictionary)
                        {
                            string filePath = "/data/quot/out/" + vm.ID + "/" + item.Value + "/PDFAndExcel/" + item.Key.Replace(".jpg", ".pdf") + ";";
                            vm.SendEmail.Attachs += Utils.GetMapPath("~" + filePath);
                        }

                        bool isSendSuccessful = Email.SendEmail(currentUser.UserName, vm.SendEmail);
                        if (!isSendSuccessful)
                        {
                            result.Success = false;
                            result.Info = "电子邮件发送失败！";
                            return result;
                        }
                    }

                    int affectRows = context.SaveChanges();
                    if (affectRows > 0)
                    {
                        result.Success = true;
                    }
                    else
                    {
                        result.Success = false;
                        result.Info = "出错了！";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Info = "出错了！" + ex.Message;
                LogHelper.WriteError(ex);
            }
            return result;
        }

        /// <summary>
        /// 获取报价单信息
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public VMQuoteEdit GetDetailByID(VMERPUser currentUser, int id)
        {
            VMQuoteEdit vm = null;
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                var query = context.Quot_Quot.Find(id);
                vm = new VMQuoteEdit
                {
                    ID = query.ID,
                    ValidDate = query.ValidDate,
                    ValidDateFormat = Utils.DateTimeToStr(query.ValidDate),
                    CustomerID = query.CustomerID,
                    CustomerCode = query.Orders_Customers.CustomerCode,
                    IsImmediatelySend = query.IsImmediatelySend,

                    QuotNumber = query.QuotNumber,
                    QuotDate = Utils.DateTimeToStr(query.QuotDate),
                    AuthorName = query.SystemUser.UserName,
                    QuotTimes = query.QuotTimes,
                    StatusID = query.StatusID,
                    StatusName = GetQuoteStatusEnum_Description(query.StatusID),
                    CheckSuggest = query.CheckSuggest,

                    ExchangeRate = query.ExchangeRate,
                    CurrencyExchangeUSD = query.CurrencyExchangeUSD,
                    CurrencyExchangeRMB = query.CurrencyExchangeRMB,
                    TermsID = query.TermsID,
                    PortID = query.PortID,

                    //从客户资料取
                    AllowancePercent = query.Orders_Customers.Allowance,
                    CommissionPercent = query.Orders_Customers.Commission,
                    PalletPc = query.Orders_Customers.Palletpc,
                    MiscImportLoadPercent = query.Orders_Customers.MiscImportLoadAmount,

                    ST_CREATEUSER = query.ST_CREATEUSER,
                    ApproverIndex = query.ApproverIndex,
                };
                vm.listProducts = GetQuoteProducts(currentUser, id, false);
                vm.listProducts_Mixed = GetQuoteProducts(currentUser, id, false, 2);
                vm.listHistory = GetQuoteHistories(currentUser, id);
            }
            return vm;
        }

        /// <summary>
        /// 删除报价的产品
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="id"></param>
        /// <param name="IsDelete"></param>
        /// <returns></returns>
        public DBOperationStatus DeleteProduct(VMERPUser currentUser, int id, bool IsDelete)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Quot_QuotProduct.Find(id);
                    if (query != null)
                    {
                        query.IsDelete = true;
                        query.DT_MODIFYDATE = DateTime.Now;
                        query.ST_MODIFYUSER = currentUser.UserID;
                        query.IPAddress = CommonCode.GetIP();

                        #region 添加删除报价单的记录

                        if (IsDelete)
                        {
                            //insert quot product history
                            Mapper.CreateMap<Quot_QuotProduct, Quot_QuotProductHistory>();
                            var productHistory = Mapper.Map<Quot_QuotProductHistory>(query);
                            productHistory.QuotProductID = query.ID;
                            productHistory.Status = "删除";
                            productHistory.ST_CREATEUSER = currentUser.UserID;
                            productHistory.DT_CREATEDATE = DateTime.Now;
                            query.Quot_QuotProductHistory.Add(productHistory);
                        }

                        #endregion 添加删除报价单的记录

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
        /// 保存 修改状态
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public DBOperationStatus Save_ChangeStatus(VMERPUser currentUser, int id, QuoteStatusEnum quoteStatusEnum)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Quot_Quot.Find(id);
                    if (query != null)
                    {
                        query.StatusID = (int)quoteStatusEnum;
                        query.DT_MODIFYDATE = DateTime.Now;
                        query.ST_MODIFYUSER = currentUser.UserID;
                        query.IPAddress = CommonCode.GetIP();

                        var quotHistory = new DAL.Quot_QuotHistory
                        {
                            Comment = GetQuoteStatusEnum_Description((int)quoteStatusEnum),
                            DT_CREATEDATE = DateTime.Now,
                            ST_CREATEUSER = currentUser.UserID,
                            IPAddress = CommonCode.GetIP(),
                        };
                        query.Quot_QuotHistory.Add(quotHistory);

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
        /// 生成报价单模板
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public VMAjaxProcessResult MakeExcel(VMERPUser currentUser, int id)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                var query = context.Quot_Quot.Find(id);
                var customers = context.Orders_Customers.Find(query.CustomerID);
                if (string.IsNullOrEmpty(customers.QuoteTemplateFileName))
                {
                    result.IsSuccess = false;
                    result.Msg = "请先修改该客户的报价单模板。您可以点击[<a href='/Customer/Edit/" + customers.OCID + "' target='_blank'>修改客户信息</a>]给该客户配置报价单模板！";
                    return result;
                }

                if (IsCreateQuoteTemplate(query.TemplateLastCreateTime, query.DT_MODIFYDATE))
                {
                    //创建报价单模板
                    CreateQuoteTemplate(currentUser, id, customers.QuoteTemplateFileName);

                    query.TemplateLastCreateTime = DateTime.Now;

                    context.SaveChanges();
                    result.Msg = "Yes";
                    return result;
                }
            }
            result.Msg = "No";
            return result;
        }

        /// <summary>
        /// 是否创建报价单模板
        /// </summary>
        /// <param name="TemplateLastCreateTime"></param>
        /// <param name="DT_MODIFYDATE"></param>
        /// <returns></returns>
        private static bool IsCreateQuoteTemplate(DateTime? TemplateLastCreateTime, DateTime DT_MODIFYDATE)
        {
            bool IsCreateQuoteTemplate = false;
            if (!TemplateLastCreateTime.HasValue)
            {
                IsCreateQuoteTemplate = true;
            }
            else if ((DateTime)TemplateLastCreateTime < DT_MODIFYDATE)
            {
                IsCreateQuoteTemplate = true;
            }
            return IsCreateQuoteTemplate;
        }

        #endregion UserMethod

        #region PublicMethod

        private VMQuoteProduct GetViewModelFromDBEntity(DAL.Quot_QuotProduct entity, List<DAL.Com_DataDictionary> dictionaries)
        {
            return new VMQuoteProduct()
            {
                #region 公共的

                ID = entity.ID,
                No = entity.No,
                NoFactory = entity.NoFactory,
                Name = entity.Name,
                Desc = entity.Desc,
                Image = entity.Image,
                Length = entity.Length,
                Height = entity.Height,
                Width = entity.Width,
                Weight = entity.Weight,
                IngredientZh = entity.IngredientZh,
                IngredientEn = entity.IngredientEn,
                MOQZh = entity.MOQZh,
                PDQPackRate = entity.PDQPackRate,
                PDQLength = entity.PDQLength,
                PDQWidth = entity.PDQWidth,
                PDQHeight = entity.PDQHeight,
                InnerBoxRate = entity.InnerBoxRate,
                InnerLength = entity.InnerLength,
                InnerWidth = entity.InnerWidth,
                InnerHeight = entity.InnerHeight,
                InnerWeight = entity.InnerWeight,
                InnerWeightGross = entity.InnerWeightGross,
                OuterBoxRate = entity.OuterBoxRate,
                OuterLength = entity.OuterLength,
                OuterWidth = entity.OuterWidth,
                OuterHeight = entity.OuterHeight,
                PriceFactory = entity.PriceFactory,
                MiscImportLoad = entity.MiscImportLoad,
                SRP = entity.SRP,
                CtnsPallet = entity.CtnsPallet,
                DutyPercent = entity.DutyPercent,
                Remarks = entity.Remarks,
                Comment = entity.Comment,
                UPC = entity.UPC,

                LengthIN = entity.LengthIN,
                HeightIN = entity.HeightIN,
                WidthIN = entity.WidthIN,
                WeightLBS = entity.WeightLBS,
                PDQLengthIN = entity.PDQLengthIN,
                PDQWidthIN = entity.PDQWidthIN,
                PDQHeightIN = entity.PDQHeightIN,
                InnerLengthIN = entity.InnerLengthIN,
                InnerWidthIN = entity.InnerWidthIN,
                InnerHeightIN = entity.InnerHeightIN,
                OuterVolume = entity.OuterVolume,
                InnerVolume = entity.InnerVolume,
                InnerWeightLBS = entity.InnerWeightLBS,
                InnerWeightGrossLBS = entity.InnerWeightGrossLBS,
                OuterLengthIN = entity.OuterLengthIN,
                OuterWidthIN = entity.OuterWidthIN,
                OuterHeightIN = entity.OuterHeightIN,
                FOBFTY = entity.FOBFTY,
                FOBNET = entity.FOBNET,
                Rate = entity.Rate,
                FinalFOB = entity.FinalFOB,
                MiscImportLoadAmount = entity.MiscImportLoadAmount,
                PcsPallet = entity.PcsPallet,
                PalletPc = entity.PalletPc,
                Duty = entity.Duty,
                FOBChinaLCL = entity.FOBChinaLCL,
                Cost = entity.Cost,
                Commission = entity.Commission,
                CommissionAmount = entity.CommissionAmount,

                Allowance = entity.Allowance,
                CustomerID = entity.CustomerID,
                CustomerNo = entity.Orders_Customers == null ? null : entity.Orders_Customers.CustomerCode,
                CustomerName = entity.Orders_Customers == null ? null : entity.Orders_Customers.CustomerName,
                FreightRate = entity.FreightRate,
                Agent = entity.Agent,
                FactoryID = entity.FactoryID,
                FactoryName = entity.Factory == null ? null : entity.Factory.Abbreviation,

                StyleID = entity.StyleID,
                StyleName = _dictionaryServices.GetDictionary_StyleName(entity.StyleID, dictionaries),
                StyleNumber = _dictionaryServices.GetDictionary_StyleNumber(entity.StyleID, dictionaries),
                PortID = entity.PortID,
                PortName = _dictionaryServices.GetDictionary_PortName(entity.PortID, dictionaries),
                PortEnName = _dictionaryServices.GetDictionary_PortEnName(entity.PortID, dictionaries),
                PackingMannerZhID = entity.PackingMannerZhID,
                PackingMannerZhName = _dictionaryServices.GetDictionary_PackingMannerZhName(entity.PackingMannerZhID, dictionaries),
                PackingMannerEnID = entity.PackingMannerEnID,
                PackingMannerEnName = entity.PackingMannerEnID == null ? "" : _dictionaryServices.GetDictionary_PackingMannerEnName(entity.PackingMannerZhID, dictionaries),
                UnitID = entity.UnitID,
                UnitName = _dictionaryServices.GetDictionary_UnitName(entity.UnitID, dictionaries),
                UnitEngName = _dictionaryServices.GetDictionaryByAlias(entity.UnitID, dictionaries),
                CurrencyType = entity.CurrencyType,
                CurrencyName = _dictionaryServices.GetDictionary_CurrencyName(entity.CurrencyType, dictionaries),
                CurrencySign = _dictionaryServices.GetDictionary_CurrencySign(entity.CurrencyType, dictionaries),

                HTS = entity.HTS,
                HSCode = entity.HSCode,

                #endregion 公共的

                DT_MODIFYDATE = entity.DT_MODIFYDATE,
                FOBChinaPort = entity.FOBChinaPort ?? 0,
                ProductID = entity.ProductID,
                OuterWeightGross = entity.OuterWeightGross,
                OuterWeightNet = entity.OuterWeightNet,
                OuterWeightGrossLBS = entity.OuterWeightGrossLBS,
                OuterWeightNetLBS = entity.OuterWeightNetLBS,
                FOBUS = entity.FOBUS,
                DDP = entity.DDP,
                POE = entity.POE,
                MU = entity.MU,
                AgentAmount = entity.AgentAmount,
                ELC = entity.ELC,
                Retail = entity.Retail,
                Freight = entity.Freight,
                CheckSuggest = entity.CheckSuggest,
                PortEnID = entity.PortEnID,
                TermsID = entity.TermsID,
                IsDelete = entity.IsDelete,

                QuotID = entity.QuotID,
                MOQEn = _productServices.CalculateMOQ(entity.MOQEn, entity.Cost),
                QuoteTemplateFileName = entity.QuoteTemplateFileName,
                ELCFill = entity.ELCFill,

                DutyPercentFormatter = entity.DutyPercent.HasValue ? entity.DutyPercent + "%" : "",
                CommissionFormatter = entity.Commission.HasValue ? entity.DutyPercent + "%" : "",
                MiscImportLoadFormatter = entity.MiscImportLoad.HasValue ? entity.MiscImportLoad + "%" : "",

                FactoryID_ForQuote = entity.FactoryID_ForQuote.HasValue ? entity.FactoryID_ForQuote.Value : entity.FactoryID,
                INR = entity.INR,
                TypeOfWood = entity.TypeOfWood,
                ColorEngName = _dictionaryServices.GetDictionaryByAlias(entity.ColorID, dictionaries),
                IsProductFitting = entity.IsProductFitting,
                IsProductMixed = entity.IsProductMixed,
                ParentProductMixedID = entity.ParentProductMixedID,

                Qty = entity.Qty ?? 0,
                ExchangeRate = entity.ExchangeRate,
                CurrencyExchangeUSD = entity.CurrencyExchangeUSD,
                CurrencyExchangeRMB = entity.CurrencyExchangeRMB,
            };
        }

        private VMQuoteProductHistory GetViewModelFromDBEntity_History(DAL.Quot_QuotProductHistory entity, List<DAL.Com_DataDictionary> dictionaries, List<DAL.Factory> factories, List<DAL.Orders_Customers> customers)
        {
            return new VMQuoteProductHistory()
            {
                #region 公共的

                ID = entity.ID,
                No = entity.No,
                NoFactory = entity.NoFactory,
                Name = entity.Name,
                Desc = entity.Desc,
                Image = entity.Image,
                Length = entity.Length,
                Height = entity.Height,
                Width = entity.Width,
                Weight = entity.Weight,
                IngredientZh = entity.IngredientZh,
                IngredientEn = entity.IngredientEn,
                MOQZh = entity.MOQZh,
                MOQEn = entity.MOQEn,
                PDQPackRate = entity.PDQPackRate,
                PDQLength = entity.PDQLength,
                PDQWidth = entity.PDQWidth,
                PDQHeight = entity.PDQHeight,
                InnerBoxRate = entity.InnerBoxRate,
                InnerLength = entity.InnerLength,
                InnerWidth = entity.InnerWidth,
                InnerHeight = entity.InnerHeight,
                InnerWeight = entity.InnerWeight,
                InnerWeightGross = entity.InnerWeightGross,
                OuterBoxRate = entity.OuterBoxRate,
                OuterLength = entity.OuterLength,
                OuterWidth = entity.OuterWidth,
                OuterHeight = entity.OuterHeight,
                PriceFactory = entity.PriceFactory,
                MiscImportLoad = entity.MiscImportLoad,
                SRP = entity.SRP,
                CtnsPallet = entity.CtnsPallet,
                DutyPercent = entity.DutyPercent,
                Remarks = entity.Remarks,
                Comment = entity.Comment,
                UPC = entity.UPC,

                LengthIN = entity.LengthIN,
                HeightIN = entity.HeightIN,
                WidthIN = entity.WidthIN,
                WeightLBS = entity.WeightLBS,
                PDQLengthIN = entity.PDQLengthIN,
                PDQWidthIN = entity.PDQWidthIN,
                PDQHeightIN = entity.PDQHeightIN,
                InnerLengthIN = entity.InnerLengthIN,
                InnerWidthIN = entity.InnerWidthIN,
                InnerHeightIN = entity.InnerHeightIN,
                OuterVolume = entity.OuterVolume,
                InnerVolume = entity.InnerVolume,
                InnerWeightLBS = entity.InnerWeightLBS,
                InnerWeightGrossLBS = entity.InnerWeightGrossLBS,
                OuterLengthIN = entity.OuterLengthIN,
                OuterWidthIN = entity.OuterWidthIN,
                OuterHeightIN = entity.OuterHeightIN,
                FOBFTY = entity.FOBFTY,
                FOBNET = entity.FOBNET,
                Rate = entity.Rate,
                FinalFOB = entity.FinalFOB,
                MiscImportLoadAmount = entity.MiscImportLoadAmount,
                PcsPallet = entity.PcsPallet,
                PalletPc = entity.PalletPc,
                Duty = entity.Duty,
                FOBChinaLCL = entity.FOBChinaLCL,
                Cost = entity.Cost,
                Commission = entity.Commission,
                CommissionAmount = entity.CommissionAmount,

                Allowance = entity.Allowance,
                CustomerID = entity.CustomerID,
                CustomerNo = entity.Orders_Customers.CustomerCode,
                CustomerName = entity.Orders_Customers.CustomerName,
                FreightRate = entity.FreightRate,
                Agent = entity.Agent,
                FactoryID = entity.FactoryID,
                FactoryName = entity.Factory.Abbreviation,

                StyleID = entity.StyleID,
                StyleName = _dictionaryServices.GetDictionary_StyleName(entity.StyleID, dictionaries),
                StyleNumber = _dictionaryServices.GetDictionary_StyleNumber(entity.StyleID, dictionaries),
                PortID = entity.PortID,
                PortName = _dictionaryServices.GetDictionary_PortName(entity.PortID, dictionaries),
                PortEnName = _dictionaryServices.GetDictionary_PortEnName(entity.PortID, dictionaries),
                PackingMannerZhID = entity.PackingMannerZhID,
                PackingMannerZhName = _dictionaryServices.GetDictionary_PackingMannerZhName(entity.PackingMannerZhID, dictionaries),
                PackingMannerEnID = entity.PackingMannerEnID,
                PackingMannerEnName = entity.PackingMannerEnID == null ? "" : _dictionaryServices.GetDictionary_PackingMannerEnName(entity.PackingMannerZhID, dictionaries),
                UnitID = entity.UnitID,
                UnitName = _dictionaryServices.GetDictionary_UnitName(entity.UnitID, dictionaries),
                CurrencyType = entity.CurrencyType,
                CurrencyName = _dictionaryServices.GetDictionary_CurrencyName(entity.CurrencyType, dictionaries),
                CurrencySign = _dictionaryServices.GetDictionary_CurrencySign(entity.CurrencyType, dictionaries),

                HTS = entity.HTS,
                HSCode = entity.HSCode,

                #endregion 公共的

                DT_MODIFYDATE = entity.DT_MODIFYDATE,
                FOBChinaPort = entity.FOBChinaPort ?? 0,
                ProductID = entity.Quot_QuotProduct.ProductID,
                OuterWeightGross = entity.OuterWeightGross,
                OuterWeightNet = entity.OuterWeightNet,
                OuterWeightGrossLBS = entity.OuterWeightGrossLBS,
                OuterWeightNetLBS = entity.OuterWeightNetLBS,
                FOBUS = entity.FOBUS,
                DDP = entity.DDP,
                POE = entity.POE,
                MU = entity.MU,
                AgentAmount = entity.AgentAmount,
                ELC = entity.ELC,
                Retail = entity.Retail,
                Freight = entity.Freight,
                CheckSuggest = entity.CheckSuggest,
                PortEnID = entity.PortEnID,
                TermsID = entity.TermsID,
                IsDelete = entity.IsDelete,

                Status = entity.Status,
                DT_CREATEDATEFormat = Utils.DateTimeToStr2(entity.DT_CREATEDATE),

                QuoteTemplateFileName = entity.QuoteTemplateFileName,
                ELCFill = entity.ELCFill,
                FactoryID_ForQuote = entity.FactoryID_ForQuote,
                INR = entity.INR,
                TypeOfWood = entity.TypeOfWood,
            };
        }

        private static DAL.Quot_QuotProduct SetDBEntityFromViewModel(VMQuoteProduct entity, int userID, DAL.Quot_QuotProduct productFromDB = null)
        {
            if (productFromDB == null)
            {
                productFromDB = new DAL.Quot_QuotProduct();
                productFromDB.ID = entity.ID;
                productFromDB.QuotID = entity.QuotID;
                productFromDB.CustomerID = entity.CustomerID;
                productFromDB.FactoryID = entity.FactoryID;

                productFromDB.No = entity.No;
                productFromDB.NoFactory = entity.NoFactory;
                productFromDB.Name = entity.Name;
                productFromDB.Desc = entity.Desc;
                productFromDB.Image = entity.Image;
                productFromDB.Length = entity.Length;
                productFromDB.Height = entity.Height;
                productFromDB.Width = entity.Width;
                productFromDB.Weight = entity.Weight;
                productFromDB.IngredientZh = entity.IngredientZh;
                productFromDB.IngredientEn = entity.IngredientEn;
                productFromDB.MOQZh = entity.MOQZh;
                productFromDB.MOQEn = entity.MOQEn;
                productFromDB.PDQPackRate = entity.PDQPackRate;
                productFromDB.PDQLength = entity.PDQLength;
                productFromDB.PDQWidth = entity.PDQWidth;
                productFromDB.PDQHeight = entity.PDQHeight;
                productFromDB.InnerBoxRate = entity.InnerBoxRate;
                productFromDB.InnerLength = entity.InnerLength;
                productFromDB.InnerWidth = entity.InnerWidth;
                productFromDB.InnerHeight = entity.InnerHeight;
                productFromDB.InnerWeight = entity.InnerWeight;
                productFromDB.InnerWeightGross = entity.InnerWeightGross;
                productFromDB.OuterBoxRate = entity.OuterBoxRate;
                productFromDB.OuterLength = entity.OuterLength;
                productFromDB.OuterWidth = entity.OuterWidth;
                productFromDB.OuterHeight = entity.OuterHeight;
                //productFromDB.PriceFactory = entity.PriceFactory;
                productFromDB.MiscImportLoad = entity.MiscImportLoad;
                productFromDB.SRP = entity.SRP;
                productFromDB.CtnsPallet = entity.CtnsPallet;
                productFromDB.DutyPercent = entity.DutyPercent;
                productFromDB.Remarks = entity.Remarks;
                productFromDB.Comment = entity.Comment;
                productFromDB.UPC = entity.UPC;

                productFromDB.LengthIN = entity.LengthIN;
                productFromDB.HeightIN = entity.HeightIN;
                productFromDB.WidthIN = entity.WidthIN;
                productFromDB.WeightLBS = entity.WeightLBS;
                productFromDB.PDQLengthIN = entity.PDQLengthIN;
                productFromDB.PDQWidthIN = entity.PDQWidthIN;
                productFromDB.PDQHeightIN = entity.PDQHeightIN;
                productFromDB.InnerLengthIN = entity.InnerLengthIN;
                productFromDB.InnerWidthIN = entity.InnerWidthIN;
                productFromDB.InnerHeightIN = entity.InnerHeightIN;
                productFromDB.OuterVolume = entity.OuterVolume;
                productFromDB.InnerVolume = entity.InnerVolume;
                productFromDB.InnerWeightLBS = entity.InnerWeightLBS;
                productFromDB.InnerWeightGrossLBS = entity.InnerWeightGrossLBS;
                productFromDB.OuterLengthIN = entity.OuterLengthIN;
                productFromDB.OuterWidthIN = entity.OuterWidthIN;
                productFromDB.OuterHeightIN = entity.OuterHeightIN;
                productFromDB.FOBFTY = entity.FOBFTY;
                productFromDB.FOBNET = entity.FOBNET;
                productFromDB.PcsPallet = entity.PcsPallet;
                productFromDB.PalletPc = entity.PalletPc;
                productFromDB.FOBChinaLCL = entity.FOBChinaLCL;
                productFromDB.Cost = entity.Cost;
                productFromDB.Commission = entity.Commission;

                productFromDB.Allowance = entity.Allowance;
                productFromDB.Agent = entity.Agent;

                productFromDB.StyleID = entity.StyleID;
                productFromDB.PackingMannerZhID = entity.PackingMannerZhID;
                productFromDB.PackingMannerEnID = entity.PackingMannerEnID;
                productFromDB.UnitID = entity.UnitID;
                productFromDB.CurrencyType = entity.CurrencyType;

                productFromDB.HTS = entity.HTS;
                productFromDB.HSCode = entity.HSCode;

                productFromDB.OuterWeightGross = entity.OuterWeightGross;
                productFromDB.OuterWeightNet = entity.OuterWeightNet;
                productFromDB.OuterWeightGrossLBS = entity.OuterWeightGrossLBS;
                productFromDB.OuterWeightNetLBS = entity.OuterWeightNetLBS;

                productFromDB.IsDelete = entity.IsDelete;

                productFromDB.FOBUS = entity.FOBUS;
                productFromDB.DDP = entity.DDP;
                productFromDB.POE = entity.POE;
                productFromDB.ProductID = entity.ProductID;
                productFromDB.CheckSuggest = entity.CheckSuggest;
                productFromDB.PortEnID = entity.PortEnID;
                productFromDB.FOBChinaPort = entity.FOBChinaPort;
                productFromDB.ValidDate = entity.ValidDate;
                productFromDB.PriceInputDate = entity.PriceInputDate;

                productFromDB.ST_CREATEUSER = userID;
                productFromDB.DT_CREATEDATE = DateTime.Now;
                productFromDB.ST_MODIFYUSER = userID;
                productFromDB.DT_MODIFYDATE = DateTime.Now;
                productFromDB.IPAddress = CommonCode.GetIP();

                productFromDB.QuoteTemplateFileName = entity.QuoteTemplateFileName;
                productFromDB.ELCFill = entity.ELCFill;
                productFromDB.Season = entity.Season;
                productFromDB.ColorID = entity.ColorID;
                productFromDB.ProductCopyRight = entity.ProductCopyRight;
                productFromDB.IsProductFitting = entity.IsProductFitting;
                productFromDB.IsProductMixed = entity.IsProductMixed;
                productFromDB.ParentProductMixedID = entity.ParentProductMixedID;

                productFromDB.ExchangeRate = entity.ExchangeRate;
                productFromDB.CurrencyExchangeUSD = entity.CurrencyExchangeUSD;
                productFromDB.CurrencyExchangeRMB = entity.CurrencyExchangeRMB;


            }

            productFromDB.OuterVolume = entity.OuterVolume;
            productFromDB.FreightRate = entity.FreightRate;
            productFromDB.Freight = entity.Freight;
            productFromDB.Rate = entity.Rate;
            productFromDB.FinalFOB = entity.FinalFOB;

            productFromDB.Duty = entity.Duty;
            productFromDB.CommissionAmount = entity.CommissionAmount;
            productFromDB.MiscImportLoadAmount = entity.MiscImportLoadAmount;
            productFromDB.AgentAmount = entity.AgentAmount;
            productFromDB.ELC = entity.ELC;
            productFromDB.Retail = entity.Retail;
            productFromDB.MU = entity.MU;
            productFromDB.Cost = entity.Cost;
            productFromDB.TermsID = entity.TermsID;
            productFromDB.PortID = entity.PortID;
            productFromDB.FactoryID_ForQuote = entity.FactoryID_ForQuote;

            productFromDB.INR = entity.INR;
            productFromDB.TypeOfWood = entity.TypeOfWood;

            return productFromDB;
        }

        private static DAL.Quot_QuotProduct SetDBEntityFromViewModel(DAL.Product entity, int userID, DAL.Quot_QuotProduct productFromDB = null)
        {
            if (productFromDB == null)
            {
                productFromDB = new DAL.Quot_QuotProduct();
                productFromDB.ID = entity.ID;
                //productFromDB.QuotID = entity.QuotID;
                productFromDB.CustomerID = entity.CustomerID;
                productFromDB.FactoryID = entity.FactoryID;

                productFromDB.No = entity.No;
                productFromDB.NoFactory = entity.NoFactory;
                productFromDB.Name = entity.Name;
                productFromDB.Desc = entity.Desc;
                productFromDB.Image = entity.Image;
                productFromDB.Length = entity.Length;
                productFromDB.Height = entity.Height;
                productFromDB.Width = entity.Width;
                productFromDB.Weight = entity.Weight;
                productFromDB.IngredientZh = entity.IngredientZh;
                productFromDB.IngredientEn = entity.IngredientEn;
                productFromDB.MOQZh = entity.MOQZh;
                productFromDB.MOQEn = entity.MOQEn;
                productFromDB.PDQPackRate = entity.PDQPackRate;
                productFromDB.PDQLength = entity.PDQLength;
                productFromDB.PDQWidth = entity.PDQWidth;
                productFromDB.PDQHeight = entity.PDQHeight;
                productFromDB.InnerBoxRate = entity.InnerBoxRate;
                productFromDB.InnerLength = entity.InnerLength;
                productFromDB.InnerWidth = entity.InnerWidth;
                productFromDB.InnerHeight = entity.InnerHeight;
                productFromDB.InnerWeight = entity.InnerWeight;
                productFromDB.InnerWeightGross = entity.InnerWeightGross;
                productFromDB.OuterBoxRate = entity.OuterBoxRate;
                productFromDB.OuterLength = entity.OuterLength;
                productFromDB.OuterWidth = entity.OuterWidth;
                productFromDB.OuterHeight = entity.OuterHeight;
                productFromDB.PriceFactory = entity.PriceFactory;
                productFromDB.MiscImportLoad = entity.MiscImportLoad;
                productFromDB.SRP = entity.SRP;
                productFromDB.CtnsPallet = entity.CtnsPallet;
                productFromDB.DutyPercent = entity.DutyPercent;
                productFromDB.Remarks = entity.Remarks;
                productFromDB.Comment = entity.Comment;
                productFromDB.UPC = entity.UPC;

                productFromDB.LengthIN = entity.LengthIN;
                productFromDB.HeightIN = entity.HeightIN;
                productFromDB.WidthIN = entity.WidthIN;
                productFromDB.WeightLBS = entity.WeightLBS;
                productFromDB.PDQLengthIN = entity.PDQLengthIN;
                productFromDB.PDQWidthIN = entity.PDQWidthIN;
                productFromDB.PDQHeightIN = entity.PDQHeightIN;
                productFromDB.InnerLengthIN = entity.InnerLengthIN;
                productFromDB.InnerWidthIN = entity.InnerWidthIN;
                productFromDB.InnerHeightIN = entity.InnerHeightIN;
                productFromDB.OuterVolume = entity.OuterVolume;
                productFromDB.InnerVolume = entity.InnerVolume;
                productFromDB.InnerWeightLBS = entity.InnerWeightLBS;
                productFromDB.InnerWeightGrossLBS = entity.InnerWeightGrossLBS;
                productFromDB.OuterLengthIN = entity.OuterLengthIN;
                productFromDB.OuterWidthIN = entity.OuterWidthIN;
                productFromDB.OuterHeightIN = entity.OuterHeightIN;
                productFromDB.FOBFTY = entity.FOBFTY;
                productFromDB.FOBNET = entity.FOBNET;
                productFromDB.PcsPallet = entity.PcsPallet;
                productFromDB.PalletPc = entity.PalletPc;
                productFromDB.FOBChinaLCL = entity.FOBChinaLCL;
                productFromDB.Cost = entity.Cost;
                productFromDB.Commission = entity.Commission;

                productFromDB.Allowance = entity.Allowance;
                productFromDB.Agent = entity.Agent;

                productFromDB.StyleID = entity.StyleID;
                productFromDB.PackingMannerZhID = entity.PackingMannerZhID;
                productFromDB.PackingMannerEnID = entity.PackingMannerEnID;
                productFromDB.UnitID = entity.UnitID;
                productFromDB.CurrencyType = entity.CurrencyType;

                productFromDB.HTS = entity.HTS;
                productFromDB.HSCode = entity.HSCode;

                productFromDB.OuterWeightGross = entity.OuterWeightGross;
                productFromDB.OuterWeightNet = entity.OuterWeightNet;
                productFromDB.OuterWeightGrossLBS = entity.OuterWeightGrossLBS;
                productFromDB.OuterWeightNetLBS = entity.OuterWeightNetLBS;

                productFromDB.IsDelete = entity.Deleted;

                //productFromDB.FOBUS = entity.FOBUS;
                //productFromDB.DDP = entity.DDP;
                //productFromDB.POE = entity.POE;
                productFromDB.ProductID = entity.ID;
                //productFromDB.CheckSuggest = entity.CheckSuggest;
                //productFromDB.PortEnID = entity.PortEnID;
                productFromDB.FOBChinaPort = entity.FOBChinaPort;
                productFromDB.ValidDate = entity.ValidDate;
                productFromDB.PriceInputDate = entity.PriceInputDate;

                productFromDB.ST_CREATEUSER = userID;
                productFromDB.DT_CREATEDATE = DateTime.Now;
                productFromDB.ST_MODIFYUSER = userID;
                productFromDB.DT_MODIFYDATE = DateTime.Now;
                productFromDB.IPAddress = CommonCode.GetIP();

                //productFromDB.QuoteTemplateFileName = entity.QuoteTemplateFileName;
                //productFromDB.ELCFill = entity.ELCFill;
                productFromDB.Season = entity.Season;
                productFromDB.ColorID = entity.ColorID;
                productFromDB.ProductCopyRight = entity.ProductCopyRight;
                productFromDB.IsProductFitting = entity.IsProductFitting;
                productFromDB.IsProductMixed = entity.IsProductMixed;
                productFromDB.ParentProductMixedID = entity.ParentProductMixedID;
                productFromDB.Qty = entity.Qty;
            }

            productFromDB.OuterVolume = entity.OuterVolume;
            productFromDB.FreightRate = entity.FreightRate;
            //productFromDB.Freight = entity.Freight;
            productFromDB.Rate = entity.Rate;
            productFromDB.FinalFOB = entity.FinalFOB;

            productFromDB.Duty = entity.Duty;
            productFromDB.CommissionAmount = entity.CommissionAmount;
            productFromDB.MiscImportLoadAmount = entity.MiscImportLoadAmount;
            //productFromDB.AgentAmount = entity.AgentAmount;
            //productFromDB.ELC = entity.ELC;
            //productFromDB.Retail = entity.Retail;
            //productFromDB.MU = entity.MU;
            productFromDB.Cost = entity.Cost;
            //productFromDB.TermsID = entity.TermsID;
            productFromDB.PortID = entity.PortID;
            //productFromDB.FactoryID_ForQuote = entity.FactoryID_ForQuote;

            //productFromDB.INR = entity.INR;
            //productFromDB.TypeOfWood = entity.TypeOfWood;

            return productFromDB;
        }

        /// <summary>
        /// 选择报价单。销售订单页面用到了
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public List<VMQuoteEdit> SelectQuote(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows, VMSelectQuote vm)
        {
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                List<VMQuoteEdit> list_vm = new List<VMQuoteEdit>();
                var query = context.Quot_Quot.Where(d => !d.IsDelete);

                switch (vm.SelectQuoteOccasion)
                {
                    case SelectQuoteOccasionEnum.Default:
                        query = query.Where(d => d.StatusID == (int)QuoteStatusEnum.HadConfirm);
                        break;

                    case SelectQuoteOccasionEnum.Sample:
                        query = query.Where(d => d.StatusID != (int)QuoteStatusEnum.OutLine && d.StatusID != (int)QuoteStatusEnum.HadInvalid);
                        break;

                    default:
                        break;
                }

                #region 筛选

                if (!string.IsNullOrEmpty(vm.QuotNumber))
                {
                    query = query.Where(d => d.QuotNumber.Contains(vm.QuotNumber));
                }
                if (vm.CustomerID > 0)
                {
                    query = query.Where(d => d.Orders_Customers.OCID == vm.CustomerID);
                }

                #endregion 筛选

                #region 排序

                if (sortColumnsNames != null && sortColumnsNames.Count > 0
                    && sortColumnOrders != null && sortColumnOrders.Count > 0
                    && sortColumnsNames.Count == sortColumnOrders.Count)
                {
                    query = query.OrderBy(sortColumnsNames, sortColumnOrders);
                }
                else
                {
                    query = query.OrderByDescending(p => p.DT_MODIFYDATE);
                }

                #endregion 排序

                totalRows = query.Count();//获取总条数

                var dataFromDB = query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();//分页

                foreach (var item in dataFromDB)
                {
                    list_vm.Add(new VMQuoteEdit()
                    {
                        ID = item.ID,
                        QuotNumber = item.QuotNumber,
                        CustomerCode = item.Orders_Customers.CustomerCode,
                        QuotDate = Utils.DateTimeToStr(item.QuotDate),
                        ValidDateFormat = Utils.DateTimeToStr(item.ValidDate),
                        OrderID = item.OrderID,
                        AuthorName = item.SystemUser.UserName,
                        StatusName = GetQuoteStatusEnum_Description(item.StatusID),
                    });
                }
                return list_vm;
            }
        }

        #endregion PublicMethod

        #region Json获取数据

        /// <summary>
        /// 查询报价单的产品
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="QuotID"></param>
        /// <param name="IsDelete"></param>
        /// <returns></returns>
        public List<VMQuoteProduct> GetQuoteProducts(VMERPUser currentUser, int QuotID, bool IsDelete, int ProductMixed_Type = 0)
        {
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                List<VMQuoteProduct> list_vm = new List<VMQuoteProduct>();
                var query = context.Quot_QuotProduct.Include("Orders_Customers").Include("Factory").Where(d => d.QuotID == QuotID && d.IsDelete == IsDelete);
                if (ProductMixed_Type == 0)
                {
                    //普通的
                    query = query.Where(d => !d.ParentProductMixedID.HasValue);
                }
                else if (ProductMixed_Type == 1)
                {
                    //如果是混装产品，显示混装产品的详情里面的。
                    query = query.Where(d => !d.IsProductMixed || (d.IsProductMixed && d.ParentProductMixedID.HasValue));
                }
                else if (ProductMixed_Type == 2)
                {
                    //只显示混装产品的详情里面的。
                    query = query.Where(d => d.IsProductMixed && d.ParentProductMixedID.HasValue);
                }


                List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                List<DAL.HarmonizedSystem> list_HarmonizedSystem = context.HarmonizedSystems.Where(d => !d.IsDelete).ToList();

                foreach (var item in query)
                {
                    var item_product = GetViewModelFromDBEntity(item, list_Com_DataDictionary);

                    if (item_product.IsProductFitting)
                    {
                        var query_ProductFitting = context.ProductFittings.Where(d => d.ProductID == item_product.ProductID && !d.Deleted && d.ModuleType == (int)ModuleTypeEnum.Product);
                        decimal? PriceFactory = 0;
                        foreach (var item2 in query_ProductFitting)
                        {
                            PriceFactory += item2.Qty * item2.PriceFactory * (1 + item2.FeesRate / 100);
                        }
                        //FTY PRICE=PriceFactory+配件单价*配件数量*（1+配件跟单费用%）
                        item_product.PriceFactory += PriceFactory ?? 0;
                    }

                    if (item_product.FactoryID_ForQuote.HasValue)
                    {
                        var query_factory = context.Factories.Find(item_product.FactoryID_ForQuote.Value);
                        if (query_factory != null)
                        {
                            item_product.Factory_EnglishName = query_factory.EnglishName;
                            item_product.Factory_EnglishAddress = query_factory.EnglishAddress;
                        }
                    }

                    item_product.list_ProductIngredient = context.ProductIngredients.Where(d => d.ModuleType == (int)ModuleTypeEnum.Product && d.ProductID == item_product.ProductID).OrderByDescending(d => d.IngredientPercent).Select(p => new VMProductIngredients
                    {
                        IngredientName = p.IngredientName,
                        IngredientPercent = p.IngredientPercent,
                        ProductID = p.ProductID,
                    }).ToList();

                    item_product.SeasonDepartmentNumber = _dictionaryServices.GetDictionary_SeasonDepartmentNumber(item.Season, list_Com_DataDictionary);

                    item_product.ProductFrom = (short)OrderProductSource.FromQuoteProduct;
                    if (list_HarmonizedSystem.Where(d => d.ID == item_product.HTS).Count() > 0)
                    {
                        item_product.HTS_Name = list_HarmonizedSystem.Where(d => d.ID == item_product.HTS).FirstOrDefault().HSCode;
                    }
                    if (list_HarmonizedSystem.Where(d => d.ID == item_product.HSCode).Count() > 0)
                    {
                        item_product.HSCode_Name = list_HarmonizedSystem.Where(d => d.ID == item_product.HSCode).FirstOrDefault().HSCode;
                    }

                    if (item.Orders_Customers != null)
                    {
                        var contact = item.Orders_Customers.Orders_Contacts.Where(d => !d.IsDelete);
                        if (contact.Count() == 1)
                        {
                            item_product.Buyer = contact.FirstOrDefault().FirstName + " " + contact.FirstOrDefault().LastName;
                        }
                        else if (contact.Where(d => d.IsDefault).Count() == 1)
                        {
                            var defaultContact = contact.Where(d => d.IsDefault).FirstOrDefault();
                            item_product.Buyer = defaultContact.FirstName + " " + defaultContact.LastName;
                        }
                    }
                    list_vm.Add(item_product);
                }
                return list_vm;
            }
        }

        public List<VMQuoteProductHistory> GetQuoteProductHistories(VMERPUser currentUser, int QuotProductID)
        {
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                List<VMQuoteProductHistory> list_vm = new List<VMQuoteProductHistory>();
                var query = context.Quot_QuotProduct.Find(QuotProductID);

                var query_product = context.Products.Find(query.ProductID);
                if (query_product != null)
                {
                    int? RootProductID = query_product.RootProductID;
                    var query_quotproduct = context.Quot_QuotProduct.Where(d => d.QuotID == query.QuotID);

                    var listProduct = context.Products.Where(d => d.RootProductID != null).ToList();
                    List<int> list_QuotProductID = new List<int>();//获取该报价单的产品ID列表
                    foreach (var item in query_quotproduct)
                    {
                        var temp = listProduct.Where(d => d.ID == item.ProductID);
                        if (temp.Count() > 0)
                        {
                            if (temp.First().RootProductID == RootProductID)
                            {
                                list_QuotProductID.Add(item.ID);
                            }
                        }
                    }
                    var match = context.Quot_QuotProductHistory.Include("Orders_Customers").Include("Factory").Where(p => p.No == query.No && list_QuotProductID.Contains(p.QuotProductID));

                    List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                    List<DAL.Factory> list_Factory = context.Factories.Where(d => d.IsDelete == 0).ToList();
                    List<DAL.Orders_Customers> list_Orders_Customers = context.Orders_Customers.Where(d => !d.IsDelete).ToList();

                    foreach (var item in match.OrderBy(d => d.DT_CREATEDATE))
                    {
                        list_vm.Add(GetViewModelFromDBEntity_History(item, list_Com_DataDictionary, list_Factory, list_Orders_Customers));
                    }
                }

                return list_vm;
            }
        }

        public List<VMQuoteHistory> GetQuoteHistories(VMERPUser currentUser, int id)
        {
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                List<DAL.SystemUser> list_SystemUser = context.SystemUsers.ToList();
                var list_vm = new List<VMQuoteHistory>();
                var match = context.Quot_QuotHistory.Where(p => p.QuotID == id).OrderByDescending(p => p.DT_CREATEDATE);
                foreach (var item in match)
                {
                    var newHistory = new VMQuoteHistory
                    {
                        ST_CREATEUSER = list_SystemUser.Find(d => d.UserID == item.ST_CREATEUSER).UserName,
                        DT_CREATEDATE = item.DT_CREATEDATE,
                        Comment = item.Comment,
                        CheckSuggest = item.CheckSuggest,
                    };
                    list_vm.Add(newHistory);
                }
                return list_vm;
            }
        }

        public List<VMViewProductList> GetViewProductList(VMERPUser currentUser, List<int> idList)
        {
            List<VMViewProductList> list_vm = new List<VMViewProductList>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Quot_Quot.Where(d => idList.Contains(d.ID));
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForQuote);
                    foreach (var item in query)
                    {
                        var query_product = item.Quot_QuotProduct.Where(d => !d.IsDelete && !d.ParentProductMixedID.HasValue);
                        foreach (var item_product in query_product)
                        {
                            list_vm.Add(new VMViewProductList()
                            {
                                No = item_product.No,
                                Image = item_product.Image,
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return list_vm;
        }

        public List<DTOOrderCustomers> GetCustomer(VMERPUser currentUser, int id)
        {
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                var list_vm = new List<DTOOrderCustomers>();
                var match = context.Orders_Customers.Where(p => p.OCID == id);
                foreach (var item in match)
                {
                    List<VMFreightRate> list_FreightRate = new List<VMFreightRate>();
                    if (item.Orders_FreightRate.Where(d => !d.IsDelete ?? true).Count() > 0)
                    {
                        foreach (var item_Freight in item.Orders_FreightRate.Where(d => !d.IsDelete ?? true).OrderBy(d => d.Type))
                        {
                            list_FreightRate.Add(new VMFreightRate()
                            {
                                FreightRate = item_Freight.FreightRate,
                                Type = item_Freight.Type ?? 0,
                                PortID = item_Freight.PortID,
                            });
                        }
                    }
                    var vm = new DTOOrderCustomers
                    {
                        OCID = item.OCID,
                        Agent = item.Agent,
                        Allowance = item.Allowance,
                        Commission = item.Commission,
                        CreateDate = item.CreateDate,
                        CustomerCode = item.CustomerCode,
                        QuoteTemplateFileName = item.QuoteTemplateFileName,
                        Palletpc = item.Palletpc,
                        MiscImportLoadAmount = item.MiscImportLoadAmount,
                        ELCFill = item.ELCFill,
                        SelectCustomer = item.SelectCustomer,
                        VMFreight = list_FreightRate,
                    };
                    list_vm.Add(vm);
                }
                return list_vm;
            }
        }

        public List<DTOContacts> GetContacts(VMERPUser currentUser, int id)
        {
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                var list_vm = new List<DTOContacts>();
                var match = context.Orders_Contacts.Where(p => p.OCID == id && !p.IsDelete).OrderByDescending(p => p.CreateDate);
                foreach (var item in match)
                {
                    var newHistory = new DTOContacts
                    {
                        OCID = item.OCID,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        FullName = item.FullName,
                        Duty = item.Duty,
                        MobilePhone = item.MobilePhone,
                        TelPhone = item.TelPhone,
                        Fax = item.Fax,
                        Email = item.Email,
                        IsDefault = item.IsDefault,
                        CreateDate = item.CreateDate,
                    };
                    list_vm.Add(newHistory);
                }
                return list_vm;
            }
        }

        #endregion Json获取数据

        #region 报价单模板

        /// <summary>
        /// 创建报价单模板
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="quotID"></param>
        /// <param name="QuoteTemplateFileName"></param>
        private string CreateQuoteTemplate(VMERPUser currentUser, int quotID, string QuoteTemplateFileName)
        {
            QuotProductTypeEnum QuotProductTypeEnum = (QuotProductTypeEnum)EnumHelper.GetCustomEnum(typeof(QuotProductTypeEnum), QuoteTemplateFileName);
            var listQuoteProducts = GetQuoteProducts(currentUser, quotID, false, 1);
            if (listQuoteProducts.Count > 0)
            {
                List<string> list = MakerExcel_Quote.BuildQuot(listQuoteProducts, QuotProductTypeEnum);
                return string.Join(";", list.ToArray());
            }
            return null;
        }

        /// <summary>
        /// 获取报价单模板生成的文件夹列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IOrderedEnumerable<DirectoryInfo> GetTemplateDirectoryArray(int id)
        {
            if (!string.IsNullOrEmpty(id.ToString()))
            {
                string QuotePath = Utils.GetMapPath("/data/quot/out/" + id);
                if (Directory.Exists(QuotePath))
                {
                    DirectoryInfo directory = new DirectoryInfo(QuotePath);
                    var directoryArray = directory.GetDirectories("*", SearchOption.TopDirectoryOnly);

                    var thisDirectorys = directoryArray.OrderByDescending(d => d.Name);//最新生成的报价单的文件夹
                    return thisDirectorys;
                }
            }
            return null;
        }

        /// <summary>
        /// 下载报价单模板
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="id"></param>
        public bool TemplateDownLoad(int id)
        {
            var thisDirectorys = GetTemplateDirectoryArray(id);
            if (thisDirectorys != null && thisDirectorys.Count() > 0)
            {
                var thisDirectoryFullName = thisDirectorys.FirstOrDefault().FullName + "/PDFAndExcel.zip";
                if (File.Exists(thisDirectoryFullName))
                {
                    CommonCode.DownLoadFile(thisDirectoryFullName, DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".zip");//下载压缩文件
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取最新生成的报价单模板的图片路径
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="id"></param>
        public Dictionary<string, string> GetQuoteTemplatePath(VMERPUser currentUser, int id)
        {
            Dictionary<string, string> di = new Dictionary<string, string>();
            var thisDirectorys = GetTemplateDirectoryArray(id);
            if (thisDirectorys != null && thisDirectorys.Count() > 0)
            {
                var thisDirectoryFullName = thisDirectorys.FirstOrDefault().FullName;
                if (Directory.Exists(thisDirectoryFullName + "/jpg"))
                {
                    FileInfo[] fi = new DirectoryInfo(thisDirectoryFullName + "/jpg").GetFiles();
                    foreach (var item in fi)
                    {
                        di.Add(item.Name, thisDirectorys.FirstOrDefault().Name);
                    }
                }
            }
            return di;
        }

        #endregion 报价单模板

        #region 定时缓存报价单

        /// <summary>
        /// 缓存一个报价单
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public bool CacheOneQuote(int userID, VMQuoteEdit vm)
        {
            bool result = false;
            try
            {
                var dto = new DTOMongo<VMQuoteEdit>() { LastUpdateDate = DateTime.Now, UserID = userID, Type = MongoCachedTypes.QuoteEditing, Data = vm };
                var dtoJSON = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
                var document = BsonDocument.Parse(dtoJSON);

                var builder = Builders<BsonDocument>.Filter;
                var filter = builder.Eq("UserID", userID);
                filter &= builder.Eq("Type", MongoCachedTypes.QuoteEditing);
                filter &= builder.Eq("Data.ID", vm.ID);

                new MongoDBHelper().UpsetOne(document, MongoDBHelper.CollectionName_UserCache, filter);
                result = true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return result;
        }

        /// <summary>
        /// 获取缓存的报价单
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public VMQuoteEdit GetOneCachedQuote(int userID, int id)
        {
            VMQuoteEdit vm = null;
            try
            {
                var builder = Builders<BsonDocument>.Filter;
                var filter = builder.Eq("UserID", userID);
                filter &= builder.Eq("Type", MongoCachedTypes.QuoteEditing);
                filter &= builder.Eq("Data.ID", id);

                BsonDocument document = new MongoDBHelper().GetOneRecord(filter, MongoDBHelper.CollectionName_UserCache);
                if (document != null)
                {
                    DTOMongo<VMQuoteEdit> cachedData = BsonSerializer.Deserialize<DTOMongo<VMQuoteEdit>>(document);
                    if (cachedData != null)
                    {
                        vm = cachedData.Data;
                    }
                }
            }
            catch (Exception ex)
            {
                //RemoveOneCachedQuote(userID, id);
                LogHelper.WriteError(ex);
            }
            return vm;
        }

        /// <summary>
        /// 移除缓存的报价单
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="productID"></param>
        /// <returns></returns>
        public bool RemoveOneCachedQuote(int userID, int id)
        {
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("UserID", userID);
            filter &= builder.Eq("Type", MongoCachedTypes.QuoteEditing);
            filter &= builder.Eq("Data.ID", id);

            return new MongoDBHelper().RemoveOneRecord(filter, MongoDBHelper.CollectionName_UserCache);
        }

        #endregion 定时缓存报价单

        #region 审批流

        /// <summary>
        /// 首页——获取待审核的报价单数量
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public int GetPendingApproveCountByUser(VMERPUser currentUser)
        {
            int count = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Quot_Quot.Where(p => !p.IsDelete);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForQuote);
                    query = query.Where(d => d.StatusID == (short)QuoteStatusEnum.PendingCheck);

                    var result = (from q in query
                                  select new
                                  {
                                      CreateUserID = q.ST_CREATEUSER,
                                      ApproverIndex = q.ApproverIndex,
                                      //StatusID = q.StatusID,
                                      //QuotNumber = q.QuotNumber
                                  }).ToList();

                    // GetApproverIndexAndUsers 已被删除，需要通过部门来区别是否属于自己的审核范围内
                    count = result.Count;
                    //ApprovalServices approvalServices = new ApprovalServices();
                    //Dictionary<int, List<int>> dictIndexesAndUsers = approvalServices.GetApproverIndexAndUsers(WorkflowTypes.ApprovalQuot, currentUser);
                    //foreach (var q in result)
                    //{
                    //    foreach (var kvp in dictIndexesAndUsers)
                    //    {
                    //        if (q.ApproverIndex.HasValue && q.ApproverIndex == kvp.Key && kvp.Value.Contains(q.CreateUserID))
                    //        {
                    //            count++;
                    //            break;
                    //        }
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return count;
        }

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
            if (StatusID == (int)QuoteStatusEnum.NotPassCheck)
            {
                isPass = false;
            }

            List<int> validWaitingApproveStatus = new List<int>(){
                            (int)QuoteStatusEnum.PendingCheck,
                            (int)QuoteStatusEnum.NotPassCheck
                        };
            ApprovalService.ExcuteApproval(new ApprovalInfo
            {
                WorkflowType = WorkflowTypes.ApprovalQuot,
                IsPass = isPass,
                IdentityID = identityID,
                ValidWaitingApproveStatus = validWaitingApproveStatus,
                StatusApproving = (int)QuoteStatusEnum.PendingCheck,
                StatusNextTo = (int)QuoteStatusEnum.PassedCheck,
                StatusRejected = (int)QuoteStatusEnum.NotPassCheck,
                ApproveOpinion = CheckSuggest,
                ApproveUserID = UserID,
                LogMethod = () =>
                {
                    if (!historyAdded)
                    {
                        //insert quot history
                        var history = new DAL.Quot_QuotHistory
                        {
                            Comment = GetQuoteStatusEnum_Description(StatusID),
                            CheckSuggest = CheckSuggest,
                            DT_CREATEDATE = DateTime.Now,
                            ST_CREATEUSER = UserID,
                            IPAddress = CommonCode.GetIP(),
                        };
                        return history;
                    }
                    return null;
                }
            });
        }

        #endregion 审批流

        public List<VMQuoteProduct> GetProducts_Mixed(VMERPUser currentUser, int ID)
        {
            List<VMQuoteProduct> listModel = new List<VMQuoteProduct>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Quot_QuotProduct.Where(p => p.IsProductMixed && p.ParentProductMixedID == ID && !p.IsDelete);

                    // 先查出字典表数据
                    List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                    List<DAL.HarmonizedSystem> list_HarmonizedSystem = context.HarmonizedSystems.Where(d => !d.IsDelete).ToList();

                    foreach (var item in query)
                    {
                        VMQuoteProduct vm_product = GetViewModelFromDBEntity(item, list_Com_DataDictionary);

                        #region 计算Model

                        var HarmonizedSystem = list_HarmonizedSystem.Where(d => d.ID == vm_product.HTS);
                        if (HarmonizedSystem.Count() > 0)
                        {
                            vm_product.HTS_Name = HarmonizedSystem.FirstOrDefault().HSCode;
                        }

                        HarmonizedSystem = list_HarmonizedSystem.Where(d => d.ID == vm_product.HSCode);
                        if (HarmonizedSystem.Count() > 0)
                        {
                            vm_product.HSCode_Name = HarmonizedSystem.FirstOrDefault().HSCode;
                        }

                        vm_product.MOQEn = _productServices.CalculateMOQ(vm_product.MOQEn, vm_product.Cost);

                        #endregion 计算Model

                        vm_product.DutyPercentFormatter = vm_product.DutyPercent.HasValue ? vm_product.DutyPercent + "%" : "";
                        vm_product.CommissionFormatter = vm_product.Commission + "%";
                        vm_product.MiscImportLoadFormatter = vm_product.MiscImportLoad.HasValue ? vm_product.MiscImportLoad + "%" : "";
                        vm_product.FactoryID_ForQuote = vm_product.FactoryID;

                        vm_product.PriceFactoryFormatter = vm_product.PriceFactory.HasValue ? vm_product.CurrencySign + vm_product.PriceFactory : "";

                        if (vm_product.PriceFactory.HasValue)
                        {
                            vm_product.ProductAmountFormatter = vm_product.CurrencySign + (vm_product.PriceFactory * vm_product.Qty);
                        }

                        vm_product.Qty2 = vm_product.Qty;

                        int HSCode = vm_product.HSCode ?? 0;
                        if (HSCode > 0)
                        {
                            vm_product.IsNeedInspection = ConstsMethod.IsNeedInspection(list_HarmonizedSystem, HSCode);
                            vm_product.IsNeedInspectionName = vm_product.IsNeedInspection ? "是" : "否";
                        }

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
    }
}