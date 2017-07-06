using ERP.BLL.Consts;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.Inspection;
using ERP.Models.Product;
using ERP.Models.Purchase;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.ERP.Inspection
{
    public class InspectionServices
    {
        private ERP.Dictionary.DictionaryServices _dictionaryServices = new ERP.Dictionary.DictionaryServices();

        public static string GetStatusEnum_Description(int id)
        {
            return CommonCode.GetStatusEnumName(id, typeof(InspectionStatusEnum));
        }

        /// <summary>
        /// 查询所有信息
        /// </summary>
        /// <returns></returns>
        public List<DTOInspection> GetAll(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows, VMInspectionSearch vm_search)
        {
            List<DTOInspection> list = new List<DTOInspection>();
            totalRows = 0;

            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                try
                {
                    var query = context.ThirdParty_Inspection.Include("Purchase_Contract").Where(d => d.TypeID == (int)vm_search.TypeID && !d.IsDelete);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForThirdParty);

                    if (currentUser.HierachyType == HierachyType.Agency)
                    {
                        var userHierachy = new List<int?>()
                        {
                            currentUser.HierachyID,
                            context.SystemUsers.FirstOrDefault(p => p.HierarchyID == currentUser.HierachyID).Hierarchy.Hierarchy2.HierarchyID
                        };

                        query = query.Where(p => userHierachy.Contains(p.Purchase_Contract.Factory.Hierarchy));
                    }

                    #region 筛选条件

                    if (!string.IsNullOrEmpty(vm_search.PurchaseNumber))
                    {
                        query = query.Where(d => d.Purchase_Contract.PurchaseNumber.Contains(vm_search.PurchaseNumber));
                    }
                    if (!string.IsNullOrEmpty(vm_search.FactoryName))
                    {
                        query = query.Where(d => d.Purchase_Contract.Factory.Name.Contains(vm_search.FactoryName));
                    }
                    if (!string.IsNullOrEmpty(vm_search.InspectionName))
                    {
                        query = query.Where(d => d.InspectionName.Contains(vm_search.InspectionName));
                    }
                    if (!string.IsNullOrEmpty(vm_search.StatusID))
                    {
                        int i = Utils.StrToInt(vm_search.StatusID, 0);
                        query = query.Where(d => d.StatusID == i);
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

                        foreach (var item in dataFromDB)
                        {
                            int? CurrencyType = item.Purchase_Contract.Purchase_ContractBatch.FirstOrDefault().Purchase_ContractProduct.FirstOrDefault().OrderProduct.CurrencyType;
                            string CurrencySign = Keys.RMB_Sign;

                            list.Add(new DTOInspection()
                            {
                                ID = item.ID,
                                PurchaseNumber = item.Purchase_Contract.PurchaseNumber,
                                InspectionName = item.InspectionName,
                                FactoryName = item.Purchase_Contract.Factory.Name,
                                StartTime = Utils.DateTimeToStr(item.StartTime),
                                EndTime = Utils.DateTimeToStr(item.EndTime),
                                InspectionContent = item.InspectionContent,
                                StatusID = item.StatusID,
                                StatusName = GetStatusEnum_Description(item.StatusID),
                                DT_MODIFYDATE = Utils.DateTimeToStr2(item.DT_MODIFYDATE),
                                InspectionAuditFeeFormatter = CurrencySign + item.InspectionAuditFee,
                                InspectionAuditFee_ForFactoryFormatter = CurrencySign + (item.InspectionAuditFee_ForFactory ?? 0.00m),
                                InspectionDetectFeeFormatter = CurrencySign + item.InspectionDetectFee,
                                InspectionDetectFee_ForFactoryFormatter = CurrencySign + (item.InspectionDetectFee_ForFactory ?? 0.00m),
                                InspectionSamplingFeeFormatter = CurrencySign + item.InspectionSamplingFee,
                                InspectionSamplingFee_ForFactoryFormatter = CurrencySign + (item.InspectionSamplingFee_ForFactory ?? 0.00m),
                            });
                        }
                    }

                    #endregion 给Model赋值
                }
                catch (Exception ex)
                {
                    LogHelper.WriteError(ex);
                }
            }
            return list;
        }

        public VMInspection GetDetailByID(VMERPUser currentUser, int id)
        {
            VMInspection list = new VMInspection();
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                try
                {
                    var query = context.ThirdParty_Inspection.Where(n => n.ID == id).FirstOrDefault();
                    List<DAL.SystemUser> list_SystemUser = context.SystemUsers.ToList();

                    List<VMInspectionAuditNoticeHistory> list_InspectionAuditNoticeHistory = new List<VMInspectionAuditNoticeHistory>();
                    var query_history = context.ThirdParty_InspectionAuditNoticeHistory.Where(d => d.InspectionID == id).OrderBy(d => d.ST_CREATEUSER);
                    if (query_history.Count() > 0)
                    {
                        foreach (var item in query_history)
                        {
                            list_InspectionAuditNoticeHistory.Add(new VMInspectionAuditNoticeHistory()
                            {
                                ID = item.ID,
                                Comment = item.Comment,
                                ST_CREATEUSER = list_SystemUser.Find(d => d.UserID == item.ST_CREATEUSER).UserName,
                                DT_CREATEDATE = Utils.DateTimeToStr2(item.DT_CREATEDATE),
                                StatusName = GetStatusEnum_Description(item.StatusID),
                            });
                        }
                    }

                    List<VMInspectionDetectNoticeHistory> list_InspectionDetectNoticeHistory = new List<VMInspectionDetectNoticeHistory>();
                    var query_history2 = context.ThirdParty_InspectionDetectNoticeHistory.Where(d => d.InspectionID == id).OrderBy(d => d.ST_CREATEUSER);
                    if (query_history2.Count() > 0)
                    {
                        foreach (var item in query_history2)
                        {
                            list_InspectionDetectNoticeHistory.Add(new VMInspectionDetectNoticeHistory()
                            {
                                ID = item.ID,
                                Comment = item.Comment,
                                ST_CREATEUSER = list_SystemUser.Find(d => d.UserID == item.ST_CREATEUSER).UserName,
                                DT_CREATEDATE = Utils.DateTimeToStr2(item.DT_CREATEDATE),
                                StatusName = GetStatusEnum_Description(item.StatusID),
                            });
                        }
                    }

                    List<VMInspectionSamplingNoticeHistory> list_InspectionSamplingNoticeHistory = new List<VMInspectionSamplingNoticeHistory>();
                    var query_history3 = context.ThirdParty_InspectionSamplingNoticeHistory.Where(d => d.InspectionID == id).OrderBy(d => d.ST_CREATEUSER);
                    if (query_history3.Count() > 0)
                    {
                        foreach (var item in query_history3)
                        {
                            list_InspectionSamplingNoticeHistory.Add(new VMInspectionSamplingNoticeHistory()
                            {
                                ID = item.ID,
                                Comment = item.Comment,
                                ST_CREATEUSER = list_SystemUser.Find(d => d.UserID == item.ST_CREATEUSER).UserName,
                                DT_CREATEDATE = Utils.DateTimeToStr2(item.DT_CREATEDATE),
                                StatusName = GetStatusEnum_Description(item.StatusID),
                            });
                        }
                    }
                    List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                    int? CurrencyType = query.Purchase_Contract.Purchase_ContractBatch.FirstOrDefault().Purchase_ContractProduct.FirstOrDefault().OrderProduct.CurrencyType;

                    list = new VMInspection()
                    {
                        ID = query.ID,
                        PurchaseNumber = query.Purchase_Contract.PurchaseNumber,
                        FactoryName = query.Purchase_Contract.Factory.Name,
                        FactoryEmail = query.Purchase_Contract.Factory.EmailAdress,
                        InspectionName = query.InspectionName,
                        InspectionEmail = query.InspectionEmail,
                        InspectionPhoneNumber = query.InspectionPhoneNumber,
                        InspectionFax = query.InspectionFax,
                        StartTime = Utils.DateTimeToStr(query.StartTime),
                        EndTime = Utils.DateTimeToStr(query.EndTime),
                        InspectionAuditFee = query.InspectionAuditFee,
                        InspectionAuditFee_ForFactory = query.InspectionAuditFee_ForFactory,
                        InspectionDetectFee = query.InspectionDetectFee,
                        InspectionDetectFee_ForFactory = query.InspectionDetectFee_ForFactory,
                        InspectionSamplingFee = query.InspectionSamplingFee,
                        InspectionSamplingFee_ForFactory = query.InspectionSamplingFee_ForFactory,
                        StatusID = query.StatusID,
                        StatusName = GetStatusEnum_Description(query.StatusID),
                        CurrencyType = CurrencyType,
                        CurrencySign = Keys.RMB_Sign,
                        InspectionContent = query.InspectionContent,
                        list_InspectionAuditNotice = (from p in context.ThirdParty_InspectionAuditNotice
                                                      where p.InspectionID == id && !p.IsDelete
                                                      select new VMInspectionAuditNotice
                                                      {
                                                          ID = p.ID,
                                                          Name = p.Name,
                                                          Comment = p.Comment,
                                                      }).ToList(),

                        list_InspectionDetectNotice = (from p in context.ThirdParty_InspectionDetectNotice
                                                       where p.InspectionID == id && !p.IsDelete
                                                       select new VMInspectionDetectNotice
                                                       {
                                                           ID = p.ID,
                                                           InspectionID = p.InspectionID,
                                                           Comment = p.Comment,
                                                           //FinishTime = p.FinishTime,
                                                           Qty = p.Qty,
                                                           ProductID = p.ProductID,
                                                           No = (from a in context.Products
                                                                 where a.ID == p.ProductID
                                                                 select a.No).FirstOrDefault(),
                                                           ProductDetectFee = p.ProductDetectFee,
                                                           ProductDetectFee_ForFactory = p.ProductDetectFee_ForFactory,
                                                       }).ToList(),
                        list_InspectionSamplingNotice = (from p in context.ThirdParty_InspectionSamplingNotice
                                                         where p.InspectionID == id && !p.IsDelete
                                                         select new VMInspectionSamplingNotice
                                                         {
                                                             ID = p.ID,
                                                             Name = p.Name,
                                                             Comment = p.Comment,
                                                         }).ToList(),
                        list_UpLoadFile = (from p in context.UpLoadFiles
                                           where p.ModuleType == (int)UploadFileType.InspectionAuditNotice && p.LinkID == id && !p.IsDelete
                                           select new VMUpLoadFile
                                           {
                                               ID = p.ID,
                                               LinkID = p.LinkID,
                                               DT_CREATEDATE = p.DT_CREATEDATE,
                                               DisplayFileName = p.DisplayFileName,
                                               ServerFileName = p.ServerFileName,
                                           }).ToList(),
                        list_InspectionAuditNoticeHistory = list_InspectionAuditNoticeHistory,
                        list_InspectionDetectNoticeHistory = list_InspectionDetectNoticeHistory,
                        list_InspectionSamplingNoticeHistory = list_InspectionSamplingNoticeHistory,
                    };
                }
                catch (Exception ex)
                {
                    LogHelper.WriteError(ex);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取采购合同产品列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<VMPurchaseProduct> GetDetailByIDD(VMERPUser currentUser, int id)
        {
            VMPurchase vm = new VMPurchase();

            List<VMPurchaseProduct> list_vm = new List<VMPurchaseProduct>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var PurchaseID = context.ThirdParty_Inspection.Find(id).PurchaseID;
                    var query = context.Purchase_Contract.Where(d => d.ID == PurchaseID && !d.IsDelete && d.ContractType == (int)ContractTypeEnum.Default);
                    var dataFromDB = query.FirstOrDefault();
                    if (dataFromDB != null)
                    {
                        List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());
                        foreach (var item_batch in dataFromDB.Purchase_ContractBatch)
                        {
                            var query_product = item_batch.Purchase_ContractProduct.Where(d => !d.IsDelete && !d.ParentProductMixedID.HasValue);
                            foreach (var item_product in query_product)
                            {
                                string CurrentSign = _dictionaryServices.GetDictionary_CurrencySign(item_product.OrderProduct.CurrencyType, list_Com_DataDictionary);
                                list_vm.Add(new VMPurchaseProduct()
                                {
                                    ProductID = item_product.OrderProduct.ProductID,
                                    Name = item_product.OrderProduct.Name,
                                    No = item_product.OrderProduct.No,
                                });
                            }

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

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public DBOperationStatus Save(VMERPUser currentUser, VMInspection vm)
        {
            DBOperationStatus result = new DBOperationStatus();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    #region 保存

                    var query = context.ThirdParty_Inspection.Find(vm.ID);
                    query.InspectionName = vm.InspectionName;
                    query.InspectionEmail = vm.InspectionEmail;
                    query.InspectionPhoneNumber = vm.InspectionPhoneNumber;
                    query.InspectionFax = vm.InspectionFax;
                    query.StartTime = Utils.StrToDateTime(vm.StartTime);
                    query.EndTime = Utils.StrToDateTime(vm.EndTime);
                    query.StatusID = vm.StatusID;

                    query.ST_MODIFYUSER = currentUser.UserID;
                    query.DT_MODIFYDATE = DateTime.Now;
                    query.IPAddress = CommonCode.GetIP();

                    #region 第三方验厂

                    if (vm.TypeID == (int)InspectionTypeEnum.AuditNotice)
                    {
                        if (vm.list_InspectionAuditNotice != null)
                        {
                            List<int> listDeleteID = context.ThirdParty_InspectionAuditNotice.Where(d => d.InspectionID == vm.ID && !d.IsDelete).Select(d => d.ID).Except(vm.list_InspectionAuditNotice.Select(d => d.ID)).ToList();
                            foreach (var item in listDeleteID)
                            {
                                context.ThirdParty_InspectionAuditNotice.Find(item).IsDelete = true;
                            }
                            string Comment = "";
                            foreach (var item in vm.list_InspectionAuditNotice)
                            {
                                if (item.ID > 0)
                                {
                                    //编辑
                                    var query_InspectionAuditNotice = query.ThirdParty_InspectionAuditNotice.Where(d => d.ID == item.ID).First();
                                    query_InspectionAuditNotice.Name = item.Name;
                                    query_InspectionAuditNotice.Comment = item.Comment;

                                    query_InspectionAuditNotice.ST_MODIFYUSER = currentUser.UserID;
                                    query_InspectionAuditNotice.DT_MODIFYDATE = DateTime.Now;
                                }
                                else
                                {
                                    //添加
                                    DAL.ThirdParty_InspectionAuditNotice query_InspectionAuditNotice = new ThirdParty_InspectionAuditNotice()
                                    {
                                        ID = item.ID,
                                        Name = item.Name,
                                        Comment = item.Comment,

                                        DT_CREATEDATE = DateTime.Now,
                                        ST_CREATEUSER = currentUser.UserID,
                                        DT_MODIFYDATE = DateTime.Now,
                                        ST_MODIFYUSER = currentUser.UserID,
                                        IsDelete = false,
                                        IPAdress = CommonCode.GetIP(),
                                    };
                                    query.ThirdParty_InspectionAuditNotice.Add(query_InspectionAuditNotice);
                                }
                            }

                            DAL.ThirdParty_InspectionAuditNoticeHistory history = new ThirdParty_InspectionAuditNoticeHistory()
                            {
                                InspectionID = vm.ID,
                                StatusID = vm.StatusID,
                                Comment = Comment,

                                DT_CREATEDATE = DateTime.Now,
                                ST_CREATEUSER = currentUser.UserID,
                                IPAddress = CommonCode.GetIP(),
                                IsDelete = false,
                            };
                            context.ThirdParty_InspectionAuditNoticeHistory.Add(history);
                        }
                    }

                    #endregion 第三方验厂

                    #region 第三方检测

                    if (vm.TypeID == (int)InspectionTypeEnum.DetectNotice)
                    {
                        if (vm.list_InspectionDetectNotice != null)
                        {
                            var listDeleteID = context.ThirdParty_InspectionDetectNotice.Where(d => d.InspectionID == vm.ID && !d.IsDelete).Select(d => d.ID).Except(vm.list_InspectionDetectNotice.Select(d => d.ID));
                            foreach (var item in listDeleteID)
                            {
                                context.ThirdParty_InspectionDetectNotice.Find(item).IsDelete = true;
                            }
                            string Comment = "";
                            foreach (var item in vm.list_InspectionDetectNotice)
                            {
                                if (item.ID > 0)
                                {
                                    //编辑
                                    var query_InspectionDetectNotice = query.ThirdParty_InspectionDetectNotice.Where(d => d.ID == item.ID).First();
                                    query_InspectionDetectNotice.ProductID = item.ProductID;
                                    query_InspectionDetectNotice.Qty = item.Qty;
                                    //query_InspectionDetectNotice.FinishTime = item.FinishTime;
                                    query_InspectionDetectNotice.Comment = item.Comment;

                                    query_InspectionDetectNotice.ST_MODIFYUSER = currentUser.UserID;
                                    query_InspectionDetectNotice.DT_MODIFYDATE = DateTime.Now;
                                }
                                else
                                {
                                    //添加
                                    DAL.ThirdParty_InspectionDetectNotice query_InspectionDetectNotice = new ThirdParty_InspectionDetectNotice()
                                    {
                                        ProductID = item.ProductID,
                                        Qty = item.Qty,

                                        Comment = item.Comment,

                                        DT_CREATEDATE = DateTime.Now,
                                        ST_CREATEUSER = currentUser.UserID,
                                        DT_MODIFYDATE = DateTime.Now,
                                        ST_MODIFYUSER = currentUser.UserID,
                                        IsDelete = false,
                                        IPAdress = CommonCode.GetIP(),
                                    };
                                    query.ThirdParty_InspectionDetectNotice.Add(query_InspectionDetectNotice);
                                }
                            }

                            DAL.ThirdParty_InspectionDetectNoticeHistory history = new ThirdParty_InspectionDetectNoticeHistory()
                            {
                                InspectionID = vm.ID,

                                ProductID = 0,
                                Qty = 0,
                                FinishTime = DateTime.Now,
                                StatusID = vm.StatusID,
                                Comment = Comment,

                                DT_CREATEDATE = DateTime.Now,
                                ST_CREATEUSER = currentUser.UserID,
                                IsDelete = false,
                                IPAddress = CommonCode.GetIP(),
                            };
                            context.ThirdParty_InspectionDetectNoticeHistory.Add(history);
                        }
                    }

                    #endregion 第三方检测

                    #region 第三方抽检

                    if (vm.TypeID == (int)InspectionTypeEnum.SamplingNotice)
                    {
                        if (vm.list_InspectionSamplingNotice != null)
                        {
                            List<int> listDeleteID = context.ThirdParty_InspectionSamplingNotice.Where(d => d.InspectionID == vm.ID && !d.IsDelete).Select(d => d.ID).Except(vm.list_InspectionSamplingNotice.Select(d => d.ID)).ToList();
                            foreach (var item in listDeleteID)
                            {
                                context.ThirdParty_InspectionSamplingNotice.Find(item).IsDelete = true;
                            }
                            string Comment = "";
                            foreach (var item in vm.list_InspectionSamplingNotice)
                            {
                                if (item.ID > 0)
                                {
                                    //编辑
                                    var query_InspectionSamplingNotice = query.ThirdParty_InspectionSamplingNotice.Where(d => d.ID == item.ID).First();
                                    query_InspectionSamplingNotice.Name = item.Name;
                                    query_InspectionSamplingNotice.Comment = item.Comment;

                                    query_InspectionSamplingNotice.ST_MODIFYUSER = currentUser.UserID;
                                    query_InspectionSamplingNotice.DT_MODIFYDATE = DateTime.Now;
                                }
                                else
                                {
                                    //添加
                                    DAL.ThirdParty_InspectionSamplingNotice query_InspectionSamplingNotice = new ThirdParty_InspectionSamplingNotice()
                                    {
                                        ID = item.ID,
                                        Name = item.Name,
                                        Comment = item.Comment,

                                        DT_CREATEDATE = DateTime.Now,
                                        ST_CREATEUSER = currentUser.UserID,
                                        DT_MODIFYDATE = DateTime.Now,
                                        ST_MODIFYUSER = currentUser.UserID,
                                        IsDelete = false,
                                        IPAdress = CommonCode.GetIP(),
                                    };
                                    query.ThirdParty_InspectionSamplingNotice.Add(query_InspectionSamplingNotice);
                                }
                            }

                            DAL.ThirdParty_InspectionSamplingNoticeHistory history = new ThirdParty_InspectionSamplingNoticeHistory()
                            {
                                InspectionID = vm.ID,
                                StatusID = vm.StatusID,
                                Comment = Comment,

                                DT_CREATEDATE = DateTime.Now,
                                ST_CREATEUSER = currentUser.UserID,
                                IPAddress = CommonCode.GetIP(),
                                IsDelete = false,
                            };
                            context.ThirdParty_InspectionSamplingNoticeHistory.Add(history);
                        }
                    }

                    #endregion 第三方抽检

                    #endregion 保存

                    int affectRows = context.SaveChanges();

                    if (vm.StatusID == (short)InspectionStatusEnum.AlreadySent && vm.SendEmail != null)
                    {
                        // 将附件的URL替换成本地文件
                        vm.SendEmail.Attachs = ConstsMethod.ReplaceURLToLocalPath(vm.SendEmail.Attachs);

                        Email.SendEmail(currentUser.UserName, vm.SendEmail);//发送邮件
                    }

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
                result = DBOperationStatus.Failed;
            }
            return result;
        }

        /// <summary>
        /// 保存第三方验厂结果、第三方检测结果、第三方抽检结果
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public DBOperationStatus Save_InputResult(VMERPUser currentUser, VMInspection vm)
        {
            DBOperationStatus result = new DBOperationStatus();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.ThirdParty_Inspection.Where(p => p.ID == vm.ID);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForPurchase);
                    var dataFromDB = query.FirstOrDefault();
                    if (dataFromDB != null)
                    {
                        dataFromDB.StatusID = vm.StatusID;
                        dataFromDB.ST_MODIFYUSER = currentUser.UserID;
                        dataFromDB.DT_MODIFYDATE = DateTime.Now;

                        if (vm.TypeID == (int)InspectionTypeEnum.AuditNotice)
                        {
                            dataFromDB.InspectionContent = vm.InspectionContent;

                            DAL.ThirdParty_InspectionAuditNoticeHistory history = new ThirdParty_InspectionAuditNoticeHistory()
                            {
                                Comment = dataFromDB.InspectionContent,
                                InspectionID = vm.ID,
                                StatusID = vm.StatusID,

                                DT_CREATEDATE = DateTime.Now,
                                ST_CREATEUSER = currentUser.UserID,
                                IPAddress = CommonCode.GetIP(),
                                IsDelete = false,
                            };
                            context.ThirdParty_InspectionAuditNoticeHistory.Add(history);
                        }

                        if (vm.TypeID == (int)InspectionTypeEnum.DetectNotice)
                        {
                            DAL.ThirdParty_InspectionDetectNoticeHistory history = new ThirdParty_InspectionDetectNoticeHistory()
                            {
                                InspectionID = vm.ID,

                                ProductID = 0,
                                Qty = 0,
                                FinishTime = DateTime.Now,
                                Comment = "",
                                StatusID = vm.StatusID,

                                DT_CREATEDATE = DateTime.Now,
                                ST_CREATEUSER = currentUser.UserID,
                                IsDelete = false,
                                IPAddress = CommonCode.GetIP(),
                            };
                            context.ThirdParty_InspectionDetectNoticeHistory.Add(history);
                        }

                        if (vm.TypeID == (int)InspectionTypeEnum.SamplingNotice)
                        {
                            DAL.ThirdParty_InspectionSamplingNoticeHistory history = new ThirdParty_InspectionSamplingNoticeHistory()
                            {
                                Comment = dataFromDB.InspectionContent,
                                InspectionID = vm.ID,
                                StatusID = vm.StatusID,

                                DT_CREATEDATE = DateTime.Now,
                                ST_CREATEUSER = currentUser.UserID,
                                IPAddress = CommonCode.GetIP(),
                                IsDelete = false,
                            };
                            context.ThirdParty_InspectionSamplingNoticeHistory.Add(history);
                        }

                        if (vm.list_UpLoadFile != null)
                        {
                            int newID = 0;
                            List<UpLoadFile> list_UpLoadFile = context.UpLoadFiles.ToList();
                            foreach (var item in vm.list_UpLoadFile)
                            {
                                if (item.ID != 0)
                                {
                                    //更新
                                    var query_UpLoadFiles = list_UpLoadFile.Find(d => d.ID == item.ID);
                                    query_UpLoadFiles.IsDelete = item.IsDelete;
                                    query_UpLoadFiles.DT_MODIFYDATE = DateTime.Now;
                                    query_UpLoadFiles.ST_MODIFYUSER = currentUser.UserID;
                                }
                                else
                                {
                                    if (!item.IsDelete)
                                    {
                                        //添加
                                        context.UpLoadFiles.Add(new UpLoadFile()
                                        {
                                            ID = --newID,
                                            DisplayFileName = item.DisplayFileName,
                                            ServerFileName = item.ServerFileName,
                                            DT_CREATEDATE = item.DT_CREATEDATE,
                                            DT_MODIFYDATE = DateTime.Now,
                                            ST_CREATEUSER = currentUser.UserID,
                                            ST_MODIFYUSER = currentUser.UserID,
                                            IPAddress = CommonCode.GetIP(),
                                            IsDelete = false,
                                            LinkID = vm.ID,
                                            ModuleType = (int)UploadFileType.InspectionAuditNotice,
                                        });
                                    }
                                }
                            }
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

        /// <summary>
        /// 保存第三方验厂费用、第三方检测费用、第三方抽检费用
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public DBOperationStatus Save_InputFees(VMERPUser currentUser, VMInspection vm)
        {
            DBOperationStatus result = new DBOperationStatus();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.ThirdParty_Inspection.Where(p => p.ID == vm.ID);
                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForPurchase);
                    var dataFromDB = query.FirstOrDefault();
                    if (dataFromDB != null)
                    {
                        dataFromDB.ST_MODIFYUSER = currentUser.UserID;
                        dataFromDB.DT_MODIFYDATE = DateTime.Now;

                        #region 第三方验厂

                        if (vm.TypeID == (int)InspectionTypeEnum.AuditNotice)
                        {
                            dataFromDB.InspectionAuditFee = vm.InspectionAuditFee;
                            dataFromDB.InspectionAuditFee_ForFactory = vm.InspectionAuditFee_ForFactory;

                            DAL.ThirdParty_InspectionAuditNoticeHistory history = new ThirdParty_InspectionAuditNoticeHistory()
                            {
                                Comment = "修改验厂费用",
                                InspectionID = vm.ID,
                                StatusID = dataFromDB.StatusID,

                                DT_CREATEDATE = DateTime.Now,
                                ST_CREATEUSER = currentUser.UserID,
                                IPAddress = CommonCode.GetIP(),
                                IsDelete = false,
                            };
                            context.ThirdParty_InspectionAuditNoticeHistory.Add(history);
                        }

                        #endregion 第三方验厂

                        #region 第三方检测

                        if (vm.TypeID == (int)InspectionTypeEnum.DetectNotice)
                        {
                            dataFromDB.InspectionDetectFee = vm.InspectionDetectFee;
                            dataFromDB.InspectionDetectFee_ForFactory = vm.InspectionDetectFee_ForFactory;

                            if (vm.list_InspectionDetectNotice != null)
                            {
                                foreach (var item in vm.list_InspectionDetectNotice)
                                {
                                    if (item.ID > 0)
                                    {
                                        //编辑
                                        var query_InspectionAuditNotice = dataFromDB.ThirdParty_InspectionDetectNotice.Where(d => d.ID == item.ID).FirstOrDefault();
                                        query_InspectionAuditNotice.ProductDetectFee = item.ProductDetectFee;
                                        query_InspectionAuditNotice.ProductDetectFee_ForFactory = item.ProductDetectFee_ForFactory;

                                        query_InspectionAuditNotice.ST_MODIFYUSER = currentUser.UserID;
                                        query_InspectionAuditNotice.DT_MODIFYDATE = DateTime.Now;
                                    }
                                }
                            }

                            DAL.ThirdParty_InspectionDetectNoticeHistory history = new ThirdParty_InspectionDetectNoticeHistory()
                            {
                                InspectionID = vm.ID,

                                ProductID = 0,
                                Qty = 0,
                                FinishTime = DateTime.Now,
                                Comment = "修改检测费用",
                                StatusID = dataFromDB.StatusID,

                                DT_CREATEDATE = DateTime.Now,
                                ST_CREATEUSER = currentUser.UserID,
                                IsDelete = false,
                                IPAddress = CommonCode.GetIP(),
                            };
                            context.ThirdParty_InspectionDetectNoticeHistory.Add(history);
                        }

                        #endregion 第三方检测

                        #region 第三方抽检

                        if (vm.TypeID == (int)InspectionTypeEnum.SamplingNotice)
                        {
                            dataFromDB.InspectionSamplingFee = vm.InspectionSamplingFee;
                            dataFromDB.InspectionSamplingFee_ForFactory = vm.InspectionSamplingFee_ForFactory;

                            DAL.ThirdParty_InspectionSamplingNoticeHistory history = new ThirdParty_InspectionSamplingNoticeHistory()
                            {
                                Comment = "修改抽检费用",
                                InspectionID = vm.ID,
                                StatusID = dataFromDB.StatusID,

                                DT_CREATEDATE = DateTime.Now,
                                ST_CREATEUSER = currentUser.UserID,
                                IPAddress = CommonCode.GetIP(),
                                IsDelete = false,
                            };
                            context.ThirdParty_InspectionSamplingNoticeHistory.Add(history);
                        }

                        #endregion 第三方验厂
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
    }
}