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
    internal class MakerExcel_Documents_S56
    {
        /// <summary>
        /// S56的报检
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionReceipt_S56<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
        {
            DTOInspectionReceipt vm = model as DTOInspectionReceipt;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);//加粗，罗马字体
            var boldFont2 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12, false);//不加粗，罗马字体
            var boldFont3 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 11);//加粗，Arial
            var boldFont4 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 14);//加粗，Arial
            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Center, VerticalAlignment.Center, false);//无边框，不加粗，罗马字体          
            ICellStyle cellStyle3 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Center, VerticalAlignment.Center, true);//有边框，不加粗，罗马字体

            ICellStyle cellStyle2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, true);//有边框，加粗，罗马字体
            ICellStyle cellStyle4 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, false);//无边框，加粗，罗马字体

            ICellStyle cellStyle5 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Left, VerticalAlignment.Center, false);//无边框，不加粗，罗马字体,不换行
            cellStyle5.WrapText = false;

            ICellStyle cellStyle6 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Left, VerticalAlignment.Center, false);//无边框，不加粗，罗马字体,不换行,靠左
            cellStyle6.WrapText = false;

            ICellStyle cellStyle10 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Left, VerticalAlignment.Center, false);//无边框，不加粗，罗马字体,换行,靠左


            //无边框，加粗，Arial字体，不换行，靠左 11
            ICellStyle cellStyle7 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle7.WrapText = false;
            //无边框，加粗，Arial字体，换行，靠左 11
            ICellStyle cellStyle11 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3, HorizontalAlignment.Left, VerticalAlignment.Center, false);

            //无边框，加粗，Arial字体，不换行，靠左 14
            ICellStyle cellStyle9 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont4, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle9.WrapText = false;
            //有边框，居中，加粗,Arial字体
            ICellStyle cellStyle8 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3, HorizontalAlignment.Center, VerticalAlignment.Center, true);

            IFont boldFont_IsItalic = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);
            boldFont_IsItalic.IsItalic = true;

            ICellStyle cellStyle_boldFont_IsItalic = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont_IsItalic);//加粗罗马字体，斜体

            int thisRow = 0;

            switch (makerTypeEnum)
            {
                case MakerTypeEnum.InspectionReceipt_Home:

                    #region 报检——报检首页

                    ExcelHelper.SetCellValue(sheet, 0, ExcelHelper.GetNumByChar('A'), "To:" + vm.FactoryName, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 1, ExcelHelper.GetNumByChar('A'), "ATTN:" + vm.FactoryContact, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 2, ExcelHelper.GetNumByChar('A'), "关于S56客人" + vm.PurchaseNumber + vm.HsName + "定单PO#:", cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 3, ExcelHelper.GetNumByChar('A'), vm.POID + "的报植检资料如下：", cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 5, ExcelHelper.GetNumByChar('B'), vm.HsName, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('B'), vm.HSCodeName, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('B'), vm.InvNo, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('B'), vm.ProductsQuauity, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('B'), vm.ProductsBoxNum, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('B'), "USD" + vm.ProductAmount, cellStyle5);

                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('A'), "由", cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('B'), vm.PortName, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('C'), "运到", cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('D'), "美国", cellStyle5);

                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('A'), "请于" + vm.ClaimFaxDate + "办理" + vm.INTypeName + "并换通关单", cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 15, ExcelHelper.GetNumByChar('D'), "制单人：" + vm.CreateUserName, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 16, ExcelHelper.GetNumByChar('D'), "日期：" + CommonCode.GetDateTime3(vm.CreateDate), cellStyle5);

                    #endregion 报检——报检首页                    #endregion 报检——报检首页

                    break;

                case MakerTypeEnum.InspectionReceipt_PackingList:

                    #region 报检——装箱单

                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont_IsItalic);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('I'), vm.InvNo, cellStyle6);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle6);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('I'), vm.POID, cellStyle10);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle6);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('I'), CommonCode.GetDateTime3(vm.CreateDate), cellStyle6);

                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('B'), "" + vm.INPortName + " ,CHINA", cellStyle6);
                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('B'), "" + vm.INEndPortName, cellStyle6);
                    ExcelHelper.SetCellValue(sheet, 15, ExcelHelper.GetNumByChar('B'), "   FOB " + vm.INPortName + " ,CHINA", cellStyle5);




                    int? SelectQty = 0;
                    int? SelectBoxQty = 0;
                    decimal? SumOuterWeightGross = 0;
                    decimal? SumOuterWeightNet = 0;
                    decimal? SumOuterVolume = 0;

                    thisRow = 19;

                    foreach (var item in vm.list_ShipmentOrderProduct)
                    {
                        SelectQty += item.SelectQty;
                        SelectBoxQty += item.SelectBoxQty;
                        SumOuterWeightGross += item.SumOuterWeightGross;
                        SumOuterWeightNet += item.SumOuterWeightNet;
                        SumOuterVolume += item.SumOuterVolume;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow, ExcelHelper.GetNumByChar('C'), ExcelHelper.GetNumByChar('E'));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.No, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.SkuNumber, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.HsEngName, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.SelectQty, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), item.SelectBoxQty, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), item.SumOuterWeightGross, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), item.SumOuterWeightNet, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), item.SumOuterVolume, cellStyle3);

                        ++thisRow;
                    }

                    ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow, ExcelHelper.GetNumByChar('C'), ExcelHelper.GetNumByChar('E'));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), SelectQty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), SelectBoxQty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), SumOuterWeightGross, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), SumOuterWeightNet, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), SumOuterVolume, cellStyle2);

                    thisRow += 6;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('H'), thisRow, ExcelHelper.GetNumByChar('I'), thisRow + 6);

                    #endregion 报检——装箱单

                    break;

                case MakerTypeEnum.InspectionReceipt_CommercialInvoice:

                    #region 报检——发票

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont_IsItalic);
                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('H'), vm.InvNo, cellStyle6);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle6);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('H'), vm.POID, cellStyle10);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle6);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('H'), CommonCode.GetDateTime3(vm.CreateDate), cellStyle6);

                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('B'), "" + vm.INPortName + " ,CHINA", cellStyle6);
                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('B'), "" + vm.INEndPortName, cellStyle6);
                    ExcelHelper.SetCellValue(sheet, 14, ExcelHelper.GetNumByChar('B'), "   FOB " + vm.INPortName + " ,CHINA", cellStyle5);

                    thisRow = 18;

                    SelectQty = 0;
                    decimal SumPrice = 0;

                    foreach (var item in vm.list_ShipmentOrderProduct)
                    {
                        SelectQty += item.SelectQty;
                        SumPrice += item.SumPrice;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow, ExcelHelper.GetNumByChar('D'), ExcelHelper.GetNumByChar('G'));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.No, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.SkuNumber, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.SelectQty, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.HsEngName, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), "", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), "", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), Keys.USD_Sign + item.ProductPrice, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), Keys.USD_Sign + item.SumPrice, cellStyle3);

                        ++thisRow;
                    }

                    ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow, ExcelHelper.GetNumByChar('D'), ExcelHelper.GetNumByChar('G'));
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), SelectQty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), Keys.USD_Sign + SumPrice, cellStyle2);

                    thisRow += 6;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('H'), thisRow, ExcelHelper.GetNumByChar('I'), thisRow + 6);

                    #endregion 报检——发票

                    break;

                case MakerTypeEnum.InspectionReceipt_SaleContract:

                    #region 报检——销售合同

                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle7);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('E'), "S/C NO.:" + vm.InvNo, cellStyle7);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle7);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('E'), "PO No.:" + vm.POID, cellStyle11);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle7);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('E'), "DATE:" + CommonCode.GetDateTime3(vm.CustomerDate), cellStyle7);

                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('A'), "Loading port: " + vm.INPortName + " ,CHINA", cellStyle7);
                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('A'), "Destination: " + vm.INEndPortName, cellStyle7);

                    thisRow = 20;

                    SelectQty = 0;
                    SumPrice = 0;

                    foreach (var item in vm.list_ShipmentOrderProduct)
                    {
                        SelectQty += item.SelectQty;
                        SumPrice += item.SumPrice;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.No, cellStyle8);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.SkuNumber, cellStyle8);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.HsEngName, cellStyle8);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.SelectQty, cellStyle8);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), Keys.USD_Sign + item.ProductPrice, cellStyle8);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Keys.USD_Sign + item.SumPrice, cellStyle8);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle8);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle8);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle8);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), SelectQty, cellStyle8);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle8);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Keys.USD_Sign + SumPrice, cellStyle8);

                    thisRow += 3;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "1. SHIPMENT DATE:" + vm.ShippingDateStart, cellStyle7);

                    thisRow += 2;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "3. TERM OF THE PAYMENT : " + vm.TeamOfThePayment, cellStyle7);

                    thisRow += 2;
                    for (int i = 0; i < vm.list_ShipmentOrderProduct.Count + 3; i++)
                    {
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                    }
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "THE  SELLERS :", cellStyle9);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "THE BUYERS :", cellStyle9);

                    ExcelHelper.CreatePicture(workbook, sheet, "/data/Documents/S188/tom.png", 75, 0, 800, 0, ExcelHelper.GetNumByChar('B'), thisRow + 1, ExcelHelper.GetNumByChar('C'), thisRow + 4);

                    ExcelHelper.CreatePicture(workbook, sheet, "/data/Documents/S56/Raanna.png", 75, 0, 800, 0, ExcelHelper.GetNumByChar('F'), thisRow, ExcelHelper.GetNumByChar('F'), thisRow + 5);

                    #endregion 报检——销售合同

                    break;

                case MakerTypeEnum.InspectionReceipt_Commission:
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// S56的报关
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionCustoms_S56<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
        {
            VMInspectionCustoms vm = model as VMInspectionCustoms;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);//加粗，罗马字体
            var boldFont2 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12, false);//不加粗，罗马字体
            var boldFont3 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 11);//加粗，Arial
            var boldFont4 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 14);//加粗，Arial
            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Center, VerticalAlignment.Center, false);//无边框，不加粗，罗马字体          
            ICellStyle cellStyle3 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Center, VerticalAlignment.Center, true);//有边框，不加粗，罗马字体

            ICellStyle cellStyle2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, true);//有边框，加粗，罗马字体
            ICellStyle cellStyle4 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, false);//无边框，加粗，罗马字体

            ICellStyle cellStyle5 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Left, VerticalAlignment.Center, false);//无边框，不加粗，罗马字体,不换行
            cellStyle5.WrapText = false;

            ICellStyle cellStyle10 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, false);//无边框，加粗，罗马字体.不换行
            cellStyle10.WrapText = false;

            ICellStyle cellStyle13 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Left, VerticalAlignment.Center, false);//无边框，加粗，罗马字体.不换行,靠左
            cellStyle13.WrapText = false;

            ICellStyle cellStyle6 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Left, VerticalAlignment.Center, false);//无边框，不加粗，罗马字体,不换行,靠左
            cellStyle6.WrapText = false;

            //无边框，加粗，Arial字体，不换行，靠左  11号
            ICellStyle cellStyle7 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle7.WrapText = false;
            //无边框，加粗，Arial字体，换行，靠左  11号
            ICellStyle cellStyle14 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3, HorizontalAlignment.Left, VerticalAlignment.Center, false);

            //有边框，加粗，Arial字体，不换行，靠左  11号
            ICellStyle cellStyle12 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3, HorizontalAlignment.Left, VerticalAlignment.Center, true);
            cellStyle12.WrapText = false;
            //无边框，加粗，Arial字体，不换行，靠左  14号
            ICellStyle cellStyle9 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont4, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle9.WrapText = false;
            //有边框，居中，加粗,Arial字体
            ICellStyle cellStyle8 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle11 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Center, VerticalAlignment.Center, false);//无边框，不加粗，罗马字体，靠左，不换行          
            IFont boldFont_IsItalic = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);
            boldFont_IsItalic.IsItalic = true;

            ICellStyle cellStyle_boldFont_IsItalic = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont_IsItalic);//加粗罗马字体，斜体
            int thisRow = 0;

            switch (makerTypeEnum)
            {
                #region 报关

                case MakerTypeEnum.InspectionCustoms_PackingList:

                    #region 报关——装箱单

                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont_IsItalic);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('J'), vm.InvoiceNO, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('J'), vm.POID, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('J'), CommonCode.GetDateTime3(vm.CreateDate), cellStyle5);

                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('B'), "" + vm.PortName, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('B'), "" + vm.DestinationPortName, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 14, ExcelHelper.GetNumByChar('B'), " FOB " + vm.PortName + " ,CHINA", cellStyle5);

                    int? SelectQty = 0;
                    int? SelectBoxQty = 0;
                    decimal? SumOuterWeightGross = 0;
                    decimal? SumOuterWeightNet = 0;
                    decimal? SumOuterVolume = 0;

                    thisRow = 18;

                    foreach (var item in vm.list_ShipmentOrderProduct)
                    {
                        SelectQty += item.SelectQty;
                        SelectBoxQty += item.SelectBoxQty;
                        SumOuterWeightGross += item.SumOuterWeightGross;
                        SumOuterWeightNet += item.SumOuterWeightNet;
                        SumOuterVolume += item.SumOuterVolume;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow, ExcelHelper.GetNumByChar('C'), ExcelHelper.GetNumByChar('E'));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.No, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.SkuNumber, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.HsEngName, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.SelectQty, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), item.SelectBoxQty, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), item.SumOuterWeightGross, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), item.SumOuterWeightNet, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), item.SumOuterVolume, cellStyle3);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow, ExcelHelper.GetNumByChar('C'), ExcelHelper.GetNumByChar('E'));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), SelectQty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), SelectBoxQty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), SumOuterWeightGross, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), SumOuterWeightNet, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), SumOuterVolume, cellStyle2);

                    thisRow = thisRow + 2;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "境内货源地：" + vm.TerritoryOfOriginOfGoods, cellStyle6);

                    #endregion 报关——装箱单

                    break;

                case MakerTypeEnum.InspectionCustoms_CommercialInvoice:

                    #region 报关——发票

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont_IsItalic);
                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('I'), vm.InvoiceNO, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('I'), vm.POID, cellStyle11);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('I'), CommonCode.GetDateTime3(vm.CreateDate), cellStyle5);

                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('B'), "" + vm.PortName, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('B'), "" + vm.DestinationPortName, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('B'), " FOB " + vm.PortName + " ,CHINA", cellStyle5);

                    thisRow = 17;

                    SelectQty = 0;
                    decimal SumPrice = 0;

                    foreach (var item in vm.list_ShipmentOrderProduct)
                    {
                        SelectQty += item.SelectQty;
                        SumPrice += item.SumPrice;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow, ExcelHelper.GetNumByChar('D'), ExcelHelper.GetNumByChar('G'));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.No, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.SkuNumber, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.SelectQty, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.HsEngName, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), "", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), "", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), Keys.USD_Sign + item.ProductPrice, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), Keys.USD_Sign + item.SumPrice, cellStyle3);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow, ExcelHelper.GetNumByChar('D'), ExcelHelper.GetNumByChar('G'));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), SelectQty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), Keys.USD_Sign + SumPrice, cellStyle2);

                    thisRow += 2;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), " 境内货源地：" + vm.TerritoryOfOriginOfGoods, cellStyle13);

                    #endregion 报关——发票

                    break;

                case MakerTypeEnum.InspectionCustoms_SaleContract:

                    #region 报关——销售合同

                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont_IsItalic);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('F'), vm.InvoiceNO, cellStyle7);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle7);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('F'), vm.POID, cellStyle14);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle7);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('F'), CommonCode.GetDateTime3(vm.CustomerDate), cellStyle7);

                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('B'), "" + vm.PortName + " ,CHINA", cellStyle7);
                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('B'), "" + vm.DestinationPortName, cellStyle7);

                    thisRow = 21;

                    SelectQty = 0;
                    SumPrice = 0;

                    foreach (var item in vm.list_ShipmentOrderProduct)
                    {
                        SelectQty += item.SelectQty;
                        SumPrice += item.SumPrice;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.No, cellStyle12);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.SkuNumber, cellStyle12);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.HsEngName, cellStyle12);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.SelectQty, cellStyle12);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), Keys.USD_Sign + item.ProductPrice, cellStyle12);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Keys.USD_Sign + item.SumPrice, cellStyle12);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "", cellStyle12);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle12);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "TOTAL:", cellStyle12);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), SelectQty, cellStyle12);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle12);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Keys.USD_Sign + SumPrice, cellStyle12);

                    thisRow += 3;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "1. SHIPMENT DATE:" + vm.ShippingDateStart, cellStyle7);

                    thisRow += 2;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "3. TERM OF THE PAYMENT : " + vm.TeamOfThePayment, cellStyle7);

                    thisRow += 2;
                    for (int i = 0; i < vm.list_ShipmentOrderProduct.Count + 3; i++)
                    {
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                    }
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "THE  SELLERS :", cellStyle9);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "THE BUYERS :", cellStyle9);

                    ExcelHelper.CreatePicture(workbook, sheet, "/data/Documents/S188/tom.png", 75, 0, 800, 0, ExcelHelper.GetNumByChar('B'), thisRow + 1, ExcelHelper.GetNumByChar('C'), thisRow + 4);

                    ExcelHelper.CreatePicture(workbook, sheet, "/data/Documents/S56/Raanna.png", 75, 0, 800, 0, ExcelHelper.GetNumByChar('F'), thisRow, ExcelHelper.GetNumByChar('F'), thisRow + 5);

                    #endregion 报关——销售合同

                    break;

                #endregion 报关

                default:
                    break;
            }
        }

        /// <summary>
        /// S56的清关
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionClearance_S56<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
        {
            VMInspectionClearance vm = model as VMInspectionClearance;

            var font_16 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 16, false);
            var font_16_bold = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 16, true);

            var font_12 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12, false);
            var font_12_bold = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12, true);

            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, font_16, HorizontalAlignment.Center, VerticalAlignment.Center, true);

            ICellStyle cellStyle_16_bold_left = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, font_16_bold, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle_16_bold_left.WrapText = false;

            ICellStyle cellStyle_16_left = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, font_16, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle_16_left.WrapText = false;

            ICellStyle cellStyle_12_center_border = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, font_12, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle_12_bold_center_border = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, font_12_bold, HorizontalAlignment.Center, VerticalAlignment.Center, true);

            ICellStyle cellStyle_12_bold_left = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, font_12_bold, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle_12_bold_left.WrapText = false;

            ICellStyle cellStyle_12_left = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, font_12, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle_12_left.WrapText = false;

            int thisRow = 0;

            switch (makerTypeEnum)
            {
                case MakerTypeEnum.InspectionClearance_CommercialInvoice:

                    #region 清关——发票

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('F'), "Inv. No.:" + vm.InvoiceNO, cellStyle_16_bold_left);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('F'), "PO NO.: " + vm.POID, cellStyle_16_bold_left);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('F'), "Date: " + CommonCode.GetDateTime3(vm.CreateDate), cellStyle_16_bold_left);

                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('A'), "Loading port: " + vm.PortName + ", CHINA", cellStyle_16_bold_left);
                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('A'), "Destination: " + vm.DestinationPortName, cellStyle_16_bold_left);

                    thisRow = 17;

                    int Qty = 0;
                    int BoxQty = 0;
                    decimal SumSalePrice = 0;

                    decimal? SumOuterWeightGross = 0;
                    decimal? SumOuterWeightNet = 0;

                    foreach (var item in vm.list_OrderProduct_PackingList)
                    {
                        Qty += item.Qty;
                        BoxQty += item.BoxQty ?? 0;
                        SumSalePrice += item.SumSalePrice;

                        SumOuterWeightGross += item.SumOuterWeightGross;
                        SumOuterWeightNet += item.SumOuterWeightNet;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.POID, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.SkuNumber, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.No, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.Desc, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Keys.USD_Sign + Math.Round(item.SalePrice, 2), cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), Keys.USD_Sign + Math.Round(item.SumSalePrice, 2), cellStyle);

                        ++thisRow;
                    }

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), Qty, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), Keys.USD_Sign + Math.Round(SumSalePrice, 2), cellStyle);

                    ++thisRow;
                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL: USD " + CommonCode.GetEnMoneyString(Math.Round(SumSalePrice, 2).ToString()) + " ONLY. ", cellStyle_16_left);

                    thisRow += 8;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('E'), thisRow, ExcelHelper.GetNumByChar('E'), thisRow + 6, 75, 0, 500, 0);

                    #endregion 清关——发票

                    break;

                case MakerTypeEnum.InspectionClearance_PackingList:

                    #region 清关——装箱单

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('G'), "Inv. No.:" + vm.InvoiceNO, cellStyle_12_bold_left);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('G'), "PO NO.: " + vm.POID, cellStyle_12_bold_left);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('G'), "Date: " + CommonCode.GetDateTime3(vm.CreateDate), cellStyle_12_bold_left);

                    thisRow = 13;

                    Qty = 0;
                    BoxQty = 0;
                    SumOuterWeightGross = 0;
                    SumOuterWeightNet = 0;
                    decimal? SumOuterVolume = 0;

                    foreach (var item in vm.list_OrderProduct_PackingList)
                    {
                        Qty += item.Qty;
                        BoxQty += item.BoxQty ?? 0;

                        SumOuterWeightGross += item.SumOuterWeightGross;
                        SumOuterWeightNet += item.SumOuterWeightNet;
                        SumOuterVolume += item.SumOuterVolume;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.POID, cellStyle_12_center_border);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.SkuNumber, cellStyle_12_center_border);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.No, cellStyle_12_center_border);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.Desc, cellStyle_12_center_border);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.Qty, cellStyle_12_center_border);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.BoxQty, cellStyle_12_center_border);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), item.SumOuterWeightGross, cellStyle_12_center_border);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), item.SumOuterWeightNet, cellStyle_12_center_border);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), item.SumOuterVolume, cellStyle_12_center_border);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL", cellStyle_12_bold_center_border);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle_12_bold_center_border);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle_12_bold_center_border);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle_12_bold_center_border);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), Qty, cellStyle_12_bold_center_border);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), BoxQty, cellStyle_12_bold_center_border);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), SumOuterWeightGross, cellStyle_12_bold_center_border);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), SumOuterWeightNet, cellStyle_12_bold_center_border);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), SumOuterVolume, cellStyle_12_bold_center_border);

                    thisRow += 8;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('E'), thisRow, ExcelHelper.GetNumByChar('G'), thisRow + 8);

                    #endregion 清关——装箱单

                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// S56的结汇
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionExchange_S56<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
        {

        }
    }
}