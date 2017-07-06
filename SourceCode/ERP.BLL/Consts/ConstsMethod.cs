using ERP.BLL.ERP.Dictionary;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.Purchase;
using ERP.Models.ShippingMark;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ERP.BLL.Consts
{
    /// <summary>
    /// BLL中常用的方法
    /// </summary>
    public class ConstsMethod
    {
        /// <summary>
        /// 把Url路径替换为物理路径
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ReplaceURLToLocalPath(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }
            return url.Replace(ConfigurationManager.AppSettings[AdminConsts.RESOURCE_SERVER_CONFIGURATION_VIRTUALPATH], ConfigurationManager.AppSettings[AdminConsts.RESOURCE_SERVER_CONFIGURATION_PHYSICSPATH]);
        }

        /// <summary>
        /// 获取上传的文件列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uploadFileType"></param>
        /// <returns></returns>
        public static List<VMUpLoadFile> GetUploadFileList(int id, UploadFileType uploadFileType)
        {
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = (from p in context.UpLoadFiles
                                 where p.ModuleType == (int)uploadFileType && p.LinkID == id && !p.IsDelete
                                 select new VMUpLoadFile
                                 {
                                     ID = p.ID,
                                     LinkID = p.LinkID,
                                     DT_CREATEDATE = p.DT_CREATEDATE,
                                     DisplayFileName = p.DisplayFileName,
                                     ServerFileName = p.ServerFileName,
                                 }).ToList();
                    return query;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return null;
        }

        /// <summary>
        /// 保存上传的文件
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="id"></param>
        /// <param name="UpLoadFileList"></param>
        /// <param name="context"></param>
        /// <param name="uploadFileType"></param>
        public static void SaveFileUpload(VMERPUser currentUser, int id, List<VMUpLoadFile> UpLoadFileList, ERPEntitiesNew context, UploadFileType uploadFileType)
        {
            if (UpLoadFileList != null)
            {
                int newID = 0;
                List<UpLoadFile> list_UpLoadFile = context.UpLoadFiles.ToList();
                foreach (var item in UpLoadFileList)
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
                                LinkID = id,
                                ModuleType = (short)uploadFileType,
                            });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 是否需要报检
        /// </summary>
        /// <param name="listHS"></param>
        /// <param name="hsID"></param>
        public static bool IsNeedInspection(List<DAL.HarmonizedSystem> listHS, int hsID)
        {
            bool bResult = false;
            var hsInfo = listHS.Where(p => p.ID == hsID && !p.IsDelete).FirstOrDefault();
            if (hsInfo != null)
            {
                foreach (var item in hsInfo.HS_Child)
                {
                    if (item.IsCheck)
                    {
                        bResult = true;
                        break;
                    }
                }
            }

            return bResult;
        }

        /// <summary>
        /// 报检类型
        /// </summary>
        /// <param name="list_HarmonizedSystems"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <param name="query_HarmonizedSystem"></param>
        /// <param name="HSCodeID"></param>
        /// <returns></returns>
        public static string GetHSCodeName(List<DAL.HarmonizedSystem> list_HarmonizedSystems, List<DAL.Com_DataDictionary> list_Com_DataDictionary, DAL.HarmonizedSystem query_HarmonizedSystem, int HSCodeID)
        {
            var query_HS_Child = query_HarmonizedSystem.HS_Child.Where(p => p.HSID == HSCodeID).FirstOrDefault();
            string hsCodeName = string.Empty;

            if (query_HS_Child != null)
            {
                string[] arrTagID = query_HS_Child.TagID.Split(',');
                foreach (string item in arrTagID)
                {
                    int temp = Utils.StrToInt(item, 0);
                    var dbComddInfo = list_Com_DataDictionary.Where(p => p.Code == temp).FirstOrDefault();

                    if (dbComddInfo != null)
                    {
                        string sTagName = dbComddInfo.Name;

                        if (string.IsNullOrEmpty(hsCodeName))
                        {
                            hsCodeName = sTagName;
                        }
                        else
                        {
                            hsCodeName += ";" + sTagName;
                        }
                    }
                }
            }
            return hsCodeName;
        }

        /// <summary>
        /// 用hsid检查是否需要报检
        /// </summary>
        /// <param name="listHS"></param>
        /// <param name="hsID"></param>
        /// <param name="HsName">报关品名</param>
        /// <returns>如果HS被删除，编码、品名返回空值</returns>
        public static bool GetCustomsClearance(List<DAL.HarmonizedSystem> listHS, int hsID, out string HsName, out string HsCode, out string HsEngName)
        {
            HsCode = string.Empty;
            HsName = string.Empty;
            HsEngName = string.Empty;

            bool bResult = false;
            var hsInfo = listHS.Where(p => p.ID == hsID && !p.IsDelete).FirstOrDefault();
            if (hsInfo != null)
            {
                HsCode = hsInfo.HSCode;
                HsName = hsInfo.CodeName;
                HsEngName = hsInfo.CodeEngName;
                foreach (var item1 in hsInfo.HS_Child)
                {
                    if (item1.IsCheck)
                    {
                        bResult = true;
                        break;
                    }
                }
            }

            return bResult;
        }

        /// <summary>
        /// 执行审批流数据写入
        /// </summary>
        /// <param name="iKid"></param>
        /// <param name="bIsPass"></param>
        /// <param name="wfType"></param>
        /// <param name="iLoginUserID"></param>
        /// <param name="listAllowAduitStatus"></param>
        /// <param name="listLogicStatus"></param>
        /// <param name="sAuditIdea"></param>
        public static void InsertAuditStream(int iKid, bool bIsPass, WorkflowTypes wfType, int iLoginUserID, List<int> listAllowAduitStatus, int[] listLogicStatus, string sAuditIdea)
        {
            Workflow.ApprovalInfo objApprovalInfo = new Workflow.ApprovalInfo();
            objApprovalInfo.IdentityID = iKid;
            objApprovalInfo.IsPass = bIsPass;
            objApprovalInfo.WorkflowType = wfType;
            objApprovalInfo.ApproveUserID = iLoginUserID;
            objApprovalInfo.ApproveOpinion = sAuditIdea;
            objApprovalInfo.ValidWaitingApproveStatus = listAllowAduitStatus;//待审批、审批不通过
            objApprovalInfo.StatusApproving = listLogicStatus[0];//待审批
            objApprovalInfo.StatusRejected = listLogicStatus[1];//不通过
            objApprovalInfo.StatusNextTo = listLogicStatus[2];//透过
            objApprovalInfo.LogMethod = () => { return null; };

            Workflow.ApprovalService.ExcuteApproval(objApprovalInfo);
        }

        /// <summary>
        /// 创建ShippingMark
        /// </summary>
        /// <param name="context"></param>
        /// <param name="filesAsolutePathList"></param>
        /// <param name="filesPhysicalPathList"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <param name="item"></param>
        public static VMAjaxProcessResult CreateShippingMark(ERPEntitiesNew context, List<string> filesAsolutePathList, List<string> filesPhysicalPathList, List<Com_DataDictionary> list_Com_DataDictionary, DAL.Delivery_ShipmentOrderProduct item_product, List<string> list_ShippingMark)
        {

            VMAjaxProcessResult result = new VMAjaxProcessResult();

            DictionaryServices _dictionaryServices = new DictionaryServices();

            string AcceptInformation_CompanyName = "";
            string AcceptInformation_StreetAddress = "";
            string AcceptInformation_CustomerReg = "";
            string AcceptInformation_Comment = "";

            int? ShippingMark_AcceptInformationID = item_product.OrderProduct.Purchase_ContractProduct.Where(d => !d.IsDelete).First().Purchase_ContractBatch.Purchase_Contract.ShippingMark_AcceptInformationID;
            var query_Orders_AcceptInformation = context.Orders_AcceptInformation.Find(ShippingMark_AcceptInformationID);
            if (query_Orders_AcceptInformation != null)
            {
                AcceptInformation_CompanyName = query_Orders_AcceptInformation.CompanyName;
                AcceptInformation_StreetAddress = query_Orders_AcceptInformation.StreetAddress;

                AcceptInformation_CustomerReg = query_Orders_AcceptInformation.City + ","
                    + context.Reg_Area.Where(p => p.ARID == query_Orders_AcceptInformation.Province).FirstOrDefault().AreaName + ","
                    + query_Orders_AcceptInformation.PostalCode + ","
                    + context.Reg_Country.Where(p => p.COID == query_Orders_AcceptInformation.Country).FirstOrDefault().CountryName;

                AcceptInformation_Comment = query_Orders_AcceptInformation.Comment;
            }

            var query_ContractProduct = item_product.OrderProduct.Purchase_ContractProduct.Where(d => !d.IsDelete).FirstOrDefault();
            string CurrentSign = _dictionaryServices.GetDictionary_CurrencySign(item_product.OrderProduct.CurrencyType, list_Com_DataDictionary);
            decimal PriceFactory = item_product.OrderProduct.PriceFactory;
            decimal ProductAmount = PriceFactory * item_product.OrderProduct.Qty;
            string PackageName = query_ContractProduct.PackageName;
            if (string.IsNullOrEmpty(PackageName))//TODO 暂时加上
            {
                PackageName = _dictionaryServices.GetDictionary_PackingMannerZhName(item_product.OrderProduct.PackingMannerZhID, list_Com_DataDictionary);
            }
            string SelectCustomer = item_product.OrderProduct.Order.Orders_Customers.SelectCustomer;

            string IsFragile3 = "";
            if (item_product.OrderProduct.Purchase_ContractProduct.First().IsFragile.HasValue)
            {
                IsFragile3 = "否";
                if (item_product.OrderProduct.Purchase_ContractProduct.First().IsFragile.Value)
                {
                    IsFragile3 = "是";
                }
            }

            string SeasonPrefix = "";
            string SeasonSuffix = "";
            string SeasonName = "";
            string SeasonAlias = "";
            string SeasonDepartmentNumber = "";

            var query_TempSeason = list_Com_DataDictionary.Where(d => d.TableKind == (int)DictionaryTableKind.Season && d.Code == item_product.OrderProduct.Season);
            if (query_TempSeason != null && query_TempSeason.Count() > 0)
            {
                SeasonName = query_TempSeason.First().Name;
                SeasonAlias = query_TempSeason.First().Alias;
                SeasonDepartmentNumber = query_TempSeason.First().DepartmentNumber;

                if (!string.IsNullOrEmpty(SeasonName))
                {
                    SeasonPrefix = SeasonName;
                    SeasonSuffix = SeasonAlias;

                    if (!string.IsNullOrEmpty(SeasonAlias))
                    {
                        SeasonName = SeasonName + " " + SeasonAlias;
                    }
                }
            }

            var CarbinetType = (int)PurchaseProduct_CarbinetTypeEnum.StandardContainer;

            var temp_OrderProduct = item_product.OrderProduct;
            if (temp_OrderProduct != null)
            {
                if (temp_OrderProduct.Purchase_ContractProduct.First().CarbinetType.HasValue)
                {
                    CarbinetType = temp_OrderProduct.Purchase_ContractProduct.First().CarbinetType.Value;
                }
            }


            string ProductUPC = "";
            string InnerUPC = "";
            string OuterUPC = "";

            var query_PackProductsUPC = item_product.OrderProduct.Purchase_PackProductsUPC;
            if (query_PackProductsUPC != null && query_PackProductsUPC.Count() > 0)
            {
                ProductUPC = query_PackProductsUPC.First().ProductUPC;
                InnerUPC = query_PackProductsUPC.First().InnerUPC;
                OuterUPC = query_PackProductsUPC.First().OuterUPC;
            }

            var BoxQty = CalculateHelper.GetBoxQty(item_product.OrderProduct.Qty, item_product.OrderProduct.OuterBoxRate);

            var query_purchaseProduct = new VMPurchaseProduct()
            {
                ID = item_product.ID,
                ProductID = item_product.OrderProduct.ProductID,
                Image = item_product.OrderProduct.Image,
                PriceFactory = PriceFactory,
                PriceFactoryFormatter = CurrentSign + PriceFactory,
                CurrentSign = CurrentSign,
                Name = item_product.OrderProduct.Name,
                No = item_product.OrderProduct.No,
                Qty = item_product.OrderProduct.Qty,
                PackageName = PackageName,
                UnitName = _dictionaryServices.GetDictionary_UnitName(item_product.OrderProduct.UnitID, list_Com_DataDictionary),
                ProductAmount = ProductAmount,
                ProductAmountFormatter = CurrentSign + ProductAmount,
                InnerBoxRate = item_product.OrderProduct.InnerBoxRate,
                OuterBoxRate = item_product.OrderProduct.OuterBoxRate,
                PDQPackRate = item_product.OrderProduct.PDQPackRate,
                StyleName = _dictionaryServices.GetDictionary_StyleName(item_product.OrderProduct.StyleID, list_Com_DataDictionary),
                MixedMode = query_ContractProduct.MixedMode,
                OtherComment = query_ContractProduct.OtherComment,
                SkuNumber = item_product.OrderProduct.SkuNumber,
                CustomerID = item_product.OrderProduct.CustomerID,
                CustomerCode = SelectCustomer,
                POID = item_product.OrderProduct.Order.POID,
                OuterWeightGross = item_product.OrderProduct.OuterWeightGross,
                OuterWeightNet = item_product.OrderProduct.OuterWeightNet,
                OuterVolume = item_product.OrderProduct.OuterVolume,
                Desc = item_product.OrderProduct.Desc,
                LengthIN = item_product.OrderProduct.LengthIN,
                WidthIN = item_product.OrderProduct.WidthIN,
                HeightIN = item_product.OrderProduct.HeightIN,
                UPC = item_product.OrderProduct.UPC,
                WeightLBS = item_product.OrderProduct.WeightLBS,
                DestinationPortID = item_product.OrderProduct.Order.DestinationPortID,
                DestinationPortName = _dictionaryServices.GetDictionary_DestinationPortName(item_product.OrderProduct.Order.DestinationPortID, list_Com_DataDictionary),

                Season = item_product.OrderProduct.Season,
                SeasonName = SeasonName,
                SeasonPrefix = SeasonPrefix,
                SeasonSuffix = SeasonSuffix,
                SeasonDepartmentNumber = SeasonDepartmentNumber,

                IsFragile2 = item_product.OrderProduct.Purchase_ContractProduct.First().IsFragile,
                IsFragile3 = IsFragile3,
                OuterLengthIN = item_product.OrderProduct.OuterLengthIN,
                OuterWidthIN = item_product.OrderProduct.OuterWidthIN,
                OuterHeightIN = item_product.OrderProduct.OuterHeightIN,
                OuterWeightGrossLBS = item_product.OrderProduct.OuterWeightGrossLBS,

                DepartmentName = _dictionaryServices.GetDictionaryByAlias(item_product.OrderProduct.Department, list_Com_DataDictionary),
                CarbinetType = CarbinetType,
                ProductUPC = ProductUPC,
                InnerPKUPC = InnerUPC,
                OuterPKUPC = OuterUPC,
                CaseOrderQty = BoxQty,
                InnerGrossWeightLBS = item_product.OrderProduct.InnerWeightLBS,
                InnerNetWeightLBS = item_product.OrderProduct.InnerWeightGrossLBS,
                ColorName = item_product.OrderProduct.ColorName,

                OuterLength = item_product.OrderProduct.OuterLength,
                OuterWidth = item_product.OrderProduct.OuterWidth,
                OuterHeight = item_product.OrderProduct.OuterHeight,

                SkuCode = item_product.OrderProduct.SkuCode,
                UnitEngName = _dictionaryServices.GetDictionaryByAlias(item_product.OrderProduct.UnitID, list_Com_DataDictionary),
                AcceptInformation_StreetAddress = AcceptInformation_StreetAddress,
                AcceptInformation_CustomerReg = AcceptInformation_CustomerReg,
                AcceptInformation_CompanyName = AcceptInformation_CompanyName,
                AcceptInformation_Comment = AcceptInformation_Comment,

                PreTicketed = item_product.OrderProduct.PreTicketed,
                OfStoreReadyInnersEnclosed = item_product.OrderProduct.OfStoreReadyInnersEnclosed,
            };
            int temp = 0;

            switch (SelectCustomer)
            {
                case "DG":
                    temp = 10;
                    break;

                case "F20":
                    temp = 20;
                    break;

                case "S13":
                    temp = 30;
                    break;

                case "S135":
                    temp = 40;
                    break;

                case "S164":
                    temp = 50;
                    break;

                case "S188":
                    temp = 60;
                    break;

                case "S235":
                    temp = 70;
                    break;

                case "S60":
                    temp = 80;
                    break;

                case "S05":
                    temp = 90;
                    break;

                case "S220":
                    temp = 100;
                    break;

                case "S10":
                    temp = 110;
                    break;

                case "S56":
                    temp = 120;
                    break;

                default:
                    break;
            }
            if (temp != 0)
            {
                ShippingMarkEnum shippingMarkEnum = (ShippingMarkEnum)EnumHelper.GetCustomEnumByDesc(typeof(ShippingMarkEnum), temp.ToString());

                result = MakerExcel_ShippingMark.CreateShippingMark(shippingMarkEnum, query_purchaseProduct);
                if (result.IsSuccess)
                {
                    List<VMShippingMark> list_ImageList2 = result.Data as List<VMShippingMark>;
                    foreach (var item in list_ImageList2)
                    {
                        filesAsolutePathList.Add(Utils.GetMapPath(item.ImagePath));
                        filesPhysicalPathList.Add(ConstsMethod.ReplaceURLToLocalPath(Utils.GetMapPath(item.ImagePath)));
                        list_ShippingMark.Add(item.Description);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 把上传的图片、pdf加入到list中
        /// </summary>
        /// <param name="list_PdfFile"></param>
        /// <param name="ServerFileName"></param>
        public static void GetUploadToPdfList(List<string> list_PdfFile, string ServerFileName)
        {
            string extension = Utils.GetFileExt(ServerFileName);
            string extensionName = extension.ToLower();
            string path = ConstsMethod.ReplaceURLToLocalPath(Utils.GetMapPath(ServerFileName));
            if (extensionName == "jpg")
            {
                string newPdfPath = path.Replace("." + extension, ".pdf");
                ITextSharpHelper.JpgToPdf(path, newPdfPath);
                list_PdfFile.Add(newPdfPath);
            }
            else if (extensionName == "png")
            {
                string newPdfPath = path.Replace("." + extension, ".pdf");
                ITextSharpHelper.JpgToPdf(path, newPdfPath);
                list_PdfFile.Add(newPdfPath);
            }
            else if (extensionName == "pdf")
            {
                list_PdfFile.Add(path);
            }
            else if (extensionName == "xls" || extensionName == "xlsx")
            {
                string pdfPath = path.Replace("." + extension, ".pdf");
                string errMsg = "";
                AsposeX asposeX = new Tools.AsposeX();
                //生成pdf文件
                asposeX.ExcelToPdf(path, pdfPath, out errMsg);
                list_PdfFile.Add(pdfPath);
            }
            else if (extensionName == "doc" || extensionName == "docx")
            {
                string pdfPath = path.Replace("." + extension, ".pdf");
                string errMsg = "";
                AsposeX asposeX = new Tools.AsposeX();
                //生成pdf文件
                asposeX.WordToPdf(path, pdfPath, out errMsg);
                list_PdfFile.Add(pdfPath);
            }
        }

        /// <summary>
        /// 深度复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="RealObject"></param>
        /// <returns></returns>
        public static T Copy<T>(T RealObject)
        {
            using (Stream objectStream = new MemoryStream())
            {
                //利用 System.Runtime.Serialization序列化与反序列化完成引用对象的复制     
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, RealObject);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }


    }
}