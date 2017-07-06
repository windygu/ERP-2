using ERP.Models.CustomEnums;
using ERP.Models.Order;
using ERP.Models.Product;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ERP.BLL.ERP.Consts
{
    public class MakerExcel_BuyingConfirmation
    {
        /// <summary>
        /// 根据excel模板生成excel
        /// </summary>
        /// <returns></returns>
        public static string Maker<T>(T model, MakerTypeEnum MakerTypeEnum, int id, string No, string SelectCustomer)
        {
            if (model == null)
            {
                return null;
            }
            List<string> outFileList = new List<string>();
            try
            {
                string templatePath = Utils.GetMapPath("~/data/BuyingConfirmation/" + SelectCustomer + ".xls");
                int type = 1;
                if (SelectCustomer == SelectCustomerEnum.S13.ToString())
                {
                    List<VMOrderProduct> list_OrderProduct = model as List<VMOrderProduct>;

                    if (list_OrderProduct.First().CustomerSeasonPrefix == "Christmas" || list_OrderProduct.First().CustomerSeasonPrefix == "Lawn & Garden")
                    {
                        templatePath = Utils.GetMapPath("~/data/BuyingConfirmation/S13/圣诞 春天花园.xls");
                        type = 1;
                    }
                    else
                    {
                        templatePath = Utils.GetMapPath("~/data/BuyingConfirmation/S13/秋天 鬼节 情人节 复活节 国庆节 爱尔兰节 狂欢节  SC  - 可编辑.xls");
                        type = 2;
                    }
                }
                else if (SelectCustomer == SelectCustomerEnum.S56.ToString())
                {
                    templatePath = Utils.GetMapPath("~/data/BuyingConfirmation/S56/SaleConfirmation.xlsx");
                }

                if (!File.Exists(templatePath))
                {
                    throw new Exception("模板文件不存在！" + templatePath);
                }

                //生成的Excel文件的文件夹
                string outDir = "/data/Template/Out/" + MakerTypeEnum.ToString() + "/" + id;
                string outRealDir = HttpContext.Current.Server.MapPath("~" + outDir);
                if (!Directory.Exists(outRealDir))
                {
                    Directory.CreateDirectory(outRealDir);
                }

                //Excel文件
                string OutExcelName = No + " - BUYING CONFIRMATION";
                if (SelectCustomer == SelectCustomerEnum.S13.ToString() || SelectCustomer == SelectCustomerEnum.S56.ToString())
                {
                    OutExcelName = No + " - SALE CONFIRMATION";
                }

                if (SelectCustomer == SelectCustomerEnum.S56.ToString())
                {
                    IWorkbook newWorkbook = new XSSFWorkbook(new FileStream(templatePath, FileMode.Open, FileAccess.Read));
                    ISheet newSheet = newWorkbook.GetSheetAt(0);
                    Bind_S56(model, newWorkbook, newSheet, MakerTypeEnum);

                    ExcelHelper.SaveFile_PDFAndExcel(outFileList, outRealDir, newWorkbook, OutExcelName, "xlsx");

                    return outDir + "/PDFAndExcel/" + OutExcelName + ".xlsx";
                }

                IWorkbook workbook = new HSSFWorkbook(new FileStream(templatePath, FileMode.Open, FileAccess.Read));

                HSSFSheet sheet = (HSSFSheet)workbook.GetSheetAt(0);
                SelectCustomerEnum customerCodeEnum = (SelectCustomerEnum)EnumHelper.GetCustomEnum(typeof(SelectCustomerEnum), SelectCustomer);

                switch (customerCodeEnum)
                {
                    case SelectCustomerEnum.S188:
                        Bind_S188(model, workbook, sheet, MakerTypeEnum);
                        break;

                    case SelectCustomerEnum.S13:
                        Bind_S13(model, workbook, sheet, MakerTypeEnum, type);
                        break;

                    default:
                        break;
                }

                ExcelHelper.SaveFile_PDFAndExcel(outFileList, outRealDir, workbook, OutExcelName);

                return outDir + "/PDFAndExcel/" + OutExcelName + ".xls";
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
                throw;
            }
        }

        private static void Bind_S188<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
        {
            IGrouping<string, VMOrderProduct> item2 = model as IGrouping<string, VMOrderProduct>;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);
            var bold3 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 10, true);

            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, null, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle3 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold3, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle4 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, null, HorizontalAlignment.Left, VerticalAlignment.Center, true);

            cellStyle4.BorderBottom = BorderStyle.Thin;
            cellStyle4.BorderLeft = BorderStyle.None;
            cellStyle4.BorderRight = BorderStyle.None;
            cellStyle4.BorderTop = BorderStyle.None;
            cellStyle4.WrapText = false;

            ICellStyle cellStyle_boldFont = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont);

            IFont boldFont_IsItalic = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);
            boldFont_IsItalic.IsItalic = true;
            ICellStyle cellStyle_boldFont_IsItalic = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont_IsItalic);

            switch (makerTypeEnum)
            {
                case MakerTypeEnum.Order_BuyingConfirmation:

                    int thisRow = 18;

                    List<string> list_CategoryManager = new List<string>();
                    List<string> list_PortName = new List<string>();
                    List<string> list_PaymentType = new List<string>();
                    DateTime OrderDateStart = DateTime.Now;
                    DateTime OrderDateEnd = DateTime.Now;
                    int sumQty = 0;
                    decimal sumAmount = 0;
                    int? sumBoxQty = 0;

                    string CurrencySign = "";
                    foreach (var item in item2)
                    {
                        if (!list_CategoryManager.Contains(item.CategoryManager) && !string.IsNullOrEmpty(item.CategoryManager))
                        {
                            list_CategoryManager.Add(item.CategoryManager);
                        }

                        if (!list_PortName.Contains(item.PortEngName + ", CHINA") && !string.IsNullOrEmpty(item.PortEngName))
                        {
                            list_PortName.Add(item.PortEngName + ", CHINA");
                        }

                        if (!list_PaymentType.Contains(item.PaymentType) && !string.IsNullOrEmpty(item.PaymentType))
                        {
                            list_PaymentType.Add(item.PaymentType);
                        }

                        OrderDateStart = item.OrderDateStart;
                        OrderDateEnd = item.OrderDateEnd;
                        sumQty += item.Qty;
                        sumAmount += item.SalePrice * item.Qty;
                        int? BoxQty = CalculateHelper.GetBoxQty(item.Qty, item.OuterBoxRate);
                        sumBoxQty += BoxQty;
                        CurrencySign = item.CurrencySign;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow, ExcelHelper.GetNumByChar('D'), ExcelHelper.GetNumByChar('H'));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.POID, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.SkuNumber, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.No, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.Desc, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), "", cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), "", cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), "", cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), "CHINA", cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), item.UPC, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('K'), item.OuterBoxRate, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('L'), item.InnerBoxRate, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('M'), item.OuterVolume, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('N'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('O'), BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('P'), item.CurrencySign + item.SalePrice, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('Q'), item.CurrencySign + item.SalePrice * item.OuterBoxRate, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('R'), item.CurrencySign + item.SalePrice * item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('S'), CommonCode.GetBoolString(item.QualityAssuranceTesting), cellStyle);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow, ExcelHelper.GetNumByChar('D'), ExcelHelper.GetNumByChar('H'));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('K'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('L'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('M'), "TOTAL", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('N'), sumQty, cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('O'), sumBoxQty, cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('P'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('Q'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('R'), CurrencySign + sumAmount, cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('S'), "", cellStyle);

                    ++thisRow;

                    DateTime now = DateTime.Now;
                    string dateString = now.ToString(@"dd-MMM-yy", System.Globalization.CultureInfo.InvariantCulture);

                    ExcelHelper.SetCellValue(sheet, 0, ExcelHelper.GetNumByChar('I'), dateString, cellStyle4);
                    ExcelHelper.SetCellValue(sheet, 1, ExcelHelper.GetNumByChar('I'), CommonCode.ListToString(list_CategoryManager), cellStyle4);

                    thisRow = thisRow + 7;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), CommonCode.ListToString(list_PortName), cellStyle4);

                    thisRow = thisRow + 2;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), CommonCode.ListToString(list_PaymentType), cellStyle4);

                    thisRow = thisRow + 2;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), Utils.DateTimeToStr5(OrderDateStart), cellStyle4);

                    thisRow = thisRow + 2;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), Utils.DateTimeToStr5(OrderDateEnd), cellStyle4);

                    thisRow = thisRow + 2;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), Utils.DateTimeToStr5(OrderDateEnd.AddDays(120)), cellStyle4);//固定上面的END日期+120天

                    thisRow = thisRow + 7;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('D'), thisRow, ExcelHelper.GetNumByChar('F'), thisRow + 8);

                    break;

                default:
                    break;
            }
        }

        private static void Bind_S13<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum, int type)
        {
            List<VMOrderProduct> item2 = model as List<VMOrderProduct>;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 12);
            var bold3 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 10, true);
            var bold4 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 11, false);
            var bold5 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 8, true);
            var bold6 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 11, true);
            var bold9 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Garamond", 8, false);

            ICellStyle cellStyle4 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold4, HorizontalAlignment.Center, VerticalAlignment.Center, false);
            cellStyle4.WrapText = false;

            ICellStyle cellStyle13 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold4, HorizontalAlignment.Center, VerticalAlignment.Center, false);

            ICellStyle cellStyle3 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold5, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            ICellStyle cellStyle5 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold5, HorizontalAlignment.Right, VerticalAlignment.Center, false);

            ICellStyle cellStyle6 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold6, HorizontalAlignment.Right, VerticalAlignment.Center, false);

            ICellStyle cellStyle7 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold4, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            ICellStyle cellStyle8 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold3, HorizontalAlignment.Right, VerticalAlignment.Center, false);
            ICellStyle cellStyle2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold4, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle2.BorderTop = BorderStyle.Double;

            switch (makerTypeEnum)
            {
                case MakerTypeEnum.Order_BuyingConfirmation:

                    int thisRow = 15;

                    var item3 = item2.First();

                    if (type == 1)//圣诞 春天花园
                    {
                        #region 圣诞 春天花园

                        ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('C'), item3.POID.StrToUpper());

                        ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('F'), item3.AssistantName.StrToUpper());
                        ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('F'), item3.AssistantTel.StrToUpper());
                        ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('F'), item3.AssistantEmail.StrToUpper());
                        ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('G'), item3.BuyerName.StrToUpper());

                        decimal? OuterVolume = 0;
                        decimal? SalePrice = 0;
                        foreach (var item in item2)
                        {
                            OuterVolume += item.OuterVolume * item.BoxQty;
                            SalePrice += item.SalePrice * item.Qty;

                            ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                            ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                            ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                            ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                            ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                            ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                            ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                            ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                            ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow + 2, ExcelHelper.GetNumByChar('C'), ExcelHelper.GetNumByChar('C'));
                            ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow + 2, ExcelHelper.GetNumByChar('D'), ExcelHelper.GetNumByChar('D'));
                            ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow + 4, ExcelHelper.GetNumByChar('G'), ExcelHelper.GetNumByChar('G'));
                            ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow + 4, ExcelHelper.GetNumByChar('H'), ExcelHelper.GetNumByChar('H'));

                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "HL#:", cellStyle6);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.SkuNumber.StrToUpper(), cellStyle4);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.Desc.StrToUpper(), cellStyle13);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.Qty, cellStyle4);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "INR/INR:", cellStyle5);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.INR, cellStyle4);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), Keys.USD_Sign + item.SalePrice, cellStyle4);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), Keys.USD_Sign + item.RetailPrice, cellStyle4);

                            ++thisRow;

                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "FTY#:", cellStyle6);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.No.StrToUpper(), cellStyle4);

                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "INR:", cellStyle5);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.InnerBoxRate, cellStyle4);

                            ++thisRow;

                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "CASE:", cellStyle5);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.OuterBoxRate, cellStyle4);

                            ++thisRow;

                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "COLOR:", cellStyle5);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.ColorName.StrToUpper(), cellStyle4);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "CUBE FT.:", cellStyle5);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.OuterVolume, cellStyle4);

                            ++thisRow;

                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "MATERIAL CONTENT (%):", cellStyle5);

                            List<string> list_IngredientZh = new List<string>();
                            if (item.list_ProductIngredient != null)
                            {
                                List<VMProductIngredients> temp = item.list_ProductIngredient.OrderByDescending(d => d.IngredientPercent).ToList();
                                for (int i = 0; i < temp.Count(); i++)
                                {
                                    var item4 = temp[i];

                                    string Percent = "";
                                    if (item4.IngredientPercent > 0)
                                    {
                                        Percent = item4.IngredientPercent + "% ";
                                    }

                                    string IngredientName = item4.IngredientName;
                                    if (!string.IsNullOrEmpty(item4.IngredientName))
                                    {
                                        IngredientName = item4.IngredientName.Trim().StrToUpper();
                                    }
                                    list_IngredientZh.Add(Percent + IngredientName);
                                }
                            }

                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), CommonCode.ListToString(list_IngredientZh), cellStyle4);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "GW/NW:", cellStyle5);
                            if (item.OuterWeightGrossLBS.HasValue && item.OuterWeightNetLBS.HasValue)
                            {
                                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.OuterWeightGrossLBS + "/" + item.OuterWeightNetLBS + "LBS", cellStyle4);
                            }

                            ++thisRow;

                            ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow, ExcelHelper.GetNumByChar('C'), ExcelHelper.GetNumByChar('D'));
                            ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow, ExcelHelper.GetNumByChar('E'), ExcelHelper.GetNumByChar('H'));

                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "COMMODITY INFORMATION:", cellStyle5);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "CASE DIMENSIONS : " + item.OuterLengthIN + "X" + item.OuterWidthIN + "X" + item.OuterHeightIN + "IN", cellStyle3);

                            ++thisRow;

                            ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow, ExcelHelper.GetNumByChar('A'), ExcelHelper.GetNumByChar('B'));
                            ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow, ExcelHelper.GetNumByChar('C'), ExcelHelper.GetNumByChar('H'));

                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TESTING/DOCUMENT REQUIREMENT:	", cellStyle5);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.TestingStandardsFilename.StrToUpper(), cellStyle7);

                            ++thisRow;
                            ++thisRow;
                        }

                        thisRow += 2;

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), OuterVolume, cellStyle4);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), "", cellStyle4);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), Keys.USD_Sign + SalePrice, cellStyle4);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), "", cellStyle4);

                        List<int?> list_FactoryID = new List<int?>();

                        ++thisRow;

                        foreach (var item in item2)
                        {
                            if (!list_FactoryID.Contains(item.FactoryID))
                            {
                                list_FactoryID.Add(item.FactoryID);

                                ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                                ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "MANUFACTURER:", cellStyle8);
                                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.Factory_EnglishName.StrToUpper(), cellStyle7);

                                ++thisRow;
                                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "ADDRESS:", cellStyle8);
                                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.Factory_EnglishAddress.StrToUpper(), cellStyle7);

                                ++thisRow;
                            }
                        }

                        ++thisRow;

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "FOB " + item3.PortEngName.StrToUpper(), cellStyle7);
                        ++thisRow;

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), CommonCode.GetDateTime5(item3.OrderDateStart) + " / " + CommonCode.GetDateTime3(item3.OrderDateEnd), cellStyle7);
                        ++thisRow;

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "T/T AT SIGHT", cellStyle7);//item3.PaymentType

                        ExcelHelper.CreatePicture_SaleContract2(workbook, sheet, ExcelHelper.GetNumByChar('E'), thisRow, ExcelHelper.GetNumByChar('F'), thisRow + 7);

                        thisRow += 8;
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item3.BuyerName.StrToUpper() + "  , BUYER", cellStyle2);

                        #endregion 圣诞 春天花园
                    }
                    else
                    {
                        #region 秋天 鬼节 情人节 复活节 国庆节 爱尔兰节 狂欢节

                        ICellStyle cellStyle9 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold9, HorizontalAlignment.Center, VerticalAlignment.Center, true);
                        ICellStyle cellStyle10 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold9, HorizontalAlignment.Left, VerticalAlignment.Center, false);
                        ICellStyle cellStyle11 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold9, HorizontalAlignment.Left, VerticalAlignment.Center, true);
                        ICellStyle cellStyle12 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold9, HorizontalAlignment.Left, VerticalAlignment.Center, true);
                        cellStyle12.WrapText = false;

                        cellStyle10.BorderBottom = BorderStyle.None;
                        cellStyle10.BorderLeft = BorderStyle.None;
                        cellStyle10.BorderRight = BorderStyle.None;
                        cellStyle10.BorderTop = BorderStyle.None;

                        ExcelHelper.SetCellValue(sheet, 4, ExcelHelper.GetNumByChar('B'), item3.POID.StrToUpper(), cellStyle10);
                        ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('B'), item3.BuyerName.StrToUpper(), cellStyle10);
                        ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('B'), item3.BuyerEmail.StrToUpper());

                        ExcelHelper.SetCellValue(sheet, 21, ExcelHelper.GetNumByChar('A'), "T/T AT SIGHT", cellStyle9);//item3.PaymentType
                        ExcelHelper.SetCellValue(sheet, 21, ExcelHelper.GetNumByChar('E'), Utils.DateTimeToStr(item3.OrderDateStart), cellStyle9);

                        ExcelHelper.SetCellValue(sheet, 23, ExcelHelper.GetNumByChar('A'), item3.TransportTypeName.StrToUpper(), cellStyle9);
                        ExcelHelper.SetCellValue(sheet, 23, ExcelHelper.GetNumByChar('C'), item3.PortEngName.StrToUpper(), cellStyle9);
                        ExcelHelper.SetCellValue(sheet, 23, ExcelHelper.GetNumByChar('E'), Utils.DateTimeToStr(item3.OrderDateEnd), cellStyle9);

                        ExcelHelper.SetCellValue(sheet, 44, ExcelHelper.GetNumByChar('D'), Utils.DateTimeToStr4(DateTime.Now), cellStyle10);

                        thisRow = 49;

                        decimal? OuterVolume = 0;
                        decimal? SalePrice = 0;
                        int index = 0;
                        foreach (var item in item2)
                        {
                            index++;
                            bool isShow = false;

                            OuterVolume += item.OuterVolume * item.BoxQty;
                            SalePrice += item.SalePrice * item.Qty;
                            if (isShow)//暂时不填值
                            {
                                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "HL#:", cellStyle11);
                                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "INR/INR:", cellStyle11);
                            }
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.SkuNumber.StrToUpper(), cellStyle11);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.Qty, cellStyle11);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.INR, cellStyle11);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), Keys.USD_Sign + item.SalePrice, cellStyle11);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), Keys.USD_Sign + item.RetailPrice, cellStyle11);

                            ++thisRow;
                            if (isShow)
                            {
                                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "Vendor #:", cellStyle11);
                                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "INR:", cellStyle11);
                            }
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.No.StrToUpper(), cellStyle11);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.InnerBoxRate, cellStyle11);

                            ++thisRow;
                            if (isShow)
                            {
                                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "Description:", cellStyle11);
                            }
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.Desc.StrToUpper(), cellStyle11);

                            ++thisRow;

                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.LengthIN + "X" + item.WidthIN + "X" + item.HeightIN + " " + item.ColorName, cellStyle11);

                            if (isShow)
                            {
                                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "Case Pack:", cellStyle11);
                            }
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.OuterBoxRate, cellStyle11);

                            ++thisRow;
                            if (isShow)
                            {
                                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "Material content", cellStyle11);
                                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "Cubic Feet:", cellStyle11);
                            }

                            List<string> list_IngredientZh = new List<string>();
                            if (item.list_ProductIngredient != null)
                            {
                                List<VMProductIngredients> temp = item.list_ProductIngredient.OrderByDescending(d => d.IngredientPercent).ToList();
                                for (int i = 0; i < temp.Count(); i++)
                                {
                                    var item4 = temp[i];

                                    string Percent = "";
                                    if (item4.IngredientPercent > 0)
                                    {
                                        Percent = item4.IngredientPercent + "% ";
                                    }

                                    string IngredientName = item4.IngredientName;
                                    if (!string.IsNullOrEmpty(item4.IngredientName))
                                    {
                                        IngredientName = item4.IngredientName.Trim().StrToUpper();
                                    }
                                    list_IngredientZh.Add(Percent + IngredientName);
                                }
                            }

                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), CommonCode.ListToString(list_IngredientZh), cellStyle11);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.OuterVolume, cellStyle11);

                            ++thisRow;
                            if (isShow)
                            {
                                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "Breadown (%) :", cellStyle11);
                            }

                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.Remarks.StrToUpper(), cellStyle12);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "CASE DIMENSIONS / WEIGHT':" + item.OuterLengthIN + "X" + item.OuterWidthIN + "X" + item.OuterHeightIN + "in / " + item.OuterWeightGrossLBS + "lbs", cellStyle12);

                            ++thisRow;
                            ++thisRow;
                        }

                        ExcelHelper.SetCellValue(sheet, 29, ExcelHelper.GetNumByChar('F'), OuterVolume.Round(4), cellStyle10);
                        ExcelHelper.SetCellValue(sheet, 29, ExcelHelper.GetNumByChar('G'), "", cellStyle10);
                        ExcelHelper.SetCellValue(sheet, 30, ExcelHelper.GetNumByChar('F'), Keys.USD_Sign + SalePrice.Round(2), cellStyle10);
                        ExcelHelper.SetCellValue(sheet, 30, ExcelHelper.GetNumByChar('G'), "", cellStyle10);

                        #endregion 秋天 鬼节 情人节 复活节 国庆节 爱尔兰节 狂欢节
                    }

                    break;

                default:
                    break;
            }
        }

        private static void Bind_S56<T>(T model, IWorkbook workbook, ISheet sheet, MakerTypeEnum makerTypeEnum)
        {
            List<VMOrderProduct> item2 = model as List<VMOrderProduct>;

            var font_11 = ExcelHelper.GetFontStyle((XSSFWorkbook)workbook, "Times New Roman", 11, false);
            var font_11_bold = ExcelHelper.GetFontStyle((XSSFWorkbook)workbook, "Times New Roman", 11, true);

            ICellStyle cellStyle = ExcelHelper.GetCellStyle((XSSFWorkbook)workbook, font_11, HorizontalAlignment.Center, VerticalAlignment.Center, true);

            ICellStyle cellStyle_11_bold_center_border = ExcelHelper.GetCellStyle((XSSFWorkbook)workbook, font_11_bold, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle_11_center_border = ExcelHelper.GetCellStyle((XSSFWorkbook)workbook, font_11, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle_11_bold_center = ExcelHelper.GetCellStyle((XSSFWorkbook)workbook, font_11_bold, HorizontalAlignment.Center, VerticalAlignment.Center, false);

            ICellStyle cellStyle_11_bold_left_border_WrapText = ExcelHelper.GetCellStyle((XSSFWorkbook)workbook, font_11_bold, HorizontalAlignment.Left, VerticalAlignment.Center, true);

            ICellStyle cellStyle_11_bold_left_border = ExcelHelper.GetCellStyle((XSSFWorkbook)workbook, font_11_bold, HorizontalAlignment.Left, VerticalAlignment.Center, true);
            cellStyle_11_bold_left_border.WrapText = false;

            ICellStyle cellStyle_11_left = ExcelHelper.GetCellStyle((XSSFWorkbook)workbook, font_11, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle_11_left.WrapText = false;

            ICellStyle cellStyle_11_bold_left = ExcelHelper.GetCellStyle((XSSFWorkbook)workbook, font_11_bold, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle_11_bold_left.WrapText = false;

            ICellStyle cellStyle_11_left_border = ExcelHelper.GetCellStyle((XSSFWorkbook)workbook, font_11, HorizontalAlignment.Left, VerticalAlignment.Center, true);

            ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('G'), "AC Moore PO#" + item2.First().POID, cellStyle_11_left);
            ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('G'), "Ship by Date:" + CommonCode.GetDateTime3(item2.First().OrderDateStart), cellStyle_11_left);
            ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('G'), "Loading Port: " + item2.First().PortEngName + ",China", cellStyle_11_left);

            int thisRow = 14;

            int Qty = 0;
            decimal? SumSalePrice = 0;
            decimal? SumOuterVolume = 0;

            foreach (var item in item2)
            {
                Qty += item.Qty;
                SumSalePrice += item.SumSalePrice;

                SumOuterVolume += item.SumOuterVolume;

                ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.No, cellStyle_11_center_border);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.SkuNumber, cellStyle_11_center_border);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.SkuCode, cellStyle_11_center_border);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), Keys.USD_Sign + item.RetailPrice, cellStyle_11_center_border);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.HSCode, cellStyle_11_center_border);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.Desc, cellStyle_11_center_border);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), item.InnerBoxRate, cellStyle_11_center_border);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), item.OuterBoxRate, cellStyle_11_center_border);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), item.OuterVolume, cellStyle_11_center_border);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), item.SumOuterVolume, cellStyle_11_center_border);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('K'), Keys.USD_Sign + Math.Round(item.SalePrice, 2), cellStyle_11_center_border);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('L'), item.Qty, cellStyle_11_center_border);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('M'), Keys.USD_Sign + Math.Round(item.SumSalePrice, 2), cellStyle_11_center_border);

                ++thisRow;

                ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                string str = "";
                string str2 = "";
                if (!string.IsNullOrEmpty(item.StyleNumber) && item.StyleNumber != "1")
                {
                    str2 = " x each design";
                }
                if (item.InnerBoxRate.HasValue && item.InnerBoxRate.Value > 0)
                {
                    str += "Inner Case: " + item.InnerBoxRate + str2 + ".";
                }

                str += "Master Case:" + item.OuterBoxRate / Utils.StrToInt(item.StyleNumber, 1) + str2;

                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), str, cellStyle_11_bold_left_border);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle_11_bold_left_border);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle_11_bold_left_border);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle_11_bold_left_border);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle_11_bold_left_border);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), "", cellStyle_11_bold_left_border);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), "", cellStyle_11_bold_left_border);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), "", cellStyle_11_bold_left_border);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), "", cellStyle_11_bold_left_border);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), "", cellStyle_11_bold_left_border);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('K'), "", cellStyle_11_bold_left_border);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('L'), "", cellStyle_11_bold_left_border);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('M'), "", cellStyle_11_bold_left_border);

                ++thisRow;
            }

            ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "", cellStyle_11_bold_center);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle_11_bold_center);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle_11_bold_center);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle_11_bold_center);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle_11_bold_center);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), "Total:", cellStyle_11_bold_center);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), "", cellStyle_11_bold_center);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), "", cellStyle_11_bold_center);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), "", cellStyle_11_bold_center);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), SumOuterVolume, cellStyle_11_bold_center);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('K'), "", cellStyle_11_bold_center);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('L'), Qty, cellStyle_11_bold_center);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('M'), Keys.USD_Sign + SumSalePrice.Round(2), cellStyle_11_bold_center);

            thisRow += 10;
            ExcelHelper.CreatePicture(workbook, sheet, "/data/BuyingConfirmation/S56/tom.png", 75, 0, 700, 0, ExcelHelper.GetNumByChar('G'), thisRow, ExcelHelper.GetNumByChar('L'), thisRow + 11);
        }
    }
}