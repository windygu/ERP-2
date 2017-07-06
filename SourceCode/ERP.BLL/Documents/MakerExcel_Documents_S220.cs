using ERP.BLL.ERP.Consts;
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
    /// <summary>
    /// S220清关与结汇一样。有信用证文件
    /// </summary>
    internal class MakerExcel_Documents_S220
    {
        /// <summary>
        /// S220的报检
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionReceipt_S220<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
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

                    ExcelHelper.SetCellValue(sheet, 0, ExcelHelper.GetNumByChar('B'), vm.FactoryName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 1, ExcelHelper.GetNumByChar('B'), vm.FactoryContact, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 2, ExcelHelper.GetNumByChar('B'), vm.PurchaseNumber, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 2, ExcelHelper.GetNumByChar('F'), vm.INTypeName, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 3, ExcelHelper.GetNumByChar('B'), vm.HsName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 4, ExcelHelper.GetNumByChar('B'), vm.HSCodeName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 5, ExcelHelper.GetNumByChar('B'), vm.SCNO, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('B'), vm.ProductsQuauity, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('B'), vm.ProductsBoxNum, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('B'), Keys.USD_Sign + " " + vm.ProductAmount, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), "请于" + vm.ClaimFaxDate + "日之前安排" + vm.INTypeName + "并提供通关单给我司，谢谢！", cellStyle_boldFont);
                    // ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), "请于" );

                    //ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('B'), "请于" + vm.ClaimFaxDate + "日之前安排" + vm.INTypeName + "并提供通关单给我司，谢谢！", cellStyle_boldFont);
                    //ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('C'), "请于" + vm.ClaimFaxDate + "日之前安排" + vm.INTypeName + "并提供通关单给我司，谢谢！", cellStyle_boldFont);
                    //ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('D'), "请于" + vm.ClaimFaxDate + "日之前安排" + vm.INTypeName + "并提供通关单给我司，谢谢！", cellStyle_boldFont);



                    ExcelHelper.SetCellValue(sheet, 14, ExcelHelper.GetNumByChar('E'), vm.CreateUserName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 15, ExcelHelper.GetNumByChar('E'), vm.CreateDateForamtter, cellStyle_boldFont);

                    #endregion 报检——报检首页

                    break;

                case MakerTypeEnum.InspectionReceipt_PackingList:

                    #region 报检——装箱单

                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), "TO: " + vm.CustomerName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('E'), "Inv. No.:  " + vm.InvNo);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('E'), "PO NO.:  " + vm.POID);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('E'), "Date:  " + CommonCode.GetDateTime3(vm.CreateDate));

                    thisRow = 18;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), vm.HsEngName, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), vm.ProductsQuauity, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), vm.ProductsBoxNum, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), vm.WeightGrossSum, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), vm.WeightNetSum, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), vm.CUFT, cellStyle);

                    ++thisRow;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle);

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), vm.ProductsQuauity, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), vm.ProductsBoxNum, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), vm.WeightGrossSum, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), vm.WeightNetSum, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), vm.CUFT, cellStyle2);

                    #endregion 报检——装箱单

                    break;

                case MakerTypeEnum.InspectionReceipt_CommercialInvoice:

                    #region 报检——发票

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('D'), "Inv. No.:  " + vm.InvNo);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('D'), "PO NO.:  " + vm.POID);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('D'), "Date:  " + CommonCode.GetDateTime3(vm.CreateDate));
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('A'), "Loading port:" + vm.INPortName + ",CHINA", cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('A'), "Destination: " + vm.INEndPortName, cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, 15, ExcelHelper.GetNumByChar('D'), "Terms of Sale: FOB " + vm.INPortName, cellStyle_boldFont2);

                    thisRow = 18;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), vm.ProductsQuauity, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), vm.HsEngName, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), Keys.USD_Sign + vm.ProductPrice, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), Keys.USD_Sign + vm.ProductAmount, cellStyle);

                    ++thisRow;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "Total:", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), vm.ProductsQuauity, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), Keys.USD_Sign + vm.ProductAmount, cellStyle2);

                    #endregion 报检——发票

                    break;

                case MakerTypeEnum.InspectionReceipt_SaleContract:

                    #region 报检——销售合同

                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('D'), "S/C NO.: " + vm.InvNo);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('D'), "DATE:" + CommonCode.GetDateTime3(vm.CustomerDate));
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('A'), "LOADING PORT: " + vm.INPortName + ",CHINA", cellStyle_boldFont2);

                    ExcelHelper.SetCellValue(sheet, 14, ExcelHelper.GetNumByChar('A'), "DESTINATION: " + vm.INEndPortName, cellStyle_boldFont2);

                    ExcelHelper.SetCellValue(sheet, 19, ExcelHelper.GetNumByChar('D'), "          FOB " + vm.INPortName + ",CHINA", cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, 22, ExcelHelper.GetNumByChar('A'), vm.POID, cellStyle);
                    ExcelHelper.SetCellValue(sheet, 22, ExcelHelper.GetNumByChar('B'), vm.HsEngName, cellStyle);
                    ExcelHelper.SetCellValue(sheet, 22, ExcelHelper.GetNumByChar('C'), vm.ProductsQuauity, cellStyle);
                    ExcelHelper.SetCellValue(sheet, 22, ExcelHelper.GetNumByChar('D'), Keys.USD_Sign + vm.ProductPrice, cellStyle);
                    ExcelHelper.SetCellValue(sheet, 22, ExcelHelper.GetNumByChar('E'), Keys.USD_Sign + vm.ProductAmount, cellStyle);

                    ExcelHelper.SetCellValue(sheet, 23, ExcelHelper.GetNumByChar('A'), "TOTAL", cellStyle2);
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
        /// S220的报关
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionCustoms_S220<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
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

                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('E'), "Inv. No.:" + vm.InvoiceNO);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('E'), "PO NO.:" + vm.POID);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('E'), "Date: " + CommonCode.GetDateTime3(vm.CreateDate));

                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('A'), "Loading port:" + vm.PortName + ",CHINA", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 14, ExcelHelper.GetNumByChar('A'), "Destination:" + vm.DestinationPortName, cellStyle_boldFont);

                    thisRow = 18;

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

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "", cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.HsEngName, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.ProductsBoxNum, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.SumOuterWeightGross, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.SumOuterWeightNet, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), item.SumOuterVolume, cellStyle);
                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), Qty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), ProductsBoxNum, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), SumOuterWeightGross, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), SumOuterWeightNet, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), SumOuterVolume, cellStyle2);

                    thisRow += 6;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('D'), thisRow, ExcelHelper.GetNumByChar('F'), thisRow + 6);

                    #endregion 报关——装箱单

                    break;

                case MakerTypeEnum.InspectionCustoms_CommercialInvoice:

                    #region 报关——发票

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('D'), "Inv. No.:" + vm.InvoiceNO);//TODO:
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('D'), "PO NO.:" + vm.POID);//TODO:
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.CustomerReg);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('D'), "Date: " + CommonCode.GetDateTime3(vm.CreateDate));//TODO:

                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('A'), "Loading port:   " + vm.PortName + ",CHINA", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('A'), "Destination: " + vm.DestinationPortName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 15, ExcelHelper.GetNumByChar('D'), "Terms of Sale: FOB " + vm.PortName, cellStyle_boldFont);

                    thisRow = 18;
                    ProductsBoxNum = 0;
                    Qty = 0;
                    decimal Amount = 0;
                    foreach (var item in vm.list_OrdersProducts)
                    {
                        ProductsBoxNum += item.ProductsBoxNum;
                        Qty += item.Qty;
                        Amount += item.Amount;
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "", cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.HsEngName, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), Keys.USD_Sign + item.ProductPrice, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), Keys.USD_Sign + item.Amount, cellStyle);
                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "Total", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), Qty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), Keys.USD_Sign + Amount, cellStyle2);

                    thisRow += 6;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('F'), thisRow, ExcelHelper.GetNumByChar('F'), thisRow + 6);
                    break;

                #endregion 报关——发票

                case MakerTypeEnum.InspectionCustoms_SaleContract:

                    #region 报关——销售合同

                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('D'), "S/C NO.: " + vm.InvoiceNO, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('D'), "Date:" + CommonCode.GetDateTime3(vm.CustomerDate), cellStyle_boldFont);
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

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), vm.POID, cellStyle);//TODO:POID  如果有多个poid，为空
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.HsEngName, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), Keys.USD_Sign + item.ProductPrice, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), Keys.USD_Sign + item.Amount, cellStyle);
                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle2);
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
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "3. TERMS OF PAYMENT :" + vm.TeamOfThePayment, cellStyle_boldFont);

                    thisRow += 6;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "THE  SELLERS :", cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "THE BUYERS :", cellStyle_boldFont2);

                    ExcelHelper.CreatePicture(workbook, sheet, "/data/Documents/S188/tom.png", 75, 0, 800, 0, ExcelHelper.GetNumByChar('A'), thisRow + 1, ExcelHelper.GetNumByChar('A'), thisRow + 4);

                    ExcelHelper.CreatePicture(workbook, sheet, "/data/Documents/S220/Davis.png", 75, 0, 800, 0, ExcelHelper.GetNumByChar('D'), thisRow + 1, ExcelHelper.GetNumByChar('D'), thisRow + 6);//TODO:显示不出来这图片，咋回事了
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
        /// S220的清关
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionClearance_S220<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
        {
            VMInspectionClearance vm = model as VMInspectionClearance;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 11);
            var boldFont_Underline = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 11);
            boldFont_Underline.Underline = FontUnderlineType.Single;

            var bold3 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 11, false);

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

                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('E'), "Invoice NO.:" + vm.InvoiceNO, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('E'), "DATE:" + CommonCode.GetDateTime3(vm.CreateDate), cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('E'), "PO#:" + vm.POID, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('E'), "Payment Terms:" + vm.TeamOfThePayment, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('A'), "Port of Loading:" + vm.PortName + ", CHINA", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 14, ExcelHelper.GetNumByChar('A'), "Port of Destination:" + vm.DestinationPortName, cellStyle_boldFont);

                    thisRow = 20;

                    int Qty = 0;
                    int BoxQty = 0;
                    decimal SumSalePrice = 0;

                    List<string> list_SkuNumber = new List<string>();

                    foreach (var item in vm.list_OrderProduct_PackingList)
                    {
                        Qty += item.Qty;
                        BoxQty += item.BoxQty ?? 0;
                        SumSalePrice += item.SumSalePrice;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.UPC, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.Desc, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), Keys.USD_Sign + Math.Round(item.SalePrice, 2), cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Keys.USD_Sign + Math.Round(item.SumSalePrice, 2), cellStyle);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), BoxQty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), Qty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Keys.USD_Sign + Math.Round(SumSalePrice, 2), cellStyle2);

                    thisRow += 2;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "SAY TOTAL U.S.DOLLARS " + CommonCode.GetEnMoneyString(SumSalePrice.ToString()) + " ONLY", cellStyle_boldFont);

                    thisRow += 5;

                    foreach (var item in vm.list_Factory)
                    {
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.EnglishName, cellStyle3);
                        ++thisRow;

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.EnglishAddress, cellStyle3);
                        ++thisRow;
                    }

                    thisRow += 4;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('D'), thisRow, ExcelHelper.GetNumByChar('E'), thisRow + 8);

                    #endregion 清关——发票

                    break;

                case MakerTypeEnum.InspectionClearance_PackingList:

                    #region 清关——装箱单

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('H'), "Invoice NO.:" + vm.InvoiceNO, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('H'), "DATE:" + CommonCode.GetDateTime3(vm.CreateDate), cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('H'), "PO#:" + vm.POID, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('H'), "Container#:" + vm.CabinetNumberList, cellStyle_boldFont);

                    thisRow = 14;

                    Qty = 0;
                    BoxQty = 0;
                    decimal? OuterWeightNet = 0;
                    decimal? OuterWeightGross = 0;
                    decimal? OuterVolume = 0;

                    foreach (var item in vm.list_OrderProduct_PackingList)
                    {
                        Qty += item.Qty;
                        BoxQty += item.BoxQty ?? 0;
                        OuterWeightNet += item.OuterWeightNet * item.BoxQty;
                        OuterWeightGross += item.OuterWeightGross * item.BoxQty;
                        OuterVolume += item.OuterVolume * item.BoxQty;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.UPC, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.Desc, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.OuterBoxRate, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.OuterWeightNet * item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), item.OuterWeightGross * item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), item.OuterVolume * item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), item.OuterLength, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), item.OuterWidth, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('K'), item.OuterHeight, cellStyle);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), BoxQty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), Qty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), OuterWeightNet, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), OuterWeightGross, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), OuterVolume, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('K'), "", cellStyle2);

                    thisRow += 5;

                    foreach (var item in vm.list_OrderProduct_PackingList)
                    {
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.UPC, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), CommonCode.ListToString(item.list_Ingredient), cellStyle3);

                        ++thisRow;
                    }

                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('C'), thisRow, ExcelHelper.GetNumByChar('E'), thisRow + 8, 75, 0, 400, 0);

                    #endregion 清关——装箱单

                    break;

                case MakerTypeEnum.InspectionClearance_BeneficiaryStatement:

                    #region 清关——受益人声明

                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('H'), "Invoice NO.:" + vm.InvoiceNO, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('H'), "DATE:" + CommonCode.GetDateTime3(vm.CreateDate), cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('H'), "PO#:" + vm.POID, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('H'), "Container#:" + vm.CabinetNumberList, cellStyle_boldFont);

                    thisRow = 20;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('E'), thisRow, ExcelHelper.GetNumByChar('G'), thisRow + 8, 75, 0, 800, 0);

                    #endregion 清关——受益人声明

                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// S220的结汇
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionExchange_S220<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
        {
            VMInspectionExchange vm = model as VMInspectionExchange;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 11);
            var boldFont_Underline = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 11);
            boldFont_Underline.Underline = FontUnderlineType.Single;

            var bold3 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 11, false);

            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold3, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle3 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold3, HorizontalAlignment.Left, VerticalAlignment.Center, false);

            ICellStyle cellStyle_boldFont = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont);
            ICellStyle cellStyle_boldFont_Underline = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont_Underline);

            int Qty = 0;
            int BoxQty = 0;
            decimal SumSalePrice = 0;
            decimal? OuterWeightNet = 0;
            decimal? OuterWeightGross = 0;
            decimal? OuterVolume = 0;

            int thisRow = 0;

            switch (makerTypeEnum)
            {
                case MakerTypeEnum.InspectionExchange_CommercialInvoice:

                    #region 结汇——发票

                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('E'), "Invoice NO.:" + vm.InvoiceNO, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('E'), "DATE:" + CommonCode.GetDateTime3(vm.CreateDate), cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('E'), "PO#:" + vm.POID, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('E'), "Payment Terms:" + vm.TeamOfThePayment, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('A'), "Port of Loading:" + vm.PortName + ", CHINA", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 14, ExcelHelper.GetNumByChar('A'), "Port of Destination:" + vm.DestinationPortName, cellStyle_boldFont);

                    thisRow = 20;

                    foreach (var item in vm.list_OrderProduct_PackingList)
                    {
                        Qty += item.Qty;
                        BoxQty += item.BoxQty ?? 0;
                        SumSalePrice += item.SumSalePrice;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.UPC, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.Desc, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), Keys.USD_Sign + Math.Round(item.SalePrice, 2), cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Keys.USD_Sign + Math.Round(item.SumSalePrice, 2), cellStyle);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), BoxQty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), Qty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Keys.USD_Sign + Math.Round(SumSalePrice, 2), cellStyle2);

                    thisRow += 2;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "SAY TOTAL U.S.DOLLARS " + CommonCode.GetEnMoneyString(SumSalePrice.ToString()) + " ONLY", cellStyle_boldFont);

                    thisRow += 5;

                    foreach (var item in vm.list_Factory)
                    {
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.EnglishName, cellStyle3);
                        ++thisRow;

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.EnglishAddress, cellStyle3);
                        ++thisRow;
                    }

                    thisRow += 4;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('D'), thisRow, ExcelHelper.GetNumByChar('E'), thisRow + 8);

                    #endregion 结汇——发票

                    break;

                case MakerTypeEnum.InspectionExchange_PackingList:

                    #region 结汇——装箱单

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('H'), "Invoice NO.:" + vm.InvoiceNO, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('H'), "DATE:" + CommonCode.GetDateTime3(vm.CreateDate), cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('H'), "PO#:" + vm.POID, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('H'), "Container#:" + vm.CabinetNumberList, cellStyle_boldFont);

                    thisRow = 14;

                    foreach (var item in vm.list_OrderProduct_PackingList)
                    {
                        Qty += item.Qty;
                        BoxQty += item.BoxQty ?? 0;
                        OuterWeightNet += item.OuterWeightNet * item.BoxQty;
                        OuterWeightGross += item.OuterWeightGross * item.BoxQty;
                        OuterVolume += item.OuterVolume * item.BoxQty;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.UPC, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.Desc, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.OuterBoxRate, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.OuterWeightNet * item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), item.OuterWeightGross * item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), item.OuterVolume * item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), item.OuterLength, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), item.OuterWidth, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('K'), item.OuterHeight, cellStyle);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), BoxQty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), Qty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), OuterWeightNet, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), OuterWeightGross, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), OuterVolume, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('K'), "", cellStyle2);

                    thisRow += 5;

                    foreach (var item in vm.list_OrderProduct_PackingList)
                    {
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.UPC, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), CommonCode.ListToString(item.list_Ingredient), cellStyle3);

                        ++thisRow;
                    }

                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('C'), thisRow, ExcelHelper.GetNumByChar('E'), thisRow + 8, 75, 0, 400, 0);

                    #endregion 结汇——装箱单

                    break;

                case MakerTypeEnum.InspectionExchange_BeneficiaryStatement:

                    #region 结汇——受益人声明

                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('H'), "Invoice NO.:" + vm.InvoiceNO, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('H'), "DATE:" + CommonCode.GetDateTime3(vm.CreateDate), cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('H'), "PO#:" + vm.POID, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('H'), "Container#:" + vm.CabinetNumberList, cellStyle_boldFont);

                    thisRow = 20;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('E'), thisRow, ExcelHelper.GetNumByChar('G'), thisRow + 8, 75, 0, 800, 0);

                    #endregion 结汇——受益人声明

                    break;

                default:
                    break;
            }
        }
    }
}