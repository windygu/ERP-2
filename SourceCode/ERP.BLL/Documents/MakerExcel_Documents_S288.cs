using ERP.BLL.ERP.Consts;
using ERP.Models.CustomEnums;
using ERP.Models.Factory;
using ERP.Models.InspectionClearance;
using ERP.Models.InspectionCustoms;
using ERP.Models.InspectionExchange;
using ERP.Models.InspectionReceipt;
using ERP.Tools;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.Documents
{
    /// <summary>
    /// S288客人的清关的发票不需要分开，结汇的发票根据PO号分开。清关和结汇的装箱单根据柜子分开。
    /// </summary>
    public class MakerExcel_Documents_S288
    {
        /// <summary>
        /// S288的报检
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionReceipt_S288<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
        {
            DTOInspectionReceipt vm = model as DTOInspectionReceipt;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);
            var boldFont3 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12, false);
            var boldFont2 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 11);
            var boldFont4 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 11, false);

            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3, HorizontalAlignment.Center, VerticalAlignment.Center, true);//有边框不加粗 罗马字体
            ICellStyle cellStyle2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, true);//有边框有加粗 罗马

            ICellStyle cellStyle3 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont4, HorizontalAlignment.Center, VerticalAlignment.Center, true);//有边框 不加粗
            ICellStyle cellStyle4 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Center, VerticalAlignment.Center, true);//有边框 有加粗

            ICellStyle cellStyle_boldFont = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont);//字体加粗 罗马
            ICellStyle cellStyle_boldFont2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2);//字体加粗 Arial

            ICellStyle cellStyle5 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3, HorizontalAlignment.Center, VerticalAlignment.Center, false);//无边框不加粗

            IFont boldFont_IsItalic = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);
            boldFont_IsItalic.IsItalic = true;
            ICellStyle cellStyle_boldFont_IsItalic = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont_IsItalic);
            int thisRow = 0;

            switch (makerTypeEnum)
            {
                case MakerTypeEnum.InspectionReceipt_Home:

                    #region 报检——报检首页

                    ExcelHelper.SetCellValue(sheet, 0, ExcelHelper.GetNumByChar('B'), vm.FactoryName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 1, ExcelHelper.GetNumByChar('B'), vm.FactoryContact, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 2, ExcelHelper.GetNumByChar('B'), vm.PurchaseNumber, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 3, ExcelHelper.GetNumByChar('B'), vm.HsName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 4, ExcelHelper.GetNumByChar('B'), vm.HSCodeName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 5, ExcelHelper.GetNumByChar('B'), vm.SCNO, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('B'), vm.ProductsQuauity, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('B'), vm.ProductsBoxNum, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('B'), Keys.USD_Sign + vm.ProductAmount, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), "由", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('B'), vm.PortName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('C'), "海运到", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('D'), "美国", cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('B'), vm.INTypeName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('D'), vm.ClaimFaxDate, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('A'), "或出境货物报检单。", cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 14, ExcelHelper.GetNumByChar('F'), vm.CreateUserName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 15, ExcelHelper.GetNumByChar('F'), vm.CreateDateForamtter, cellStyle_boldFont);

                    #endregion 报检——报检首页

                    break;

                case MakerTypeEnum.InspectionReceipt_PackingList:

                    #region 报检——装箱单

                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), " TO: " + vm.CustomerName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('E'), vm.InvNo, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('E'), vm.POID, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('E'), CommonCode.GetDateTime3(vm.CreateDate), cellStyle5);

                    thisRow = 14;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), vm.HsEngName, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), vm.ProductsBoxNum, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), vm.ProductsQuauity, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), vm.WeightGrossSum, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), vm.WeightNetSum, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), vm.CUFT, cellStyle);

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), vm.ProductsBoxNum, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), vm.ProductsQuauity, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), vm.WeightGrossSum, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), vm.WeightNetSum, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), vm.CUFT, cellStyle2);

                    #endregion 报检——装箱单

                    break;

                case MakerTypeEnum.InspectionReceipt_CommercialInvoice:

                    #region 报检——发票

                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('D'), vm.InvNo, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('D'), vm.POID, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('D'), CommonCode.GetDateTime3(vm.CreateDate), cellStyle5);

                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('A'), "Loading port: " + vm.INPortName + ", CHINA", cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('A'), "Destination: " + vm.INEndPortName, cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, 14, ExcelHelper.GetNumByChar('D'), vm.TradeTypeName + " " + vm.INPortName, cellStyle_boldFont2);

                    thisRow = 17;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), vm.HsEngName, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), vm.ProductsQuauity, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), Keys.USD_Sign + vm.ProductPrice, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), Keys.USD_Sign + vm.ProductAmount, cellStyle);

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "Total:", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), vm.ProductsQuauity, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), Keys.USD_Sign + vm.ProductAmount, cellStyle2);

                    #endregion 报检——发票

                    break;

                case MakerTypeEnum.InspectionReceipt_SaleContract:

                    #region 报检——销售合同

                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('D'), vm.SCNO, cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('D'), CommonCode.GetDateTime3(vm.CustomerDate), cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 16, ExcelHelper.GetNumByChar('D'), vm.TradeTypeName + " " + vm.INPortName, cellStyle_boldFont2);

                    ExcelHelper.SetCellValue(sheet, 19, ExcelHelper.GetNumByChar('A'), vm.HsEngName, cellStyle);
                    ExcelHelper.SetCellValue(sheet, 19, ExcelHelper.GetNumByChar('B'), vm.ProductsQuauity, cellStyle);
                    ExcelHelper.SetCellValue(sheet, 19, ExcelHelper.GetNumByChar('C'), Keys.USD_Sign + vm.ProductPrice, cellStyle);
                    ExcelHelper.SetCellValue(sheet, 19, ExcelHelper.GetNumByChar('D'), Keys.USD_Sign + vm.ProductAmount, cellStyle);
                    ExcelHelper.SetCellValue(sheet, 20, ExcelHelper.GetNumByChar('A'), "TOTAL", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, 20, ExcelHelper.GetNumByChar('C'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, 20, ExcelHelper.GetNumByChar('B'), vm.ProductsQuauity, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, 20, ExcelHelper.GetNumByChar('D'), Keys.USD_Sign + vm.ProductAmount, cellStyle2);

                    ExcelHelper.SetCellValue(sheet, 23, ExcelHelper.GetNumByChar('A'), "1.SHIPMENT DATE:" + vm.OrderDateStart, cellStyle_boldFont2);//TODO:核算单中的shippingwindow的起始日期
                    ExcelHelper.SetCellValue(sheet, 24, ExcelHelper.GetNumByChar('A'), "2.SHIPPING MARKS:" + "TO BE ADVISED BY THE BUYER", cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, 25, ExcelHelper.GetNumByChar('A'), "3.TERM OF THE PAYMENT:" + vm.TeamOfThePayment, cellStyle_boldFont2);

                    #endregion 报检——销售合同

                    break;

                case MakerTypeEnum.InspectionReceipt_Commission:
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// S288的报关
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionCustoms_S288<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
        {
            VMInspectionCustoms vm = model as VMInspectionCustoms;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);
            var boldFont3 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12, false);
            var boldFont2 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 11);
            var boldFont4 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 11, false);

            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3, HorizontalAlignment.Center, VerticalAlignment.Center, true);//有边框不加粗 罗马字体
            ICellStyle cellStyle2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, true);//有边框有加粗 罗马

            ICellStyle cellStyle3 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont4, HorizontalAlignment.Center, VerticalAlignment.Center, true);//有边框 不加粗
            ICellStyle cellStyle4 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Center, VerticalAlignment.Center, true);//有边框 有加粗

            ICellStyle cellStyle5 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3, HorizontalAlignment.Left, VerticalAlignment.Center, false);//无边框不加粗

            ICellStyle cellStyle_boldFont = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont);//字体加粗 罗马

            ICellStyle cellStyle_boldFont2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2);//字体加粗 Arial

            IFont boldFont_IsItalic = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);
            boldFont_IsItalic.IsItalic = true;
            ICellStyle cellStyle_boldFont_IsItalic = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont_IsItalic);
            int thisRow = 0;

            switch (makerTypeEnum)
            {
                #region 报关

                case MakerTypeEnum.InspectionCustoms_PackingList:

                    #region 报关——装箱单

                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('E'), vm.InvoiceNO, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('E'), vm.POID, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('E'), CommonCode.GetDateTime3(vm.CreateDate), cellStyle5);

                    thisRow = 14;

                    int ProductsBoxNum = 0;//总产品箱数
                    int Qty = 0;//总数量
                    decimal SumOuterWeightGross = 0;//总毛重
                    decimal SumOuterWeightNet = 0;//总净重
                    decimal? SumOuterVolume = 0;//总体积
                    foreach (var item in vm.list_OrdersProducts)
                    {
                        ProductsBoxNum += item.ProductsBoxNum;
                        Qty += item.Qty;
                        SumOuterWeightGross += item.SumOuterWeightGross;
                        SumOuterWeightNet += item.SumOuterWeightNet;
                        SumOuterVolume += item.SumOuterVolume;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.HsEngName, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.ProductsBoxNum, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.SumOuterWeightGross, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.SumOuterWeightNet, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.SumOuterVolume, cellStyle);
                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), ProductsBoxNum, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), Qty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), SumOuterWeightGross, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), SumOuterWeightNet, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), SumOuterVolume, cellStyle2);

                    thisRow += 6;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('C'), thisRow, ExcelHelper.GetNumByChar('D'), thisRow + 6);

                    #endregion 报关——装箱单

                    break;

                case MakerTypeEnum.InspectionCustoms_CommercialInvoice:

                    #region 报关——发票

                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('D'), vm.InvoiceNO, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('D'), vm.POID, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('D'), CommonCode.GetDateTime3(vm.CreateDate), cellStyle5);

                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('A'), "Loading port:   " + vm.PortName + " ,CHINA", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('A'), "Destination: " + vm.DestinationPortName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 14, ExcelHelper.GetNumByChar('D'), vm.TradeTypeName + " " + vm.PortName, cellStyle_boldFont);

                    thisRow = 17;
                    ProductsBoxNum = 0;
                    Qty = 0;
                    decimal Amount = 0;
                    foreach (var item in vm.list_OrdersProducts)
                    {
                        ProductsBoxNum += item.ProductsBoxNum;
                        Qty += item.Qty;
                        Amount += item.Amount;
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.HsEngName, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), Keys.USD_Sign + item.ProductPrice, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), Keys.USD_Sign + item.Amount, cellStyle);
                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), Qty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), Keys.USD_Sign + Amount, cellStyle2);

                    #endregion 报关——发票

                    thisRow += 6;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('C'), thisRow, ExcelHelper.GetNumByChar('D'), thisRow + 6);
                    break;

                case MakerTypeEnum.InspectionCustoms_SaleContract:

                    #region 报关——销售合同

                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('D'), vm.SCNo, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('D'), CommonCode.GetDateTime3(vm.CustomerDate), cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 16, ExcelHelper.GetNumByChar('D'), vm.TradeTypeName + " " + vm.PortName, cellStyle_boldFont);

                    thisRow = 19;
                    ProductsBoxNum = 0;
                    Qty = 0;
                    Amount = 0;
                    foreach (var item in vm.list_OrdersProducts)
                    {
                        ProductsBoxNum += item.ProductsBoxNum;
                        Qty += item.Qty;
                        Amount += item.Amount;
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.HsEngName, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), Keys.USD_Sign + item.ProductPrice, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), Keys.USD_Sign + item.Amount, cellStyle);
                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), Qty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), Keys.USD_Sign + Amount, cellStyle2);
                    thisRow += 2;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "REMARKS:", cellStyle_boldFont);
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "1.SHIPPING DATE:" + vm.OrderDateStart, cellStyle_boldFont);//TODO:x销售合同shipping wundow的起始日期
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "2.SHIPPING MARKS:  TO BE ADVISED", cellStyle_boldFont);
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "3.TERM OF THE PAYMENT : " + vm.TeamOfThePayment, cellStyle_boldFont);

                    thisRow += 2;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "THE  SELLERS :", cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "THE BUYERS :", cellStyle_boldFont2);

                    ExcelHelper.CreatePicture(workbook, sheet, "/data/Documents/S188/tom.png", 75, 0, 800, 0, ExcelHelper.GetNumByChar('A'), thisRow + 1, ExcelHelper.GetNumByChar('A'), thisRow + 4);

                    ExcelHelper.CreatePicture(workbook, sheet, "/data/Documents/S288/Lee.png", 75, 0, 800, 0, ExcelHelper.GetNumByChar('D'), thisRow + 1, ExcelHelper.GetNumByChar('D'), thisRow + 6);
                    thisRow += 2;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('B'), thisRow, ExcelHelper.GetNumByChar('C'), thisRow + 6);

                    #endregion 报关——销售合同

                    break;

                #endregion 报关

                default:
                    break;
            }
        }

        /// <summary>
        /// S288的清关
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionClearance_S288<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
        {
            VMInspectionClearance vm = model as VMInspectionClearance;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);
            var boldFont2 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 10, false);//不加粗
            var boldFont3 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 10);//加粗

            var boldFont4 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12, false);

            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle_boldFont = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3);

            var bold3 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 10, false);
            ICellStyle cellStyle3 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold3);

            ICellStyle cellStyle4 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold3, HorizontalAlignment.Right, VerticalAlignment.Bottom, false);

            IFont boldFont_IsItalic = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);
            boldFont_IsItalic.IsItalic = true;
            ICellStyle cellStyle_boldFont_IsItalic = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont_IsItalic);

            ICellStyle cellStyle5 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Left, VerticalAlignment.Center, false);//字体不加粗，没有边框，左对齐，换行

            ICellStyle cellStyle10 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Left, VerticalAlignment.Center, true);//字体不加粗，有边框，左对齐，不换行

            ICellStyle cellStyle6 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Center, VerticalAlignment.Center, true);//字体不加粗，有边框，左右居中，换行

            ICellStyle cellStyle7 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3, HorizontalAlignment.Center, VerticalAlignment.Center, true);//字体加粗，有边框，左右居中，换行

            //字体不加粗，无边框，靠右,不换行
            //IFont  boldFont222= ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 10,false );//不加粗
            //boldFont222.IsItalic = true;
            ICellStyle cellStyle8 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle8.WrapText = false;

            //左对齐，不加粗，不换行,无边框
            ICellStyle cellStyle9 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle9.WrapText = false;

            ICellStyle cellStyle11 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont4, HorizontalAlignment.Center, VerticalAlignment.Center, false);

            int thisRow = 0;

            switch (makerTypeEnum)
            {
                case MakerTypeEnum.InspectionClearance_CommercialInvoice:

                    #region 清关——发票

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('H'), vm.InvoiceNO, cellStyle8);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('H'), CommonCode.GetDateTime3(vm.CreateDate), cellStyle8);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('H'), CommonCode.GetDateTime3(vm.ShipDateStart), cellStyle8);
                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('H'), CommonCode.GetDateTime3(Utils.StrToDateTime(vm.CustomerDate)), cellStyle8);
                    ExcelHelper.SetCellValue(sheet, 14, ExcelHelper.GetNumByChar('H'), vm.PortOfEntryName, cellStyle8);
                    ExcelHelper.SetCellValue(sheet, 17, ExcelHelper.GetNumByChar('H'), vm.PortName + ", CHINA", cellStyle8);

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('B'), vm.CustomerName, cellStyle8);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('B'), vm.CustomerStreet, cellStyle8);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('B'), vm.CustomerReg, cellStyle8);

                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('B'), vm.AcceptInformation_CompanyName, cellStyle8);
                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('B'), vm.AcceptInformation_StreetAddress, cellStyle8);
                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('B'), vm.AcceptInformation_CustomerReg, cellStyle8);

                    ExcelHelper.SetCellValue(sheet, 16, ExcelHelper.GetNumByChar('B'), vm.InvoiceOF, cellStyle8);
                    ExcelHelper.SetCellValue(sheet, 18, ExcelHelper.GetNumByChar('B'), vm.PortName + ", CHINA", cellStyle8);
                    ExcelHelper.SetCellValue(sheet, 20, ExcelHelper.GetNumByChar('B'), vm.POID, cellStyle8);

                    thisRow = 23;

                    int Qty = 0;
                    int BoxQty = 0;
                    decimal SumSalePrice = 0;

                    decimal? SumOuterWeightGross = 0;
                    decimal? SumOuterWeightNet = 0;
                    decimal? discounts = 0;

                    foreach (var item in vm.list_OrderProduct_Invoice)
                    {
                        Qty += item.Qty;
                        BoxQty += item.BoxQty ?? 0;
                        SumSalePrice += item.SumSalePrice;

                        SumOuterWeightGross += item.SumOuterWeightGross;
                        SumOuterWeightNet += item.SumOuterWeightNet;
                        discounts += item.SumSalePrice * item.MiscPercent / 100;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.InspectionClearance_Factory_EnglishName, cellStyle6);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.No, cellStyle6);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.SkuNumber, cellStyle6);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.Desc, cellStyle10);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.ProductUPC, cellStyle6);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Math.Round(item.SalePrice, 2), cellStyle6);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), item.RetailPrice, cellStyle6);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), item.Qty, cellStyle6);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), item.BoxQty, cellStyle6);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), Keys.USD_Sign + Math.Round(item.SumSalePrice, 2), cellStyle6);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle7);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle7);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle7);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle7);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle7);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), "", cellStyle7);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), "", cellStyle7);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), Qty, cellStyle7);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), "", cellStyle7);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), Keys.USD_Sign + Math.Round(SumSalePrice, 2), cellStyle7);

                    ++thisRow;
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), Qty + " pcs, " + BoxQty + " cartons", cellStyle9);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), Keys.USD_Sign + Math.Round(SumSalePrice, 2), cellStyle_boldFont);

                    discounts = discounts.Round(2);

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), SumOuterWeightGross + " kgs", cellStyle5);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), Keys.USD_Sign + discounts, cellStyle_boldFont);

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), SumOuterWeightNet + " kgs", cellStyle5);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), Keys.USD_Sign + (SumSalePrice - discounts), cellStyle_boldFont);

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), CommonCode.GetEnMoneyString(Math.Round(SumSalePrice, 2).ToString()) + " ONLY", cellStyle8);

                    thisRow += 3;
                    thisRow = ExcelHelper.Insert_FactoryEnglishName_Address_Clearance(sheet, vm.list_OrderProduct_Invoice, thisRow, cellStyle_boldFont);

                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('G'), thisRow, ExcelHelper.GetNumByChar('H'), thisRow + 8);

                    #endregion 清关——发票

                    break;

                case MakerTypeEnum.InspectionClearance_PackingList:

                    #region 清关——装箱单

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('F'), vm.InvoiceNO);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('F'), CommonCode.GetDateTime3(vm.CreateDate));

                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('F'), CommonCode.GetDateTime3(vm.ShipDateStart));
                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('F'), CommonCode.GetDateTime3(Utils.StrToDateTime(vm.CustomerDate)));

                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('F'), "HOUSTON, TX");
                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('F'), vm.PortOfEntryName);

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('B'), vm.CustomerName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('B'), vm.CustomerStreet);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('B'), vm.CustomerReg);

                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('B'), vm.AcceptInformation_CompanyName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('B'), vm.AcceptInformation_StreetAddress);
                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('B'), vm.AcceptInformation_CustomerReg);

                    ExcelHelper.SetCellValue(sheet, 16, ExcelHelper.GetNumByChar('B'), vm.POID, cellStyle11);

                    thisRow = 20;

                    Qty = 0;
                    BoxQty = 0;
                    SumOuterWeightGross = 0;
                    SumOuterWeightNet = 0;
                    decimal? SumOuterVolume = 0;
                    decimal? SumOuterVolume_CUFT = 0;

                    foreach (var item in vm.list_OrderProduct_PackingList)
                    {
                        Qty += item.Qty;
                        BoxQty += item.BoxQty ?? 0;
                        SumOuterWeightGross += item.SumOuterWeightGross;
                        SumOuterWeightNet += item.SumOuterWeightNet;
                        SumOuterVolume += item.SumOuterVolume;
                        decimal? OuterVolume_CUFT = item.OuterVolume * item.BoxQty;
                        SumOuterVolume_CUFT += OuterVolume_CUFT;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.Desc, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.Qty, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.OuterVolume, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), OuterVolume_CUFT, cellStyle4);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), "cbft", cellStyle3);

                        ++thisRow;
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "UPC # " + item.ProductUPC, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "Master Pack: ", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.OuterBoxRate, cellStyle3);

                        ++thisRow;
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "ITEM # " + item.No, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "Cartons: ", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.BoxQty, cellStyle3);

                        ++thisRow;
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "AH SKU # " + item.SkuNumber, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "Gross Wt.: ", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.SumOuterWeightGross + "kgs", cellStyle3);

                        ++thisRow;
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "Net Wt.: ", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.SumOuterWeightNet + "kgs", cellStyle3);

                        ++thisRow;
                        ++thisRow;
                    }

                    ExcelHelper.SetCellValue(sheet, 20, ExcelHelper.GetNumByChar('A'), "Case Label", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, 21, ExcelHelper.GetNumByChar('A'), "BOX of", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, 22, ExcelHelper.GetNumByChar('A'), "PO", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, 23, ExcelHelper.GetNumByChar('A'), "Case Pack Quantity", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, 24, ExcelHelper.GetNumByChar('A'), "Dept   43 - HOLIDAY", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, 25, ExcelHelper.GetNumByChar('A'), "Vendor Part Number", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, 26, ExcelHelper.GetNumByChar('A'), "SKU Number", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, 27, ExcelHelper.GetNumByChar('A'), "Made In China", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, 28, ExcelHelper.GetNumByChar('A'), "at home ®", cellStyle3);

                    ++thisRow;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('E'), thisRow, ExcelHelper.GetNumByChar('F'), thisRow + 7);
                    if (thisRow <= 27)
                    {
                        thisRow = 28;
                    }

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), CommonCode.ListToString(vm.list_BoxNumber_CabinetNumber_SealingNumber), cellStyle3);

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "Total:", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), BoxQty + " cartons", cellStyle3);

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "Total Gross Wt:", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), SumOuterWeightGross + " kgs", cellStyle3);

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "Total Net Wt:", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), SumOuterWeightNet + " kgs", cellStyle3);

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "Total cbft: ", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), SumOuterVolume_CUFT + " cbft", cellStyle3);

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "Total cbm:  ", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), SumOuterVolume + " cbm", cellStyle3);

                    #endregion 清关——装箱单

                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// S288的结汇
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionExchange_S288<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
        {
            VMInspectionExchange vm = model as VMInspectionExchange;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);
            var boldFont2 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 10, false);//不加粗
            var boldFont3 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 10);//加粗

            var boldFont4 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12, false);

            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle_boldFont = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3);

            var bold3 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 10, false);
            ICellStyle cellStyle3 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold3);

            ICellStyle cellStyle4 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold3, HorizontalAlignment.Right, VerticalAlignment.Bottom, false);

            IFont boldFont_IsItalic = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);
            boldFont_IsItalic.IsItalic = true;
            ICellStyle cellStyle_boldFont_IsItalic = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont_IsItalic);

            ICellStyle cellStyle5 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Left, VerticalAlignment.Center, false);//字体不加粗，没有边框，左对齐，换行

            ICellStyle cellStyle10 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Left, VerticalAlignment.Center, true);//字体不加粗，有边框，左对齐，不换行

            ICellStyle cellStyle6 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Center, VerticalAlignment.Center, true);//字体不加粗，有边框，左右居中，换行

            ICellStyle cellStyle7 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3, HorizontalAlignment.Center, VerticalAlignment.Center, true);//字体加粗，有边框，左右居中，换行

            //字体不加粗，无边框，靠右,不换行
            //IFont  boldFont222= ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 10,false );//不加粗
            //boldFont222.IsItalic = true;
            ICellStyle cellStyle8 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle8.WrapText = false;

            //左对齐，不加粗，不换行,无边框
            ICellStyle cellStyle9 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle9.WrapText = false;

            ICellStyle cellStyle11 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont4, HorizontalAlignment.Center, VerticalAlignment.Center, false);

            int thisRow = 0;

            switch (makerTypeEnum)
            {
                case MakerTypeEnum.InspectionExchange_CommercialInvoice:

                    #region 结汇——发票

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('H'), vm.InvoiceNO, cellStyle8);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('H'), CommonCode.GetDateTime3(vm.CreateDate), cellStyle8);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('H'), CommonCode.GetDateTime3(vm.ShipDateStart), cellStyle8);
                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('H'), CommonCode.GetDateTime3(Utils.StrToDateTime(vm.CustomerDate)), cellStyle8);
                    ExcelHelper.SetCellValue(sheet, 14, ExcelHelper.GetNumByChar('H'), vm.PortOfEntryName, cellStyle8);
                    ExcelHelper.SetCellValue(sheet, 17, ExcelHelper.GetNumByChar('H'), vm.PortName + ", CHINA", cellStyle8);

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('B'), vm.CustomerName, cellStyle8);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('B'), vm.CustomerStreet, cellStyle8);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('B'), vm.CustomerReg, cellStyle8);

                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('B'), vm.AcceptInformation_CompanyName, cellStyle8);
                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('B'), vm.AcceptInformation_StreetAddress, cellStyle8);
                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('B'), vm.AcceptInformation_CustomerReg, cellStyle8);

                    ExcelHelper.SetCellValue(sheet, 16, ExcelHelper.GetNumByChar('B'), vm.InvoiceOF, cellStyle8);
                    ExcelHelper.SetCellValue(sheet, 18, ExcelHelper.GetNumByChar('B'), vm.PortName + ", CHINA", cellStyle8);
                    ExcelHelper.SetCellValue(sheet, 20, ExcelHelper.GetNumByChar('B'), vm.POID, cellStyle8);

                    thisRow = 23;

                    int Qty = 0;
                    int BoxQty = 0;
                    decimal SumSalePrice = 0;

                    decimal? SumOuterWeightGross = 0;
                    decimal? SumOuterWeightNet = 0;
                    decimal? discounts = 0;

                    foreach (var item in vm.list_OrderProduct_Invoice)
                    {
                        Qty += item.Qty;
                        BoxQty += item.BoxQty ?? 0;
                        SumSalePrice += item.SumSalePrice;

                        SumOuterWeightGross += item.SumOuterWeightGross;
                        SumOuterWeightNet += item.SumOuterWeightNet;
                        discounts += item.SumSalePrice * item.MiscPercent / 100;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.InspectionExchange_Factory_EnglishName, cellStyle6);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.No, cellStyle6);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.SkuNumber, cellStyle6);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.Desc, cellStyle10);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.ProductUPC, cellStyle6);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Math.Round(item.SalePrice, 2), cellStyle6);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), item.RetailPrice, cellStyle6);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), item.Qty, cellStyle6);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), item.BoxQty, cellStyle6);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), Keys.USD_Sign + Math.Round(item.SumSalePrice, 2), cellStyle6);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle7);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle7);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle7);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle7);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle7);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), "", cellStyle7);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), "", cellStyle7);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), Qty, cellStyle7);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), "", cellStyle7);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), Keys.USD_Sign + Math.Round(SumSalePrice, 2), cellStyle7);

                    ++thisRow;
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), Qty + " pcs, " + BoxQty + " cartons", cellStyle9);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), Keys.USD_Sign + Math.Round(SumSalePrice, 2), cellStyle_boldFont);

                    discounts = discounts.Round(2);

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), SumOuterWeightGross + " kgs", cellStyle5);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), Keys.USD_Sign + discounts, cellStyle_boldFont);

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), SumOuterWeightNet + " kgs", cellStyle5);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), Keys.USD_Sign + (SumSalePrice - discounts), cellStyle_boldFont);

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), CommonCode.GetEnMoneyString(Math.Round(SumSalePrice, 2).ToString()) + " ONLY", cellStyle8);

                    thisRow += 3;
                    thisRow = ExcelHelper.Insert_FactoryEnglishName_Address_Exchange(sheet, vm.list_OrderProduct_Invoice, thisRow, cellStyle_boldFont);

                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('G'), thisRow, ExcelHelper.GetNumByChar('H'), thisRow + 8);

                    #endregion 结汇——发票

                    break;

                case MakerTypeEnum.InspectionExchange_PackingList:

                    #region 结汇——装箱单

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('F'), vm.InvoiceNO);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('F'), CommonCode.GetDateTime3(vm.CreateDate));

                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('F'), CommonCode.GetDateTime3(vm.ShipDateStart));
                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('F'), CommonCode.GetDateTime3(Utils.StrToDateTime(vm.CustomerDate)));

                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('F'), "HOUSTON, TX");
                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('F'), vm.PortOfEntryName);

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('B'), vm.CustomerName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('B'), vm.CustomerStreet);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('B'), vm.CustomerReg);

                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('B'), vm.AcceptInformation_CompanyName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('B'), vm.AcceptInformation_StreetAddress);
                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('B'), vm.AcceptInformation_CustomerReg);

                    ExcelHelper.SetCellValue(sheet, 16, ExcelHelper.GetNumByChar('B'), vm.POID, cellStyle11);

                    thisRow = 20;

                    Qty = 0;
                    BoxQty = 0;
                    SumOuterWeightGross = 0;
                    SumOuterWeightNet = 0;
                    decimal? SumOuterVolume = 0;
                    decimal? SumOuterVolume_CUFT = 0;

                    foreach (var item in vm.list_OrderProduct_PackingList)
                    {
                        Qty += item.Qty;
                        BoxQty += item.BoxQty ?? 0;
                        SumOuterWeightGross += item.SumOuterWeightGross;
                        SumOuterWeightNet += item.SumOuterWeightNet;
                        SumOuterVolume += item.SumOuterVolume;
                        decimal? OuterVolume_CUFT = item.OuterVolume * item.BoxQty;
                        SumOuterVolume_CUFT += OuterVolume_CUFT;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.Desc, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.Qty, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.OuterVolume, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), OuterVolume_CUFT, cellStyle4);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), "cbft", cellStyle3);

                        ++thisRow;
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "UPC # " + item.ProductUPC, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "Master Pack: ", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.OuterBoxRate, cellStyle3);

                        ++thisRow;
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "ITEM # " + item.No, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "Cartons: ", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.BoxQty, cellStyle3);

                        ++thisRow;
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "AH SKU # " + item.SkuNumber, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "Gross Wt.: ", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.SumOuterWeightGross + "kgs", cellStyle3);

                        ++thisRow;
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "Net Wt.: ", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.SumOuterWeightNet + "kgs", cellStyle3);

                        ++thisRow;
                        ++thisRow;
                    }

                    ExcelHelper.SetCellValue(sheet, 20, ExcelHelper.GetNumByChar('A'), "Case Label", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, 21, ExcelHelper.GetNumByChar('A'), "BOX of", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, 22, ExcelHelper.GetNumByChar('A'), "PO", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, 23, ExcelHelper.GetNumByChar('A'), "Case Pack Quantity", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, 24, ExcelHelper.GetNumByChar('A'), "Dept   43 - HOLIDAY", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, 25, ExcelHelper.GetNumByChar('A'), "Vendor Part Number", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, 26, ExcelHelper.GetNumByChar('A'), "SKU Number", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, 27, ExcelHelper.GetNumByChar('A'), "Made In China", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, 28, ExcelHelper.GetNumByChar('A'), "at home ®", cellStyle3);

                    ++thisRow;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('E'), thisRow, ExcelHelper.GetNumByChar('F'), thisRow + 7);
                    if (thisRow <= 27)
                    {
                        thisRow = 28;
                    }

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), CommonCode.ListToString(vm.list_BoxNumber_CabinetNumber_SealingNumber), cellStyle3);

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "Total:", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), BoxQty + " cartons", cellStyle3);

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "Total Gross Wt:", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), SumOuterWeightGross + " kgs", cellStyle3);

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "Total Net Wt:", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), SumOuterWeightNet + " kgs", cellStyle3);

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "Total cbft: ", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), SumOuterVolume_CUFT + " cbft", cellStyle3);

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "Total cbm:  ", cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), SumOuterVolume + " cbm", cellStyle3);

                    #endregion 结汇——装箱单

                    break;

                default:
                    break;
            }
        }
    }
}