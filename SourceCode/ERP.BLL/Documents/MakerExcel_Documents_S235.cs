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
    internal class MakerExcel_Documents_S235
    {
        /// <summary>
        /// S235的报检
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionReceipt_S235<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
        {
            DTOInspectionReceipt vm = model as DTOInspectionReceipt;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);
            var boldFont3 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12, false);
            var boldFont2 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 11);
            var boldFont4 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 11, false);

            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3, HorizontalAlignment.Center, VerticalAlignment.Center, true);//有边框不加粗 罗马字体
            ICellStyle cellStyle2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, true);//有边框有加粗 罗马

            ICellStyle cellStyle3 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont4, HorizontalAlignment.Center, VerticalAlignment.Center, true);//有边框 不加粗 Arial
            ICellStyle cellStyle4 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Center, VerticalAlignment.Center, true);//有边框 有加粗 Arial

            ICellStyle cellStyle_boldFont = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont);//字体加粗 罗马
            ICellStyle cellStyle_boldFont2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2);//字体加粗 Arial

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
                    ExcelHelper.SetCellValue(sheet, 5, ExcelHelper.GetNumByChar('B'), vm.InvNo
                        , cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('B'), vm.ProductsQuauity, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('B'), vm.ProductsBoxNum, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('B'), Keys.USD_Sign + vm.ProductAmount, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('B'), "请于" + vm.ClaimFaxDate + "日之前安排" + vm.INTypeName + "并提供通关单给我司，谢谢！", cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 14, ExcelHelper.GetNumByChar('E'), vm.CreateUserName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 15, ExcelHelper.GetNumByChar('E'), vm.CreateDateForamtter, cellStyle_boldFont);

                    #endregion 报检——报检首页

                    break;

                case MakerTypeEnum.InspectionReceipt_PackingList:

                    #region 报检——装箱单

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('A'), "TO: " + vm.CustomerName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('E'), "Inv. No.:  " + vm.InvNo, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('E'), "Date:  " + CommonCode.GetDateTime3(vm.CreateDate), cellStyle_boldFont);

                    thisRow = 14;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), vm.HsEngName, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), vm.ProductsBoxNum, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), vm.ProductsQuauity, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), vm.WeightGrossSum, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), vm.WeightNetSum, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), vm.CUFT, cellStyle);

                    ++thisRow;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), vm.ProductsBoxNum, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), vm.ProductsQuauity, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), vm.WeightGrossSum, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), vm.WeightNetSum, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), vm.CUFT, cellStyle2);

                    #endregion 报检——装箱单

                    break;

                case MakerTypeEnum.InspectionReceipt_CommercialInvoice:

                    #region 报检——发票

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('D'), "Inv. No.:" + vm.InvNo);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('D'), "Date: " + CommonCode.GetDateTime3(vm.CreateDate));
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('A'), "Loading port:" + vm.INPortName + " ,CHINA", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('A'), "Destination: " + vm.INEndPortName + " ,USA", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 14, ExcelHelper.GetNumByChar('D'), "Terms of Sale: FOB " + vm.INPortName, cellStyle_boldFont);

                    thisRow = 17;

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

                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('D'), "S/C NO.: " + vm.InvNo, cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('D'), "DATE:" + CommonCode.GetDateTime3(vm.CustomerDate), cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('A'), "DESTINATION: " + vm.INEndPortName, cellStyle_boldFont2);

                    ExcelHelper.SetCellValue(sheet, 18, ExcelHelper.GetNumByChar('D'), "          FOB " + vm.INPortName, cellStyle_boldFont2);

                    ExcelHelper.SetCellValue(sheet, 21, ExcelHelper.GetNumByChar('B'), vm.HsEngName, cellStyle4);
                    ExcelHelper.SetCellValue(sheet, 21, ExcelHelper.GetNumByChar('C'), vm.ProductsQuauity, cellStyle4);
                    ExcelHelper.SetCellValue(sheet, 21, ExcelHelper.GetNumByChar('D'), Keys.USD_Sign + vm.ProductPrice, cellStyle4);
                    ExcelHelper.SetCellValue(sheet, 21, ExcelHelper.GetNumByChar('E'), Keys.USD_Sign + vm.ProductAmount, cellStyle4);

                    ExcelHelper.SetCellValue(sheet, 22, ExcelHelper.GetNumByChar('B'), "TOTAL", cellStyle4);
                    ExcelHelper.SetCellValue(sheet, 22, ExcelHelper.GetNumByChar('C'), vm.ProductsQuauity, cellStyle4);
                    ExcelHelper.SetCellValue(sheet, 22, ExcelHelper.GetNumByChar('D'), "", cellStyle4);
                    ExcelHelper.SetCellValue(sheet, 22, ExcelHelper.GetNumByChar('E'), Keys.USD_Sign + vm.ProductAmount, cellStyle4);

                    ExcelHelper.SetCellValue(sheet, 25, ExcelHelper.GetNumByChar('A'), "1.SHIPMENT DATE:" + vm.ShippingDateStart, cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, 26, ExcelHelper.GetNumByChar('A'), "2.SHIPPING MARK:" + "TO BE ADVISED .", cellStyle_boldFont2);

                    ExcelHelper.SetCellValue(sheet, 27, ExcelHelper.GetNumByChar('A'), "3.TERM OF THE PAYMENT:" + vm.TeamOfThePayment, cellStyle_boldFont2);

                    #endregion 报检——销售合同

                    break;

                case MakerTypeEnum.InspectionReceipt_Commission:
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// S235的报关
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionCustoms_S235<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
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
                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('E'), "Inv. No.:" + vm.InvoiceNO, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('E'), "Date: " + CommonCode.GetDateTime3(vm.CreateDate));
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle_boldFont);

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

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "", cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.HsEngName, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.ProductsBoxNum, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.Qty, cellStyle);

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.SumOuterWeightGross, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.SumOuterWeightNet, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), item.SumOuterVolume, cellStyle);
                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), ProductsBoxNum, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), Qty, cellStyle2);

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
                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('D'), "Inv. No.:" + vm.InvoiceNO);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('D'), "Date: " + CommonCode.GetDateTime3(vm.CreateDate));
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.CustomerReg);

                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('A'), "Loading port:   " + vm.PortName + ",CHINA"
                        , cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('A'), "Destination: " + vm.DestinationPortName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 14, ExcelHelper.GetNumByChar('D'), "                                         Terms of Sale: " + vm.TradeType + "  " + vm.PortName, cellStyle_boldFont);

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

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "", cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.HsEngName, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), Keys.USD_Sign + item.ProductPrice, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), Keys.USD_Sign + item.Amount, cellStyle);
                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), Qty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle);
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
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('D'), "DATE:" + CommonCode.GetDateTime3(vm.CustomerDate), cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle_boldFont);

                    //ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('A'), "LOADING PORT:" + vm.PortName + ",CHINA", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('A'), "DESTINATION:" + vm.DestinationPortName, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 18, ExcelHelper.GetNumByChar('D'), "          FOB " + vm.PortName, cellStyle_boldFont);

                    thisRow = 21;
                    ProductsBoxNum = 0;
                    Qty = 0;
                    Amount = 0;
                    foreach (var item in vm.list_OrdersProducts)
                    {
                        ProductsBoxNum += item.ProductsBoxNum;
                        Qty += item.Qty;
                        Amount += item.Amount;
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "", cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.HsEngName, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), Keys.USD_Sign + item.ProductPrice, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), Keys.USD_Sign + item.Amount, cellStyle);
                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "TOTAL:", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), Qty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), Keys.USD_Sign + Amount, cellStyle2);
                    thisRow += 2;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "REMARKS:", cellStyle_boldFont);
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "1.SHIPPING DATE:ON/BEFORE " + vm.ShippingDateStart, cellStyle_boldFont);
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "2.SHIPPING MARK:  TO BE ADVISED", cellStyle_boldFont);
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "3. TERMS OF PAYMENT : " + vm.TeamOfThePayment, cellStyle_boldFont);

                    thisRow += 4;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "THE  SELLERS :", cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "THE BUYERS :", cellStyle_boldFont2);

                    ExcelHelper.CreatePicture(workbook, sheet, "/data/Documents/S188/tom.png", 75, 0, 800, 0, ExcelHelper.GetNumByChar('A'), thisRow + 1, ExcelHelper.GetNumByChar('A'), thisRow + 4);

                    ExcelHelper.CreatePicture(workbook, sheet, "/data/Documents/S235/CBALORA.png", 75, 0, 800, 0, ExcelHelper.GetNumByChar('D'), thisRow + 1, ExcelHelper.GetNumByChar('D'), thisRow + 6);
                    thisRow += 2;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('B'), thisRow, ExcelHelper.GetNumByChar('C'), thisRow + 6);//哲联公章

                    #endregion 报关——销售合同

                    break;

                #endregion 报关

                default:
                    break;
            }
        }

        /// <summary>
        /// S235的清关
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionClearance_S235<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
        {
            VMInspectionClearance vm = model as VMInspectionClearance;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 10);
            var boldFont2 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 11);
            var boldFont_Underline = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 10);
            boldFont_Underline.Underline = FontUnderlineType.Single;

            var bold3 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 10, false);
            var bold4 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 11, false);

            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold3, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle3 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold3, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle3.WrapText = false;

            ICellStyle cellStyle4 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold4, HorizontalAlignment.Left, VerticalAlignment.Center, false);

            cellStyle4.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle4.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;

            ICellStyle cellStyle6 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold4, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle6.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;

            ICellStyle cellStyle5 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold4, HorizontalAlignment.Left, VerticalAlignment.Center, true);

            ICellStyle cellStyle_boldFont = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2);
            ICellStyle cellStyle_boldFont_Underline = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont_Underline);

            int thisRow = 0;

            switch (makerTypeEnum)
            {
                case MakerTypeEnum.InspectionClearance_CommercialInvoice:

                    #region 清关——发票

                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('G'), "Invoice No.:" + vm.InvoiceNO, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('G'), "Invoice Date:" + CommonCode.GetDateTime3(vm.CreateDate), cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('G'), "PO#:" + vm.POID, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('B'), "Port of Loading:" + vm.PortName + ", CHINA", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 14, ExcelHelper.GetNumByChar('B'), "Port of Destination:" + vm.DestinationPortName + ", USA", cellStyle_boldFont);

                    thisRow = 19;

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

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.UPC, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.No, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.Desc, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), Keys.USD_Sign + Math.Round(item.SalePrice, 2), cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), Keys.USD_Sign + Math.Round(item.SumSalePrice, 2), cellStyle);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "TOTAL", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), BoxQty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Qty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), Keys.USD_Sign + Math.Round(SumSalePrice, 2), cellStyle2);

                    thisRow += 2;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "SAY TOTAL U.S.DOLLARS " + CommonCode.GetEnMoneyString(SumSalePrice.ToString()) + " ONLY", cellStyle_boldFont);

                    thisRow += 4;

                    thisRow = ExcelHelper.Insert_FactoryEnglishName_Address(sheet, vm.list_OrderProduct_PackingList, thisRow, cellStyle_boldFont, 'B');

                    thisRow += 10;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('E'), thisRow, ExcelHelper.GetNumByChar('G'), thisRow + 8);

                    #endregion 清关——发票

                    break;

                case MakerTypeEnum.InspectionClearance_PackingList:

                    #region 清关——装箱单

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('I'), "Invoice No.:" + vm.InvoiceNO, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('I'), "Date:" + CommonCode.GetDateTime3(vm.CreateDate), cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('I'), "PO#:" + vm.POID, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('A'), "Ocean Vessel / Voy No. :" + vm.OceanVessel, cellStyle_boldFont);

                    thisRow = 11;
                    foreach (var item in vm.list_CabinetNumber_Cabinet_SealingNumber)
                    {
                        string temp = "Container\\Size\\Seal No.:" + item;
                        if (thisRow > 11)
                        {
                            temp = "                                           " + item;
                        }
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), temp, cellStyle_boldFont);

                        ++thisRow;
                    }

                    thisRow += 4;

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
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.No, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.Desc, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.OuterBoxRate, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), item.OuterWeightNet * item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), item.OuterWeightGross * item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), item.OuterVolume * item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), item.OuterHeight, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('K'), item.OuterWidth, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('L'), item.OuterLength, cellStyle);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Qty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), OuterWeightNet, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), OuterWeightGross, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), OuterVolume, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('K'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('L'), "", cellStyle2);

                    thisRow += 5;
                    foreach (var item in vm.list_OrderProduct_PackingList)
                    {
                        if (item.list_Ingredient != null && item.list_Ingredient.Count > 0)
                        {
                            ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.UPC, cellStyle3);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), CommonCode.ListToString(item.list_Ingredient), cellStyle3);
                            thisRow++;
                        }
                    }

                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('J'), thisRow, ExcelHelper.GetNumByChar('L'), thisRow + 8);

                    #endregion 清关——装箱单

                    break;

                case MakerTypeEnum.InspectionClearance_CertificateOfOrigin:

                    #region 清关——原产地证明

                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('I'), "Inv. No.:" + vm.InvoiceNO, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('I'), "Date:" + CommonCode.GetDateTime3(vm.CreateDate), cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('I'), "P.O.#:" + vm.POID, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 18, ExcelHelper.GetNumByChar('A'), "WE HEREBY CERTIFY THAT THE GOODS UNDER INVOICE NO.: " + vm.InvoiceNO + " WERE ALL MADE IN CHINA.", cellStyle_boldFont);

                    thisRow = 21;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('F'), thisRow, ExcelHelper.GetNumByChar('H'), thisRow + 8, 75, 0, 700, 0);

                    #endregion 清关——原产地证明

                    break;

                case MakerTypeEnum.InspectionClearance_PackingListBuContainer:

                    #region 清关——装箱单

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('I'), "Invoice No.:" + vm.InvoiceNO, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('I'), "Date:" + CommonCode.GetDateTime3(vm.CreateDate), cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('I'), "PO#:" + vm.POID, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('A'), "Container / Seal No.:" + CommonCode.ListToString(vm.list_CabinetNumber_Cabinet_SealingNumber), cellStyle_boldFont);

                    thisRow = 14;

                    Qty = 0;
                    BoxQty = 0;
                    OuterWeightNet = 0;
                    OuterWeightGross = 0;
                    OuterVolume = 0;

                    foreach (var item in vm.list_OrderProduct_PackingList)
                    {
                        Qty += item.Qty;
                        BoxQty += item.BoxQty ?? 0;
                        OuterWeightNet += item.OuterWeightNet * item.BoxQty;
                        OuterWeightGross += item.OuterWeightGross * item.BoxQty;
                        OuterVolume += item.OuterVolume * item.BoxQty;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.UPC, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.No, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.Desc, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.OuterBoxRate, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), item.OuterWeightNet * item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), item.OuterWeightGross * item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), item.OuterVolume * item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), item.OuterHeight, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('K'), item.OuterWidth, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('L'), item.OuterLength, cellStyle);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Qty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), OuterWeightNet, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), OuterWeightGross, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), OuterVolume, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('K'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('L'), "", cellStyle2);

                    thisRow += 5;
                    foreach (var item in vm.list_OrderProduct_PackingList)
                    {
                        if (item.list_Ingredient != null && item.list_Ingredient.Count > 0)
                        {
                            ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.UPC, cellStyle3);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), CommonCode.ListToString(item.list_Ingredient), cellStyle3);
                            thisRow++;
                        }
                    }

                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('J'), thisRow, ExcelHelper.GetNumByChar('L'), thisRow + 8);

                    #endregion 清关——装箱单

                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// S235的结汇
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionExchange_S235<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
        {
            VMInspectionExchange vm = model as VMInspectionExchange;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 10);
            var boldFont2 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 11);
            var boldFont_Underline = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 10);
            boldFont_Underline.Underline = FontUnderlineType.Single;

            var bold3 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 10, false);
            var bold4 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 11, false);

            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold3, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle3 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold3, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle3.WrapText = false;

            ICellStyle cellStyle4 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold4, HorizontalAlignment.Left, VerticalAlignment.Center, false);

            cellStyle4.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle4.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;

            ICellStyle cellStyle6 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold4, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle6.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;

            ICellStyle cellStyle5 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, bold4, HorizontalAlignment.Left, VerticalAlignment.Center, true);

            ICellStyle cellStyle_boldFont = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2);
            ICellStyle cellStyle_boldFont_Underline = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont_Underline);

            int thisRow = 0;

            switch (makerTypeEnum)
            {
                case MakerTypeEnum.InspectionExchange_CommercialInvoice:

                    #region 结汇——发票

                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('G'), "Invoice No.:" + vm.InvoiceNO, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('G'), "Invoice Date:" + CommonCode.GetDateTime3(vm.CreateDate), cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('G'), "PO#:" + vm.POID, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('B'), "Port of Loading:" + vm.PortName + ", CHINA", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 14, ExcelHelper.GetNumByChar('B'), "Port of Destination:" + vm.DestinationPortName + ", USA", cellStyle_boldFont);

                    thisRow = 19;

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

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.UPC, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.No, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.Desc, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), Keys.USD_Sign + Math.Round(item.SalePrice, 2), cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), Keys.USD_Sign + Math.Round(item.SumSalePrice, 2), cellStyle);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "TOTAL", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), BoxQty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Qty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), Keys.USD_Sign + Math.Round(SumSalePrice, 2), cellStyle2);

                    thisRow += 2;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "SAY TOTAL U.S.DOLLARS " + CommonCode.GetEnMoneyString(SumSalePrice.ToString()) + " ONLY", cellStyle_boldFont);

                    thisRow += 4;

                    thisRow = ExcelHelper.Insert_FactoryEnglishName_Address(sheet, vm.list_OrderProduct_PackingList, thisRow, cellStyle_boldFont, 'B');

                    thisRow += 10;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('E'), thisRow, ExcelHelper.GetNumByChar('G'), thisRow + 8);

                    #endregion 结汇——发票

                    break;

                case MakerTypeEnum.InspectionExchange_PackingList:

                    #region 结汇——装箱单

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('I'), "Invoice No.:" + vm.InvoiceNO, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('I'), "Date:" + CommonCode.GetDateTime3(vm.CreateDate), cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('I'), "PO#:" + vm.POID, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('A'), "Ocean Vessel / Voy No. :" + vm.OceanVessel, cellStyle_boldFont);

                    thisRow = 11;
                    foreach (var item in vm.list_CabinetNumber_Cabinet_SealingNumber)
                    {
                        string temp = "Container\\Size\\Seal No.:" + item;
                        if (thisRow > 11)
                        {
                            temp = "                                           " + item;
                        }
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), temp, cellStyle_boldFont);

                        ++thisRow;
                    }

                    thisRow += 4;

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
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.No, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.Desc, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.OuterBoxRate, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), item.OuterWeightNet * item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), item.OuterWeightGross * item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), item.OuterVolume * item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), item.OuterHeight, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('K'), item.OuterWidth, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('L'), item.OuterLength, cellStyle);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Qty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), OuterWeightNet, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), OuterWeightGross, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), OuterVolume, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('K'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('L'), "", cellStyle2);

                    thisRow += 5;
                    foreach (var item in vm.list_OrderProduct_PackingList)
                    {
                        if (item.list_Ingredient != null && item.list_Ingredient.Count > 0)
                        {
                            ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.UPC, cellStyle3);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), CommonCode.ListToString(item.list_Ingredient), cellStyle3);
                            thisRow++;
                        }
                    }

                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('J'), thisRow, ExcelHelper.GetNumByChar('L'), thisRow + 8);

                    #endregion 结汇——装箱单

                    break;

                case MakerTypeEnum.InspectionExchange_CertificateOfOrigin:

                    #region 结汇——原产地证明

                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('I'), "Inv. No.:" + vm.InvoiceNO, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('I'), "Date:" + CommonCode.GetDateTime3(vm.CreateDate), cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('I'), "P.O.#:" + vm.POID, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 18, ExcelHelper.GetNumByChar('A'), "WE HEREBY CERTIFY THAT THE GOODS UNDER INVOICE NO.: " + vm.InvoiceNO + " WERE ALL MADE IN CHINA.", cellStyle_boldFont);

                    thisRow = 21;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('F'), thisRow, ExcelHelper.GetNumByChar('H'), thisRow + 8, 75, 0, 700, 0);

                    #endregion 结汇——原产地证明

                    break;

                case MakerTypeEnum.InspectionExchange_PackingListBuContainer:

                    #region 结汇——装箱单

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('I'), "Invoice No.:" + vm.InvoiceNO, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('I'), "Date:" + CommonCode.GetDateTime3(vm.CreateDate), cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('I'), "PO#:" + vm.POID, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('A'), "Container / Seal No.:" + CommonCode.ListToString(vm.list_CabinetNumber_Cabinet_SealingNumber), cellStyle_boldFont);

                    thisRow = 14;

                    Qty = 0;
                    BoxQty = 0;
                    OuterWeightNet = 0;
                    OuterWeightGross = 0;
                    OuterVolume = 0;

                    foreach (var item in vm.list_OrderProduct_PackingList)
                    {
                        Qty += item.Qty;
                        BoxQty += item.BoxQty ?? 0;
                        OuterWeightNet += item.OuterWeightNet * item.BoxQty;
                        OuterWeightGross += item.OuterWeightGross * item.BoxQty;
                        OuterVolume += item.OuterVolume * item.BoxQty;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.UPC, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.No, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.Desc, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.OuterBoxRate, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), item.OuterWeightNet * item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), item.OuterWeightGross * item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), item.OuterVolume * item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), item.OuterHeight, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('K'), item.OuterWidth, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('L'), item.OuterLength, cellStyle);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Qty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), OuterWeightNet, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), OuterWeightGross, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), OuterVolume, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('K'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('L'), "", cellStyle2);

                    thisRow += 5;
                    foreach (var item in vm.list_OrderProduct_PackingList)
                    {
                        if (item.list_Ingredient != null && item.list_Ingredient.Count > 0)
                        {
                            ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.UPC, cellStyle3);
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), CommonCode.ListToString(item.list_Ingredient), cellStyle3);
                            thisRow++;
                        }
                    }

                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('J'), thisRow, ExcelHelper.GetNumByChar('L'), thisRow + 8);

                    #endregion 结汇——装箱单

                    break;

                default:
                    break;
            }
        }
    }
}