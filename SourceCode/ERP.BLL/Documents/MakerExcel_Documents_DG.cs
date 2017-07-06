﻿using ERP.BLL.ERP.Consts;
using ERP.Models.CustomEnums;
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
    internal class MakerExcel_Documents_DG
    {
        /// <summary>
        /// DG的报检
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionReceipt_DG<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
        {
            DTOInspectionReceipt vm = model as DTOInspectionReceipt;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);
            var boldFont2 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 11);

            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, null, HorizontalAlignment.Center, VerticalAlignment.Center, true);//有边框
            ICellStyle cellStyle2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, true);//有边框有加粗
            ICellStyle cellStyle_boldFont = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont);//字体加粗
            ICellStyle cellStyle_boldFont2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2);

            IFont boldFont_IsItalic = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);
            boldFont_IsItalic.IsItalic = true;
            ICellStyle cellStyle_boldFont_IsItalic = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont_IsItalic);

            int thisRow = 0;

            switch (makerTypeEnum)
            {
                case MakerTypeEnum.InspectionReceipt_Home:

                    #region 报检——报检首页

                    ExcelHelper.SetCellValue(sheet, 0, ExcelHelper.GetNumByChar('B'), vm.PurchaseNumber + "报检资料如下：", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 1, ExcelHelper.GetNumByChar('B'), vm.HsName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 2, ExcelHelper.GetNumByChar('B'), vm.HSCodeName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 3, ExcelHelper.GetNumByChar('B'), vm.SCNO, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 4, ExcelHelper.GetNumByChar('B'), vm.ProductsQuauity + "PCS", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 5, ExcelHelper.GetNumByChar('B'), vm.ProductsBoxNum + "CTNS", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('B'), Keys.USD_Sign + vm.ProductAmount, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), "由", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('B'), vm.PortName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('C'), "海运运到", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('D'), "美国", cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('B'), vm.INTypeName + ",", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('C'), "出凭条时间：" + vm.ClaimFaxDate, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('E'), vm.CreateUserName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('E'), vm.CreateDateForamtter, cellStyle_boldFont);

                    #endregion 报检——报检首页

                    break;

                case MakerTypeEnum.InspectionReceipt_PackingList:

                    #region 报检——装箱单

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('G'), vm.InvNo);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('G'), CommonCode.GetDateTime3(vm.CreateDate));
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.CustomerReg);

                    thisRow = 12;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), vm.POID, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), vm.HsEngName, cellStyle);

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), vm.ProductsBoxNum, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), vm.ProductsQuauity, cellStyle);

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), vm.WeightGrossSum, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), vm.WeightNetSum, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), vm.CUFT, cellStyle);

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle2);

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), vm.ProductsBoxNum, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), vm.ProductsQuauity, cellStyle2);

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), vm.WeightGrossSum, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), vm.WeightNetSum, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), vm.CUFT, cellStyle2);
                    ++thisRow;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "Shipping Marks:", cellStyle_boldFont);
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "SKU#");
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), GetYearNo(vm.SeasonPrefix), cellStyle_boldFont);//TODO:季节代码变量
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "PO#");
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "CASE PACK");
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "GW.WT.      LBS  ");
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "CU.FT.   ");

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "MADE IN CHINA   ");
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "DESCRIPTION");

                    #endregion 报检——装箱单

                    break;

                case MakerTypeEnum.InspectionReceipt_CommercialInvoice:

                    #region 报检——发票

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('E'), vm.InvNo);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('E'), CommonCode.GetDateTime3(vm.CreateDate));
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('A'), "Loading port:" + vm.INPortName + ",CHINA", cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('A'), "Destination: " + vm.INEndPortName + "  ,USA", cellStyle_boldFont2);

                    thisRow = 15;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), vm.POID, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), vm.ProductsQuauity, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), vm.HsEngName, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), Keys.USD_Sign + vm.ProductPrice, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), Keys.USD_Sign + vm.ProductAmount, cellStyle);
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "Total:", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), vm.ProductsQuauity, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), Keys.USD_Sign + vm.ProductAmount, cellStyle2);

                    ++thisRow;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "Shipping Marks:", cellStyle_boldFont);
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "SKU#");
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), GetYearNo(vm.SeasonPrefix), cellStyle_boldFont);//TODO:季节代码变量
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "PO#");
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "CASE PACK");
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "GW.WT.      LBS  ");
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "CU.FT.   ");

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "MADE IN CHINA   ");
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "DESCRIPTION");

                    #endregion 报检——发票

                    break;

                case MakerTypeEnum.InspectionReceipt_SaleContract:

                    #region 报检——销售合同

                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('E'), vm.InvNo, cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('E'), "DATE:" + CommonCode.GetDateTime3(vm.CustomerDate), cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('A'), "Loading port: " + vm.INPortName + ",CHINA", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 14, ExcelHelper.GetNumByChar('A'), "Destination: " + vm.INEndPortName, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 19, ExcelHelper.GetNumByChar('D'), "          FOB " + vm.INPortName, cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, 22, ExcelHelper.GetNumByChar('A'), vm.POID, cellStyle2);//TODO:核算单po，如一格报关编号有多个po则不显示为空
                    ExcelHelper.SetCellValue(sheet, 22, ExcelHelper.GetNumByChar('B'), vm.HsEngName, cellStyle);
                    ExcelHelper.SetCellValue(sheet, 22, ExcelHelper.GetNumByChar('C'), vm.ProductsQuauity, cellStyle);
                    ExcelHelper.SetCellValue(sheet, 22, ExcelHelper.GetNumByChar('D'), Keys.USD_Sign + vm.ProductPrice, cellStyle);
                    ExcelHelper.SetCellValue(sheet, 22, ExcelHelper.GetNumByChar('E'), Keys.USD_Sign + vm.ProductAmount, cellStyle);

                    ExcelHelper.SetCellValue(sheet, 23, ExcelHelper.GetNumByChar('B'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, 23, ExcelHelper.GetNumByChar('C'), vm.ProductsQuauity, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, 23, ExcelHelper.GetNumByChar('D'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, 23, ExcelHelper.GetNumByChar('E'), Keys.USD_Sign + vm.ProductAmount, cellStyle2);

                    ExcelHelper.SetCellValue(sheet, 26, ExcelHelper.GetNumByChar('A'), "1.SHIPMENT DATE:" + vm.ShippingDateStart, cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, 27, ExcelHelper.GetNumByChar('A'), "2.SHIPPING MARKS:" + "TO BE ADVISED BY THE BUYER", cellStyle_boldFont2);

                    ExcelHelper.SetCellValue(sheet, 28, ExcelHelper.GetNumByChar('A'), "3.TERM OF THE PAYMENT:" + vm.TeamOfThePayment, cellStyle_boldFont2);

                    #endregion 报检——销售合同

                    break;

                case MakerTypeEnum.InspectionReceipt_Commission:
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// DG的报关
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionCustoms_DG<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
        {
            VMInspectionCustoms vm = model as VMInspectionCustoms;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);
            ICellStyle cellStyle_boldFont2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont);
            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, null, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle_boldFont = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont);

            IFont boldFont_IsItalic = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);
            boldFont_IsItalic.IsItalic = true;
            ICellStyle cellStyle_boldFont_IsItalic = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont_IsItalic);
            int thisRow = 0;

            switch (makerTypeEnum)
            {
                #region 报关

                case MakerTypeEnum.InspectionCustoms_PackingList:

                    #region 报关——装箱单

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('F'), "Inv. No.:" + vm.InvoiceNO, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('F'), "Date: " + CommonCode.GetDateTime3(vm.CreateDate));
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle_boldFont);

                    thisRow = 12;

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

                        ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow, ExcelHelper.GetNumByChar('B'), ExcelHelper.GetNumByChar('C'));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "", cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.HsEngName, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.ProductsBoxNum, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.SumOuterWeightGross, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), item.SumOuterWeightNet, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), item.SumOuterVolume, cellStyle);
                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow, ExcelHelper.GetNumByChar('B'), ExcelHelper.GetNumByChar('C'));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), ProductsBoxNum, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), Qty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), SumOuterWeightGross, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), SumOuterWeightNet, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), SumOuterVolume, cellStyle2);
                    thisRow += 3;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "Shipping Marks:", cellStyle_boldFont);
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "SKU#");
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), GetYearNo(vm.list_ShipmentOrderProduct.First().SeasonPrefix), cellStyle_boldFont);//TODO:季节代码变量
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "PO#");
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "CASE PACK");
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "GW.WT.      LBS  ");
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "CU.FT.   ");

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "MADE IN CHINA   ");
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "DESCRIPTION");

                    thisRow -= 6;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('E'), thisRow, ExcelHelper.GetNumByChar('F'), thisRow + 6);

                    #endregion 报关——装箱单

                    break;

                case MakerTypeEnum.InspectionCustoms_CommercialInvoice:

                    #region 报关——发票

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('E'), vm.InvoiceNO);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('E'), CommonCode.GetDateTime3(vm.CreateDate));

                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.CustomerReg);

                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('A'), "Loading port:   " + vm.PortName + ",CHINA"
                        , cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('A'), "Destination: " + vm.DestinationPortName, cellStyle_boldFont);

                    thisRow = 15;
                    ProductsBoxNum = 0;
                    Qty = 0;
                    decimal Amount = 0;
                    foreach (var item in vm.list_OrdersProducts)
                    {
                        ProductsBoxNum += item.ProductsBoxNum;
                        Qty += item.Qty;
                        Amount += item.Amount;
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), vm.POID, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.HsEngName, cellStyle);

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), Keys.USD_Sign + item.ProductPrice, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), Keys.USD_Sign + item.Amount, cellStyle);
                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), vm.POID, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "TOTAL:", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), Qty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), Keys.USD_Sign + Amount, cellStyle2);

                    thisRow += 2;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "Shipping Marks:", cellStyle_boldFont);
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "SKU#");
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), GetYearNo(vm.list_ShipmentOrderProduct.First().SeasonPrefix), cellStyle_boldFont);//TODO:季节代码变量
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "PO#");
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "CASE PACK");
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "GW.WT.      LBS  ");
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "CU.FT.   ");

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "MADE IN CHINA   ");
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "DESCRIPTION");

                    thisRow -= 6;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('C'), thisRow, ExcelHelper.GetNumByChar('D'), thisRow + 6);

                    break;

                #endregion 报关——发票

                case MakerTypeEnum.InspectionCustoms_SaleContract:

                    #region 报关——销售合同

                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('E'), vm.InvoiceNO, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('E'), CommonCode.GetDateTime3(vm.CustomerDate), cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('A'), "LOADING PORT:" + vm.PortName + ",CHINA", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 14, ExcelHelper.GetNumByChar('A'), "DESTINATION:" + vm.DestinationPortName, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 19, ExcelHelper.GetNumByChar('D'), "          FOB " + vm.PortName, cellStyle_boldFont);

                    thisRow = 22;
                    ProductsBoxNum = 0;
                    Qty = 0;
                    Amount = 0;
                    foreach (var item in vm.list_OrdersProducts)
                    {
                        ProductsBoxNum += item.ProductsBoxNum;
                        Qty += item.Qty;
                        Amount += item.Amount;
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), vm.POID, cellStyle);//TODO:核算单po，如一格报关编号有多个po则不显示为空
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.HsEngName, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), Keys.USD_Sign + item.ProductPrice, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), Keys.USD_Sign + item.Amount, cellStyle);
                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle2);

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), Qty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), Keys.USD_Sign + Amount, cellStyle2);
                    thisRow += 2;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "REMARKS:", cellStyle_boldFont);
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "1.SHIPPING DATE:" + vm.ShippingDateStart, cellStyle_boldFont);
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "2.SHIPPING MARKS:  TO BE ADVISED", cellStyle_boldFont);
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "3. TERMS OF PAYMENT : T/T AT SIGHT. ", cellStyle_boldFont);

                    thisRow += 4;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "THE  SELLERS :", cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "THE BUYERS :", cellStyle_boldFont2);

                    ExcelHelper.CreatePicture(workbook, sheet, "/data/Documents/S188/tom.png", 75, 0, 800, 0, ExcelHelper.GetNumByChar('A'), thisRow + 1, ExcelHelper.GetNumByChar('A'), thisRow + 4);

                    ExcelHelper.CreatePicture(workbook, sheet, "/data/Documents/DG/Maria.png", 75, 0, 800, 0, ExcelHelper.GetNumByChar('D'), thisRow + 1, ExcelHelper.GetNumByChar('E'), thisRow + 6);
                    //thisRow += 2;
                    //ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('B'), thisRow, ExcelHelper.GetNumByChar('C'), thisRow + 6);//哲联公章

                    #endregion 报关——销售合同

                    break;

                #endregion 报关

                default:
                    break;
            }
        }

        /// <summary>
        /// DG的清关
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionClearance_DG<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
        {
            VMInspectionClearance vm = model as VMInspectionClearance;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 10);
            var boldFont_Underline = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 12);
            boldFont_Underline.Underline = FontUnderlineType.Single;

            var bold3 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 10, false);

            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold3, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle3 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold3, HorizontalAlignment.Left, VerticalAlignment.Center, false);

            ICellStyle cellStyle_boldFont = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont);
            ICellStyle cellStyle_boldFont_Underline = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont_Underline);

            int thisRow = 0;

            switch (makerTypeEnum)
            {
                case MakerTypeEnum.InspectionClearance_CommercialInvoice:

                    #region 清关——发票

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('E'), "INV. NO.:" + vm.InvoiceNO, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('E'), "DATE:" + CommonCode.GetDateTime3(vm.CreateDate), cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('E'), "DATE OF SALE:" + Utils.DateTimeToStr(Utils.StrToDateTime(vm.CustomerDate), "dd/MM/yyyy"), cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('A'), "SHIPPED BY SHANGHAI Jet CRAFTS INC PER_ON/ABOUT  " + Utils.DateTimeToStr(vm.ActualShippingDate, "dd/MM/yyyy") + "   FROM", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('A'), vm.PortName + " TO FINAL WAREHOUSE DESTINATION " + vm.DestinationPortName + " WITH PORT OF ENTRY", cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 15, ExcelHelper.GetNumByChar('E'), "TERMS OF SALE:FOB " + vm.PortName, cellStyle_boldFont);

                    thisRow = 18;

                    int Qty = 0;
                    int BoxQty = 0;
                    decimal SumSalePrice = 0;

                    List<string> list_SkuNumber = new List<string>();
                    List<string> list_POID = new List<string>();
                    List<string> list_No_POID = new List<string>();

                    foreach (var item in vm.list_OrderProduct_PackingList)
                    {
                        Qty += item.Qty;
                        BoxQty += item.BoxQty ?? 0;
                        SumSalePrice += item.SumSalePrice;

                        list_SkuNumber.Add(item.SkuNumber);
                        list_POID.Add(item.POID);
                        list_No_POID.Add(item.No + "(P.O." + item.POID + ")");

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.SkuNumber, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.POID, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.Desc, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Keys.USD_Sign + Math.Round(item.SalePrice, 2), cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), Keys.USD_Sign + Math.Round(item.SumSalePrice, 2), cellStyle);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), Qty, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), BoxQty, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), Keys.USD_Sign + SumSalePrice, cellStyle);

                    thisRow += 2;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "1) ITEM NO.: " + CommonCode.ListToString(list_No_POID), cellStyle_boldFont);
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "2) BANK REF. NUMBER FOR P.O.#" + CommonCode.ListToString(list_POID) + "     " + vm.CreditNumber, cellStyle_boldFont);
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "3) COUNTRY OF ORIGIN: CHINA", cellStyle_boldFont);

                    thisRow += 4;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "NAME AND ADDRESS OF THE ACTUAL MANUFACTURER FOR P.O.#" + CommonCode.ListToString(list_POID) + " AS BELOW:", cellStyle_boldFont);

                    thisRow += 20;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "S.K.U. # :" + CommonCode.ListToString(list_SkuNumber), cellStyle_boldFont);

                    ++thisRow;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "P.O.# " + CommonCode.ListToString(list_POID), cellStyle_boldFont);

                    thisRow += 2;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), GetYearNo(vm.list_OrderProduct_PackingList.First().SeasonPrefix), cellStyle_boldFont);

                    #endregion 清关——发票

                    break;

                case MakerTypeEnum.InspectionClearance_PackingList:

                    #region 清关——装箱单

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('E'), "INV. NO.:" + vm.InvoiceNO, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('E'), "DATE:" + CommonCode.GetDateTime3(vm.CreateDate), cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('E'), "DATE OF SALE:" + Utils.DateTimeToStr(Utils.StrToDateTime(vm.CustomerDate), "dd/MM/yyyy"), cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('E'), "TERM OF PAYMENT:" + vm.PaymentType, cellStyle_boldFont);

                    thisRow = 13;

                    Qty = 0;
                    BoxQty = 0;
                    SumSalePrice = 0;

                    decimal? SumOuterWeightNet = 0;
                    decimal? SumOuterWeightGross = 0;
                    decimal? SumOuterVolume = 0;

                    list_SkuNumber = new List<string>();
                    list_POID = new List<string>();
                    list_No_POID = new List<string>();

                    foreach (var item in vm.list_OrderProduct_PackingList)
                    {
                        Qty += item.Qty;
                        BoxQty += item.BoxQty ?? 0;
                        SumOuterWeightNet = item.SumOuterWeightNet;
                        SumOuterWeightGross = item.SumOuterWeightGross;
                        SumOuterVolume = item.SumOuterVolume;

                        list_SkuNumber.Add(item.SkuNumber);
                        list_POID.Add(item.POID);
                        list_No_POID.Add(item.No + "(P.O." + item.POID + ")");

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.SkuNumber, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.POID, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.Desc, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.SumOuterWeightNet, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), item.SumOuterWeightGross, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), item.SumOuterVolume, cellStyle);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), Qty, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "TOTAL", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), BoxQty, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), SumOuterWeightNet, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), SumOuterWeightGross, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), SumOuterVolume, cellStyle);

                    ++thisRow;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "1) ITEM NO.: " + CommonCode.ListToString(list_No_POID), cellStyle_boldFont);
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "2) BANK REF. NUMBER FOR P.O.#" + CommonCode.ListToString(list_POID) + "     " + vm.CreditNumber, cellStyle_boldFont);
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "NAME AND ADDRESS OF THE ACTUAL MANUFACTURER FOR P.O.# :" + vm.POID + " AS BELOW:", cellStyle_boldFont);

                    thisRow += 4;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "S.K.U. # :" + CommonCode.ListToString(list_SkuNumber), cellStyle_boldFont);

                    ++thisRow;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "P.O.# " + CommonCode.ListToString(list_POID), cellStyle_boldFont);

                    thisRow += 2;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), GetYearNo(vm.list_OrderProduct_PackingList.First().SeasonPrefix), cellStyle_boldFont);

                    #endregion 清关——装箱单

                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 获取年份代码
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        private static string GetYearNo(string SeasonPrefix)
        {
            string YearNo = "F";
            if (SeasonPrefix != null)
            {
                if (SeasonPrefix.Contains("Easter") || SeasonPrefix.Contains("Lawn & Garden"))
                {
                    YearNo = "S";
                }
            }
            return YearNo + DateTime.Now.Year.ToString().Substring(0, 2);
        }

        /// <summary>
        /// DG的结汇
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionExchange_DG<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
        {
            VMInspectionExchange vm = model as VMInspectionExchange;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 10);
            var boldFont_Underline = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 12);
            boldFont_Underline.Underline = FontUnderlineType.Single;

            var bold3 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 10, false);

            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold3, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle3 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold3, HorizontalAlignment.Left, VerticalAlignment.Center, false);

            ICellStyle cellStyle_boldFont = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont);
            ICellStyle cellStyle_boldFont_Underline = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont_Underline);

            int thisRow = 0;

            switch (makerTypeEnum)
            {
                case MakerTypeEnum.InspectionExchange_CommercialInvoice:

                    #region 结汇——发票

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('E'), "INV. NO.:" + vm.InvoiceNO, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('E'), "DATE:" + CommonCode.GetDateTime3(vm.CreateDate), cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('E'), "DATE OF SALE:" + Utils.DateTimeToStr(Utils.StrToDateTime(vm.CustomerDate), "dd/MM/yyyy"), cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('A'), "SHIPPED BY SHANGHAI Jet CRAFTS INC PER_ON/ABOUT  " + Utils.DateTimeToStr(vm.ActualShippingDate, "dd/MM/yyyy") + "   FROM", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('A'), vm.PortName + " TO FINAL WAREHOUSE DESTINATION " + vm.DestinationPortName + " WITH PORT OF ENTRY", cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 15, ExcelHelper.GetNumByChar('E'), "TERMS OF SALE:FOB " + vm.PortName, cellStyle_boldFont);

                    thisRow = 18;

                    int Qty = 0;
                    int BoxQty = 0;
                    decimal SumSalePrice = 0;

                    List<string> list_SkuNumber = new List<string>();
                    List<string> list_POID = new List<string>();
                    List<string> list_No_POID = new List<string>();

                    foreach (var item in vm.list_OrderProduct_PackingList)
                    {
                        Qty += item.Qty;
                        BoxQty += item.BoxQty ?? 0;
                        SumSalePrice += item.SumSalePrice;

                        list_SkuNumber.Add(item.SkuNumber);
                        list_POID.Add(item.POID);
                        list_No_POID.Add(item.No + "(P.O." + item.POID + ")");

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.SkuNumber, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.POID, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.Desc, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Keys.USD_Sign + Math.Round(item.SalePrice, 2), cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), Keys.USD_Sign + Math.Round(item.SumSalePrice, 2), cellStyle);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), Qty, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), BoxQty, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), Keys.USD_Sign + SumSalePrice, cellStyle);

                    thisRow += 2;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "1) ITEM NO.: " + CommonCode.ListToString(list_No_POID), cellStyle_boldFont);
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "2) BANK REF. NUMBER FOR P.O.#" + CommonCode.ListToString(list_POID) + "     " + vm.CreditNumber, cellStyle_boldFont);
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "3) COUNTRY OF ORIGIN: CHINA", cellStyle_boldFont);

                    thisRow += 4;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "NAME AND ADDRESS OF THE ACTUAL MANUFACTURER FOR P.O.#" + CommonCode.ListToString(list_POID) + " AS BELOW:", cellStyle_boldFont);

                    thisRow += 20;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "S.K.U. # :" + CommonCode.ListToString(list_SkuNumber), cellStyle_boldFont);

                    ++thisRow;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "P.O.# " + CommonCode.ListToString(list_POID), cellStyle_boldFont);

                    thisRow += 2;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), GetYearNo(vm.list_OrderProduct_PackingList.First().SeasonPrefix) + DateTime.Now.Year.ToString().Substring(0, 2), cellStyle_boldFont);

                    #endregion 结汇——发票

                    break;

                case MakerTypeEnum.InspectionExchange_PackingList:

                    //DG客户不需要装箱单

                    break;

                default:
                    break;
            }
        }
    }
}