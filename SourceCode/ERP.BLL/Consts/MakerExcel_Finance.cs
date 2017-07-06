using ERP.Models.CustomEnums;
using ERP.Models.Finance;
using ERP.Tools;
using ERP.Tools.Logs;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ERP.BLL.ERP.Consts
{
    public class MakerExcel_Finance
    {
        /// <summary>
        /// 生成财务的Excel——工厂往来帐
        /// </summary>
        /// <param name="list_vm"></param>
        /// <returns></returns>
        public static string BuildFinance_Factory(List<VMFinanceProduct> list_vm)
        {
            if (list_vm.Count() <= 0)
            {
                return null;
            }
            try
            {
                // 遍历产品信息生成Excel文件
                string outRelativePath = "/data/Template/Out/Finance/" + CommonCode.GetTimeStamp();//相对路径
                string outRealDir = HttpContext.Current.Server.MapPath("~" + outRelativePath);
                if (!Directory.Exists(outRealDir))
                {
                    Directory.CreateDirectory(outRealDir);
                }

                List<int?> list_FactoryID = new List<int?>();
                foreach (var item in list_vm)
                {
                    if (!list_FactoryID.Contains(item.FactoryID))
                    {
                        list_FactoryID.Add(item.FactoryID);
                    }
                }

                List<string> outFileList = new List<string>();

                string templatePath = HttpContext.Current.Server.MapPath("~/data/Template/Finance_Factory.xls");
                if (!File.Exists(templatePath))
                {
                    throw new Exception("模板文件不存在！" + templatePath);
                }

                foreach (var FactoryID in list_FactoryID)
                {
                    IWorkbook workbook = workbook = new HSSFWorkbook(new FileStream(templatePath, FileMode.Open, FileAccess.Read));
                    HSSFSheet sheetQuoteSheet = (HSSFSheet)workbook.GetSheetAt(0);

                    var font = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Calibri", 12);
                    ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, font, HorizontalAlignment.Left, VerticalAlignment.Center, true);

                    int thisRow = 2;
                    string FactoryAbbreviation = "";
                    string OrderNumber = "";

                    foreach (var item_product in list_vm)
                    {
                        if (item_product.FactoryID == FactoryID)
                        {
                            FactoryAbbreviation = item_product.FactoryAbbreviation;
                            OrderNumber = item_product.OrderNumber;

                            #region 插入数据

                            ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('A'), item_product.OrderNumber, cellStyle);
                            ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('B'), Utils.DateTimeToStr4(item_product.ShippingDate), cellStyle);
                            ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('C'), item_product.CustomerCode, cellStyle);
                            ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('D'), item_product.POID, cellStyle);
                            ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('E'), item_product.FactoryAbbreviation, cellStyle);
                            ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('F'), item_product.PurchaseNumber, cellStyle);
                            ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('G'), Utils.DateTimeToStr4(item_product.FactoryDate), cellStyle);
                            ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('H'), item_product.No, cellStyle);
                            ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('I'), item_product.Name, cellStyle);
                            ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('J'), item_product.CurrencySign + item_product.AllAmount, cellStyle);

                            ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('K'), item_product.SettlementPeriod, cellStyle);
                            ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('L'), item_product.CurrencySign + item_product.InvoicedAmount, cellStyle);
                            ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('M'), Utils.DateTimeToStr4(item_product.PaymentDate), cellStyle);
                            ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('N'), item_product.CurrencySign + item_product.PaymentAmount, cellStyle);
                            ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('O'), item_product.DocumentIndexUpLoadDate, cellStyle);
                            ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('P'), item_product.AllOutContractAmount, cellStyle);
                            ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('Q'), item_product.InspectionAuditFee_ForFactory, cellStyle);
                            ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('R'), item_product.InspectionSamplingFee_ForFactory, cellStyle);
                            ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('S'), item_product.InspectionVerificationFee_ForFactory, cellStyle);
                            ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('T'), item_product.InspectionDetectFee_ForFactory, cellStyle);

                            ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('U'), item_product.RegisterFees, cellStyle);
                            ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('V'), item_product.OtherExpensesDeduction, cellStyle);
                            ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('W'), item_product.AllAmount_Factory, cellStyle);
                            ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('X'), item_product.FactoryFees, cellStyle);
                            ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('Y'), item_product.Comment, cellStyle);
                            ++thisRow;

                            #endregion 插入数据
                        }
                    }

                    ExcelHelper.SaveFile(outRelativePath, outRealDir, workbook, "工厂往来帐 - " + OrderNumber + " - " + FactoryAbbreviation);
                }

                //生成压缩文件
                ExcelHelper.Zip(outRealDir);
                return outRelativePath + ".zip";
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
                throw;
            }
        }

        /// <summary>
        /// 生成财务的Excel——利润分析
        /// </summary>
        /// <param name="list_vm"></param>
        /// <returns></returns>
        public static string BuildFinance_Analysis(List<VMFinanceProduct> list_vm)
        {
            if (list_vm.Count() <= 0)
            {
                return null;
            }
            try
            {
                string outRelativePath = "/data/Template/Out/Finance/" + CommonCode.GetTimeStamp();//相对路径
                string outRealDir = HttpContext.Current.Server.MapPath("~" + outRelativePath);
                if (!Directory.Exists(outRealDir))
                {
                    Directory.CreateDirectory(outRealDir);
                }

                List<string> outFileList = new List<string>();

                string templatePath = HttpContext.Current.Server.MapPath("~/data/Template/Finance_Analysis.xls");
                if (!File.Exists(templatePath))
                {
                    throw new Exception("模板文件不存在！" + templatePath);
                }

                IWorkbook workbook = workbook = new HSSFWorkbook(new FileStream(templatePath, FileMode.Open, FileAccess.Read));
                HSSFSheet sheetQuoteSheet = (HSSFSheet)workbook.GetSheetAt(0);

                var font = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Calibri", 12);
                ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, font, HorizontalAlignment.Left, VerticalAlignment.Center, true);

                int thisRow = 2;

                foreach (var item_product in list_vm)
                {
                    #region 插入数据

                    var PreRecordedStatus = "";
                    if (item_product.PreRecordedStatus != (int)PreRecordedStatusEnum.Empty)
                    {
                        PreRecordedStatus = item_product.PreRecordedStatus == (int)PreRecordedStatusEnum.Yes ? "Y" : "N";
                    }

                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('A'), item_product.OrderNumber, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('B'), item_product.CustomerCode, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('C'), item_product.BuyersAbbreviation, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('D'), item_product.POID, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('E'), item_product.FactoryAbbreviation, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('F'), item_product.PurchaseNumber, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('G'), item_product.No, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('H'), item_product.Name, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('I'), item_product.CurrencySign + item_product.AllAmount, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('J'), item_product.CurrencySign + item_product.InvoicedAmount, cellStyle);

                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('K'), item_product.POPrice.RoundToString(), cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('L'), item_product.POAmount.RoundToString(), cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('M'), item_product.CommissionPercent, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('N'), item_product.CommissionAmount.RoundToString(), cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('O'), item_product.FOBUSDAmount.RoundToString(), cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('P'), item_product.USDExchangeRate, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('Q'), item_product.FOBRMBAmount, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('R'), Utils.DateTimeToStr4(item_product.ClearanceDate), cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('S'), item_product.ClearanceAmount, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('T'), item_product.BankFees, cellStyle);

                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('U'), item_product.InvoiceNo, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('V'), item_product.AllOutContractAmount, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('W'), item_product.InspectionAuditFee_ForFactory, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('X'), item_product.InspectionSamplingFee_ForFactory, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('Y'), item_product.InspectionVerificationFee_ForFactory, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('Z'), item_product.InspectionDetectFee_ForFactory, cellStyle);

                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('A', 'A'), item_product.RegisterFees, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('B', 'A'), item_product.OtherExpensesDeduction, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('C', 'A'), item_product.AllAmount_Factory, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('D', 'A'), item_product.FactoryFees, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('E', 'A'), item_product.PacksDetectFees, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('F', 'A'), item_product.InspectionAuditFee, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('G', 'A'), item_product.InspectionSamplingFee, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('H', 'A'), item_product.InspectionVerificationFee, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('I', 'A'), item_product.InspectionDetectFee, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('J', 'A'), item_product.PortCharges, cellStyle);

                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('K', 'A'), item_product.InternationalCourierFees, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('L', 'A'), item_product.OtherFees, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('M', 'A'), item_product.CompanyManagementRate, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('N', 'A'), item_product.CompanyManagementAmount, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('O', 'A'), item_product.AllAmount_CompanyManagement, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('P', 'A'), item_product.RefundRate, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('Q', 'A'), item_product.RefundAmount, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('R', 'A'), item_product.GrossProfitAmount, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('S', 'A'), item_product.GrossProfitPercent, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('T', 'A'), item_product.Comment, cellStyle);

                    ++thisRow;

                    #endregion 插入数据
                }

                return ExcelHelper.SaveFile(outRelativePath, outRealDir, workbook, "利润分析表");
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
                throw;
            }
        }

        /// <summary>
        /// 生成财务的Excel——自营出口明细表
        /// </summary>
        /// <param name="list_vm"></param>
        /// <returns></returns>
        public static string BuildFianace_SelfExportList(List<VMFinanceProduct> list_vm)
        {
            if (list_vm.Count() <= 0)
            {
                return null;
            }
            try
            {
                string outRelativePath = "/data/Template/Out/Finance/" + CommonCode.GetTimeStamp();//相对路径
                string outRealDir = HttpContext.Current.Server.MapPath("~" + outRelativePath);
                if (!Directory.Exists(outRealDir))
                {
                    Directory.CreateDirectory(outRealDir);
                }

                List<string> outFileList = new List<string>();

                string templatePath = HttpContext.Current.Server.MapPath("~/data/Template/Fianace_SelfExportList.xls");
                if (!File.Exists(templatePath))
                {
                    throw new Exception("模板文件不存在！" + templatePath);
                }

                IWorkbook workbook = workbook = new HSSFWorkbook(new FileStream(templatePath, FileMode.Open, FileAccess.Read));
                HSSFSheet sheetQuoteSheet = (HSSFSheet)workbook.GetSheetAt(0);

                var font = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Calibri", 12);
                ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, font, HorizontalAlignment.Left, VerticalAlignment.Center, true);

                int thisRow = 2;

                foreach (var item_product in list_vm)
                {
                    #region 插入数据

                    var PreRecordedStatus = "";
                    if (item_product.PreRecordedStatus != (int)PreRecordedStatusEnum.Empty)
                    {
                        PreRecordedStatus = item_product.PreRecordedStatus == (int)PreRecordedStatusEnum.Yes ? "Y" : "N";
                    }

                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('A'), item_product.OrderNumber, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('B'), Utils.DateTimeToStr4(item_product.ShippingDate), cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('C'), item_product.CustomerCode, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('D'), item_product.Qty, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('E'), item_product.CurrencySign + item_product.InvoicedAmount, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('F'), Utils.DateTimeToStr4(item_product.CertifiedInvoiceDate), cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('G'), Utils.DateTimeToStr4(item_product.ClearanceDate), cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('H'), item_product.ClearanceAmount, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('I'), item_product.InvoiceNo, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('J'), item_product.CustomsNumber, cellStyle);

                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('K'), item_product.CustomsAmount, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('L'), item_product.HSCode, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('M'), item_product.HSCodeName, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('N'), item_product.PreRecordedStatus, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('O'), item_product.DocumentIndexUpLoadDate, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('P'), item_product.RefundRate, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('Q'), item_product.RefundAmount, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('R'), item_product.Comment, cellStyle);

                    ++thisRow;

                    #endregion 插入数据
                }

                return ExcelHelper.SaveFile(outRelativePath, outRealDir, workbook, "自营出口明细表");
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
                throw;
            }
        }

        /// <summary>
        /// 生成财务的Excel——订单明细一览表
        /// </summary>
        /// <param name="list_vm"></param>
        /// <returns></returns>
        public static string BuildFinance_DetailList(List<VMFinanceProduct> list_vm)
        {
            if (list_vm.Count() <= 0)
            {
                return null;
            }
            try
            {
                string outRelativePath = "/data/Template/Out/Finance/" + CommonCode.GetTimeStamp();//相对路径
                string outRealDir = HttpContext.Current.Server.MapPath("~" + outRelativePath);
                if (!Directory.Exists(outRealDir))
                {
                    Directory.CreateDirectory(outRealDir);
                }

                List<string> outFileList = new List<string>();

                string templatePath = HttpContext.Current.Server.MapPath("~/data/Template/Finance_DetailList.xls");
                if (!File.Exists(templatePath))
                {
                    throw new Exception("模板文件不存在！" + templatePath);
                }

                IWorkbook workbook = workbook = new HSSFWorkbook(new FileStream(templatePath, FileMode.Open, FileAccess.Read));
                HSSFSheet sheetQuoteSheet = (HSSFSheet)workbook.GetSheetAt(0);

                var font = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Calibri", 12);
                ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, font, HorizontalAlignment.Left, VerticalAlignment.Center, true);

                int thisRow = 2;

                foreach (var item_product in list_vm)
                {
                    #region 插入数据

                    var PreRecordedStatus = "";
                    if (item_product.PreRecordedStatus != (int)PreRecordedStatusEnum.Empty)
                    {
                        PreRecordedStatus = item_product.PreRecordedStatus == (int)PreRecordedStatusEnum.Yes ? "Y" : "N";
                    }

                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('A'), item_product.OrderNumber, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('B'), Utils.DateTimeToStr4(item_product.CustomerDate), cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('C'), item_product.OrderDateStart + " ~ " + item_product.OrderDateEnd, cellStyle);//客户要求船期：Shipping Window
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('D'), Utils.DateTimeToStr4(item_product.ShippingDate), cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('E'), item_product.CustomerCode, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('F'), item_product.BuyersAbbreviation, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('G'), item_product.POID, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('H'), item_product.FactoryAbbreviation, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('I'), item_product.PurchaseNumber, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('J'), Utils.DateTimeToStr4(item_product.PurchaseDate), cellStyle);

                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('K'), Utils.DateTimeToStr4(item_product.DateStart), cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('L'), Utils.DateTimeToStr4(item_product.FactoryDate), cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('M'), item_product.SeasonName, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('N'), item_product.No, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('O'), item_product.Name, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('P'), item_product.Qty, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('Q'), item_product.PriceFactory, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('R'), item_product.CurrencySign + item_product.AllAmount, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('S'), item_product.SettlementPeriod, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('T'), item_product.CurrencySign + item_product.InvoicedAmount, cellStyle);

                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('U'), Utils.DateTimeToStr4(item_product.PaymentDate), cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('V'), item_product.CurrencySign + item_product.PaymentAmount, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('W'), Utils.DateTimeToStr4(item_product.CertifiedInvoiceDate), cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('X'), item_product.POPrice.RoundToString(), cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('Y'), item_product.POAmount.RoundToString(), cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('Z'), item_product.CommissionPercent, cellStyle);

                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('A', 'A'), item_product.CommissionAmount.RoundToString(), cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('B', 'A'), item_product.FOBUSDAmount.RoundToString(), cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('C', 'A'), item_product.USDExchangeRate, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('D', 'A'), item_product.FOBRMBAmount, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('E', 'A'), Utils.DateTimeToStr4(item_product.ClearanceDate), cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('F', 'A'), item_product.ClearanceAmount, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('G', 'A'), item_product.BankFees, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('H', 'A'), item_product.InvoiceNo, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('I', 'A'), item_product.CustomsNumber, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('J', 'A'), item_product.CustomsAmount, cellStyle);

                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('K', 'A'), item_product.HSCode, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('L', 'A'), item_product.HSCodeName, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('M', 'A'), PreRecordedStatus, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('N', 'A'), item_product.DocumentIndexUpLoadDate, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('O', 'A'), item_product.VolumeCUFT.RoundToString(), cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('P', 'A'), item_product.AllOutContractAmount, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('Q', 'A'), item_product.InspectionAuditFee_ForFactory, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('R', 'A'), item_product.InspectionSamplingFee_ForFactory, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('S', 'A'), item_product.InspectionVerificationFee_ForFactory, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('T', 'A'), item_product.InspectionDetectFee_ForFactory, cellStyle);

                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('U', 'A'), item_product.RegisterFees, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('V', 'A'), item_product.OtherExpensesDeduction, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('W', 'A'), item_product.AllAmount_Factory, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('X', 'A'), item_product.FactoryFees, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('Y', 'A'), item_product.PacksDetectFees, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('Z', 'A'), item_product.InspectionAuditFee, cellStyle);

                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('A', 'B'), item_product.InspectionSamplingFee, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('B', 'B'), item_product.InspectionVerificationFee, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('C', 'B'), item_product.InspectionDetectFee, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('D', 'B'), item_product.PortCharges, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('E', 'B'), item_product.InternationalCourierFees, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('F', 'B'), item_product.OtherFees, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('G', 'B'), item_product.CompanyManagementRate, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('H', 'B'), item_product.CompanyManagementAmount, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('I', 'B'), item_product.AllAmount_CompanyManagement, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('J', 'B'), item_product.RefundRate, cellStyle);

                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('K', 'B'), item_product.RefundAmount, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('L', 'B'), item_product.GrossProfitAmount, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('M', 'B'), item_product.GrossProfitPercent, cellStyle);
                    ExcelHelper.SetCellValue(sheetQuoteSheet, thisRow, ExcelHelper.GetNumByChar('N', 'B'), item_product.Comment, cellStyle);
                    ++thisRow;

                    #endregion 插入数据
                }

                return ExcelHelper.SaveFile(outRelativePath, outRealDir, workbook, "订单明细一览表");
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
                throw;
            }
        }
    }
}