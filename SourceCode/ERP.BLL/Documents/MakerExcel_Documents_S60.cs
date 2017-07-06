using ERP.BLL.ERP.Consts;
using ERP.Models.CustomEnums;
using ERP.Models.Factory;
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
    public class MakerExcel_Documents_S60
    {
        /// <summary>
        /// S60的报检
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionReceipt_S60<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
        {
            DTOInspectionReceipt vm = model as DTOInspectionReceipt;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12, false);//不加粗
            var boldFont2 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);//加粗

            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, true);//有边框不加粗
            ICellStyle cellStyle2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Center, VerticalAlignment.Center, true);//有边框有加粗
            ICellStyle cellStyle_boldFont = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont);//字体加粗
            ICellStyle cellStyle_boldFont2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2);//字体加粗

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
                    ExcelHelper.SetCellValue(sheet, 3, ExcelHelper.GetNumByChar('B'), vm.HsEngName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 4, ExcelHelper.GetNumByChar('B'), vm.HSCodeName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 5, ExcelHelper.GetNumByChar('B'), vm.SCNO, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('B'), vm.ProductsQuauity, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('B'), vm.ProductsBoxNum, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('B'), vm.ProductAmount, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), "由", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('B'), vm.PortName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('C'), "海运到", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('D'), "美国", cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('B'), vm.INTypeName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('D'), vm.ClaimFaxDate, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('A'), "或" + vm.INTypeName + "报检单", cellStyle_boldFont);//TODO:熏蒸的货物还需要熏蒸草稿

                    ExcelHelper.SetCellValue(sheet, 14, ExcelHelper.GetNumByChar('F'), vm.CreateUserName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 15, ExcelHelper.GetNumByChar('F'), vm.CreateDateForamtter, cellStyle_boldFont);

                    #endregion 报检——报检首页

                    break;

                case MakerTypeEnum.InspectionReceipt_PackingList:

                    #region 报检——装箱单

                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), "TO:" + vm.DestinationPort_CompanyName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('E'), vm.InvNo);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.DestinationPort_StreetAddress, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('E'), vm.POID);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.DestinationPort_Address, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('E'), CommonCode.GetDateTime3(vm.CreateDate));

                    thisRow = 15;
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

                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), "TO:" + vm.DestinationPort_CompanyName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('D'), vm.InvNo);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.DestinationPort_StreetAddress, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('D'), vm.POID);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.DestinationPort_Address, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('D'), CommonCode.GetDateTime3(vm.CreateDate));

                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('A'), "Loading port:     " + vm.INPortName + " ,CHINA", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('A'), "Destination:     " + vm.INEndPortName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 15, ExcelHelper.GetNumByChar('D'), vm.TradeTypeName + " " + vm.INPortName, cellStyle_boldFont);

                    thisRow = 18;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), vm.HsEngName, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), vm.ProductsQuauity, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), vm.ProductPrice, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), vm.ProductAmount, cellStyle);

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "Total:", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), vm.ProductsQuauity, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, 19, ExcelHelper.GetNumByChar('D'), vm.ProductAmount, cellStyle2);

                    #endregion 报检——发票

                    break;

                case MakerTypeEnum.InspectionReceipt_SaleContract:

                    #region 报检——销售合同

                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), "TO:" + vm.DestinationPort_CompanyName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('D'), vm.InvNo, cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.DestinationPort_StreetAddress, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('D'), "DATE:" + CommonCode.GetDateTime3(vm.CustomerDate), cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('A'), vm.DestinationPort_Address, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 17, ExcelHelper.GetNumByChar('D'), vm.TradeTypeName + " " + vm.INPortName, cellStyle_boldFont2);

                    ExcelHelper.SetCellValue(sheet, 20, ExcelHelper.GetNumByChar('A'), vm.HsEngName, cellStyle);
                    ExcelHelper.SetCellValue(sheet, 20, ExcelHelper.GetNumByChar('B'), vm.ProductsQuauity, cellStyle);
                    ExcelHelper.SetCellValue(sheet, 20, ExcelHelper.GetNumByChar('C'), Keys.USD_Sign + vm.ProductPrice, cellStyle);
                    ExcelHelper.SetCellValue(sheet, 20, ExcelHelper.GetNumByChar('D'), Keys.USD_Sign + vm.ProductAmount, cellStyle);
                    ExcelHelper.SetCellValue(sheet, 21, ExcelHelper.GetNumByChar('A'), "TOTAL", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, 21, ExcelHelper.GetNumByChar('C'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, 21, ExcelHelper.GetNumByChar('B'), vm.ProductsQuauity, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, 21, ExcelHelper.GetNumByChar('D'), vm.ProductAmount, cellStyle2);

                    ExcelHelper.SetCellValue(sheet, 24, ExcelHelper.GetNumByChar('A'), "1.SHIPMENT DATE:" + vm.ShippingDateStart, cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, 25, ExcelHelper.GetNumByChar('A'), "2.SHIPPING MARKS:" + "TO BE ADVISED BY THE BUYER", cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, 26, ExcelHelper.GetNumByChar('A'), "3.TERM OF THE PAYMENT:" + vm.TeamOfThePayment, cellStyle_boldFont2);

                    #endregion 报检——销售合同

                    break;

                case MakerTypeEnum.InspectionReceipt_Commission:
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// S60的报关
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionCustoms_S60<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
        {
            VMInspectionCustoms vm = model as VMInspectionCustoms;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);
            ICellStyle cellStyle_boldFont2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont);
            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, null, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle_boldFont = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont);//字体加粗

            IFont boldFont_IsItalic = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);
            boldFont_IsItalic.IsItalic = true;
            ICellStyle cellStyle_boldFont_IsItalic = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont_IsItalic);
            int thisRow = 0;

            switch (makerTypeEnum)
            {
                #region 报关

                case MakerTypeEnum.InspectionCustoms_PackingList:

                    #region 报关——装箱单

                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), "TO:" + vm.DestinationPort_CompanyName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('E'), vm.InvoiceNO);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.DestinationPort_StreetAddress, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('E'), vm.POID);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.DestinationPort_Address, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('E'), CommonCode.GetDateTime3(vm.CreateDate));

                    thisRow = 15;

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

                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), "TO:" + vm.DestinationPort_CompanyName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('D'), vm.InvoiceNO);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.DestinationPort_StreetAddress, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('D'), vm.POID);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.DestinationPort_Address, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('D'), CommonCode.GetDateTime3(vm.CreateDate));

                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('A'), "Loading port:   " + vm.PortName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('A'), "Destination: " + vm.DestinationPortName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 15, ExcelHelper.GetNumByChar('D'), vm.PortName + " ", cellStyle_boldFont);

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

                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), "TO:" + vm.DestinationPort_CompanyName, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('D'), vm.InvoiceNO, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.DestinationPort_StreetAddress, cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('D'), CommonCode.GetDateTime3(vm.CustomerDate), cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('A'), vm.DestinationPort_Address, cellStyle_boldFont);

                    ExcelHelper.SetCellValue(sheet, 16, ExcelHelper.GetNumByChar('D'), vm.PortName + " ", cellStyle_boldFont);

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
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "1.SHIPPING DATE:" + vm.ShippingDateStart, cellStyle_boldFont);
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "2.SHIPPING MARKS:  TO BE ADVISED", cellStyle_boldFont);
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "3.TERM OF THE PAYMENT : " + vm.TeamOfThePayment, cellStyle_boldFont);

                    thisRow += 2;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "THE  SELLERS :", cellStyle_boldFont2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "THE BUYERS :", cellStyle_boldFont2);

                    ExcelHelper.CreatePicture(workbook, sheet, "/data/Documents/S188/tom.png", 75, 0, 800, 0, ExcelHelper.GetNumByChar('A'), thisRow + 1, ExcelHelper.GetNumByChar('A'), thisRow + 4);

                    ExcelHelper.CreatePicture(workbook, sheet, "/data/Documents/S60/Relecca.png", 75, 0, 800, 0, ExcelHelper.GetNumByChar('D'), thisRow + 2, ExcelHelper.GetNumByChar('D'), thisRow + 3);
                    thisRow += 1;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('B'), thisRow, ExcelHelper.GetNumByChar('C'), thisRow + 6);

                    #endregion 报关——销售合同

                    break;

                #endregion 报关

                default:
                    break;
            }
        }

        /// <summary>
        /// S60的结汇
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionExchange_S60<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
        {
            VMInspectionExchange vm = model as VMInspectionExchange;

            var font_12 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12, false);
            var font_12_bold = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12, true);

            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, font_12, HorizontalAlignment.Center, VerticalAlignment.Center, true);

            ICellStyle cellStyle_12_center_border = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, font_12, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle_12_bold_center_border = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, font_12_bold, HorizontalAlignment.Center, VerticalAlignment.Center, true);

            ICellStyle cellStyle_12_bold_left = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, font_12_bold, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle_12_bold_left.WrapText = false;

            ICellStyle cellStyle_12_left = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, font_12, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle_12_left.WrapText = false;

            int thisRow = 0;

            switch (makerTypeEnum)
            {
                case MakerTypeEnum.InspectionExchange_CommercialInvoice:

                    #region 结汇——发票

                    //S60要求固定
                    //ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName);
                    //ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet);
                    //ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.CustomerReg);

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('G'), "Inv. No.:" + vm.InvoiceNO, cellStyle_12_left);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('G'), "BIG LOTS PO NO.:" + vm.POID, cellStyle_12_left);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('G'), "EHI PO NO.:" + vm.EHIPO, cellStyle_12_left);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('G'), "Date:" + CommonCode.GetDateTime3(vm.CreateDate), cellStyle_12_left);

                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('A'), "Loading port:" + vm.PortName, cellStyle_12_bold_left);
                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('A'), "Destination:" + vm.DestinationPortName, cellStyle_12_bold_left);
                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('A'), "Terms of sale: " + vm.TradeTypeName + " " + vm.PortName, cellStyle_12_bold_left);
                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('A'), "Terms of payment:" + vm.PaymentType, cellStyle_12_bold_left);

                    thisRow = 15;
                    thisRow = 16;

                    int SumQty = 0;
                    int SumBoxQty = 0;
                    decimal SumSalePrice = 0;

                    foreach (var item in vm.list_OrderProduct_PackingList)
                    {
                        SumQty += item.Qty;
                        SumBoxQty += item.BoxQty ?? 0;
                        SumSalePrice += item.SumSalePrice;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.POID, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.SkuNumber, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.No, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.Desc, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.HSCode, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), Keys.USD_Sign + Math.Round(item.SalePrice, 2), cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), Keys.USD_Sign + Math.Round(item.SumSalePrice, 2), cellStyle);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle_12_bold_center_border);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle_12_bold_center_border);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle_12_bold_center_border);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), SumBoxQty, cellStyle_12_bold_center_border);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle_12_bold_center_border);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), "", cellStyle_12_bold_center_border);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), SumQty, cellStyle_12_bold_center_border);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), "", cellStyle_12_bold_center_border);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), Keys.USD_Sign + Math.Round(SumSalePrice, 2), cellStyle_12_bold_center_border);

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:U. S. DOLLARS " + CommonCode.GetEnMoneyString(SumSalePrice.ToString()) + " ONLY .", cellStyle_12_bold_left);

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "WE HEREBY CERTIFY THAT THE GOODS UNDER INV. NO: " + vm.InvoiceNO + " WERE ALL MADE IN CHINA. ", cellStyle_12_bold_left);

                    thisRow += 5;
                    thisRow = ExcelHelper.Insert_FactoryEnglishName_Address(sheet, vm.list_OrderProduct_PackingList, thisRow, cellStyle_12_bold_left);

                    thisRow += 3;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('G'), thisRow, ExcelHelper.GetNumByChar('I'), thisRow + 5);

                    #endregion 结汇——发票

                    break;

                case MakerTypeEnum.InspectionExchange_PackingList:

                    #region 结汇——装箱单

                    //ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName);
                    //ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet);
                    //ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.CustomerReg);

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('G'), "Inv. No.:" + vm.InvoiceNO,cellStyle_12_left);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('G'), "BIG LOTS PO NO.:" + vm.POID, cellStyle_12_left);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('G'), "EHI PO NO.:" + vm.EHIPO, cellStyle_12_left);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('G'), "Date:" + CommonCode.GetDateTime3(vm.CreateDate), cellStyle_12_left);

                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('A'), "Loading port:" + vm.PortName + " ,CHINA", cellStyle_12_bold_left);
                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('A'), "Destination:" + vm.DestinationPortName, cellStyle_12_bold_left);

                    thisRow = 15;

                    int Qty = 0;
                    int? BoxQty = 0;
                    decimal? SumOuterWeightGross = 0;
                    decimal? SumOuterWeightNet = 0;
                    decimal? SumOuterVolume = 0;

                    foreach (var item in vm.list_OrderProduct_PackingList)
                    {
                        Qty += item.Qty;
                        BoxQty += item.BoxQty;
                        SumOuterWeightGross += item.SumOuterWeightGross;
                        SumOuterWeightNet += item.SumOuterWeightNet;
                        SumOuterVolume += item.SumOuterVolume;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.POID, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.SkuNumber, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.No, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.Desc, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.OuterBoxRate + "PCS/CTN", cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.Qty, cellStyle);

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), item.BoxQty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), item.SumOuterWeightGross, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), item.SumOuterWeightNet, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), item.SumOuterVolume, cellStyle);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle_12_bold_center_border);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle_12_bold_center_border);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle_12_bold_center_border);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle_12_bold_center_border);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle_12_bold_center_border);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Qty, cellStyle_12_bold_center_border);

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), BoxQty, cellStyle_12_bold_center_border);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), SumOuterWeightGross, cellStyle_12_bold_center_border);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), SumOuterWeightNet, cellStyle_12_bold_center_border);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), SumOuterVolume, cellStyle_12_bold_center_border);

                    thisRow += 4;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('G'), thisRow, ExcelHelper.GetNumByChar('I'), thisRow + 5);

                    #endregion 结汇——装箱单

                    break;

                default:
                    break;
            }
        }
    }
}