using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.Sample;
using ERP.Tools;
using Microsoft.JScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.BLL.ERP.Sample
{
    /// <summary>
    /// 寄样管理业务逻辑处理类
    /// </summary>
    public class SampleService
    {
        /// <summary>
        /// 用筛选条件查询待待/已打样列表数据
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="sortColumnsNames"></param>
        /// <param name="sortColumnOrders"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRows"></param>
        /// <param name="obj">筛选条件对象</param>
        /// <returns></returns>
        public List<DTOSample> GetManufactureDataList(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows, VMFilterSample obj)
        {
            List<DTOSample> objList = new List<DTOSample>();
            totalRows = 0;

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Sale_SendSamples.Where(p => p.IsDelete == false);

                    #region 筛选条件

                    query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForSample);

                    if (currentUser.HierachyType == HierachyType.Agency)
                    {
                        var userHierachy = new List<int?>()
                        {
                            currentUser.HierachyID,
                            context.SystemUsers.FirstOrDefault(p => p.HierarchyID == currentUser.HierachyID).Hierarchy.Hierarchy2.HierarchyID
                        };

                        query = query.Where(p => userHierachy.Contains(p.Factory.Hierarchy));
                    }

                    query = query.Where(p => p.SampleStatus >= obj.StatusIntervalStart);
                    query = query.Where(p => p.SampleStatus <= obj.StatusIntervalEnd);

                    if (obj.SampleStatus > 0)
                    {
                        query = query.Where(p => p.SampleStatus == obj.SampleStatus);
                    }

                    if (!string.IsNullOrEmpty(obj.CustomerCode))
                    {
                        query = query.Where(p => p.Orders_Customers.CustomerCode == obj.CustomerCode);
                    }

                    if (!string.IsNullOrEmpty(obj.FactoryAbbreviation))
                    {
                        query = query.Where(p => p.Factory.Abbreviation.Contains(obj.FactoryAbbreviation) && p.Factory.DataFlag == 1);
                    }

                    if (!string.IsNullOrEmpty(obj.FacManufactureID))
                    {
                        query = query.Where(p => p.FacManufactureID == obj.FacManufactureID);
                    }

                    //样品所在办事处

                    if (!string.IsNullOrEmpty(obj.QuotDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(obj.QuotDateStart);
                        query = query.Where(p => p.Quot_Quot.QuotDate >= dt);
                    }
                    if (!string.IsNullOrEmpty(obj.QuotDateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(obj.QuotDateEnd);
                        query = query.Where(p => p.Quot_Quot.QuotDate <= dt);
                    }
                    if (!string.IsNullOrEmpty(obj.OrderDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(obj.OrderDateStart);
                        query = query.Where(p => p.Order.CustomerDate >= dt);
                    }
                    if (!string.IsNullOrEmpty(obj.OrderDateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(obj.OrderDateEnd);
                        query = query.Where(p => p.Order.CustomerDate <= dt);
                    }
                    if (!string.IsNullOrEmpty(obj.IssueDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(obj.IssueDateStart);
                        query = query.Where(p => p.CreateDate >= dt);
                    }
                    if (!string.IsNullOrEmpty(obj.IssueDateEnd))
                    {
                        DateTime dt = CommonCode.GetDateEnd(obj.IssueDateEnd);
                        query = query.Where(p => p.CreateDate <= dt);
                    }

                    if (!string.IsNullOrEmpty(obj.PFDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(obj.PFDateStart);
                        query = query.Where(p => p.DateTimeA >= dt);
                    }
                    if (!string.IsNullOrEmpty(obj.PFDateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(obj.PFDateEnd);
                        query = query.Where(p => p.DateTimeA <= dt);
                    }

                    if (!string.IsNullOrEmpty(obj.FinishDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(obj.FinishDateStart);
                        query = query.Where(p => p.DateTimeC >= dt);
                    }

                    if (!string.IsNullOrEmpty(obj.FinishDateEnd))
                    {
                        DateTime dt = CommonCode.GetDateEnd(obj.FinishDateEnd);
                        query = query.Where(p => p.DateTimeC <= dt);
                    }

                    if (!string.IsNullOrEmpty(obj.SendDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(obj.SendDateStart);
                        query = query.Where(p => p.DateTimeE >= dt);
                    }

                    if (!string.IsNullOrEmpty(obj.SendDateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(obj.SendDateEnd);
                        query = query.Where(p => p.DateTimeE <= dt);
                    }

                    if (!string.IsNullOrEmpty(obj.PSDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(obj.PSDateStart);
                        query = query.Where(p => p.DateTimeD >= dt);
                    }

                    if (!string.IsNullOrEmpty(obj.PSDateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(obj.PSDateEnd);
                        query = query.Where(p => p.DateTimeD <= dt);
                    }

                    if (!string.IsNullOrEmpty(obj.AcceptedDateStart))
                    {
                        DateTime dt = Utils.StrToDateTime(obj.AcceptedDateStart);
                        query = query.Where(p => p.DateTimeG >= dt);
                    }

                    if (!string.IsNullOrEmpty(obj.AcceptedDateEnd))
                    {
                        DateTime dt = Utils.StrToDateTime(obj.AcceptedDateEnd);
                        query = query.Where(p => p.DateTimeG <= dt);
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
                        query = query.OrderByDescending(p => p.UpdateDate);
                    }

                    #endregion 排序

                    totalRows = query.Count();

                    var dataFromDB = query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                    if (dataFromDB != null && dataFromDB.Count > 0)
                    {
                        foreach (var entity in dataFromDB)
                        {
                            objList.Add(GetDTOSampleFromDB(entity));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.Logs.LogHelper.WriteError(ex);
            }

            return objList;
        }

        /// <summary>
        /// 装载寄样信息至VM中，供查询列表数据显示用
        /// </summary>
        /// <param name="ef">寄样管理->寄样信息VM</param>
        /// <returns></returns>
        private DTOSample GetDTOSampleFromDB(Sale_SendSamples ef)
        {
            DTOSample obj = new DTOSample();

            obj.SSID = ef.SSID;
            obj.FacManufactureID = ef.FacManufactureID;
            obj.CustomerCode = ef.Orders_Customers.CustomerCode;
            obj.FactoryAbbreviation = ef.Factory.Abbreviation;
            if (ef.Quot_Quot != null)
            {
                obj.QuotNumber = ef.Quot_Quot.QuotNumber;
            }
            if (ef.Order != null)
            {
                obj.PurchaseNumber = ef.Order.OrderNumber;
            }
            obj.SampleModNum = ef.SampleModNum;
            obj.SampleStatusID = ef.SampleStatus;
            obj.SampleStatus = GetSampleStatus(ef.SampleStatus);
            obj.CreateWay = ef.CreateWay;
            obj.PaymentWay = GetPaymentWay(ef.PaymentWay ?? 0);
            obj.UpdateDateFormatter = Utils.DateTimeToStr2(ef.UpdateDate);

            //判断安排生产中的计划完成时间是否晚于新建样品单时的要求完成时间
            DateTime dtClaimFinishDate = new DateTime();
            DateTime dtPlanFinishDate = new DateTime();
            int iIsDelayFinsh = 0;//默认不延迟

            if (ef.CreateDate.HasValue)
            {
                obj.IssueDate = Utils.DateTimeToStr((DateTime)ef.CreateDate);
            }
            if (ef.DateTimeA.HasValue)
            {
                obj.ClaimFinishDate = Utils.DateTimeToStr((DateTime)ef.DateTimeA);
                dtClaimFinishDate = (DateTime)ef.DateTimeA;
            }

            if (ef.ActualFinishDate.HasValue)
            {
                obj.PlanFinishDate = Utils.DateTimeToStr((DateTime)ef.ActualFinishDate);
                dtPlanFinishDate = (DateTime)ef.ActualFinishDate;
            }

            if (dtClaimFinishDate < dtPlanFinishDate)
            {
                iIsDelayFinsh = 1;//延迟
            }
            obj.IsDelayFinsh = iIsDelayFinsh;

            if (ef.DateTimeB.HasValue)
            {
                obj.AffirmDate = Utils.DateTimeToStr((DateTime)ef.DateTimeB);
            }
            if (ef.DateTimeC.HasValue)
            {
                obj.ActualFinishDate = Utils.DateTimeToStr((DateTime)ef.DateTimeC);
            }
            if (ef.DateTimeD.HasValue)
            {
                obj.PlanSendDate = Utils.DateTimeToStr((DateTime)ef.DateTimeD);
            }

            return obj;
        }

        /// <summary>
        /// 新建打样：从不同的数据源获取数据
        /// </summary>
        /// <param name="CreateWay"></param>
        /// <param name="OCID"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public DTOSample GetNewDemand(int CreateWay, int OCID, List<int> ids)
        {
            DTOSample vmSample = new DTOSample();

            try
            {
                vmSample.CreateWay = CreateWay;
                vmSample.SampleStatusID = (int)SampleStatus.SampleStatus1;

                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    List<VMManufacture> vmManufactureList = new List<VMManufacture>();
                    VMManufacture vmManufacture = new VMManufacture();

                    //先把所有不同的工厂自编号添加至集合中
                    int iFactoryID = 0;
                    List<int> fidList = new List<int>();
                    var dtDataDictionary = context.Com_DataDictionary.Where(p => p.IsDelete == 0);//取数据字典表的全部数据，供下面关联使用

                    switch (CreateWay)
                    {
                        case 1:
                            //手工创建
                            vmSample.CustomerID = OCID;

                            var query = context.Products.Where(p => ids.Contains(p.ID));
                            if (query != null)
                            {
                                query = query.OrderByDescending(p => p.Factory.ID);

                                foreach (var item in query)
                                {
                                    iFactoryID = item.FactoryID;
                                    if (!fidList.Contains(iFactoryID))
                                    {
                                        fidList.Add(iFactoryID);
                                    }
                                }

                                List<VMProducts> vmProductsLis = new List<VMProducts>();
                                VMProducts vmP = new VMProducts();

                                foreach (int item1 in fidList)
                                {
                                    var dt = query.Where(p => p.FactoryID == item1);
                                    var dtf = dt.FirstOrDefault();

                                    vmManufacture = new VMManufacture();

                                    vmManufacture.FactureID = dtf.FactoryID;
                                    vmManufacture.FactoryAbbreviation = dtf.Factory.Abbreviation;
                                    vmManufacture.Telephone = dtf.Factory.Telephone;
                                    vmManufacture.EmailAdress = dtf.Factory.EmailAdress;
                                    vmManufacture.OfficePerson = "待分配";
                                    //添加备注、要求完成你日期、上传附件

                                    vmProductsLis = new List<VMProducts>();
                                    foreach (var item2 in dt)
                                    {
                                        vmP = new VMProducts();
                                        vmP.PDID = item2.ID;
                                        vmP.ProductID = item2.ID;
                                        vmP.ProductNo = item2.No;
                                        vmP.ProductImage = item2.Image;
                                        vmP.FactoryNo = item2.NoFactory;
                                        vmP.StyleName = dtDataDictionary.Where(p => p.Code == item2.StyleID).FirstOrDefault().Name;
                                        vmP.InnerBoxRate = item2.InnerBoxRate ?? 0;
                                        vmP.OuterBoxRate = item2.OuterBoxRate ?? 0;
                                        vmP.ProductNum = 1;

                                        vmProductsLis.Add(vmP);
                                    }

                                    vmManufacture.Products = vmProductsLis;
                                    vmManufactureList.Add(vmManufacture);
                                }

                                vmSample.Manufactures = vmManufactureList;
                            }

                            break;

                        case 2:
                            var dtQuote = context.Quot_Quot.Where(q => ids.Contains(q.ID)).FirstOrDefault(); ;
                            if (dtQuote != null)
                            {
                                vmSample.CustomerID = dtQuote.CustomerID;
                                vmSample.QTID = dtQuote.ID;

                                var dtQuoteProduct = context.Quot_QuotProduct.Where(p => p.QuotID == dtQuote.ID && !p.IsDelete);

                                dtQuoteProduct = dtQuoteProduct.OrderByDescending(p => p.Factory.ID);

                                foreach (var item in dtQuoteProduct)
                                {
                                    iFactoryID = item.FactoryID ?? 0;
                                    if (!fidList.Contains(iFactoryID))
                                    {
                                        fidList.Add(iFactoryID);
                                    }
                                }

                                List<VMProducts> vmProductsLis = new List<VMProducts>();
                                VMProducts vmP = new VMProducts();

                                foreach (int item1 in fidList)
                                {
                                    var dt = dtQuoteProduct.Where(p => p.FactoryID == item1);
                                    var dtf = dt.FirstOrDefault();

                                    vmManufacture = new VMManufacture();

                                    vmManufacture.FactureID = item1;
                                    vmManufacture.FactoryAbbreviation = dtf.Factory.Abbreviation;
                                    vmManufacture.Telephone = dtf.Factory.Telephone;
                                    vmManufacture.EmailAdress = dtf.Factory.EmailAdress;
                                    vmManufacture.OfficePerson = "待分配";
                                    //添加备注、要求完成你日期、上传附件

                                    vmProductsLis = new List<VMProducts>();
                                    foreach (var item2 in dt)
                                    {
                                        vmP = new VMProducts();
                                        vmP.PDID = item2.ID;
                                        vmP.ProductID = item2.ID;
                                        vmP.ProductNo = item2.No;
                                        vmP.ProductImage = item2.Image;
                                        vmP.FactoryNo = item2.NoFactory;
                                        vmP.StyleName = dtDataDictionary.Where(p => p.Code == item2.StyleID).FirstOrDefault().Name;
                                        vmP.InnerBoxRate = item2.InnerBoxRate ?? 0;
                                        vmP.OuterBoxRate = item2.OuterBoxRate ?? 0;
                                        vmP.ProductNum = 1;

                                        vmProductsLis.Add(vmP);
                                    }

                                    vmManufacture.Products = vmProductsLis;
                                    vmManufactureList.Add(vmManufacture);
                                }

                                vmSample.Manufactures = vmManufactureList;
                            }
                            break;

                        case 3:
                            var dtOrder = context.Orders.Where(o => ids.Contains(o.OrderID)).FirstOrDefault();
                            if (dtOrder != null)
                            {
                                vmSample.CustomerID = dtOrder.CustomerID;
                                vmSample.PHID = dtOrder.OrderID;

                                var dtOrderProduct = context.OrderProducts.Where(p => p.OrderID == dtOrder.OrderID);

                                dtOrderProduct = dtOrderProduct.OrderByDescending(p => p.Factory.ID);

                                foreach (var item in dtOrderProduct)
                                {
                                    iFactoryID = item.FactoryID ?? 0;
                                    if (!fidList.Contains(iFactoryID))
                                    {
                                        fidList.Add(iFactoryID);
                                    }
                                }

                                List<VMProducts> vmProductsLis = new List<VMProducts>();
                                VMProducts vmP = new VMProducts();

                                foreach (int item1 in fidList)
                                {
                                    var dt = dtOrderProduct.Where(p => p.FactoryID == item1);
                                    var dtf = dt.FirstOrDefault();

                                    vmManufacture = new VMManufacture();

                                    vmManufacture.FactureID = item1;
                                    vmManufacture.FactoryAbbreviation = dtf.Factory.Abbreviation;
                                    vmManufacture.Telephone = dtf.Factory.Telephone;
                                    vmManufacture.EmailAdress = dtf.Factory.EmailAdress;
                                    vmManufacture.OfficePerson = "待分配";
                                    //添加备注、要求完成你日期、上传附件

                                    vmProductsLis = new List<VMProducts>();
                                    foreach (var item2 in dt)
                                    {
                                        vmP = new VMProducts();
                                        vmP.PDID = item2.ID;
                                        vmP.ProductID = item2.ID;
                                        vmP.ProductNo = item2.No;
                                        vmP.ProductImage = item2.Image;
                                        vmP.FactoryNo = item2.NoFactory;
                                        vmP.StyleName = dtDataDictionary.Where(p => p.Code == item2.StyleID).FirstOrDefault().Name;
                                        vmP.InnerBoxRate = item2.InnerBoxRate ?? 0;
                                        vmP.OuterBoxRate = item2.OuterBoxRate ?? 0;
                                        vmP.ProductNum = 1;

                                        vmProductsLis.Add(vmP);
                                    }

                                    vmManufacture.Products = vmProductsLis;
                                    vmManufactureList.Add(vmManufacture);
                                }

                                vmSample.Manufactures = vmManufactureList;
                            }
                            break;

                        default:

                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Tools.Logs.LogHelper.WriteError(e);
            }
            return vmSample;
        }

        public DTOSample ManufactureInfo(DTOSample vmSS, int SSID, int CreateWay, int SampleStatus)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    List<VMManufacture> vmMFList = new List<VMManufacture>();
                    VMManufacture vmMF = new VMManufacture();
                    var db = context.Sale_SendSamples.Where(p => p.SSID == SSID).FirstOrDefault();

                    if (db != null)
                    {
                        vmSS.SampleStatusID = db.SampleStatus;
                        vmMF.SSID = db.SSID;
                        vmMF.CustomerID = db.CustomerID;
                        vmMF.CustomerCode = db.Orders_Customers.CustomerCode;

                        if (db.CreateWay == 2)
                        {
                            vmMF.QTID = db.QTID ?? 0;
                            vmMF.CustomerID = db.Quot_Quot.CustomerID;
                            vmMF.CustomerCode = db.Quot_Quot.Orders_Customers.CustomerCode;
                        }
                        if (db.CreateWay == 3)
                        {
                            vmMF.PHID = db.PHID ?? 0;
                            vmMF.CustomerID = db.Order.CustomerID;
                            vmMF.CustomerCode = db.Order.Orders_Customers.CustomerCode;
                        }

                        vmMF.SampleStatusID = db.SampleStatus;
                        vmMF.SampleStatus = GetSampleStatus(db.SampleStatus);
                        vmMF.CreateWay = db.CreateWay;

                        vmMF.FactureID = db.Factory.ID;
                        vmMF.FactoryAbbreviation = db.Factory.Abbreviation;
                        vmMF.FacManufactureID = db.FacManufactureID;
                        vmMF.ManufactureNote = db.NoteA;
                        if (db.SampleStatus == 4)
                        {
                            vmMF.AlterStageIdea = db.PhaseExplan_1;
                        }
                        if (db.SampleStatus == 5)
                        {
                            vmMF.AlterStageIdea = db.PhaseExplan_2;
                        }

                        //vmMF.File = db.ValueC;

                        //查询样品单中的已上传附件信息列表数据
                        VMUpLoad vmUpLoad = new VMUpLoad();
                        List<VMUpLoadFile> vmUpLoadFileList = new List<VMUpLoadFile>();
                        VMUpLoadFile vmUpLoadFile;

                        var dbUpLoadFileList = context.UpLoadFiles.Where(p => p.LinkID == db.SSID && !p.IsDelete && p.ModuleType == (int)UploadFileType.SampleUpLoad);
                        if (dbUpLoadFileList != null)
                        {
                            foreach (var item1 in dbUpLoadFileList)
                            {
                                vmUpLoadFile = new VMUpLoadFile();
                                vmUpLoadFile.ID = item1.ID;
                                vmUpLoadFile.LinkID = item1.LinkID;
                                vmUpLoadFile.ModuleType = item1.ModuleType;
                                vmUpLoadFile.IsDelete = item1.IsDelete;
                                vmUpLoadFile.DisplayFileName = item1.DisplayFileName;
                                vmUpLoadFile.ServerFileName = item1.ServerFileName;
                                vmUpLoadFile.DT_CREATEDATE = item1.DT_CREATEDATE;
                                vmUpLoadFile.ST_CREATEUSER = item1.ST_CREATEUSER;

                                vmUpLoadFileList.Add(vmUpLoadFile);
                            }
                        }

                        vmUpLoad.ID = db.SSID;
                        vmUpLoad.UpLoadType = UploadFileType.SampleUpLoad;
                        vmUpLoad.UpLoadFileList = vmUpLoadFileList;

                        vmMF.UploadFiles = vmUpLoad;

                        if (db.DateTimeA.HasValue)
                        {
                            vmMF.ClaimFinishDate = Utils.DateTimeToStr((DateTime)db.DateTimeA);
                        }

                        if (db.ActualFinishDate.HasValue)
                        {
                            vmMF.PlanFinishDate = Utils.DateTimeToStr((DateTime)db.ActualFinishDate);
                        }

                        //
                        if (db.SampleStatus >= 5)
                        {
                            vmMF.Telephone = db.Factory.Telephone;
                            vmMF.EmailAdress = db.Factory.EmailAdress;
                            vmMF.OfficePerson = "内勤";
                            vmMF.ProductsOffice = "办事处"; ;

                            vmMF.PaymentWayID = db.PaymentWay ?? 0;
                            vmMF.ExpressToLTD = db.ExpressToLTD;
                            vmMF.ToAcount = db.ToAcount;
                            vmMF.AcceptedID = db.AcceptedInfo ?? 0;
                            var dbCA = context.Orders_AcceptInformation.Where(p => p.AIID == db.AcceptedInfo && !p.IsDelete).FirstOrDefault();
                            if (dbCA != null)
                            {
                                string areaName = context.Reg_Area.Where(p => p.ARID == dbCA.Province).FirstOrDefault().AreaName;
                                string countryNmae = context.Reg_Country.Where(p => p.COID == dbCA.Country).FirstOrDefault().CountryName;
                                vmMF.AcceptedDetail = dbCA.StreetAddress + "," + dbCA.City + "," + areaName + "," + countryNmae;
                            }

                            vmMF.ExpressCost = db.ExpressCost ?? 0;

                            if (db.DateTimeC.HasValue)
                            {
                                vmMF.ActualFinishDate = Utils.DateTimeToStr((DateTime)db.DateTimeC);
                            }
                            if (db.DateTimeB.HasValue)
                            {
                                vmMF.AffirmDate = Utils.DateTimeToStr((DateTime)db.DateTimeB);
                            }

                            if (db.DateTimeD.HasValue)
                            {
                                vmMF.PlanSendDate = Utils.DateTimeToStr((DateTime)db.DateTimeD);
                            }
                        }

                        //寄件信息
                        if (db.SampleStatus >= 6)
                        {
                            vmMF.ExpressFromLTD = db.ExpressFromLTD;
                            vmMF.ExpressID = db.ExpressID;
                            vmMF.SendPieceNum = db.SendPieceNum ?? 0;
                            vmMF.ExpressCost = db.ExpressCost ?? 0;
                            vmMF.SignStatusID = db.AcceptedStatus ?? 0;
                            vmMF.SendRemark = db.NoteC;

                            if (db.DateTimeE.HasValue)
                            {
                                vmMF.ActualSendDate = Utils.DateTimeToStr((DateTime)db.DateTimeE);
                            }
                            if (db.DateTimeF.HasValue)
                            {
                                vmMF.ExpectedArrivalDate = Utils.DateTimeToStr((DateTime)db.DateTimeF);
                            }
                            if (db.DateTimeF.HasValue)
                            {
                                vmMF.ExpectedArrivalDate = Utils.DateTimeToStr((DateTime)db.DateTimeF);
                            }
                            if (db.DateTimeG.HasValue)
                            {
                                vmMF.ActualAcceptedDate = Utils.DateTimeToStr((DateTime)db.DateTimeG);
                            }
                        }

                        if (db.CreateDate.HasValue)
                        {
                            vmMF.IssueDate = Utils.DateTimeToStr((DateTime)db.CreateDate);
                        }

                        //取的寄样信息状态更新履历
                        List<VMSendSampleHis> vmssHList = new List<VMSendSampleHis>();
                        VMSendSampleHis vmssH = new VMSendSampleHis();

                        if (db.Sale_SendSampleHis != null)
                        {
                            foreach (var item in db.Sale_SendSampleHis)
                            {
                                vmssH = new VMSendSampleHis();
                                vmssH.SSHID = item.SSHID;
                                vmssH.SSID = item.SSID;
                                vmssH.AuditIdea = item.PhaseExplan;
                                vmssH.AuditCreateDate = Utils.DateTimeToStr2((DateTime)item.CreateDate);
                                vmssH.AuditUserName = item.SystemUser.UserName;
                                vmssH.SampleStatus = GetSampleStatus(item.SampleStatus);

                                vmssHList.Add(vmssH);
                            }
                        }
                        vmSS.SendSampleHis = vmssHList;

                        List<VMProducts> vmListSampleProduct = new List<VMProducts>();
                        VMProducts vmSampleProduct = new VMProducts();

                        if (db.Sale_ProductsSample != null)
                        {
                            foreach (var item1 in db.Sale_ProductsSample)
                            {
                                vmSampleProduct = new VMProducts();
                                vmSampleProduct.PSID = item1.PSID;
                                vmSampleProduct.SSID = item1.SSID;
                                vmSampleProduct.PDID = item1.PDID;
                                vmSampleProduct.ProductNum = item1.ProductSampleNum;
                                vmSampleProduct.IsMod = item1.IsMod;
                                vmMF.IsMod = item1.IsMod;
                                vmSampleProduct.SampleProductNote = item1.NoteA;

                                if (item1.PlanFinishDate.HasValue)
                                {
                                    vmSampleProduct.ProductPlanFinshDate = Utils.DateTimeToStr((DateTime)item1.PlanFinishDate);
                                }

                                if (db.CreateWay == 1)
                                {
                                    var product = context.Products.Where(p => p.ID == item1.PDID).FirstOrDefault();
                                    if (product != null)
                                    {
                                        vmSampleProduct.ProductID = product.ID;
                                        vmSampleProduct.ProductNo = product.No;
                                        vmSampleProduct.FactoryNo = product.NoFactory;
                                        vmSampleProduct.StyleName = context.Com_DataDictionary.Where(p => p.Code == product.StyleID).FirstOrDefault().Name;
                                        vmSampleProduct.InnerBoxRate = product.InnerBoxRate ?? 0;
                                        vmSampleProduct.OuterBoxRate = product.OuterBoxRate ?? 0;
                                    }
                                }

                                if (db.CreateWay == 2)
                                {
                                    var product = context.Quot_QuotProduct.Where(p => p.ID == item1.PDID).FirstOrDefault();
                                    if (product != null)
                                    {
                                        vmSampleProduct.ProductID = product.ProductID;
                                        vmSampleProduct.ProductNo = product.No;
                                        vmSampleProduct.FactoryNo = product.NoFactory;
                                        vmSampleProduct.StyleName = context.Com_DataDictionary.Where(p => p.Code == product.StyleID).FirstOrDefault().Name;
                                        vmSampleProduct.InnerBoxRate = product.InnerBoxRate ?? 0;
                                        vmSampleProduct.OuterBoxRate = product.OuterBoxRate ?? 0;
                                    }
                                }

                                if (db.CreateWay == 3)
                                {
                                    var product = context.OrderProducts.Where(p => p.ID == item1.PDID).FirstOrDefault();
                                    if (product != null)
                                    {
                                        vmSampleProduct.ProductID = product.ProductID;
                                        vmSampleProduct.ProductNo = product.No;
                                        vmSampleProduct.FactoryNo = product.NoFactory;
                                        vmSampleProduct.StyleName = context.Com_DataDictionary.Where(p => p.Code == product.StyleID).FirstOrDefault().Name;
                                        vmSampleProduct.InnerBoxRate = product.InnerBoxRate ?? 0;
                                        vmSampleProduct.OuterBoxRate = product.OuterBoxRate ?? 0;
                                    }
                                }
                                vmListSampleProduct.Add(vmSampleProduct);
                            }
                            vmMF.Products = vmListSampleProduct;
                        }

                        vmMFList.Add(vmMF);
                    }
                    vmSS.Manufactures = vmMFList;
                }
            }
            catch (Exception e)
            {
                Tools.Logs.LogHelper.WriteError(e);
            }
            return vmSS;
        }

        /// <summary>
        /// 取得寄样箱唛产品信息数据
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="htmlCode">前台编辑的富文本内容</param>
        /// <param name="vmSample">寄样信息视图模型</param>
        /// <returns>替换为真实产品信息的数据</returns>
        public DTOSample GetDetailBoxMark(VMERPUser currentUser, string htmlCode, DTOSample vmSample)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var dtSendSample = context.Sale_SendSamples.Where(s => s.SSID == vmSample.SSID);
                    var dtSendSamplePer = dtSendSample.FirstOrDefault();

                    string sBoxMark = string.Empty;
                    //查看页面中的预览
                    if (string.IsNullOrEmpty(htmlCode))
                    {
                        sBoxMark = dtSendSamplePer.ValueB;
                    }
                    else//编辑页面中的预览
                    {
                        string decodeSampleBoxMark = string.Empty;
                        decodeSampleBoxMark = GlobalObject.decodeURIComponent(GlobalObject.unescape(htmlCode));
                        decodeSampleBoxMark = decodeSampleBoxMark.Replace("Description", "1");
                        decodeSampleBoxMark = decodeSampleBoxMark.Replace("UPC#", "2");
                        decodeSampleBoxMark = decodeSampleBoxMark.Replace("INNERPACK", "3").Replace("INNER PACK", "3");
                        decodeSampleBoxMark = decodeSampleBoxMark.Replace("CASEPACK", "4").Replace("CASE PACK", "4");

                        decodeSampleBoxMark = HttpUtility.HtmlEncode(decodeSampleBoxMark);

                        sBoxMark = decodeSampleBoxMark;//寄样箱唛
                    }

                    if (dtSendSamplePer != null)
                    {
                        int iQTID = 0, iPHID = 0;

                        if (!string.IsNullOrEmpty(sBoxMark))
                        {
                            //int.TryParse(sBoxMark, out iRows);//第一个字符是编辑时的行数

                            //sBoxMark = sBoxMark.Remove(0, 1);//去掉首位行数即为箱唛内容
                            iQTID = dtSendSamplePer.QTID ?? 0;
                            iPHID = dtSendSamplePer.PHID ?? 0;

                            List<VMProducts> vmProductsList = new List<VMProducts>();
                            VMProducts vmProducts;

                            foreach (var item in dtSendSamplePer.Sale_ProductsSample)
                            {
                                //根据生产单创建方式查询产品信息
                                if (iQTID + iPHID == 0)
                                {
                                    var dtProduct = context.Products.Where(p => p.ID == item.PDID).FirstOrDefault();

                                    vmProducts = new VMProducts();
                                    vmProducts.ProductDescription = dtProduct.Desc;
                                    vmProducts.ProductUPC = dtProduct.UPC;
                                    vmProducts.InnerBoxRate = dtProduct.InnerBoxRate ?? 0;
                                    vmProducts.OuterBoxRate = dtProduct.OuterBoxRate ?? 0;

                                    vmProductsList.Add(vmProducts);
                                }

                                if (iQTID > 0)
                                {
                                    var dtProduct = dtSendSamplePer.Quot_Quot.Quot_QuotProduct.Where(p => p.ID == item.PDID).FirstOrDefault();

                                    vmProducts = new VMProducts();
                                    vmProducts.ProductDescription = dtProduct.Desc;
                                    vmProducts.ProductUPC = dtProduct.UPC;
                                    vmProducts.InnerBoxRate = dtProduct.InnerBoxRate ?? 0;
                                    vmProducts.OuterBoxRate = dtProduct.OuterBoxRate ?? 0;

                                    vmProductsList.Add(vmProducts);
                                }

                                if (iPHID > 0)
                                {
                                    var dtProduct = dtSendSamplePer.Order.OrderProducts.Where(p => p.ID == item.PDID).FirstOrDefault();

                                    vmProducts = new VMProducts();
                                    vmProducts.ProductDescription = dtProduct.Desc;
                                    vmProducts.ProductUPC = dtProduct.UPC;
                                    vmProducts.InnerBoxRate = dtProduct.InnerBoxRate ?? 0;
                                    vmProducts.OuterBoxRate = dtProduct.OuterBoxRate ?? 0;

                                    vmProductsList.Add(vmProducts);
                                }
                            }

                            List<VMManufacture> vmManufactureList = new List<VMManufacture>();
                            VMManufacture vmManufacture = new VMManufacture();

                            foreach (var item in vmProductsList)
                            {
                                vmManufacture = new VMManufacture();
                                string sData = string.Empty;
                                sData = sBoxMark;
                                sData = sData.Replace("&lt;code id=&quot;" + 1.ToString() + "&quot;&gt;" + 1.ToString() + "&lt;/code&gt;", item.ProductDescription);
                                sData = sData.Replace("&lt;code id=&quot;" + 2.ToString() + "&quot;&gt;" + 2.ToString() + "&lt;/code&gt;", item.ProductUPC);
                                sData = sData.Replace("&lt;code id=&quot;" + 3.ToString() + "&quot;&gt;" + 3.ToString() + "&lt;/code&gt;", item.InnerBoxRate.ToString());
                                sData = sData.Replace("&lt;code id=&quot;" + 4.ToString() + "&quot;&gt;" + 4.ToString() + "&lt;/code&gt;", item.OuterBoxRate.ToString());

                                sData = HttpUtility.HtmlDecode(sData);
                                //sData = GlobalObject.encodeURIComponent(sData);
                                vmManufacture.SampleBoxMark = sData;
                                vmManufactureList.Add(vmManufacture);
                            }

                            vmSample.Manufactures = vmManufactureList;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Tools.Logs.LogHelper.WriteError(e);
            }
            return vmSample;
        }

        /// <summary>
        /// 生成三位随机数
        /// </summary>
        /// <returns>三位随机数</returns>
        public int GetTreeNumRandom()
        {
            Random rd = new Random();
            return rd.Next(100, 999);
        }

        public DBOperationStatus Save(int userID, string createOperater, DTOSample vmSS)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                int affectRows = 0;

                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    DAL.Sale_SendSamples ssEF = new Sale_SendSamples();
                    DAL.Sale_ProductsSample psEF = new Sale_ProductsSample();
                    DateTime dtNow = DateTime.Now;

                    foreach (var item1 in vmSS.Manufactures)
                    {
                        ssEF = new Sale_SendSamples();
                        if (vmSS.CreateWay == 2)
                        {
                            ssEF.QTID = vmSS.QTID;
                        }
                        if (vmSS.CreateWay == 3)
                        {
                            ssEF.PHID = vmSS.PHID;
                        }
                        ssEF.CreateWay = vmSS.CreateWay;
                        ssEF.CustomerID = vmSS.CustomerID;
                        ssEF.FactureID = item1.FactureID;
                        ssEF.FacManufactureID = dtNow.ToString("yyyyMMddHHmmss") + "_" + GetTreeNumRandom().ToString();//随机生成;
                        ssEF.SampleModNum = 0;
                        ssEF.SampleStatus = vmSS.SampleStatusID;
                        ssEF.NoteA = item1.ManufactureNote;

                        if (!string.IsNullOrEmpty(item1.ClaimFinishDate))
                        {
                            ssEF.DateTimeA = Utils.StrToDateTime(item1.ClaimFinishDate);
                        }
                        //string sResult = string.Empty;
                        //ssEF.ValueC = SaveFileFromToPath(item1.File, out sResult);//上传附件

                        ssEF.IsDelete = false;
                        ssEF.CreateDate = dtNow;
                        ssEF.UpdateDate = dtNow;//默认排序用
                        ssEF.CreateUserID = userID;
                        ssEF.CreateTerminal = createOperater;

                        foreach (var item2 in item1.Products)
                        {
                            psEF = new Sale_ProductsSample();
                            psEF.PDID = item2.PDID;
                            psEF.ProductSampleNum = item2.ProductNum;
                            psEF.NoteA = item2.SampleProductNote;
                            psEF.CreateDate = dtNow;
                            psEF.IsMod = 0;

                            ssEF.Sale_ProductsSample.Add(psEF);
                        }

                        DAL.Sale_SendSampleHis efSSH = new Sale_SendSampleHis();

                        efSSH = AddSendSampleHis(efSSH, vmSS.SampleStatusID, "新建生产单", dtNow, userID);
                        ssEF.Sale_SendSampleHis.Add(efSSH);

                        context.Sale_SendSamples.Add(ssEF);
                        affectRows = context.SaveChanges();

                        if (affectRows > 0)
                        {
                            //保存上传附件列表数据
                            if (item1.UploadFiles != null)
                            {
                                UpLoadFile vmUpLoadFile;
                                int newID = 0;
                                foreach (var item2 in item1.UploadFiles.UpLoadFileList)
                                {
                                    if (!item2.IsDelete)
                                    {
                                        vmUpLoadFile = new UpLoadFile();
                                        vmUpLoadFile.ID = --newID;
                                        vmUpLoadFile.DisplayFileName = item2.DisplayFileName;
                                        vmUpLoadFile.ServerFileName = item2.ServerFileName;
                                        vmUpLoadFile.DT_CREATEDATE = item2.DT_CREATEDATE;
                                        vmUpLoadFile.DT_MODIFYDATE = dtNow;
                                        vmUpLoadFile.ST_CREATEUSER = userID;
                                        vmUpLoadFile.ST_MODIFYUSER = userID;
                                        vmUpLoadFile.IPAddress = createOperater;
                                        vmUpLoadFile.IsDelete = false;
                                        vmUpLoadFile.LinkID = ssEF.SSID;
                                        vmUpLoadFile.ModuleType = (int)UploadFileType.SampleUpLoad;

                                        context.UpLoadFiles.Add(vmUpLoadFile);
                                    }
                                }
                                affectRows = context.SaveChanges();
                            }
                        }
                    }

                    //result = DBOperationStatus.Success;
                    //return result;

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
            catch (Exception e)
            {
                Tools.Logs.LogHelper.WriteError(e);
            }
            return result;
        }

        public DBOperationStatus UpdateSampleStatus(int userID, string createOperater, int SSID, DTOSample vmSample)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var dtSS = context.Sale_SendSamples.Where(p => p.SSID == SSID).FirstOrDefault();
                    if (dtSS != null)
                    {
                        DateTime dtNow = System.DateTime.Now;

                        //安排生产时，需要记录工厂计划完成时间
                        if (vmSample.SampleStatusID == (int)SampleStatus.SampleStatus3)
                        {
                            if (!string.IsNullOrEmpty(vmSample.Manufactures[0].PlanFinishDate))
                            {
                                dtSS.ActualFinishDate = Utils.StrToDateTime(vmSample.Manufactures[0].PlanFinishDate);
                            }
                        }

                        dtSS.SampleStatus = vmSample.SampleStatusID;
                        dtSS.UpdateDate = DateTime.Now;
                        dtSS.UpdateUserID = userID;
                        dtSS.UpdateTerminal = createOperater;

                        //插入寄样信息状态变更履历
                        DAL.Sale_SendSampleHis efSSH = new Sale_SendSampleHis();

                        efSSH = AddSendSampleHis(efSSH, dtSS.SampleStatus, "更新状态", dtNow, userID);

                        dtSS.Sale_SendSampleHis.Add(efSSH);
                    }

                    //result = DBOperationStatus.Success;
                    //return result;

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
            catch (Exception e)
            {
                Tools.Logs.LogHelper.WriteError(e);
            }

            return result;
        }

        public DBOperationStatus SaveAlterStage(VMERPUser currentUser, string createOperater, int sampleStatus, DTOSample vmSample)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    int SSID = vmSample.Manufactures[0].SSID;
                    var query = context.Sale_SendSamples.Where(p => p.SSID == SSID);
                    //query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForSample, "CreateUserID");

                    var dtSS = query.FirstOrDefault();

                    if (dtSS != null)
                    {
                        DateTime dtNow = DateTime.Now;

                        foreach (var item1 in vmSample.Manufactures)
                        {
                            dtSS.SampleStatus = item1.SampleStatusID;
                            dtSS.UpdateDate = dtNow;

                            if (item1.SampleStatusID <= (int)SampleStatus.SampleStatus4)
                            {
                                dtSS.PhaseExplan_1 = item1.AlterStageIdea;

                                foreach (var item2 in item1.Products)
                                {
                                    var efSampleProduct = dtSS.Sale_ProductsSample.Where(p => p.PSID == item2.PSID).FirstOrDefault();
                                    if (efSampleProduct != null)
                                    {
                                        if (!string.IsNullOrEmpty(item2.ProductPlanFinshDate))
                                        {
                                            efSampleProduct.PlanFinishDate = Utils.StrToDateTime(item2.ProductPlanFinshDate);
                                        }

                                        dtSS.Sale_ProductsSample.Add(efSampleProduct);
                                    }
                                }
                            }
                            else
                            {
                                dtSS.PhaseExplan_2 = item1.AlterStageIdea;

                                bool bIsMod = false;

                                foreach (var item2 in item1.Products)
                                {
                                    if (item2.IsMod == 1)
                                    {
                                        bIsMod = true;
                                    }
                                    var efSampleProduct = dtSS.Sale_ProductsSample.Where(p => p.PSID == item2.PSID).FirstOrDefault();
                                    if (efSampleProduct != null)
                                    {
                                        efSampleProduct.IsMod = item2.IsMod;
                                        efSampleProduct.NoteB = item2.ModIdea;

                                        dtSS.Sale_ProductsSample.Add(efSampleProduct);
                                    }
                                }

                                if (bIsMod)
                                {
                                    dtSS.SampleModNum += 1;
                                    dtSS.SampleStatus = (int)SampleStatus.SampleStatus1;
                                    if (!string.IsNullOrEmpty(item1.ClaimFinishDate))
                                    {
                                        dtSS.DateTimeA = Utils.StrToDateTime(item1.ClaimFinishDate);
                                    }

                                    //保存上传附件列表数据
                                    if (item1.UploadFiles != null)
                                    {
                                        UpLoadFile vmUpLoadFile;
                                        List<UpLoadFile> list_UpLoadFile = context.UpLoadFiles.ToList();

                                        foreach (var item2 in item1.UploadFiles.UpLoadFileList)
                                        {
                                            //
                                            if (item2.ID == 0)
                                            {
                                                if (!item2.IsDelete)
                                                {
                                                    vmUpLoadFile = new UpLoadFile();
                                                    vmUpLoadFile.DisplayFileName = item2.DisplayFileName;
                                                    vmUpLoadFile.ServerFileName = item2.ServerFileName;
                                                    vmUpLoadFile.DT_CREATEDATE = item2.DT_CREATEDATE;
                                                    vmUpLoadFile.DT_MODIFYDATE = dtNow;
                                                    vmUpLoadFile.ST_CREATEUSER = currentUser.UserID;
                                                    vmUpLoadFile.ST_MODIFYUSER = currentUser.UserID;
                                                    vmUpLoadFile.IPAddress = createOperater;
                                                    vmUpLoadFile.IsDelete = false;
                                                    vmUpLoadFile.LinkID = SSID;
                                                    vmUpLoadFile.ModuleType = (int)UploadFileType.SampleUpLoad;

                                                    context.UpLoadFiles.Add(vmUpLoadFile);
                                                }
                                                else
                                                {
                                                }
                                            }
                                            else
                                            {
                                                //更新
                                                var query_UpLoadFiles = list_UpLoadFile.Find(d => d.ID == item2.ID);
                                                query_UpLoadFiles.IsDelete = item2.IsDelete;
                                                query_UpLoadFiles.DT_MODIFYDATE = dtNow;
                                                query_UpLoadFiles.ST_MODIFYUSER = currentUser.UserID;
                                            }
                                        }
                                    }
                                    //string sResult = string.Empty;
                                    //dtSS.ValueC = SaveFileFromToPath(item1.File, out sResult);//上传附件
                                }
                                else
                                {
                                    dtSS.DateTimeB = dtNow;//样品确认日期
                                }
                            }

                            DAL.Sale_SendSampleHis efSSH = new Sale_SendSampleHis();

                            efSSH = AddSendSampleHis(efSSH, dtSS.SampleStatus, item1.AlterStageIdea, dtNow, currentUser.UserID);

                            dtSS.Sale_SendSampleHis.Add(efSSH);
                        }
                    }

                    //result = DBOperationStatus.Success;
                    //return result;

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
            catch (Exception e)
            {
                Tools.Logs.LogHelper.WriteError(e);
            }

            return result;
        }

        public DBOperationStatus SaveSendSample(int userID, string createOperater, int sampleStatus, DTOSample vmSample)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    int SSID = vmSample.Manufactures[0].SSID;
                    var dtSS = context.Sale_SendSamples.Where(p => p.SSID == SSID).FirstOrDefault();
                    if (dtSS != null)
                    {
                        DateTime dtNow = DateTime.Now;

                        foreach (var item1 in vmSample.Manufactures)
                        {
                            //收件信息
                            if (item1.SampleStatusID == (int)SampleStatus.SampleStatus5)
                            {
                                dtSS.DateTimeC = dtNow;//样品完成日期
                                dtSS.SampleStatus = (int)SampleStatus.SampleStatus6;

                                string decodeSampleBoxMark = string.Empty;
                                decodeSampleBoxMark = GlobalObject.decodeURIComponent(item1.SampleBoxMark);
                                decodeSampleBoxMark = decodeSampleBoxMark.Replace("Description", "1");
                                decodeSampleBoxMark = decodeSampleBoxMark.Replace("UPC#", "2");
                                decodeSampleBoxMark = decodeSampleBoxMark.Replace("INNERPACK", "3").Replace("INNER PACK", "3");
                                decodeSampleBoxMark = decodeSampleBoxMark.Replace("CASEPACK", "4").Replace("CASE PACK", "4");

                                decodeSampleBoxMark = HttpUtility.HtmlEncode(decodeSampleBoxMark);

                                dtSS.ValueB = decodeSampleBoxMark;//寄样箱唛

                                //寄样标签
                                //寄样发票
                                if (!string.IsNullOrEmpty(item1.PlanSendDate))
                                {
                                    dtSS.DateTimeD = Utils.StrToDateTime(item1.PlanSendDate);
                                }

                                dtSS.PaymentWay = item1.PaymentWayID;
                                if (item1.PaymentWayID == 1)
                                {
                                    dtSS.ExpressToLTD = item1.ExpressToLTD;
                                    dtSS.ToAcount = item1.ToAcount;
                                }

                                if (item1.AcceptedID > 0)
                                {
                                    dtSS.AcceptedInfo = item1.AcceptedID;
                                }
                            }
                            //寄件信息：可能需要加“汇集中”状态：7
                            if (item1.SampleStatusID == (int)SampleStatus.SampleStatus6)
                            {
                                dtSS.SampleStatus = (int)SampleStatus.SampleStatus8;

                                dtSS.ExpressFromLTD = item1.ExpressFromLTD;
                                dtSS.ExpressID = item1.ExpressID;
                                dtSS.ExpressCost = item1.ExpressCost;
                                dtSS.SendPieceNum = item1.SendPieceNum;

                                dtSS.DateTimeE = Utils.StrToDateTime(item1.ActualSendDate);
                                dtSS.DateTimeF = Utils.StrToDateTime(item1.ExpectedArrivalDate);
                                dtSS.NoteC = item1.SendRemark;
                            }

                            //签收信息
                            if (item1.SampleStatusID == (int)SampleStatus.SampleStatus8)
                            {
                                dtSS.SampleStatus = (int)SampleStatus.SampleStatus10;
                                dtSS.AcceptedStatus = item1.SignStatusID;
                                dtSS.DateTimeG = Utils.StrToDateTime(item1.ActualAcceptedDate);
                            }

                            //插入寄样信息状态变更履历
                            DAL.Sale_SendSampleHis efSSH = new Sale_SendSampleHis();

                            efSSH = AddSendSampleHis(efSSH, dtSS.SampleStatus, "更新状态", dtNow, userID);

                            dtSS.Sale_SendSampleHis.Add(efSSH);
                        }
                    }
                    //result = DBOperationStatus.Success;
                    //return result;

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
            catch (Exception e)
            {
                Tools.Logs.LogHelper.WriteError(e);
            }

            return result;
        }

        private Sale_SendSampleHis AddSendSampleHis(DAL.Sale_SendSampleHis efSSH, int iSampleStatus, string explan, DateTime dtA, int userID)
        {
            efSSH.PhaseExplan = explan;
            efSSH.SampleStatus = iSampleStatus;
            efSSH.CreateDate = dtA;
            efSSH.AuditUserID = userID;

            return efSSH;
        }

        /// <summary>
        /// 删除寄样信息
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="createOperater"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public DBOperationStatus DeleteSample(VMERPUser currentUser, string createOperater, List<int> ids)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    DateTime dtNow = DateTime.Now;

                    var dtSS = context.Sale_SendSamples.Where(o => ids.Contains(o.SSID));
                    //dtSS = dtSS.SetDataPermissionConditions(currentUser, DataPermissionModules.ForSample);
                    var dtSSlist = dtSS.ToList();

                    foreach (var item1 in dtSSlist)
                    {
                        item1.UpdateUserID = currentUser.UserID;
                        item1.UpdateTerminal = createOperater;
                        item1.UpdateDate = dtNow;
                        item1.IsDelete = true;
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
            catch (Exception e)
            {
                Tools.Logs.LogHelper.WriteError(e);
            }

            return result;
        }

        public static string SaveFileFromToPath(string a, out string d)
        {
            try
            {
                string b = string.Empty;
                if (!string.IsNullOrEmpty(a))
                {
                    //取得上传附件的绝对路径
                    b = HttpContext.Current.Server.MapPath("~/data/UploadSampleFiles/");
                    //如果目标文件路径不存在，则创建之
                    if (!System.IO.Directory.Exists(b))
                    {
                        System.IO.Directory.CreateDirectory(b);
                    }

                    //去掉路径，保留文件名，作为保存的目标文件名
                    string c = a.Substring(a.LastIndexOf("\\") + 1, a.Length - a.LastIndexOf("\\") - 1);

                    //把源文件复制到目标文件路径中，true表示覆盖同名文件
                    System.IO.File.Copy(a, b + c, true);
                    //生成程序运行的相对路径和文件名
                    b = HttpContext.Current.Request.ApplicationPath + "../../data/UploadSampleFiles/" + c;
                }

                d = string.Empty;
                return b;
            }
            catch (Exception e)
            {
                d = e.ToString();
                return string.Empty;
            }
        }

        /// <summary>
        /// 用寄样数据状态编号置换枚举描述
        /// </summary>
        /// <param name="i">编号</param>
        /// <returns></returns>
        public static string GetSampleStatus(int id)
        {
            Dictionary<int, string> dictionary = Tools.EnumHelper.EnumHelper.GetCustomEnums<int>(typeof(SampleStatus));

            string name = "";
            foreach (var item in dictionary)
            {
                if (item.Key == id)
                {
                    name = item.Value;
                    break;
                }
            }
            return name;
        }

        /// <summary>
        /// 用寄样数据状态编号置换枚举描述
        /// </summary>
        /// <param name="i">编号</param>
        /// <returns></returns>
        public static string GetPaymentWay(int id)
        {
            Dictionary<int, string> dictionary = Tools.EnumHelper.EnumHelper.GetCustomEnums<int>(typeof(PaymentWay));

            string name = "";
            foreach (var item in dictionary)
            {
                if (item.Key == id)
                {
                    name = item.Value;
                    break;
                }
            }
            return name;
        }
    }
}